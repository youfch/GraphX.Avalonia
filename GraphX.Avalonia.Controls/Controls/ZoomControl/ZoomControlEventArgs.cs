using System;
using Avalonia;

namespace GraphX.Controls
{
    /// <summary>
    /// Delegate for content size changed event.
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="newSize">The new size</param>
    public delegate void ContentSizeChangedHandler(object sender, Size newSize);

    /// <summary>
    /// Delegate for area selected event.
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="e">Event arguments</param>
    public delegate void AreaSelectedEventHandler(object sender, AreaSelectedEventArgs e);

    /// <summary>
    /// Event arguments for area selection.
    /// </summary>
    public class AreaSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the selected area.
        /// </summary>
        public Rect Area { get; }

        public AreaSelectedEventArgs(Rect area)
        {
            Area = area;
        }
    }
}