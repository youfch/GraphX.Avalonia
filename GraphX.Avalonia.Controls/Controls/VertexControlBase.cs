using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using GraphX.Common;
using GraphX.Common.Enums;
using GraphX.Controls.Models;
using Rect = GraphX.Measure.Rect;
using Point = Avalonia.Point;
using AvaloniaPoint = GraphX.Measure.Point;

namespace GraphX.Controls
{
    [TemplatePart(Name = "PART_vertexLabel", Type = typeof(IVertexLabelControl))]
    [TemplatePart(Name = "PART_vcproot", Type = typeof(Panel))]
    public abstract class VertexControlBase : TemplatedControl, IGraphControl
    {
        protected internal IVertexLabelControl? VertexLabelControl;
        
        public event EventHandler<EventArgs>? LabelAttached;

        protected void OnLabelAttached()
        {
            LabelAttached?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<EventArgs>? LabelDetached;

        protected void OnLabelDetached()
        {
            LabelDetached?.Invoke(this, EventArgs.Empty);
        }

        public event VertexPositionChangedEH? PositionChanged;

        protected void OnPositionChanged(Point offset, Point pos)
        {
            PositionChanged?.Invoke(this, new VertexPositionEventArgs(offset, pos, this));
        }

        protected VertexControlBase()
        {
            VertexConnectionPointsList = new List<IVertexConnectionPoint>();
        }

        public virtual void UpdatePositionTraceState()
        {
            if (EventOptions?.PositionChangeNotification == true)
            {
                if (!positionTraceEnabled)
                {
                    _xChangeMonitor = new ChangeMonitor();
                    _xChangeMonitor.Bind(this, GraphAreaBase.XProperty);
                    _xChangeMonitor.ChangeDetected += ChangeMonitor_ChangeDetected;
                    
                    _yChangeMonitor = new ChangeMonitor();
                    _yChangeMonitor.Bind(this, GraphAreaBase.YProperty);
                    _yChangeMonitor.ChangeDetected += ChangeMonitor_ChangeDetected;
                    
                    positionTraceEnabled = true;
                }
            }
            else
            {
                if (positionTraceEnabled)
                {
                    if (_xChangeMonitor != null)
                    {
                        _xChangeMonitor.ChangeDetected -= ChangeMonitor_ChangeDetected;
                        _xChangeMonitor.Unbind();
                        _xChangeMonitor = null;
                    }
                    if (_yChangeMonitor != null)
                    {
                        _yChangeMonitor.ChangeDetected -= ChangeMonitor_ChangeDetected;
                        _yChangeMonitor.Unbind();
                        _yChangeMonitor = null;
                    }
                    positionTraceEnabled = false;
                }
            }
        }

        private bool positionTraceEnabled;
        private ChangeMonitor? _xChangeMonitor;
        private ChangeMonitor? _yChangeMonitor;

        private void ChangeMonitor_ChangeDetected(object? source, EventArgs args)
        {
            if (ShowLabel && VertexLabelControl != null)
                VertexLabelControl.UpdatePosition();
            OnPositionChanged(new Point(), GetPosition());
        }

        public Panel? VCPRoot { get; protected set; }

        public void HideWithEdges()
        {
            IsVisible = false;
            SetConnectionPointsVisibility(false);
            RootArea?.GetRelatedControls(this, GraphControlType.Edge, EdgesType.All).ForEach(a =>
            {
                a.IsVisible = false;
            });
        }

        public void ShowWithEdges()
        {
            IsVisible = true;
            SetConnectionPointsVisibility(true);
            RootArea?.GetRelatedControls(this, GraphControlType.Edge, EdgesType.All).ForEach(a =>
            {
                a.IsVisible = true;
            });
        }

        public List<IVertexConnectionPoint> VertexConnectionPointsList { get; protected set; }

        public VertexEventOptions? EventOptions { get; protected set; }

        private double _labelAngle;
        public double LabelAngle
        {
            get
            {
                return VertexLabelControl?.Angle ?? _labelAngle;
            }
            set
            {
                _labelAngle = value;
                if (VertexLabelControl != null) 
                    VertexLabelControl.Angle = _labelAngle;
            }
        }

        public static readonly StyledProperty<VertexShape> VertexShapeProperty =
            AvaloniaProperty.Register<VertexControlBase, VertexShape>(nameof(VertexShape), VertexShape.Rectangle);

        public VertexShape VertexShape
        {
            get => GetValue(VertexShapeProperty);
            set => SetValue(VertexShapeProperty, value);
        }

        public static readonly StyledProperty<object?> VertexProperty =
            AvaloniaProperty.Register<VertexControlBase, object?>(nameof(Vertex));

        public object? Vertex
        {
            get => GetValue(VertexProperty);
            set => SetValue(VertexProperty, value);
        }

        public static readonly StyledProperty<GraphAreaBase?> RootAreaProperty =
            AvaloniaProperty.Register<VertexControlBase, GraphAreaBase?>(nameof(RootArea));

        public GraphAreaBase? RootArea
        {
            get => GetValue(RootAreaProperty);
            set => SetValue(RootAreaProperty, value);
        }

        public static readonly StyledProperty<bool> ShowLabelProperty =
            AvaloniaProperty.Register<VertexControlBase, bool>(nameof(ShowLabel), false);

        private static void ShowLabelChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var obj = d as VertexControlBase;
            if (obj.VertexLabelControl == null) return;
            if ((bool)e.NewValue!) obj.VertexLabelControl.Show(); else obj.VertexLabelControl.Hide();
        }

        public bool ShowLabel
        {
            get => GetValue(ShowLabelProperty);
            set => SetValue(ShowLabelProperty, value);
        }

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

        public abstract void Clean();

        public Point GetPosition(bool final = false, bool round = false)
        {
            return round ?
                new Point(final ? (int)GraphAreaBase.GetFinalX(this) : (int)GraphAreaBase.GetX(this), final ? (int)GraphAreaBase.GetFinalY(this) : (int)GraphAreaBase.GetY(this)) :
                new Point(final ? GraphAreaBase.GetFinalX(this) : GraphAreaBase.GetX(this), final ? GraphAreaBase.GetFinalY(this) : GraphAreaBase.GetY(this));
        }

        internal AvaloniaPoint GetPositionGraphX(bool final = false, bool round = false)
        {
            return round ? new AvaloniaPoint(final ? (int)GraphAreaBase.GetFinalX(this) : (int)GraphAreaBase.GetX(this), final ? (int)GraphAreaBase.GetFinalY(this) : (int)GraphAreaBase.GetY(this)) : new AvaloniaPoint(final ? GraphAreaBase.GetFinalX(this) : GraphAreaBase.GetX(this), final ? GraphAreaBase.GetFinalY(this) : GraphAreaBase.GetY(this));
        }

        public Point GetCenterPosition(bool final = false)
        {
            var pos = GetPosition();
            return new Point(pos.X + Bounds.Width * .5, pos.Y + Bounds.Height * .5);
        }

        public IVertexConnectionPoint? GetConnectionPointById(int id, bool runUpdate = false)
        {
            var result = VertexConnectionPointsList.FirstOrDefault(a => a.Id == id);
            if (runUpdate && result != null)
                result.Update();
            return result;
        }

        public IVertexConnectionPoint? GetConnectionPointAt(Point position)
        {
            InvalidateMeasure();

            return VertexConnectionPointsList.FirstOrDefault(a =>
            {
                var rect = new Rect(a.RectangularSize.X, a.RectangularSize.Y, a.RectangularSize.Width, a.RectangularSize.Height);
                return rect.Contains(position.ToGraphX());
            });
        }

        public void AttachLabel(IVertexLabelControl ctrl)
        {
            VertexLabelControl = ctrl;
            OnLabelAttached();
        }

        public void DetachLabel()
        {
            if(VertexLabelControl is IAttachableControl control)
                control.Detach();
            VertexLabelControl = null;
            OnLabelDetached();
        }

        public void SetConnectionPointsVisibility(bool isVisible)
        {
            foreach (var item in VertexConnectionPointsList)
            {
                if (isVisible) item.Show(); else item.Hide();
            }
        }

        void IPositionChangeNotify.OnPositionChanged()
        {
            if (ShowLabel && VertexLabelControl != null)
                VertexLabelControl.UpdatePosition();
            OnPositionChanged(new Point(), GetPosition());
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            VertexLabelControl = VertexLabelControl ?? FindDescendant<IVertexLabelControl>("PART_vertexLabel");
            var vcpRoot = FindDescendant<Panel>("PART_vcproot");
            if (vcpRoot != null)
                VCPRoot = vcpRoot;

            if (VertexLabelControl != null)
            {
                if (ShowLabel) VertexLabelControl.Show(); else VertexLabelControl.Hide();
                InvalidateArrange();
                VertexLabelControl.UpdatePosition();
            }

            VertexConnectionPointsList = this.FindDescendantsOfType<IVertexConnectionPoint>().ToList();
        }

        protected T? FindDescendant<T>(string name) where T : class
        {
            return this.FindControl<T>(name);
        }

        protected internal virtual void UpdateEventHandling(EventType type)
        {
            switch (type)
            {
                case EventType.MouseClick:
                    if (EventOptions?.MouseClickEnabled == true)
                        PointerPressed += VertexControl_PointerPressed;
                    else
                        PointerPressed -= VertexControl_PointerPressed;
                    break;

                case EventType.MouseMove:
                    if (EventOptions?.MouseMoveEnabled == true)
                        PointerMoved += VertexControl_PointerMoved;
                    else
                        PointerMoved -= VertexControl_PointerMoved;
                    break;

                case EventType.MouseEnter:
                    if (EventOptions?.MouseEnterEnabled == true)
                        PointerEntered += VertexControl_PointerEntered;
                    else
                        PointerEntered -= VertexControl_PointerEntered;
                    break;

                case EventType.MouseLeave:
                    if (EventOptions?.MouseLeaveEnabled == true)
                        PointerExited += VertexControl_PointerExited;
                    else
                        PointerExited -= VertexControl_PointerExited;
                    break;
            }
        }

        private bool _clickTrack;
        private Point _clickTrackPoint;

        private void VertexControl_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (RootArea != null && IsVisible)
            {
                RootArea.OnVertexSelected(this, e, e.KeyModifiers);
                _clickTrack = true;
                _clickTrackPoint = e.GetCurrentPoint(RootArea).Position;
            }
            e.Handled = true;
        }

        private void VertexControl_PointerMoved(object? sender, PointerEventArgs e)
        {
            if (!_clickTrack)
                return;

            if (RootArea != null)
            {
                var curPoint = e.GetCurrentPoint(RootArea).Position;
                if (curPoint != _clickTrackPoint)
                    _clickTrack = false;
            }
        }

        private void VertexControl_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (RootArea != null && IsVisible)
            {
                RootArea.OnVertexMouseUp(this, e, e.KeyModifiers);
                if (_clickTrack)
                {
                    OnClick();
                    var keys = e.GetCurrentPoint(null).KeyModifiers;
                    RootArea.OnVertexClicked(this, e, keys);
                }
            }
            _clickTrack = false;
            e.Handled = true;
        }

        private void VertexControl_PointerEntered(object? sender, PointerEventArgs e)
        {
            if (RootArea != null && IsVisible)
                RootArea.OnVertexMouseEnter(this, e);
        }

        private void VertexControl_PointerExited(object? sender, PointerEventArgs e)
        {
            if (RootArea != null && IsVisible)
                RootArea.OnVertexMouseLeave(this, e);
        }

        protected virtual void OnClick()
        {
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            VertexControl_PointerPressed(this, e);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);
            VertexControl_PointerReleased(this, e);
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);
            VertexControl_PointerMoved(this, e);
        }

        protected override void OnPointerEnter(PointerEventArgs e)
        {
            base.OnPointerEnter(e);
            VertexControl_PointerEntered(this, e);
        }

        protected override void OnPointerLeave(PointerEventArgs e)
        {
            base.OnPointerLeave(e);
            VertexControl_PointerExited(this, e);
        }

        private bool _isVisible = true;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                IsEffectivelyVisible = value;
            }
        }

        private class ChangeMonitor : AvaloniaObject
        {
            public void Bind(AvaloniaObject el, AvaloniaProperty property)
            {
                var binding = new Binding
                {
                    Path = $"({property.Name})",
                    Source = el
                };
                Bind(ChangeMonitorProperty, binding);
            }

            public void Unbind()
            {
                Bind(ChangeMonitorProperty, (object?)null);
            }

            public event EventHandler? ChangeDetected;

            public static readonly StyledProperty<object?> ChangeMonitorProperty =
                AvaloniaProperty.Register<ChangeMonitor, object?>(nameof(ChangeMonitor));

            public object? ChangeMonitor
            {
                get => GetValue(ChangeMonitorProperty);
                set => SetValue(ChangeMonitorProperty, value);
            }

            static ChangeMonitor()
            {
                ChangeMonitorProperty.Changed.AddClassHandler<ChangeMonitor>((cm, e) =>
                {
                    cm.ChangeDetected?.Invoke(cm, EventArgs.Empty);
                });
            }
        }
    }
}
