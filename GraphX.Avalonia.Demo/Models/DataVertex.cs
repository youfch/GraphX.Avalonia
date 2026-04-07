using System;

namespace GraphX.Avalonia.Demo.Models
{
    /// <summary>
    /// Example vertex data class.
    /// </summary>
    public class DataVertex : IComparable<DataVertex>
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public DataVertex() { }

        public DataVertex(int id, string? name = null)
        {
            ID = id;
            Name = name ?? $"Node {id}";
        }

        public override string ToString()
        {
            return Name ?? $"Vertex {ID}";
        }

        public int CompareTo(DataVertex? other)
        {
            if (other == null) return 1;
            return ID.CompareTo(other.ID);
        }
    }
}