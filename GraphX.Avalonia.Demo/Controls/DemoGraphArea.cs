using System;
using System.Collections.Generic;
using GraphX.Avalonia.Demo.Models;
using GraphX.Common.Interfaces;
using GraphX.Controls;
using QuikGraph;

namespace GraphX.Avalonia.Demo.Controls
{
    /// <summary>
    /// Custom GraphArea implementation for the demo.
    /// </summary>
    public class DemoGraphArea : GraphArea<DataVertex, DataEdge, BidirectionalGraph<DataVertex, DataEdge>>
    {
        public DemoGraphArea()
        {
            // Set default properties
            ShowAllEdges = true;
            ShowAllEdgesLabels = true;
            EnableVisualProps = true;
        }
    }

    /// <summary>
    /// Generic GraphArea base class for Avalonia.
    /// </summary>
    public class GraphArea<TVertex, TEdge, TGraph> : GraphAreaBase
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : class, IEdgeListGraph<TVertex, TEdge>
    {
        /// <summary>
        /// Gets the graph data.
        /// </summary>
        public TGraph? Graph { get; protected set; }

        /// <summary>
        /// Gets the vertex list mapping.
        /// </summary>
        public IDictionary<TVertex, VertexControl> VertexList { get; } = new Dictionary<TVertex, VertexControl>();

        /// <summary>
        /// Gets the edge list mapping.
        /// </summary>
        public IDictionary<TEdge, EdgeControl> EdgeList { get; } = new Dictionary<TEdge, EdgeControl>();

        /// <summary>
        /// Gets or sets the logic core.
        /// </summary>
        public IGraphLogicCore<TVertex, TEdge, TGraph>? LogicCore { get; set; }

        /// <summary>
        /// Shows all edges.
        /// </summary>
        public bool ShowAllEdges { get; set; } = true;

        /// <summary>
        /// Shows all edge labels.
        /// </summary>
        public bool ShowAllEdgesLabels { get; set; }

        /// <summary>
        /// Enables visual properties.
        /// </summary>
        public bool EnableVisualProps { get; set; } = true;

        /// <summary>
        /// Generates the graph visual presentation.
        /// </summary>
        public void GenerateGraph(TGraph graph)
        {
            Graph = graph;
            InternalGenerateGraph();
        }

        private void InternalGenerateGraph()
        {
            if (Graph == null) return;

            // Clear existing vertices and edges
            VertexList.Clear();
            EdgeList.Clear();
            Children.Clear();

            // Create vertex controls
            foreach (var vertex in Graph.Vertices)
            {
                var vc = new VertexControl
                {
                    Vertex = vertex,
                    RootArea = this
                };
                VertexList[vertex] = vc;
                Children.Add(vc);
            }

            // Create edge controls
            foreach (var edge in Graph.Edges)
            {
                if (VertexList.TryGetValue(edge.Source, out var source) &&
                    VertexList.TryGetValue(edge.Target, out var target))
                {
                    var ec = new EdgeControl(source, target)
                    {
                        Edge = edge,
                        RootArea = this
                    };
                    EdgeList[edge] = ec;
                    Children.Add(ec);
                }
            }
        }

        /// <summary>
        /// Relayouts the graph.
        /// </summary>
        public void RelayoutGraph()
        {
            UpdateAllEdges();
        }
    }

    /// <summary>
    /// Interface for graph logic core.
    /// </summary>
    public interface IGraphLogicCore<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : class, IEdgeListGraph<TVertex, TEdge>
    {
        /// <summary>
        /// Gets or sets the graph.
        /// </summary>
        TGraph? Graph { get; set; }

        /// <summary>
        /// Computes the layout.
        /// </summary>
        void ComputeLayout();
    }
}