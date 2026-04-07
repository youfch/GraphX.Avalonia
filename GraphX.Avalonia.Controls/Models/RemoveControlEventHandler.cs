using System;

namespace GraphX.Controls.Models
{
    /// <summary>
    /// Delegate for remove control event handler.
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="args">Event arguments</param>
    public delegate void RemoveControlEventHandler(object sender, RemoveControlEventArgs args);

    /// <summary>
    /// Event arguments for remove control events.
    /// </summary>
    public class RemoveControlEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the control to remove.
        /// </summary>
        public IGraphControl Control { get; }

        /// <summary>
        /// Gets whether the data should be removed.
        /// </summary>
        public bool RemoveData { get; }

        public RemoveControlEventArgs(IGraphControl control, bool removeData = false)
        {
            Control = control;
            RemoveData = removeData;
        }
    }
}