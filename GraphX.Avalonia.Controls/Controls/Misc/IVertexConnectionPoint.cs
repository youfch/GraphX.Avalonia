using GraphX.Common.Enums;
using Rect = GraphX.Measure.Rect;

namespace GraphX.Controls
{
    /// <summary>
    /// Interface for vertex connection points.
    /// </summary>
    public interface IVertexConnectionPoint
    {
        /// <summary>
        /// Connector identifier
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Gets or sets shape form for connection point (affects math calculations for edge end placement)
        /// </summary>
        VertexShape Shape { get; set; }

        void Hide();
        void Show();

        Rect RectangularSize { get; }

        void Update();
    }
}