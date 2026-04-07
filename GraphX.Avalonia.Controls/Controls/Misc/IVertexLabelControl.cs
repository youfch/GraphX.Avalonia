namespace GraphX.Controls
{
    /// <summary>
    /// Interface for vertex label controls.
    /// </summary>
    public interface IVertexLabelControl
    {
        /// <summary>
        /// Gets or sets label drawing angle in degrees.
        /// </summary>
        double Angle { get; set; }

        /// <summary>
        /// Updates the vertex label position.
        /// </summary>
        void UpdatePosition();

        /// <summary>
        /// Hides the label.
        /// </summary>
        void Hide();

        /// <summary>
        /// Shows the label.
        /// </summary>
        void Show();
    }
}