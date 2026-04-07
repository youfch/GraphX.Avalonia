using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using GraphX.Controls.Models;
using GraphX.Common;
using GraphX.Common.Enums;
using GraphX.Common.Interfaces;

namespace GraphX.Controls
{
    public class EdgeControl : EdgeControlBase
    {
        #region Styled Properties

        public static readonly StyledProperty<double> StrokeThicknessProperty =
            AvaloniaProperty.Register<EdgeControl, double>(nameof(StrokeThickness), 5.0);

        public double StrokeThickness
        {
            get => GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        private static readonly StyledProperty<bool> IsSelfLoopedProperty =
            AvaloniaProperty.Register<EdgeControl, bool>(nameof(IsSelfLooped), false);

        private bool IsSelfLoopedInternal => Source != null && Target != null && Source.Vertex == Target.Vertex;

        public override bool IsSelfLooped
        {
            get => GetValue(IsSelfLoopedProperty);
            protected set => SetValue(IsSelfLoopedProperty, value);
        }

        #endregion

        #region Clean()

        public override void Clean()
        {
            Source = null;
            Target = null;
            Edge = null;
            RootArea = null;
            HighlightBehaviour.SetIsHighlightEnabled(this, false);
            DragBehaviour.SetIsDragEnabled(this, false);
            Linegeometry = null;
            LinePathObject = null;
            SelfLoopIndicator = null;
            EdgeLabelControls.ForEach(l => l.Dispose());
            EdgeLabelControls.Clear();

            if (EdgePointerForSource != null)
            {
                EdgePointerForSource.Dispose();
                EdgePointerForSource = null;
            }
            if (EdgePointerForTarget != null)
            {
                EdgePointerForTarget.Dispose();
                EdgePointerForTarget = null;
            }
            EventOptions?.Clean();
        }

        #endregion

        #region Vertex position tracing

        private bool _sourceTrace;
        private bool _targetTrace;
        private VertexControl _oldSource;
        private VertexControl _oldTarget;

        protected override void OnSourceChanged(object d, AvaloniaPropertyChangedEventArgs e)
        {
            SourceChanged();
        }

        protected override void OnTargetChanged(object d, AvaloniaPropertyChangedEventArgs e)
        {
            TargetChanged();
        }

        private void SourceChanged()
        {
            if (EventOptions == null)
                return;

            if (_oldSource != null)
            {
                _oldSource.PositionChanged -= source_PositionChanged;
                _oldSource.SizeChanged -= Source_SizeChanged;
            }
            _oldSource = Source;
            if (Source != null)
            {
                Source.PositionChanged += source_PositionChanged;
                Source.SizeChanged += Source_SizeChanged;
            }
            IsSelfLooped = IsSelfLoopedInternal;
            UpdateSelfLoopedEdgeData();
        }

        private void TargetChanged()
        {
            if (EventOptions == null)
                return;

            if (_oldTarget != null)
            {
                _oldTarget.PositionChanged -= source_PositionChanged;
                _oldTarget.SizeChanged -= Source_SizeChanged;
            }
            _oldTarget = Target;
            if (Target != null)
            {
                Target.PositionChanged += source_PositionChanged;
                Target.SizeChanged += Source_SizeChanged;
            }
            IsSelfLooped = IsSelfLoopedInternal;
            UpdateSelfLoopedEdgeData();
        }

        private void source_PositionChanged(object sender, EventArgs e)
        {
            UpdateEdge();
        }

        void Source_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateEdge();
        }

        #endregion

        #region Event handling

        internal void UpdateEventhandling(EventType typ)
        {
            switch (typ)
            {
                case EventType.MouseClick:
                    if (EventOptions.MouseClickEnabled)
                        AddHandler(PointerPressedEvent, EdgeControl_PointerPressed);
                    else
                        RemoveHandler(PointerPressedEvent, EdgeControl_PointerPressed);
                    break;
                case EventType.MouseDoubleClick:
                    if (EventOptions.MouseDoubleClickEnabled)
                        AddHandler(PointerPressedEvent, EdgeControl_DoubleClick);
                    else
                        RemoveHandler(PointerPressedEvent, EdgeControl_DoubleClick);
                    break;
                case EventType.MouseEnter:
                    if (EventOptions.MouseEnterEnabled)
                        AddHandler(PointerEnteredEvent, EdgeControl_PointerEntered);
                    else
                        RemoveHandler(PointerEnteredEvent, EdgeControl_PointerEntered);
                    break;
                case EventType.MouseLeave:
                    if (EventOptions.MouseLeaveEnabled)
                        AddHandler(PointerExitedEvent, EdgeControl_PointerExited);
                    else
                        RemoveHandler(PointerExitedEvent, EdgeControl_PointerExited);
                    break;
                case EventType.MouseMove:
                    if (EventOptions.MouseMoveEnabled)
                        AddHandler(PointerMovedEvent, EdgeControl_PointerMoved);
                    else
                        RemoveHandler(PointerMovedEvent, EdgeControl_PointerMoved);
                    break;
            }
        }

        #endregion

        #region Constructor

        public EdgeControl()
            : this(null, null, null)
        {
        }

        public EdgeControl(VertexControl source, VertexControl target, object edge, bool showArrows = true)
        {
            DataContext = edge;
            Source = source;
            Target = target;
            Edge = edge;
            DataContext = edge;
            this.SetCurrentValue(ShowArrowsProperty, showArrows);
            IsHiddenEdgesUpdated = true;

            if (!this.IsInDesignMode())
            {
                EventOptions = new EdgeEventOptions(this);
                foreach (var item in Enum.GetValues(typeof(EventType)).Cast<EventType>())
                    UpdateEventhandling(item);

                SourceChanged();
                TargetChanged();
            }

            IsSelfLooped = IsSelfLoopedInternal;
        }

        #endregion

        #region Event handlers

        private bool _clickTrack;
        private Point _clickTrackPoint;

        private void EdgeControl_PointerMoved(object? sender, PointerEventArgs e)
        {
            if (!_clickTrack)
                return;

            var curPoint = RootArea != null ? e.GetCurrentPoint(RootArea).Position : new Point();

            if (curPoint != _clickTrackPoint)
                _clickTrack = false;
        }

        private void EdgeControl_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (RootArea != null && IsVisible)
            {
                if (_clickTrack)
                {
                    OnClick();
                    RootArea.OnEdgeClicked(this, e, e.KeyModifiers);
                }
            }
            _clickTrack = false;
            e.Handled = true;
        }

        private void EdgeControl_PointerEntered(object? sender, PointerEventArgs e)
        {
            if (RootArea != null && IsVisible)
                RootArea.OnEdgeMouseEnter(this, e, e.KeyModifiers);
        }

        private void EdgeControl_PointerExited(object? sender, PointerEventArgs e)
        {
            if (RootArea != null && IsVisible)
                RootArea.OnEdgeMouseLeave(this, e, e.KeyModifiers);
        }

        private void EdgeControl_PointerMoved(object? sender, PointerEventArgs e)
        {
            if (RootArea != null && IsVisible)
                RootArea.OnEdgeMouseMove(this, e, e.KeyModifiers);
        }

        private void EdgeControl_DoubleClick(object? sender, PointerPressedEventArgs e)
        {
            if (RootArea != null && IsVisible)
                RootArea.OnEdgeDoubleClick(this, e, e.KeyModifiers);
        }

        private void EdgeControl_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (RootArea != null && IsVisible)
                RootArea.OnEdgeSelected(this, e, e.KeyModifiers);
            _clickTrack = true;
            _clickTrackPoint = RootArea != null ? e.GetCurrentPoint(RootArea).Position : new Point();
            e.Handled = true;
        }

        protected virtual void OnClick()
        {
            RaiseEvent(new RoutedEventArgs(ClickEvent));
        }

        #endregion

        #region Click Event

        public static readonly RoutedEvent ClickEvent =
            RoutedEvent.Register<EdgeControl, RoutedEventArgs>(nameof(Click), RoutingStrategies.Bubble);

        public event EventHandler<RoutedEventArgs> Click
        {
            add => AddHandler(ClickEvent, value);
            remove => RemoveHandler(ClickEvent, value);
        }

        #endregion

        #region Dispose

        public override void Dispose()
        {
            Clean();
        }

        #endregion

        #region GetDataEdge<T>()

        public T GetDataEdge<T>() where T : IGraphXCommonEdge
        {
            return (T)Edge;
        }

        #endregion

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            EdgeControl_PointerPressed(this, e);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);
            EdgeControl_PointerReleased(this, e);
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);
            EdgeControl_PointerMoved(this, e);
        }

        protected override void OnPointerEnter(PointerEventArgs e)
        {
            base.OnPointerEnter(e);
            EdgeControl_PointerEntered(this, e);
        }

        protected override void OnPointerLeave(PointerEventArgs e)
        {
            base.OnPointerLeave(e);
            EdgeControl_PointerExited(this, e);
        }
    }
}
