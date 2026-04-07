using Avalonia.Input;

namespace GraphX.Controls.Models
{
    /// <summary>
    /// Event arguments for vertex movement events.
    /// </summary>
    public sealed class VertexMovedEventArgs : System.EventArgs
    {
        public VertexControl VertexControl { get; }

        public VertexMovedEventArgs(VertexControl vc)
        {
            VertexControl = vc;
        }
    }
}