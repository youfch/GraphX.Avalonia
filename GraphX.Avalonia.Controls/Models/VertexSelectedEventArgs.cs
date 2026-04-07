using Avalonia.Input;
using Avalonia.Interactivity;

namespace GraphX.Controls.Models
{
    /// <summary>
    /// Event arguments for vertex selection events.
    /// </summary>
    public sealed class VertexSelectedEventArgs : System.EventArgs
    {
        public VertexControl VertexControl { get; }
        public PointerPressedEventArgs? PointerArgs { get; }
        public KeyModifiers Modifiers { get; }

        public VertexSelectedEventArgs(VertexControl vc, PointerPressedEventArgs? e, KeyModifiers keys)
        {
            VertexControl = vc;
            PointerArgs = e;
            Modifiers = keys;
        }
    }
}