using Avalonia.Input;

namespace GraphX.Controls.Models
{
    /// <summary>
    /// Event arguments for edge selection events.
    /// </summary>
    public sealed class EdgeSelectedEventArgs : System.EventArgs
    {
        public EdgeControl EdgeControl { get; }
        public PointerPressedEventArgs? PointerArgs { get; }
        public KeyModifiers Modifiers { get; }

        public EdgeSelectedEventArgs(EdgeControl ec, PointerPressedEventArgs? e, KeyModifiers keys)
        {
            EdgeControl = ec;
            PointerArgs = e;
            Modifiers = keys;
        }
    }
}