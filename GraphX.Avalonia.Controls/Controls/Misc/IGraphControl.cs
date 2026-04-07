using System;
using Avalonia;

namespace GraphX.Controls
{
    /// <summary>
    /// Common interface for graph controls (vertices and edges).
    /// </summary>
    public interface IGraphControl : IPositionChangeNotify
    {
        /// <summary>
        /// Gets the parent GraphArea control.
        /// </summary>
        GraphAreaBase RootArea { get; }

        /// <summary>
        /// Gets the position of the control.
        /// </summary>
        /// <param name="final">If true, gets the final animation position</param>
        /// <param name="round">If true, rounds the position values</param>
        /// <returns>The control position</returns>
        Point GetPosition(bool final = false, bool round = false);

        /// <summary>
        /// Sets the position of the control.
        /// </summary>
        /// <param name="pt">The position point</param>
        /// <param name="alsoFinal">If true, also sets the final position</param>
        void SetPosition(Point pt, bool alsoFinal = true);

        /// <summary>
        /// Sets the position of the control.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="alsoFinal">If true, also sets the final position</param>
        void SetPosition(double x, double y, bool alsoFinal = true);

        /// <summary>
        /// Gets or sets the visibility of the control.
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// Cleans up resources associated with the control.
        /// </summary>
        void Clean();
    }
}