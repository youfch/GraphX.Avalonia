using System;
using Avalonia;

namespace GraphX.Controls
{
    /// <summary>
    /// Interface for edge pointer controls.
    /// </summary>
    public interface IEdgePointer : IDisposable
    {
        /// <summary>
        /// Returns edge pointer center position coordinates.
        /// </summary>
        /// <returns>The position point</returns>
        Point GetPosition();

        /// <summary>
        /// Gets if the pointer has to be rotated according to edge directions.
        /// </summary>
        bool NeedRotation { get; }

        /// <summary>
        /// Updates edge pointer position and angle.
        /// </summary>
        /// <param name="position">The position</param>
        /// <param name="direction">The direction vector</param>
        /// <param name="angle">The rotation angle</param>
        /// <returns>The updated position</returns>
        Point Update(Point? position, Vector direction, double angle = 0d);

        /// <summary>
        /// Sets the pointer position manually.
        /// </summary>
        /// <param name="position">The position</param>
        void SetManualPosition(Point position);

        /// <summary>
        /// Hides the pointer.
        /// </summary>
        void Hide();

        /// <summary>
        /// Shows the pointer.
        /// </summary>
        void Show();

        /// <summary>
        /// Gets a value indicating whether the pointer is suppressed.
        /// </summary>
        bool IsSuppressed { get; }

        /// <summary>
        /// Suppresses the pointer display.
        /// </summary>
        void Suppress();

        /// <summary>
        /// Removes pointer display suppression.
        /// </summary>
        void UnSuppress();

        /// <summary>
        /// Gets or sets whether the control is visible.
        /// </summary>
        bool IsVisible { get; set; }
    }
}