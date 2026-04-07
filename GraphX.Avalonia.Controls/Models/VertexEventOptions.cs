using GraphX.Common.Enums;

namespace GraphX.Controls.Models
{
    /// <summary>
    /// Event options configuration for vertex controls.
    /// </summary>
    public sealed class VertexEventOptions
    {
        private VertexControl _vc;

        public VertexEventOptions(VertexControl vc)
        {
            _vc = vc;
        }

        private bool _mousemove = true;
        /// <summary>
        /// Gets or sets if MouseMove event should be enabled.
        /// </summary>
        public bool MouseMoveEnabled
        {
            get => _mousemove;
            set
            {
                if (_mousemove == value) return;
                _mousemove = value;
                _vc?.UpdateEventhandling(EventType.MouseMove);
            }
        }

        private bool _mouseenter = true;
        /// <summary>
        /// Gets or sets if MouseEnter event should be enabled.
        /// </summary>
        public bool MouseEnterEnabled
        {
            get => _mouseenter;
            set
            {
                if (_mouseenter == value) return;
                _mouseenter = value;
                _vc?.UpdateEventhandling(EventType.MouseEnter);
            }
        }

        private bool _mouseleave = true;
        /// <summary>
        /// Gets or sets if MouseLeave event should be enabled.
        /// </summary>
        public bool MouseLeaveEnabled
        {
            get => _mouseleave;
            set
            {
                if (_mouseleave == value) return;
                _mouseleave = value;
                _vc?.UpdateEventhandling(EventType.MouseLeave);
            }
        }

        private bool _mouseclick = true;
        /// <summary>
        /// Gets or sets if MouseClick event should be enabled.
        /// </summary>
        public bool MouseClickEnabled
        {
            get => _mouseclick;
            set
            {
                if (_mouseclick == value) return;
                _mouseclick = value;
                _vc?.UpdateEventhandling(EventType.MouseClick);
            }
        }

        private bool _mousedblclick = true;
        /// <summary>
        /// Gets or sets if MouseDoubleClick event should be enabled.
        /// </summary>
        public bool MouseDoubleClickEnabled
        {
            get => _mousedblclick;
            set
            {
                if (_mousedblclick == value) return;
                _mousedblclick = value;
                _vc?.UpdateEventhandling(EventType.MouseDoubleClick);
            }
        }

        private bool _poschange = true;
        /// <summary>
        /// Gets or sets if position trace enabled.
        /// </summary>
        public bool PositionChangeNotification
        {
            get => _poschange;
            set
            {
                if (_poschange == value) return;
                _poschange = value;
                if (_vc == null) return;
                _vc.UpdatePositionTraceState();
            }
        }

        public void Clean()
        {
            _vc = null;
        }
    }
}