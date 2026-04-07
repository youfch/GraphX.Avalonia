namespace GraphX.Controls
{
    /// <summary>
    /// Interface for trackable content.
    /// </summary>
    public interface ITrackableContent
    {
        /// <summary>
        /// Gets the content size.
        /// </summary>
        double ContentWidth { get; }

        /// <summary>
        /// Gets the content size.
        /// </summary>
        double ContentHeight { get; }
    }
}