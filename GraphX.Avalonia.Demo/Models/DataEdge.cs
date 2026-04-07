using QuikGraph;

namespace GraphX.Avalonia.Demo.Models
{
    /// <summary>
    /// Example edge data class.
    /// </summary>
    public class DataEdge : Edge<DataVertex>
    {
        public double? Weight { get; set; }
        public string? Label { get; set; }

        public DataEdge(DataVertex source, DataVertex target)
            : base(source, target)
        {
        }

        public override string ToString()
        {
            return $"{Source} → {Target}";
        }
    }
}