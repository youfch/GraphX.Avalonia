using System.Collections.Generic;
using GraphX.Avalonia.Demo.Models;
using QuikGraph;

namespace GraphX.Avalonia.Demo.Models
{
    /// <summary>
    /// Example bidirectional graph data class.
    /// </summary>
    public class DataGraph : BidirectionalGraph<DataVertex, DataEdge>
    {
        public DataGraph() : base(false) { }

        /// <summary>
        /// Creates a sample graph with some vertices and edges.
        /// </summary>
        public static DataGraph CreateSampleGraph()
        {
            var graph = new DataGraph();

            // Create vertices
            var vertices = new List<DataVertex>
            {
                new DataVertex(1, "Start"),
                new DataVertex(2, "Process A"),
                new DataVertex(3, "Process B"),
                new DataVertex(4, "Decision"),
                new DataVertex(5, "End"),
                new DataVertex(6, "Cache"),
                new DataVertex(7, "Database"),
                new DataVertex(8, "API")
            };

            // Add vertices to graph
            foreach (var vertex in vertices)
            {
                graph.AddVertex(vertex);
            }

            // Create edges
            var edges = new List<DataEdge>
            {
                new DataEdge(vertices[0], vertices[1]) { Label = "init" },
                new DataEdge(vertices[0], vertices[2]) { Label = "alt" },
                new DataEdge(vertices[1], vertices[3]) { Label = "proc" },
                new DataEdge(vertices[2], vertices[3]) { Label = "proc" },
                new DataEdge(vertices[3], vertices[4]) { Label = "yes" },
                new DataEdge(vertices[3], vertices[5]) { Label = "no" },
                new DataEdge(vertices[5], vertices[6]) { Label = "cache" },
                new DataEdge(vertices[6], vertices[7]) { Label = "db" },
                new DataEdge(vertices[7], vertices[8]) { Label = "api" },
                new DataEdge(vertices[8], vertices[4]) { Label = "done" }
            };

            // Add edges to graph
            foreach (var edge in edges)
            {
                graph.AddEdge(edge);
            }

            return graph;
        }
    }
}