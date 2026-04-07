using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using GraphX.Common.Enums;
using GraphX.Common.Exceptions;
using GraphX.Controls.Models;
using GraphX.Common.Interfaces;

namespace GraphX.Controls
{
    /// <summary>
    /// Visual vertex control
    /// </summary>
    [TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Pressed")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "PointerOver")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "PointerLeave")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Disabled")]
    [TemplatePart(Name = "PART_vertexLabel", Type = typeof(IVertexLabelControl))]
    [TemplatePart(Name = "PART_vcproot", Type = typeof(Panel))]
    public class VertexControl : VertexControlBase
    {
        static VertexControl()
        {
        }

        /// <summary>
        /// Create vertex visual control
        /// </summary>
        /// <param name="vertexData">Vertex data object</param>
        /// <param name="tracePositionChange">Listen for the vertex position changed events and fire corresponding event</param>
        /// <param name="bindToDataObject">Bind DataContext to the Vertex data. True by default. </param>
        public VertexControl(object vertexData, bool tracePositionChange = true, bool bindToDataObject = true)
        {
            if (bindToDataObject) DataContext = vertexData;
            Vertex = vertexData;

            EventOptions = new VertexEventOptions(this) { PositionChangeNotification = tracePositionChange };
            foreach (var item in Enum.GetValues(typeof(EventType)).Cast<EventType>())
                UpdateEventHandling(item);
        }

        public T FindDescendant<T>(string name) where T : class
        {
            return (T)(object)this.FindControl<T>(name);
        }

        #region Click Event

        public static readonly RoutedEvent<RoutedEventArgs> ClickEvent =
            RoutedEvent.Register<RoutedEventArgs>(nameof(Click), RoutingStrategies.Bubble, typeof(VertexControl));

        public event EventHandler<RoutedEventArgs> Click
        {
            add => AddHandler(ClickEvent, value);
            remove => RemoveHandler(ClickEvent, value);
        }

        #endregion Click Event

        #region DoubleTapped Event

        public static readonly RoutedEvent<PointerPressedEventArgs> DoubleTappedEvent =
            RoutedEvent.Register<PointerPressedEventArgs>(nameof(DoubleTapped), RoutingStrategies.Bubble, typeof(VertexControl));

        public event EventHandler<PointerPressedEventArgs> DoubleTapped
        {
            add => AddHandler(DoubleTappedEvent, value);
            remove => RemoveHandler(DoubleTappedEvent, value);
        }

        #endregion DoubleTapped Event

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Template == null) return;
            VertexLabelControl = VertexLabelControl ?? FindDescendant<IVertexLabelControl>("PART_vertexLabel");

            VCPRoot = VCPRoot ?? FindDescendant<Panel>("PART_vcproot");

            if (VertexLabelControl != null)
            {
                if (ShowLabel) VertexLabelControl.Show(); else VertexLabelControl.Hide();
                InvalidateMeasure();
                VertexLabelControl.UpdatePosition();
            }

            VertexConnectionPointsList = this.FindDescendantsOfType<IVertexConnectionPoint>().ToList();
            if (VertexConnectionPointsList.GroupBy(x => x.Id).Count(group => group.Count() > 1) > 0)
                throw new GX_InvalidDataException("Vertex connection points in VertexControl template must have unique Id!");
        }

        #region Events handling

        private void VertexControl_PointerExited(object? sender, PointerEventArgs e)
        {
            if (RootArea != null && IsVisible)
                RootArea.OnVertexMouseLeave(this, e);
            VisualStateManager.GoToState(this, "PointerLeave", true);
        }

        private void VertexControl_PointerEntered(object? sender, PointerEventArgs e)
        {
            if (RootArea != null && IsVisible)
                RootArea.OnVertexMouseEnter(this, e);
            VisualStateManager.GoToState(this, "PointerOver", true);
        }

        private void VertexControl_PointerMoved(object? sender, PointerEventArgs e)
        {
            if (RootArea != null)
                RootArea.OnVertexMouseMove(this, e);
        }

        private void VertexControl_DoubleTapped(object? sender, PointerPressedEventArgs e)
        {
            if (RootArea != null && IsVisible)
                RootArea.OnVertexDoubleClick(this, e);
        }

        #endregion Events handling

        protected override void UpdateEventHandling(EventType type)
        {
            switch (type)
            {
                case EventType.MouseClick:
                    if (EventOptions?.MouseClickEnabled == true)
                    {
                        PointerPressed += VertexControl_PointerPressed;
                        PointerMoved += VertexControl_TrackMove;
                    }
                    else
                    {
                        PointerPressed -= VertexControl_PointerPressed;
                        PointerMoved -= VertexControl_TrackMove;
                    }
                    break;

                case EventType.MouseDoubleClick:
                    if (EventOptions?.MouseDoubleClickEnabled == true)
                        DoubleTapped += VertexControl_DoubleTapped;
                    else
                        DoubleTapped -= VertexControl_DoubleTapped;
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

                case EventType.PositionChangeNotify:
                    UpdatePositionTraceState();
                    break;
            }

            PointerReleased -= VertexControl_PointerReleased;
            PointerReleased += VertexControl_PointerReleased;
        }

        private bool _clickTrack;
        private Point _clickTrackPoint;

        private void VertexControl_TrackMove(object? sender, PointerEventArgs e)
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
                    RaiseEvent(new RoutedEventArgs(ClickEvent));
                    RootArea.OnVertexClicked(this, e, e.KeyModifiers);
                }
            }
            _clickTrack = false;
            e.Handled = true;
        }

        private void VertexControl_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (RootArea != null && IsVisible)
                RootArea.OnVertexSelected(this, e, e.KeyModifiers);
            _clickTrack = true;
            _clickTrackPoint = RootArea != null ? e.GetCurrentPoint(RootArea).Position : new Point();
            e.Handled = true;
        }

        /// <summary>
        /// Cleans all potential memory-holding code
        /// </summary>
        public override void Clean()
        {
            Vertex = null;
            RootArea = null;
            HighlightBehaviour.SetIsHighlightEnabled(this, false);
            DragBehaviour.SetIsDragEnabled(this, false);
            VertexLabelControl = null;

            if (EventOptions != null)
            {
                EventOptions.PositionChangeNotification = false;
                EventOptions.Clean();
            }
        }

        /// <summary>
        /// Gets Vertex data as specified class
        /// </summary>
        /// <typeparam name="T">Class</typeparam>
        public T GetDataVertex<T>() where T : IGraphXVertex
        {
            return (T)Vertex;
        }
    }
}
