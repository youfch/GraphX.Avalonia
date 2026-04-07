using System;
using Avalonia;
using GraphX.Measure;

namespace GraphX.Controls.Helpers
{
    /// <summary>
    /// Extension methods for converting between Avalonia and GraphX.Measure types.
    /// This enables the platform-agnostic GraphX.Standard layers to work with Avalonia UI.
    /// </summary>
    public static class MeasureConverters
    {
        // Avalonia.Point → GraphX.Measure.Point
        public static Point ToGraphX(this Avalonia.Point point)
        {
            return new Point(point.X, point.Y);
        }

        // GraphX.Measure.Point → Avalonia.Point
        public static Avalonia.Point ToAvalonia(this Point point)
        {
            return new Avalonia.Point(point.X, point.Y);
        }

        // Avalonia.Size → GraphX.Measure.Size
        public static Size ToGraphX(this Avalonia.Size size)
        {
            return new Size(size.Width, size.Height);
        }

        // GraphX.Measure.Size → Avalonia.Size
        public static Avalonia.Size ToAvalonia(this Size size)
        {
            return new Avalonia.Size(size.Width, size.Height);
        }

        // Avalonia.Rect → GraphX.Measure.Rect
        public static Rect ToGraphX(this Avalonia.Rect rect)
        {
            return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        // GraphX.Measure.Rect → Avalonia.Rect
        public static Avalonia.Rect ToAvalonia(this Rect rect)
        {
            return new Avalonia.Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        // Avalonia.Vector → GraphX.Measure.Vector
        public static Vector ToGraphX(this Avalonia.Vector vector)
        {
            return new Vector(vector.X, vector.Y);
        }

        // GraphX.Measure.Vector → Avalonia.Vector
        public static Avalonia.Vector ToAvalonia(this Vector vector)
        {
            return new Avalonia.Vector(vector.X, vector.Y);
        }

        // Avalonia.Thickness → GraphX.Measure.Thickness
        public static Thickness ToGraphX(this Avalonia.Thickness thickness)
        {
            return new Thickness(thickness.Left, thickness.Top, thickness.Right, thickness.Bottom);
        }

        // GraphX.Measure.Thickness → Avalonia.Thickness
        public static Avalonia.Thickness ToAvalonia(this Thickness thickness)
        {
            return new Avalonia.Thickness(thickness.Left, thickness.Top, thickness.Right, thickness.Bottom);
        }
    }
}