using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using GraphX.Common.Enums;
using GraphX.Common.Interfaces;
using GraphX.Controls;
using GraphX.Controls.Animations;
using GraphX.Controls.Models;

namespace GraphX
{
    public abstract class GraphAreaBase : Canvas, ITrackableContent, IGraphAreaBase
    {
        protected bool IsInPrintMode;

        public abstract void SetPrintMode(bool value, bool offsetControls = true, int margin = 0);

        public bool AutoAssignMissingDataId { get; set; } = true;

        public LogicCoreChangedAction LogicCoreChangeAction
        {
            get => GetValue(LogicCoreChangeActionProperty);
            set => SetValue(LogicCoreChangeActionProperty, value);
        }

        public static readonly StyledProperty<LogicCoreChangedAction> LogicCoreChangeActionProperty =
            AvaloniaProperty.Register<GraphAreaBase, LogicCoreChangedAction>(nameof(LogicCoreChangeAction), LogicCoreChangedAction.None);

        protected GraphAreaBase()
        {
            LogicCoreChangeAction = LogicCoreChangedAction.None;
        }

        #region Attached Properties

        public static readonly AttachedProperty<double> XProperty =
            AvaloniaProperty.RegisterAttached<GraphAreaBase, Control, double>("X", double.NaN,
                inherited: true,
                defaultBindingMode: BindingMode.TwoWay,
                validate: null,
                XChanged);

        private static void XChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs<double> e)
        {
            d.SetValue(LeftProperty, e.NewValue);
        }

        public static readonly AttachedProperty<double> FinalXProperty =
            AvaloniaProperty.RegisterAttached<GraphAreaBase, Control, double>("FinalX", double.NaN);

        public static readonly AttachedProperty<double> FinalYProperty =
            AvaloniaProperty.RegisterAttached<GraphAreaBase, Control, double>("FinalY", double.NaN);

        public static readonly AttachedProperty<double> YProperty =
            AvaloniaProperty.RegisterAttached<GraphAreaBase, Control, double>("Y", double.NaN,
                inherited: true,
                defaultBindingMode: BindingMode.TwoWay,
                validate: null,
                YChanged);

        private static void YChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs<double> e)
        {
            d.SetValue(TopProperty, e.NewValue);
        }

        public static double GetX(AvaloniaObject obj)
        {
            return obj.GetValue(XProperty);
        }

        public static void SetX(AvaloniaObject obj, double value, bool alsoSetFinal = true)
        {
            obj.SetValue(XProperty, value);
            if (alsoSetFinal)
                obj.SetValue(FinalXProperty, value);
        }

        public static double GetY(AvaloniaObject obj)
        {
            return obj.GetValue(YProperty);
        }

        public static void SetY(AvaloniaObject obj, double value, bool alsoSetFinal = false)
        {
            obj.SetValue(YProperty, value);
            if (alsoSetFinal)
                obj.SetValue(FinalYProperty, value);
        }

        public static double GetFinalX(AvaloniaObject obj)
        {
            return obj.GetValue(FinalXProperty);
        }

        public static void SetFinalX(AvaloniaObject obj, double value)
        {
            obj.SetValue(FinalXProperty, value);
        }

        public static double GetFinalY(AvaloniaObject obj)
        {
            return obj.GetValue(FinalYProperty);
        }

        public static void SetFinalY(AvaloniaObject obj, double value)
        {
            obj.SetValue(FinalYProperty, value);
        }

        public static bool GetPositioningComplete(AvaloniaObject obj)
        {
            return obj.GetValue(PositioningCompleteProperty);
        }

        public static void SetPositioningComplete(AvaloniaObject obj, bool value)
        {
            obj.SetValue(PositioningCompleteProperty, value);
        }

        #region Property - ExternalSettings
        public static readonly StyledProperty<object?> ExternalSettingsProperty =
            AvaloniaProperty.Register<GraphAreaBase, object?>(nameof(ExternalSettingsOnly), null);

        public object? ExternalSettings
        {
            get => GetValue(ExternalSettingsProperty);
            set => SetValue(ExternalSettingsProperty, value);
        }
        #endregion

        #region Property - Animations

        public MoveAnimationBase? MoveAnimation
        {
            get => GetValue(MoveAnimationProperty);
            set => SetValue(MoveAnimationProperty, value);
        }

        public static readonly StyledProperty<MoveAnimationBase?> MoveAnimationProperty =
            AvaloniaProperty.Register<GraphAreaBase, MoveAnimationBase?>(nameof(MoveAnimation), null);

        public IOneWayControlAnimation? DeleteAnimation
        {
            get => GetValue(DeleteAnimationProperty);
            set => SetValue(DeleteAnimationProperty, value);
        }

        public static readonly StyledProperty<IOneWayControlAnimation?> DeleteAnimationProperty =
            AvaloniaProperty.Register<GraphAreaBase, IOneWayControlAnimation?>(nameof(DeleteAnimation), null,
                coerce: CoerceDeleteAnimation);

        private static IOneWayControlAnimation? CoerceDeleteAnimation(AvaloniaObject instance, IOneWayControlAnimation? value)
        {
            if (instance is GraphAreaBase area)
            {
                var oldAnimation = area.GetValue(DeleteAnimationProperty);
                if (oldAnimation != null)
                {
                    oldAnimation.Completed -= area.GraphAreaBase_Completed;
                }
                if (value != null)
                {
                    value.Completed += area.GraphAreaBase_Completed;
                }
            }
            return value;
        }

        private void GraphAreaBase_Completed(object? sender, ControlEventArgs e)
        {
            e.Control?.RootArea?.RemoveAnimatedControl(e.Control, e.RemoveDataObject);
        }

        protected abstract void RemoveAnimatedControl(IGraphControl ctrl, bool removeDataObject);

        public IBidirectionalControlAnimation? MouseOverAnimation
        {
            get => GetValue(MouseOverAnimationProperty);
            set => SetValue(MouseOverAnimationProperty, value);
        }

        public static readonly StyledProperty<IBidirectionalControlAnimation?> MouseOverAnimationProperty =
            AvaloniaProperty.Register<GraphAreaBase, IBidirectionalControlAnimation?>(nameof(MouseOverAnimation), null);

        #endregion

        public static readonly AttachedProperty<bool> PositioningCompleteProperty =
            AvaloniaProperty.RegisterAttached<GraphAreaBase, Control, bool>("PositioningComplete", true);

        #endregion

        #region Child EVENTS

        internal static readonly Size DesignSize = new Size(70, 25);

        public event ContentSizeChangedEventHandler? ContentSizeChanged;

        protected void OnContentSizeChanged(Rect oldSize, Rect newSize)
        {
            ContentSizeChanged?.Invoke(this, new ContentSizeChangedEventArgs(oldSize, newSize));
        }

        public event VertexSelectedEventHandler? VertexDoubleClick;

        internal virtual void OnVertexDoubleClick(VertexControl vc, PointerPressedEventArgs e)
        {
            VertexDoubleClick?.Invoke(this, new VertexSelectedEventArgs(vc, e, KeyModifiers.None));
        }

        public event VertexSelectedEventHandler? VertexSelected;

        internal virtual void OnVertexSelected(VertexControl vc, PointerPressedEventArgs e, KeyModifiers keys)
        {
            VertexSelected?.Invoke(this, new VertexSelectedEventArgs(vc, e, keys));
        }

        public event VertexSelectedEventHandler? VertexMouseUp;

        internal virtual void OnVertexMouseUp(VertexControl vc, PointerReleasedEventArgs e, KeyModifiers keys)
        {
            VertexMouseUp?.Invoke(this, new VertexSelectedEventArgs(vc, null, keys));
        }

        public event VertexSelectedEventHandler? VertexMouseEnter;

        internal virtual void OnVertexMouseEnter(VertexControl vc, PointerEventArgs e)
        {
            VertexMouseEnter?.Invoke(this, new VertexSelectedEventArgs(vc, null, KeyModifiers.None));
            MouseOverAnimation?.AnimateVertexForward(vc);
        }

        public event VertexMovedEventHandler? VertexMouseMove;

        internal virtual void OnVertexMouseMove(VertexControl vc, PointerEventArgs e)
        {
            VertexMouseMove?.Invoke(this, new VertexMovedEventArgs(vc));
        }

        public event VertexSelectedEventHandler? VertexMouseLeave;

        internal virtual void OnVertexMouseLeave(VertexControl vc, PointerEventArgs e)
        {
            VertexMouseLeave?.Invoke(this, new VertexSelectedEventArgs(vc, null, KeyModifiers.None));
            MouseOverAnimation?.AnimateVertexBackward(vc);
        }

        public event VertexSelectedEventHandler? VertexClicked;

        internal virtual void OnVertexClicked(VertexControl vc, PointerReleasedEventArgs e, KeyModifiers keys)
        {
            VertexClicked?.Invoke(this, new VertexSelectedEventArgs(vc, e, keys));
        }

        internal virtual void RaiseVertexClicked(VertexControl vc, PointerReleasedEventArgs e, KeyModifiers keys)
        {
            OnVertexClicked(vc, e, keys);
        }

        public event EventHandler? LayoutCalculationFinished;

        protected virtual void OnLayoutCalculationFinished()
        {
            LayoutCalculationFinished?.Invoke(this, null);
        }

        public event EventHandler? OverlapRemovalCalculationFinished;

        protected virtual void OnOverlapRemovalCalculationFinished()
        {
            OverlapRemovalCalculationFinished?.Invoke(this, null);
        }

        public event EventHandler? EdgeRoutingCalculationFinished;

        protected virtual void OnEdgeRoutingCalculationFinished()
        {
            EdgeRoutingCalculationFinished?.Invoke(this, null);
        }

        public event EventHandler? RelayoutFinished;

        protected virtual void OnRelayoutFinished()
        {
            RelayoutFinished?.Invoke(this, null);
        }

        public event EventHandler? GenerateGraphFinished;

        protected virtual void OnGenerateGraphFinished()
        {
            GenerateGraphFinished?.Invoke(this, null);
        }

        public event EdgeSelectedEventHandler? EdgeSelected;

        internal virtual void OnEdgeSelected(EdgeControl ec, PointerPressedEventArgs e, KeyModifiers keys)
        {
            EdgeSelected?.Invoke(this, new EdgeSelectedEventArgs(ec, e, keys));
        }

        public event EdgeSelectedEventHandler? EdgeDoubleClick;
        internal void OnEdgeDoubleClick(EdgeControl edgeControl, PointerPressedEventArgs e, KeyModifiers keys)
        {
            EdgeDoubleClick?.Invoke(this, new EdgeSelectedEventArgs(edgeControl, e, keys));
        }

        public event EdgeSelectedEventHandler? EdgeMouseMove;
        internal void OnEdgeMouseMove(EdgeControl edgeControl, PointerPressedEventArgs e, KeyModifiers keys)
        {
            EdgeMouseMove?.Invoke(this, new EdgeSelectedEventArgs(edgeControl, null, keys));
        }

        public event EdgeSelectedEventHandler? EdgeMouseEnter;
        internal void OnEdgeMouseEnter(EdgeControl edgeControl, PointerPressedEventArgs e, KeyModifiers keys)
        {
            EdgeMouseEnter?.Invoke(this, new EdgeSelectedEventArgs(edgeControl, null, keys));
            MouseOverAnimation?.AnimateEdgeForward(edgeControl);
        }

        public event EdgeSelectedEventHandler? EdgeMouseLeave;
        internal void OnEdgeMouseLeave(EdgeControl edgeControl, PointerPressedEventArgs e, KeyModifiers keys)
        {
            EdgeMouseLeave?.Invoke(this, new EdgeSelectedEventArgs(edgeControl, null, keys));
            MouseOverAnimation?.AnimateEdgeBackward(edgeControl);
        }

        #endregion

        #region ComputeEdgeRoutesByVertex()

        internal virtual void ComputeEdgeRoutesByVertex(VertexControl vc, bool vertexDataNeedUpdate = true) { }
        #endregion

        #region Virtual members

        public abstract VertexControl[] GetAllVertexControls();

        public abstract VertexControl? GetVertexControlAt(Point position);

        public abstract void RelayoutGraph(bool generateAllEdges = false);

        internal abstract bool IsEdgeRoutingEnabled { get; }
        internal abstract bool EnableParallelEdges { get; }
        internal abstract bool EdgeCurvingEnabled { get; }
        internal abstract double EdgeCurvingTolerance { get; }

        public abstract List<IGraphControl> GetRelatedControls(IGraphControl ctrl, GraphControlType resultType = GraphControlType.VertexAndEdge, EdgesType edgesType = EdgesType.Out);

        public abstract List<IGraphControl> GetRelatedVertexControls(IGraphControl ctrl, EdgesType edgesType = EdgesType.All);

        public abstract List<IGraphControl> GetRelatedEdgeControls(IGraphControl ctrl, EdgesType edgesType = EdgesType.All);

        public abstract void GenerateEdgesForVertex(VertexControl vc, EdgesType edgeType, Visibility defaultVisibility = Visibility.Visible);

        #endregion

        #region Measure & Arrange

        private Point _topLeft;

        private Point _bottomRight;

        public Rect ContentSize => new Rect(_topLeft, _bottomRight);

        public Size SideExpansionSize { get; set; }

        private const bool COUNT_ROUTE_PATHS = true;

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var minPoint = new Point(double.PositiveInfinity, double.PositiveInfinity);
            var maxPoint = new Point(double.NegativeInfinity, double.NegativeInfinity);

            foreach (var child in Children)
            {
                var x = GetX(child);
                var y = GetY(child);

                if (double.IsNaN(x) || double.IsNaN(y))
                {
                    var ec = child as EdgeControl;
                    if (ec != null)
                    {
                        x = 0;
                        y = 0;
                    }
                    else continue;

                    if (COUNT_ROUTE_PATHS && ec != null)
                    {
                        var routingInfo = ec.Edge as IRoutingInfo;
                        var rps = routingInfo?.RoutingPoints;
                        if (rps != null)
                        {
                            foreach (var item in rps)
                            {
                                minPoint = new Point(Math.Min(minPoint.X, item.X), Math.Min(minPoint.Y, item.Y));
                                maxPoint = new Point(Math.Max(maxPoint.X, item.X), Math.Max(maxPoint.Y, item.Y));
                            }
                        }
                    }
                }
                else
                {
                    minPoint = new Point(Math.Min(minPoint.X, x), Math.Min(minPoint.Y, y));
                    maxPoint = new Point(Math.Max(maxPoint.X, x), Math.Max(maxPoint.Y, y));
                }

                child.Arrange(new Rect(x, y, child.DesiredSize.Width, child.DesiredSize.Height));
            }

            return new Size(10, 10);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var oldSize = ContentSize;
            _topLeft = new Point(double.PositiveInfinity, double.PositiveInfinity);
            _bottomRight = new Point(double.NegativeInfinity, double.NegativeInfinity);

            foreach (var child in Children)
            {
                child.Measure(constraint);

                var left = GetFinalX(child);
                var top = GetFinalY(child);

                if (child.Visibility == Visibility.Collapsed) continue;

                if (double.IsNaN(left) || double.IsNaN(top))
                {
                    var ec = child as EdgeControl;
                    if (!COUNT_ROUTE_PATHS || ec == null) continue;
                    var routingInfo = ec.Edge as IRoutingInfo;
                    if (routingInfo == null) continue;
                    var rps = routingInfo.RoutingPoints;
                    if (rps == null) continue;
                    foreach (var item in rps)
                    {
                        _topLeft.X = Math.Min(_topLeft.X, item.X);
                        _topLeft.Y = Math.Min(_topLeft.Y, item.Y);

                        _bottomRight.X = Math.Max(_bottomRight.X, item.X);
                        _bottomRight.Y = Math.Max(_bottomRight.Y, item.Y);
                    }
                }
                else
                {
                    _topLeft.X = Math.Min(_topLeft.X, left);
                    _topLeft.Y = Math.Min(_topLeft.Y, top);

                    _bottomRight.X = Math.Max(_bottomRight.X, left + child.DesiredSize.Width);
                    _bottomRight.Y = Math.Max(_bottomRight.Y, top + child.DesiredSize.Height);
                }
            }

            _topLeft.X -= SideExpansionSize.Width * .5;
            _topLeft.Y -= SideExpansionSize.Height * .5;
            _bottomRight.X += SideExpansionSize.Width * .5;
            _bottomRight.Y += SideExpansionSize.Height * .5;

            var newSize = ContentSize;
            if (oldSize != newSize)
                OnContentSizeChanged(oldSize, newSize);

            return new Size(10, 10);
        }
        #endregion
    }
}
