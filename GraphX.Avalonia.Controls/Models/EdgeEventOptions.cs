using GraphX.Common.Enums;

namespace GraphX.Controls.Models
{
    /// <summary>
    /// Event options configuration for edge controls.
    /// </summary>
    public sealed class EdgeEventOptions
    {
        private EdgeControl _ec;

        public EdgeEventOptions(EdgeControl ec)
        {
            _ec = ec;
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
                if (_mousemove != value)
                {
                    _mousemove = value;
                    _ec?.UpdateEventhandling(EventType.MouseMove);
                }
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
                if (_mouseenter != value)
                {
                    _mouseenter = value;
                    _ec?.UpdateEventhandling(EventType.MouseEnter);
                }
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
                if (_mouseleave != value)
                {
                    _mouseleave = value;
                    _ec?.UpdateEventhandling(EventType.MouseLeave);
                }
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
                if (_mouseclick != value)
                {
                    _mouseclick = value;
                    _ec?.UpdateEventhandling(EventType.MouseClick);
                }
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
                if (_mousedblclick != value)
                {
                    _mousedblclick = value;
                    _ec?.UpdateEventhandling(EventType.MouseDoubleClick);
                }
            }
        }

        public void Clean()
        {
            _ec = null;
        }
    }
}