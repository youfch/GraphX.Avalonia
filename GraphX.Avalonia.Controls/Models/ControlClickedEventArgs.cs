using Avalonia.Input;

namespace GraphX.Controls.Models
{
    /// <summary>
    /// Generic event arguments for control click events.
    /// </summary>
    public class ControlClickedEventArgs<CType> : System.EventArgs
    {
        public CType Control { get; }
        public PointerPressedEventArgs? PointerArgs { get; }
        public KeyModifiers Modifiers { get; }

        public ControlClickedEventArgs(CType c, PointerPressedEventArgs? e, KeyModifiers keys)
        {
            Control = c;
            PointerArgs = e;
            Modifiers = keys;
        }
    }

    /// <summary>
    /// Event arguments for vertex click events.
    /// </summary>
    public sealed class VertexClickedEventArgs : ControlClickedEventArgs<VertexControl>
    {
        public VertexClickedEventArgs(VertexControl c, PointerPressedEventArgs? e, KeyModifiers keys)
            : base(c, e, keys)
        {
        }
    }

    /// <summary>
    /// Event arguments for edge click events.
    /// </summary>
    public sealed class EdgeClickedEventArgs : ControlClickedEventArgs<EdgeControl>
    {
        public EdgeClickedEventArgs(EdgeControl c, PointerPressedEventArgs? e, KeyModifiers keys)
            : base(c, e, keys)
        {
        }
    }
}