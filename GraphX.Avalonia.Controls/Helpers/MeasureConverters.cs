using System;
using Avalonia;

namespace GraphX.Controls.Helpers
{
    /// <summary>
    /// Extension methods for converting between Avalonia and GraphX.Measure types.
    /// This enables the platform-agnostic GraphX.Standard layers to work with Avalonia UI.
    /// </summary>
    public static class MeasureConverters
    {
        // Avalonia.Point → GraphX.Measure.Point
        public static Common.Measure.Point ToGraphX(this Point point)
        {
            return new Common.Measure.Point(point.X, point.Y);
        }

        // GraphX.Measure.Point → Avalonia.Point
        public static Point ToAvalonia(this Common.Measure.Point point)
        {
            return new Point(point.X, point.Y);
        }

        // Avalonia.Size → GraphX.Measure.Size
        public static Common.Measure.Size ToGraphX(this Size size)
        {
            return new Common.Measure.Size(size.Width, size.Height);
        }

        // GraphX.Measure.Size → Avalonia.Size
        public static Size ToAvalonia(this Common.Measure.Size size)
        {
            return new Size(size.Width, size.Height);
        }

        // Avalonia.Rect → GraphX.Measure.Rect
        public static Common.Measure.Rect ToGraphX(this Rect rect)
        {
            return new Common.Measure.Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        // GraphX.Measure.Rect → Avalonia.Rect
        public static Rect ToAvalonia(this Common.Measure.Rect rect)
        {
            return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        // Avalonia.Vector → GraphX.Measure.Vector
        public static Common.Measure.Vector ToGraphX(this Vector vector)
        {
            return new Common.Measure.Vector(vector.X, vector.Y);
        }

        // GraphX.Measure.Vector → Avalonia.Vector
        public static Vector ToAvalonia(this Common.Measure.Vector vector)
        {
            return new Vector(vector.X, vector.Y);
        }

        // Avalonia.Thickness → GraphX.Measure.Thickness
        public static Common.Measure.Thickness ToGraphX(this Thickness thickness)
        {
            return new Common.Measure.Thickness(thickness.Left, thickness.Top, thickness.Right, thickness.Bottom);
        }

        // GraphX.Measure.Thickness → Avalonia.Thickness
        public static Thickness ToAvalonia(this Common.Measure.Thickness thickness)
        {
            return new Thickness(thickness.Left, thickness.Top, thickness.Right, thickness.Bottom);
        }
    }
}