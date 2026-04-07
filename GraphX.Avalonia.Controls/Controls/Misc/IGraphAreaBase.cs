namespace GraphX.Controls
{
    /// <summary>
    /// Interface for graph area base functionality.
    /// </summary>
    public interface IGraphAreaBase
    {
        /// <summary>
        /// Gets or sets whether positioning is complete.
        /// </summary>
        bool PositioningComplete { get; set; }

        /// <summary>
        /// Updates all edge routes.
        /// </summary>
        void UpdateAllEdges();

        /// <summary>
        /// Sets the print mode.
        /// </summary>
        /// <param name="value">Whether print mode is enabled</param>
        /// <param name="offsetControls">Whether to offset controls</param>
        /// <param name="margin">The margin</param>
        void SetPrintMode(bool value, bool offsetControls = true, int margin = 0);
    }
}