using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using GraphX.Controls.Models;
using GraphX.Common.Enums;
using GraphX.Common.Exceptions;
using GraphX.Common.Interfaces;
using GraphX.Common;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace GraphX.Controls
{
    public abstract class EdgeControlBase : TemplatedControl, IGraphControl, IDisposable
    {
        #region Properties & Fields

        public bool HideEdgePointerOnVertexOverlap { get; set; } = true;

        public double HideEdgePointerByEdgeLength { get; set; } = 0.0d;

        public abstract bool IsSelfLooped { get; protected set; }

        public abstract void Dispose();

        public abstract void Clean();

        protected DoubleCollection StrokeDashArray { get; set; }

        public bool IsParallel { get; internal set; }

        protected Control SelfLoopIndicator;

        private Rect _selfLoopedEdgeLastKnownRect;

        protected virtual void OnSourceChanged(object d, AvaloniaPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnTargetChanged(object d, AvaloniaPropertyChangedEventArgs e)
        {
        }

        public GraphAreaBase RootArea
        {
            get => GetValue(RootCanvasProperty);
            set => SetValue(RootCanvasProperty, value);
        }

        public static readonly StyledProperty<GraphAreaBase> RootCanvasProperty =
            AvaloniaProperty.Register<EdgeControlBase, GraphAreaBase>(nameof(RootArea), null);

        public static readonly StyledProperty<double> SelfLoopIndicatorRadiusProperty =
            AvaloniaProperty.Register<EdgeControlBase, double>(nameof(SelfLoopIndicatorRadius), 5d);

        public double SelfLoopIndicatorRadius
        {
            get => GetValue(SelfLoopIndicatorRadiusProperty);
            set => SetValue(SelfLoopIndicatorRadiusProperty, value);
        }

        public static readonly StyledProperty<Point> SelfLoopIndicatorOffsetProperty =
            AvaloniaProperty.Register<EdgeControlBase, Point>(nameof(SelfLoopIndicatorOffset), new Point());

        public Point SelfLoopIndicatorOffset
        {
            get => GetValue(SelfLoopIndicatorOffsetProperty);
            set => SetValue(SelfLoopIndicatorOffsetProperty, value);
        }

        public static readonly StyledProperty<bool> ShowSelfLoopIndicatorProperty =
            AvaloniaProperty.Register<EdgeControlBase, bool>(nameof(ShowSelfLoopIndicator), true);

        public bool ShowSelfLoopIndicator
        {
            get => GetValue(ShowSelfLoopIndicatorProperty);
            set => SetValue(ShowSelfLoopIndicatorProperty, value);
        }

        public static readonly StyledProperty<VertexControl> SourceProperty =
            AvaloniaProperty.Register<EdgeControlBase, VertexControl>(nameof(Source), null, OnSourceChangedInternal);

        private static void OnSourceChangedInternal(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            (d as EdgeControlBase)?.OnSourceChanged(d, e);
        }

        public static readonly StyledProperty<VertexControl> TargetProperty =
            AvaloniaProperty.Register<EdgeControlBase, VertexControl>(nameof(Target), null, OnTargetChangedInternal);

        private static void OnTargetChangedInternal(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            (d as EdgeControlBase)?.OnTargetChanged(d, e);
        }

        public static readonly StyledProperty<object> EdgeProperty =
            AvaloniaProperty.Register<EdgeControlBase, object>(nameof(Edge), null);

        public static readonly StyledProperty<EdgeDashStyle> DashStyleProperty =
            AvaloniaProperty.Register<EdgeControlBase, EdgeDashStyle>(nameof(DashStyle), EdgeDashStyle.Solid, OnDashStyleChanged);

        private static void OnDashStyleChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var ec = d as EdgeControlBase;
            if (ec == null) return;
            switch ((EdgeDashStyle)e.NewValue)
            {
                case EdgeDashStyle.Solid:
                    ec.StrokeDashArray = null;
                    break;
                case EdgeDashStyle.Dash:
                    ec.StrokeDashArray = new DoubleCollection { 4.0, 2.0 };
                    break;
                case EdgeDashStyle.Dot:
                    ec.StrokeDashArray = new DoubleCollection { 1.0, 2.0 };
                    break;
                case EdgeDashStyle.DashDot:
                    ec.StrokeDashArray = new DoubleCollection { 4.0, 2.0, 1.0, 2.0 };
                    break;
                case EdgeDashStyle.DashDotDot:
                    ec.StrokeDashArray = new DoubleCollection { 4.0, 2.0, 1.0, 2.0, 1.0, 2.0 };
                    break;
                default:
                    ec.StrokeDashArray = null;
                    break;
            }
            ec.UpdateEdge(false);
        }

        public EdgeDashStyle DashStyle
        {
            get => GetValue(DashStyleProperty);
            set => SetValue(DashStyleProperty, value);
        }

        public bool CanBeParallel { get; set; } = true;

        protected EdgeControlBase()
        {
            _updateLabelPosition = true;
            Loaded += EdgeControlBase_Loaded;
        }

        private bool _isInDesignMode = false;

        private void EdgeControlBase_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= EdgeControlBase_Loaded;
            _isInDesignMode = this.IsInDesignMode();
        }

        private bool _updateLabelPosition;

        public bool UpdateLabelPosition
        {
            get => _updateLabelPosition;
            set => _updateLabelPosition = true;
        }

        public bool IsHiddenEdgesUpdated { get; set; }

        public static readonly StyledProperty<bool> ShowArrowsProperty =
            AvaloniaProperty.Register<EdgeControlBase, bool>(nameof(ShowArrows), true, OnShowArrowsChanged);

        private static void OnShowArrowsChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var ctrl = d as EdgeControlBase;
            if (ctrl == null) return;
            ctrl.UpdateEdge(false);
        }

        public bool ShowArrows
        {
            get => GetValue(ShowArrowsProperty);
            set => SetValue(ShowArrowsProperty, value);
        }

        public bool ManualDrawing { get; set; }

        protected IGeometry Linegeometry;

        protected Path LinePathObject;

        private IList<IEdgeLabelControl> _edgeLabelControls = new List<IEdgeLabelControl>();

        protected internal IList<IEdgeLabelControl> EdgeLabelControls
        {
            get => _edgeLabelControls;
            set { _edgeLabelControls = value; OnEdgeLabelUpdated(); }
        }

        protected IEdgePointer EdgePointerForSource;
        protected IEdgePointer EdgePointerForTarget;

        public IEdgePointer GetEdgePointerForSource() => EdgePointerForSource;

        public IEdgePointer GetEdgePointerForTarget() => EdgePointerForTarget;

        public EdgeEventOptions EventOptions { get; protected set; }

        public VertexControl Source
        {
            get => GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public VertexControl Target
        {
            get => GetValue(TargetProperty);
            set => SetValue(TargetProperty, value);
        }

        public object Edge
        {
            get => GetValue(EdgeProperty);
            set => SetValue(EdgeProperty, value);
        }

        public void AttachLabel(IEdgeLabelControl ctrl)
        {
            EdgeLabelControls.Add(ctrl);
            if (RootArea != null && !RootArea.Children.Contains((Control)ctrl))
                RootArea.Children.Add((Control)ctrl);
            ctrl.Show();
            var r = ctrl.GetSize();
            if (r == Rect.Empty)
            {
                ctrl.UpdateLayout();
                ctrl.UpdatePosition();
            }
        }

        public void DetachLabels(IEdgeLabelControl ctrl = null)
        {
            EdgeLabelControls.Where(l => l is IAttachableControl<EdgeControl>).Cast<IAttachableControl<EdgeControl>>().ForEach(label =>
            {
                label.Detach();
                if (RootArea != null)
                    RootArea.Children.Remove((Control)label);
            });
            EdgeLabelControls.Clear();
        }

        public void UpdateLabel()
        {
            _edgeLabelControls.Where(l => l.ShowLabel).ForEach(l =>
            {
                l.Show();
                l.UpdateLayout();
                l.UpdatePosition();
            });
        }

        #endregion Properties & Fields

        #region Position methods

        public void SetPosition(Point pt, bool alsoFinal = true)
        {
            GraphAreaBase.SetX(this, pt.X, alsoFinal);
            GraphAreaBase.SetY(this, pt.Y, alsoFinal);
        }

        public void SetPosition(double x, double y, bool alsoFinal = true)
        {
            GraphAreaBase.SetX(this, x, alsoFinal);
            GraphAreaBase.SetY(this, y, alsoFinal);
        }

        public Point GetPosition(bool final = false, bool round = false)
        {
            return new Point(
                final ? GraphAreaBase.GetFinalX(this) : GraphAreaBase.GetX(this),
                final ? GraphAreaBase.GetFinalY(this) : GraphAreaBase.GetY(this));
        }

        internal Measure.Point GetPositionGraphX(bool final = false, bool round = false)
        {
            return new Measure.Point(
                final ? GraphAreaBase.GetFinalX(this) : GraphAreaBase.GetX(this),
                final ? GraphAreaBase.GetFinalY(this) : GraphAreaBase.GetY(this));
        }

        #endregion Position methods

        #region Manual path controls

        public PathGeometry GetEdgePathManually()
        {
            if (!ManualDrawing) return null;
            return Linegeometry as PathGeometry;
        }

        public void SetEdgePathManually(PathGeometry geo)
        {
            if (!ManualDrawing) return;
            Linegeometry = geo;
            UpdateEdge();
        }

        #endregion Manual path controls

        internal void SetVisibility(Avalonia.Visibility value)
        {
            this.SetValue(VisibilityProperty, value);
        }

        internal virtual void InvalidateChildren()
        {
            EdgeLabelControls.ForEach(l => l.UpdateLayout());
            if (LinePathObject != null && Source != null)
            {
                var pos = Source.GetPosition();
                Source.SetPosition(pos.X, pos.Y);
            }
        }

        public bool IsTemplateLoaded => LinePathObject != null;

        protected virtual void OnEdgeLabelUpdated()
        {
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Template == null) return;

            LinePathObject = GetTemplatePart("PART_edgePath") as Path;
            if (LinePathObject == null)
                throw new GX_ObjectNotFoundException("EdgeControlBase Template -> Edge template must contain 'PART_edgePath' Path object to draw route points!");
            LinePathObject.Data = Linegeometry;

            if (GetTemplatePart("PART_edgeLabel") != null)
                throw new GX_ObsoleteException("PART_edgeLabel is obsolete. Please use attachable labels mechanics!");

            EdgePointerForSource = GetTemplatePart("PART_EdgePointerForSource") as IEdgePointer;
            EdgePointerForTarget = GetTemplatePart("PART_EdgePointerForTarget") as IEdgePointer;

            SelfLoopIndicator = GetTemplatePart("PART_SelfLoopedEdge") as Control;
            if (SelfLoopIndicator != null)
            {
                SelfLoopIndicator.LayoutUpdated += (sender, args) =>
                {
                    SelfLoopIndicator?.Arrange(_selfLoopedEdgeLastKnownRect);
                };
            }

            MeasureChild(EdgePointerForSource as Control);
            MeasureChild(EdgePointerForTarget as Control);
            MeasureChild(SelfLoopIndicator);

            UpdateSelfLoopedEdgeData();
            UpdateEdge();
        }

        protected void MeasureChild(Control child)
        {
            child?.Measure(Size.Infinity);
        }

        #region public PrepareEdgePath()

        public virtual void UpdateEdge(bool updateLabel = true)
        {
            if (Visibility == Avalonia.Visibility.Visible || IsHiddenEdgesUpdated)
            {
                EdgeLabelControls.ForEach(l =>
                {
                    if (l.ShowLabel) l.Show();
                    else l.Hide();
                });
                UpdateEdgeRendering(updateLabel);
            }
        }

        internal virtual void UpdateEdgeRendering(bool updateLabel = true)
        {
            if (!IsTemplateLoaded)
                ApplyTemplate();
            if (ShowArrows)
            {
                if (EdgePointerForSource?.IsVisible == true)
                    EdgePointerForSource?.Show();

                if (EdgePointerForTarget?.IsVisible == true)
                    EdgePointerForTarget?.Show();
            }
            else
            {
                EdgePointerForSource?.Hide();
                EdgePointerForTarget?.Hide();
            }
            PrepareEdgePath(true, null, updateLabel);
            if (LinePathObject == null) return;
            LinePathObject.Data = Linegeometry;
            LinePathObject.StrokeDashArray = StrokeDashArray;
        }

        internal int ParallelEdgeOffset;

        internal virtual Point GetParallelOffset(Point sourceCenter, Point targetCenter, int sideDistance)
        {
            var mainVector = new Vector(targetCenter.X - sourceCenter.X, targetCenter.Y - sourceCenter.Y);
            var joint = new Point(
                sourceCenter.X + sideDistance * (mainVector.Y / mainVector.Length),
                sourceCenter.Y - sideDistance * (mainVector.X / mainVector.Length));
            return joint;
        }

        protected internal Point? SourceConnectionPoint;

        protected internal Point? TargetConnectionPoint;

        protected bool HasSelfLoopedEdgeTemplate => SelfLoopIndicator != null;

        protected virtual void UpdateSelfLoopedEdgeData()
        {
            if (IsSelfLooped)
            {
                EdgePointerForSource?.Hide();
                EdgePointerForTarget?.Hide();

                if (!ShowSelfLoopIndicator) return;

                if (!HasSelfLoopedEdgeTemplate)
                    Linegeometry = new EllipseGeometry();
                else SelfLoopIndicator.SetValue(VisibilityProperty, Avalonia.Visibility.Visible);
            }
            else
            {
                if (HasSelfLoopedEdgeTemplate)
                    SelfLoopIndicator.SetValue(VisibilityProperty, Avalonia.Visibility.Collapsed);
            }
        }

        protected virtual void PrepareSelfLoopedEdge(Point sourcePos)
        {
            if (!ShowSelfLoopIndicator)
                return;

            var hasNoTemplate = !HasSelfLoopedEdgeTemplate;
            var pt = new Point(
                sourcePos.X + SelfLoopIndicatorOffset.X - (hasNoTemplate ? SelfLoopIndicatorRadius : SelfLoopIndicator.DesiredSize.Width),
                sourcePos.Y + SelfLoopIndicatorOffset.Y - (hasNoTemplate ? SelfLoopIndicatorRadius : SelfLoopIndicator.DesiredSize.Height));

            if (hasNoTemplate)
            {
            }
            else _selfLoopedEdgeLastKnownRect = new Rect(pt, SelfLoopIndicator.DesiredSize);
        }

        public virtual void PrepareEdgePathFromMousePointer(bool useCurrentCoords = false)
        {
            if ((Visibility != Avalonia.Visibility.Visible && !IsHiddenEdgesUpdated) && ManualDrawing || !IsTemplateLoaded) return;

            var sourceSize = new Size(Source.ActualWidth, Source.ActualHeight);

            var sourcePos = new Point(
                (useCurrentCoords ? GraphAreaBase.GetX(Source) : GraphAreaBase.GetFinalX(Source)) + sourceSize.Width * 0.5,
                (useCurrentCoords ? GraphAreaBase.GetY(Source) : GraphAreaBase.GetFinalY(Source)) + sourceSize.Height * 0.5);

            var targetSize = new Size(80, 20);

            var targetPos = new Point(0, 0);

            var routedEdge = Edge as IRoutingInfo;
            if (routedEdge == null)
                throw new GX_InvalidDataException("Edge must implement IRoutingInfo interface");

            var routeInformation = routedEdge.RoutingPoints;

            var sourcePos1 = new Point(
                (useCurrentCoords ? GraphAreaBase.GetX(Source) : GraphAreaBase.GetFinalX(Source)),
                (useCurrentCoords ? GraphAreaBase.GetY(Source) : GraphAreaBase.GetFinalY(Source)));

            var targetPos1 = new Point(0, 0);

            var hasEpSource = EdgePointerForSource != null;
            var hasEpTarget = EdgePointerForTarget != null;

            if (IsSelfLooped)
            {
                PrepareSelfLoopedEdge(sourcePos1);
                return;
            }

            var hasRouteInfo = routeInformation != null && routeInformation.Length > 1;

            var gEdge = Edge as IGraphXCommonEdge;
            Point p1;
            Point p2;

            if (gEdge?.SourceConnectionPointId != null)
            {
                var sourceCp = Source.GetConnectionPointById(gEdge.SourceConnectionPointId.Value, true);
                if (sourceCp == null)
                    throw new GX_ObjectNotFoundException($"Can't find source vertex VCP by edge source connection point Id({gEdge.SourceConnectionPointId}) : {Source}");
                if (sourceCp.Shape == VertexShape.None) p1 = sourceCp.RectangularSize.Center();
                else
                {
                    var targetCpPos = hasRouteInfo ? routeInformation[1].ToWindows() : targetPos;
                    p1 = GeometryHelper.GetEdgeEndpoint(sourceCp.RectangularSize.Center(), sourceCp.RectangularSize, targetCpPos, sourceCp.Shape);
                }
            }
            else
                p1 = GeometryHelper.GetEdgeEndpoint(sourcePos, new Rect(sourcePos1, sourceSize), (hasRouteInfo ? routeInformation[1].ToWindows() : targetPos), Source.VertexShape);

            p2 = GeometryHelper.GetEdgeEndpoint(
                targetPos, new Rect(targetPos, targetSize), hasRouteInfo ? routeInformation[routeInformation.Length - 2].ToWindows() : sourcePos, VertexShape.None);

            SourceConnectionPoint = p1;
            TargetConnectionPoint = p2;

            Linegeometry = new PathGeometry();
            PathFigure lineFigure = new PathFigure();

            if (RootArea != null && hasRouteInfo)
            {
                var routePoints = routeInformation.ToWindows().ToList();
                routePoints.Clear();
                routePoints.Add(p1);
                routePoints.Add(p2);

                if (routedEdge.RoutingPoints != null)
                    routedEdge.RoutingPoints = routePoints.ToArray().ToGraphX();

                if (RootArea.EdgeCurvingEnabled)
                {
                    var oPolyLineSegment = GeometryHelper.GetCurveThroughPoints(routePoints.ToArray(), 0.5, RootArea.EdgeCurvingTolerance);

                    if (hasEpTarget)
                    {
                        UpdateTargetEpData(oPolyLineSegment.Points[oPolyLineSegment.Points.Count - 1], oPolyLineSegment.Points[oPolyLineSegment.Points.Count - 2]);
                        oPolyLineSegment.Points.RemoveAt(oPolyLineSegment.Points.Count - 1);
                    }
                    if (hasEpSource)
                    {
                        UpdateSourceEpData(oPolyLineSegment.Points.First(), oPolyLineSegment.Points[1]);
                        oPolyLineSegment.Points.RemoveAt(0);
                    }

                    lineFigure = GeometryHelper.GetPathFigureFromPathSegments(routePoints[0], true, true, oPolyLineSegment);
                }
                else
                {
                    if (hasEpSource)
                        routePoints[0] = routePoints[0].Subtract(UpdateSourceEpData(routePoints.First(), routePoints[1]));
                    if (hasEpTarget)
                        routePoints[routePoints.Count - 1] = routePoints[routePoints.Count - 1].Subtract(UpdateTargetEpData(p2, routePoints[routePoints.Count - 2]));

                    if (gEdge.ReversePath)
                        routePoints.Reverse();

                    var pcol = new PointCollection();
                    routePoints.ForEach(a => pcol.Add(a));

                    lineFigure = new PathFigure { StartPoint = p1, Segments = new PathSegmentCollection { new PolyLineSegment { Points = pcol } }, IsClosed = false };
                }
            }
            else
            {
                bool remainHidden = false;
                if (HideEdgePointerByEdgeLength != 0d)
                {
                    if (MathHelper.GetDistanceBetweenPoints(p1, p2) <= HideEdgePointerByEdgeLength)
                    {
                        EdgePointerForSource?.Hide();
                        EdgePointerForTarget?.Hide();
                        remainHidden = true;
                    }
                    else
                    {
                        EdgePointerForSource?.Show();
                        EdgePointerForTarget?.Show();
                    }
                }

                if (hasEpSource)
                    p1 = p1.Subtract(UpdateSourceEpData(p1, p2, remainHidden));
                if (hasEpTarget)
                    p2 = p2.Subtract(UpdateTargetEpData(p2, p1, remainHidden));

                lineFigure = new PathFigure { StartPoint = gEdge.ReversePath ? p2 : p1, Segments = new PathSegmentCollection { new LineSegment { Point = gEdge.ReversePath ? p1 : p2 } }, IsClosed = false };
            }
            ((PathGeometry)Linegeometry).Figures.Add(lineFigure);

            if (_updateLabelPosition)
                EdgeLabelControls.Where(l => l.ShowLabel).ForEach(l => l.UpdatePosition());

            if (ShowArrows)
            {
                EdgePointerForSource?.Show();
                EdgePointerForTarget?.Show();
            }
            else
            {
                EdgePointerForSource?.Hide();
                EdgePointerForTarget?.Hide();
            }

            if (LinePathObject == null) return;
            LinePathObject.Data = Linegeometry;
            LinePathObject.StrokeDashArray = StrokeDashArray;
        }

        public virtual void PrepareEdgePath(bool useCurrentCoords = false, Measure.Point[] externalRoutingPoints = null, bool updateLabel = true)
        {
            if ((Visibility != Avalonia.Visibility.Visible && !IsHiddenEdgesUpdated) && Source == null || Target == null || ManualDrawing || !IsTemplateLoaded) return;

            #region Get the inputs

            var sourceTopLeft = new Point(
                (useCurrentCoords ? GraphAreaBase.GetX(Source) : GraphAreaBase.GetFinalX(Source)),
                (useCurrentCoords ? GraphAreaBase.GetY(Source) : GraphAreaBase.GetFinalY(Source)));

            var targetTopLeft = new Point(
                (useCurrentCoords ? GraphAreaBase.GetX(Target) : GraphAreaBase.GetFinalX(Target)),
                (useCurrentCoords ? GraphAreaBase.GetY(Target) : GraphAreaBase.GetFinalY(Target)));

            Size sourceSize;
            if (_isInDesignMode)
                sourceSize = new Size(80, 20);
            else
                sourceSize = new Size(Source.ActualWidth, Source.ActualHeight);

            Size targetSize;
            if (_isInDesignMode)
                targetSize = new Size(80, 20);
            else
                targetSize = new Size(Target.ActualWidth, Target.ActualHeight);

            var sourceCenter = new Point(
                sourceTopLeft.X + sourceSize.Width * .5,
                sourceTopLeft.Y + sourceSize.Height * .5);

            var targetCenter = new Point(
                targetTopLeft.X + targetSize.Width * .5,
                targetTopLeft.Y + targetSize.Height * .5);

            var routedEdge = Edge as IRoutingInfo;
            if (routedEdge == null)
                throw new GX_InvalidDataException("Edge must implement IRoutingInfo interface");

            var routeInformation = externalRoutingPoints ?? routedEdge.RoutingPoints;

            var hasEpSource = EdgePointerForSource != null;
            var hasEpTarget = EdgePointerForTarget != null;

            #endregion Get the inputs

            if (IsSelfLooped)
            {
                PrepareSelfLoopedEdge(sourceTopLeft);
                return;
            }

            var hasRouteInfo = routeInformation != null && routeInformation.Length > 1;

            var gEdge = Edge as IGraphXCommonEdge;

            Func<IVertexConnectionPoint> getSourceCpOrThrow = () =>
            {
                var cp = Source.GetConnectionPointById(gEdge.SourceConnectionPointId.Value, true);
                if (cp == null)
                    throw new GX_ObjectNotFoundException($"Can't find source vertex VCP by edge source connection point Id({gEdge.SourceConnectionPointId}) : {Source}");
                return cp;
            };
            Func<IVertexConnectionPoint> getTargetCpOrThrow = () =>
            {
                var cp = Target.GetConnectionPointById(gEdge.TargetConnectionPointId.Value, true);
                if (cp == null)
                    throw new GX_ObjectNotFoundException($"Can't find target vertex VCP by edge target connection point Id({gEdge.TargetConnectionPointId}) : {Target}");
                return cp;
            };
            Func<IVertexConnectionPoint, Point, Point, Point> getCpEndPoint = (cp, cpCenter, distantEnd) =>
            {
                Point calculatedCp;
                if (cp.Shape == VertexShape.None)
                    calculatedCp = cpCenter;
                else
                    calculatedCp = GeometryHelper.GetEdgeEndpoint(cpCenter, cp.RectangularSize, distantEnd, cp.Shape);
                return calculatedCp;
            };
            Func<bool> needParallelCalc = () => RootArea != null && !hasRouteInfo && RootArea.EnableParallelEdges && IsParallel;

            if (gEdge?.SourceConnectionPointId != null && gEdge?.TargetConnectionPointId != null)
            {
                var sourceCp = getSourceCpOrThrow();
                var targetCp = getTargetCpOrThrow();
                var sourceCpCenter = sourceCp.RectangularSize.Center();
                var targetCpCenter = targetCp.RectangularSize.Center();

                SourceConnectionPoint = getCpEndPoint(sourceCp, sourceCpCenter, targetCpCenter);
                TargetConnectionPoint = getCpEndPoint(targetCp, targetCpCenter, sourceCpCenter);
            }
            else if (gEdge?.SourceConnectionPointId != null)
            {
                var sourceCp = getSourceCpOrThrow();
                var sourceCpCenter = sourceCp.RectangularSize.Center();

                if (needParallelCalc())
                {
                    var m = new Point(targetCenter.X - sourceCenter.X, targetCenter.Y - sourceCenter.Y);
                    targetCenter = new Point(sourceCpCenter.X + m.X, sourceCpCenter.Y + m.Y);
                }
                else if (hasRouteInfo)
                {
                    targetCenter = routeInformation[1].ToWindows();
                }

                SourceConnectionPoint = getCpEndPoint(sourceCp, sourceCpCenter, targetCenter);
                TargetConnectionPoint = GeometryHelper.GetEdgeEndpoint(targetCenter, new Rect(targetTopLeft, targetSize), hasRouteInfo ? routeInformation[routeInformation.Length - 2].ToWindows() : sourceCpCenter, Target.VertexShape);
            }
            else if (gEdge?.TargetConnectionPointId != null)
            {
                var targetCp = getTargetCpOrThrow();
                var targetCpCenter = targetCp.RectangularSize.Center();

                if (needParallelCalc())
                {
                    var m = new Point(sourceCenter.X - targetCenter.X, sourceCenter.Y - targetCenter.Y);
                    sourceCenter = new Point(targetCpCenter.X + m.X, targetCpCenter.Y + m.Y);
                }
                else if (hasRouteInfo)
                {
                    sourceCenter = routeInformation[routeInformation.Length - 2].ToWindows();
                }

                SourceConnectionPoint = GeometryHelper.GetEdgeEndpoint(sourceCenter, new Rect(sourceTopLeft, sourceSize), (hasRouteInfo ? routeInformation[1].ToWindows() : targetCpCenter), Source.VertexShape);
                TargetConnectionPoint = getCpEndPoint(targetCp, targetCpCenter, sourceCenter);
            }
            else
            {
                if (needParallelCalc())
                {
                    var origSC = sourceCenter;
                    var origTC = targetCenter;
                    sourceCenter = GetParallelOffset(origSC, origTC, ParallelEdgeOffset);
                    targetCenter = GetParallelOffset(origTC, origSC, -ParallelEdgeOffset);
                }
                SourceConnectionPoint = GeometryHelper.GetEdgeEndpoint(sourceCenter, new Rect(sourceTopLeft, sourceSize), (hasRouteInfo ? routeInformation[1].ToWindows() : targetCenter), Source.VertexShape);
                TargetConnectionPoint = GeometryHelper.GetEdgeEndpoint(targetCenter, new Rect(targetTopLeft, targetSize), hasRouteInfo ? routeInformation[routeInformation.Length - 2].ToWindows() : sourceCenter, Target.VertexShape);
            }

            if (!SourceConnectionPoint.HasValue || !TargetConnectionPoint.HasValue)
                throw new GX_GeneralException("One or both connection points was not found due to an internal error.");

            var p1 = SourceConnectionPoint.Value;
            var p2 = TargetConnectionPoint.Value;

            Linegeometry = new PathGeometry();
            PathFigure lineFigure;

            if (RootArea != null && hasRouteInfo)
            {
                var routePoints = routeInformation.ToWindows().ToList();
                routePoints.Remove(routePoints.First());
                routePoints.Remove(routePoints.Last());
                routePoints.Insert(0, p1);
                routePoints.Add(p2);

                if (externalRoutingPoints == null && routedEdge.RoutingPoints != null)
                    routedEdge.RoutingPoints = routePoints.ToArray().ToGraphX();

                if (RootArea.EdgeCurvingEnabled)
                {
                    var oPolyLineSegment = GeometryHelper.GetCurveThroughPoints(routePoints.ToArray(), 0.5, RootArea.EdgeCurvingTolerance);

                    if (hasEpTarget)
                    {
                        UpdateTargetEpData(oPolyLineSegment.Points[oPolyLineSegment.Points.Count - 1], oPolyLineSegment.Points[oPolyLineSegment.Points.Count - 2]);
                        oPolyLineSegment.Points.RemoveAt(oPolyLineSegment.Points.Count - 1);
                    }
                    if (hasEpSource)
                    {
                        UpdateSourceEpData(oPolyLineSegment.Points.First(), oPolyLineSegment.Points[1]);
                        oPolyLineSegment.Points.RemoveAt(0);
                    }

                    lineFigure = GeometryHelper.GetPathFigureFromPathSegments(routePoints[0], true, true, oPolyLineSegment);
                }
                else
                {
                    if (hasEpSource)
                        routePoints[0] = routePoints[0].Subtract(UpdateSourceEpData(routePoints.First(), routePoints[1]));
                    if (hasEpTarget)
                        routePoints[routePoints.Count - 1] = routePoints[routePoints.Count - 1].Subtract(UpdateTargetEpData(p2, routePoints[routePoints.Count - 2]));

                    if (gEdge.ReversePath)
                        routePoints.Reverse();

                    var pcol = new PointCollection();
                    routePoints.ForEach(a => pcol.Add(a));

                    lineFigure = new PathFigure { StartPoint = p1, Segments = new PathSegmentCollection { new PolyLineSegment { Points = pcol } }, IsClosed = false };
                }
            }
            else
            {
                bool allowUpdateEpDataToUnsuppress = true;
                if (HideEdgePointerByEdgeLength != 0d)
                {
                    if (MathHelper.GetDistanceBetweenPoints(p1, p2) <= HideEdgePointerByEdgeLength)
                    {
                        EdgePointerForSource?.Suppress();
                        EdgePointerForTarget?.Suppress();
                        allowUpdateEpDataToUnsuppress = false;
                    }
                    else
                    {
                        EdgePointerForSource?.UnSuppress();
                        EdgePointerForTarget?.UnSuppress();
                    }
                }

                if (hasEpSource)
                    p1 = p1.Subtract(UpdateSourceEpData(p1, p2, allowUpdateEpDataToUnsuppress));
                if (hasEpTarget)
                    p2 = p2.Subtract(UpdateTargetEpData(p2, p1, allowUpdateEpDataToUnsuppress));

                lineFigure = new PathFigure { StartPoint = gEdge.ReversePath ? p2 : p1, Segments = new PathSegmentCollection { new LineSegment { Point = gEdge.ReversePath ? p1 : p2 } }, IsClosed = false };
            }
            ((PathGeometry)Linegeometry).Figures.Add(lineFigure);

            if (_updateLabelPosition && updateLabel)
                EdgeLabelControls.Where(l => l.ShowLabel).ForEach(l => l.UpdatePosition());
        }

        private Point UpdateSourceEpData(Point from, Point to, bool allowUnsuppress = true)
        {
            var dir = MathHelper.GetDirection(from, to);
            if (from == to)
            {
                if (HideEdgePointerOnVertexOverlap) EdgePointerForSource.Suppress();
                else dir = new Vector(0, 0);
            }
            else if (allowUnsuppress) EdgePointerForSource.UnSuppress();
            var result = EdgePointerForSource.Update(from, dir, EdgePointerForSource.NeedRotation ? -MathHelper.GetAngleBetweenPoints(from, to).ToDegrees() : 0);
            return EdgePointerForSource.IsVisible ? result : new Point();
        }

        private Point UpdateTargetEpData(Point from, Point to, bool allowUnsuppress = true)
        {
            var dir = MathHelper.GetDirection(from, to);
            if (from == to)
            {
                if (HideEdgePointerOnVertexOverlap) EdgePointerForTarget.Suppress();
                else dir = new Vector(0, 0);
            }
            else if (allowUnsuppress) EdgePointerForTarget.UnSuppress();
            var result = EdgePointerForTarget.Update(from, dir, EdgePointerForTarget.NeedRotation ? (-MathHelper.GetAngleBetweenPoints(from, to).ToDegrees()) : 0);
            return EdgePointerForTarget.IsVisible ? result : new Point();
        }

        #endregion public PrepareEdgePath()

        protected virtual object GetTemplatePart(string name)
        {
            return this.FindControl<Control>(name);
        }

        public virtual IList<Rect> GetLabelSizes()
        {
            return EdgeLabelControls.Select(l => l.GetSize()).ToList();
        }

        public IList<IEdgeLabelControl> GetLabelControls()
        {
            return EdgeLabelControls.ToList();
        }

        bool IGraphControl.IsVisible
        {
            get => this.IsVisible;
            set => this.IsVisible = value;
        }

        void IPositionChangeNotify.OnPositionChanged()
        {
        }
    }
}
