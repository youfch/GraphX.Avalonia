using System;
using QuikGraph;

namespace GraphX.Controls.Models.Interfaces
{
    /// <summary>
    /// Factory interface for creating graph controls.
    /// Used to create vertex and edge controls with custom styling.
    /// </summary>
    /// <typeparam name="TVertex">Vertex data type</typeparam>
    /// <typeparam name="TEdge">Edge data type</typeparam>
    public interface IGraphControlFactory<TVertex, TEdge>
        where TVertex : class
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Creates a vertex control for the specified vertex data.
        /// </summary>
        VertexControl CreateVertexControl(TVertex vertexData);

        /// <summary>
        /// Creates an edge control for the specified edge data.
        /// </summary>
        EdgeControl CreateEdgeControl(TEdge edgeData);

        /// <summary>
        /// Creates an optional edge label control.
        /// </summary>
        object? CreateEdgeLabelControl(TEdge edgeData);

        /// <summary>
        /// Creates an optional vertex label control.
        /// </summary>
        object? CreateVertexLabelControl(TVertex vertexData);

        /// <summary>
        /// Removes a vertex control.
        /// </summary>
        void RemoveVertexControl(VertexControl vertexControl);

        /// <summary>
        /// Removes an edge control.
        /// </summary>
        void RemoveEdgeControl(EdgeControl edgeControl);
    }
}