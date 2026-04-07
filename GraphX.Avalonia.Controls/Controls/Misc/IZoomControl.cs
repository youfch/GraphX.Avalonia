using Avalonia;

namespace GraphX.Controls
{
    /// <summary>
    /// Common interface for zoom control objects.
    /// </summary>
    public interface IZoomControl
    {
        /// <summary>
        /// Gets the visual presenter element.
        /// </summary>
        Visual PresenterVisual { get; }

        /// <summary>
        /// Gets or sets the zoom level.
        /// </summary>
        double Zoom { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        double Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        double Height { get; set; }

        /// <summary>
        /// Gets the actual rendered width.
        /// </summary>
        double ActualWidth { get; }

        /// <summary>
        /// Gets the actual rendered height.
        /// </summary>
        double ActualHeight { get; }
    }
}