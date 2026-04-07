using Avalonia;

namespace GraphX.Controls
{
    /// <summary>
    /// Extension methods for Point and Rect types.
    /// </summary>
    public static class PointExtensions
    {
        /// <summary>
        /// Converts Point to Vector.
        /// </summary>
        public static Vector ToVector(this Point point)
        {
            return new Vector(point.X, point.Y);
        }

        /// <summary>
        /// Converts Vector to Point.
        /// </summary>
        public static Point ToPoint(this Vector vector)
        {
            return new Point(vector.X, vector.Y);
        }
    }

    public static class RectExtensions
    {
        /// <summary>
        /// Gets the center point of the rectangle.
        /// </summary>
        public static Point Center(this Rect rect)
        {
            return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }

        /// <summary>
        /// Gets the top-left point of the rectangle.
        /// </summary>
        public static Point TopLeft(this Rect rect)
        {
            return new Point(rect.Left, rect.Top);
        }

        /// <summary>
        /// Gets the top-right point of the rectangle.
        /// </summary>
        public static Point TopRight(this Rect rect)
        {
            return new Point(rect.Right, rect.Top);
        }

        /// <summary>
        /// Gets the bottom-left point of the rectangle.
        /// </summary>
        public static Point BottomLeft(this Rect rect)
        {
            return new Point(rect.Left, rect.Bottom);
        }

        /// <summary>
        /// Gets the bottom-right point of the rectangle.
        /// </summary>
        public static Point BottomRight(this Rect rect)
        {
            return new Point(rect.Right, rect.Bottom);
        }
    }
}