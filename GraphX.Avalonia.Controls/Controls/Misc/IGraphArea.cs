using System.Collections.Generic;

namespace GraphX.Controls
{
    /// <summary>
    /// Interface for graph area controls.
    /// </summary>
    /// <typeparam name="TVertex">The vertex data type</typeparam>
    public interface IGraphArea<TVertex>
    {
        /// <summary>
        /// Gets or sets the dictionary of vertex controls.
        /// </summary>
        IDictionary<TVertex, VertexControl> VertexList { get; set; }
    }
}