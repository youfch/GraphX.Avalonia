using System;
using Avalonia;

namespace GraphX.Controls
{
    /// <summary>
    /// Interface for edge label controls.
    /// </summary>
    public interface IEdgeLabelControl : IDisposable
    {
        /// <summary>
        /// Gets or sets if label should be aligned with the edge.
        /// </summary>
        bool AlignToEdge { get; set; }

        /// <summary>
        /// Gets or sets if label is visible.
        /// </summary>
        bool ShowLabel { get; set; }

        /// <summary>
        /// Gets or sets label vertical offset.
        /// </summary>
        double LabelVerticalOffset { get; set; }

        /// <summary>
        /// Gets or sets label horizontal offset.
        /// </summary>
        double LabelHorizontalOffset { get; set; }

        /// <summary>
        /// Gets or sets label drawing angle in degrees.
        /// </summary>
        double Angle { get; set; }

        /// <summary>
        /// Updates the edge label position.
        /// </summary>
        void UpdatePosition();

        /// <summary>
        /// Updates the label layout.
        /// </summary>
        void UpdateLayout();

        /// <summary>
        /// Shows the label.
        /// </summary>
        void Show();

        /// <summary>
        /// Hides the label.
        /// </summary>
        void Hide();

        /// <summary>
        /// Gets the label rectangular size.
        /// </summary>
        /// <returns>The size rectangle</returns>
        Rect GetSize();

        /// <summary>
        /// Sets the label rectangular size.
        /// </summary>
        /// <param name="size">The size rectangle</param>
        void SetSize(Rect size);
    }
}