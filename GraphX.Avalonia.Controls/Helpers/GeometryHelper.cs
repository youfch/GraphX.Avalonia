using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using Avalonia.Media;
using GraphX.Common.Enums;

namespace GraphX.Controls
{
    /// <summary>
    /// Geometry helper methods for graph edge routing and calculations.
    /// </summary>
    public static class GeometryHelper
    {
        /// <summary>
        /// Get Intersection point on a rectangular surface.
        /// </summary>
        public static Vector? Intersects(Vector a1, Vector a2, Vector b1, Vector b2)
        {
            var a = a2 - a1;
            var b = b2 - b1;
            var aDotBPerp = a.X * b.Y - a.Y * b.X;

            if (aDotBPerp == 0)
                return null;

            var c = b1 - a1;

            var u = (c.X * a.Y - c.Y * a.X) / aDotBPerp;
            if (u < 0 || u > 1)
                return null;

            var t = (c.X * b.Y - c.Y * b.X) / aDotBPerp;

            return a1 + t * a;
        }

        /// <summary>
        /// Generate PathGeometry object with curved Path using supplied route points.
        /// </summary>
        public static PolyLineSegment GetCurveThroughPoints(Point[] points, double tension, double tolerance)
        {
            Debug.Assert(points != null);
            Debug.Assert(points.Length >= 2);
            Debug.Assert(tolerance > 0);

            var oPolyLineSegment = new PolyLineSegment();

            if (points.Length == 2)
            {
                AddPointsToPolyLineSegment(oPolyLineSegment, points[0], points[0],
                    points[1], points[1], tension, tolerance);
            }
            else
            {
                var iPoints = points.Length;

                for (var i = 0; i < iPoints; i++)
                {
                    if (i == 0)
                    {
                        AddPointsToPolyLineSegment(oPolyLineSegment, points[0],
                            points[0], points[1], points[2], tension, tolerance);
                    }
                    else if (i == iPoints - 2)
                    {
                        AddPointsToPolyLineSegment(oPolyLineSegment, points[i - 1],
                            points[i], points[i + 1], points[i + 1], tension, tolerance);
                    }
                    else if (i != iPoints - 1)
                    {
                        AddPointsToPolyLineSegment(oPolyLineSegment, points[i - 1],
                            points[i], points[i + 1], points[i + 2], tension, tolerance);
                    }
                }
                oPolyLineSegment.Points.Insert(0, points[0]);
            }

            return oPolyLineSegment;
        }

        public static IList<Point> GetCurvePointsThroughPoints(Point[] points, double tension, double tolerance)
        {
            return GetCurveThroughPoints(points, tension, tolerance).Points;
        }

        private static void AddPointsToPolyLineSegment(PolyLineSegment oPolyLineSegment, Point oPoint0, Point oPoint1, Point oPoint2, Point oPoint3, double dTension, double dTolerance)
        {
            Debug.Assert(oPolyLineSegment != null);
            Debug.Assert(dTolerance > 0);

            var iPoints = (int)((Math.Abs(oPoint1.X - oPoint2.X) +
                Math.Abs(oPoint1.Y - oPoint2.Y)) / dTolerance);

            var oPolyLineSegmentPoints = oPolyLineSegment.Points;

            if (iPoints <= 2)
            {
                oPolyLineSegmentPoints.Add(oPoint2);
            }
            else
            {
                var dSx1 = dTension * (oPoint2.X - oPoint0.X);
                var dSy1 = dTension * (oPoint2.Y - oPoint0.Y);
                var dSx2 = dTension * (oPoint3.X - oPoint1.X);
                var dSy2 = dTension * (oPoint3.Y - oPoint1.Y);

                var dAx = dSx1 + dSx2 + 2 * oPoint1.X - 2 * oPoint2.X;
                var dAy = dSy1 + dSy2 + 2 * oPoint1.Y - 2 * oPoint2.Y;
                var dBx = -2 * dSx1 - dSx2 - 3 * oPoint1.X + 3 * oPoint2.X;
                var dBy = -2 * dSy1 - dSy2 - 3 * oPoint1.Y + 3 * oPoint2.Y;

                var dCx = dSx1;
                var dCy = dSy1;
                var dDx = oPoint1.X;
                var dDy = oPoint1.Y;

                for (var i = 1; i < iPoints; i++)
                {
                    var t = (double)i / (iPoints - 1);

                    var oPoint = new Point(
                        dAx * t * t * t + dBx * t * t + dCx * t + dDx,
                        dAy * t * t * t + dBy * t * t + dCy * t + dDy
                    );

                    oPolyLineSegmentPoints.Add(oPoint);
                }
            }
        }

        public static PathFigure GetPathFigureFromPathSegments(Point oStartPoint, bool bPathFigureIsFilled, params PathSegment[] aoPathSegments)
        {
            Debug.Assert(aoPathSegments != null);

            var oPathFigure = new PathFigure { StartPoint = oStartPoint, IsFilled = bPathFigureIsFilled };
            var oSegments = oPathFigure.Segments;

            foreach (var oPathSegment in aoPathSegments)
            {
                oSegments.Add(oPathSegment);
            }
            return oPathFigure;
        }

        public static PathGeometry GetPathGeometryFromPathSegments(Point oStartPoint, bool bPathFigureIsFilled, params PathSegment[] aoPathSegments)
        {
            Debug.Assert(aoPathSegments != null);

            var oPathFigure = new PathFigure { StartPoint = oStartPoint, IsFilled = bPathFigureIsFilled };
            var oSegments = oPathFigure.Segments;

            foreach (var oPathSegment in aoPathSegments)
            {
                oSegments.Add(oPathSegment);
            }
            var oPathGeometry = new PathGeometry();
            oPathGeometry.Figures.Add(oPathFigure);

            return oPathGeometry;
        }

        /// <summary>
        /// Returns edge endpoint based on vertex math shape and rotation angle.
        /// </summary>
        public static Point GetEdgeEndpoint(Point source, Rect sourceSize, Point target, VertexShape shape, double angle = 0)
        {
            switch (shape)
            {
                case VertexShape.Circle:
                    return GetEdgeEndpointOnCircle(source, Math.Max(sourceSize.Height, sourceSize.Width) * .5, target, angle);
                case VertexShape.Ellipse:
                    return GetEdgeEndpointOnEllipse(source, sourceSize.Width * .5, sourceSize.Height * .5, target, angle);
                case VertexShape.Diamond:
                    return GetEdgeEndpointOnDiamond(source, sourceSize.Width * .5, target);
                case VertexShape.Triangle:
                    return GetEdgeEndpointOnTriangle(source, sourceSize.Width * .5, target);
                default:
                    return GetEdgeEndpointOnRectangle(source, sourceSize, target, angle);
            }
        }

        public static Point GetEdgeEndpointOnCircle(Point oVertexALocation, double dVertexARadius, Point oVertexBLocation, double angle = 0)
        {
            Debug.Assert(dVertexARadius >= 0);

            var dEdgeAngle = MathHelper.GetAngleBetweenPoints(oVertexALocation, oVertexBLocation);
            var pt = new Point(
                oVertexALocation.X + (dVertexARadius * Math.Cos(dEdgeAngle)),
                oVertexALocation.Y - (dVertexARadius * Math.Sin(dEdgeAngle))
            );
            return pt;
        }

        public static Point GetEdgeEndpointOnEllipse(Point oVertexALocation, double dVertexARadiusWidth, double dVertexARadiusHeight, Point oVertexBLocation, double angle = 0)
        {
            Debug.Assert(dVertexARadiusWidth >= 0);
            Debug.Assert(dVertexARadiusHeight >= 0);

            var sourcePoint = oVertexALocation;
            var targetPoint = oVertexBLocation;

            var dEdgeAngle = MathHelper.GetAngleBetweenPoints(sourcePoint, targetPoint);
            if (angle != 0)
                dEdgeAngle = (dEdgeAngle.ToDegrees() + angle).ToRadians();

            var pt = new Point(
                sourcePoint.X + (dVertexARadiusWidth * Math.Cos(dEdgeAngle)),
                sourcePoint.Y - (dVertexARadiusHeight * Math.Sin(dEdgeAngle))
            );
            if (angle != 0)
                pt = MathHelper.RotatePoint(pt, oVertexALocation, angle);
            return pt;
        }

        public static Point GetEdgeEndpointOnTriangle(Point oVertexLocation, double mDHalfWidth, Point otherEndpoint)
        {
            var dEdgeAngle = MathHelper.GetAngleBetweenPoints(oVertexLocation, otherEndpoint);
            var dEdgeAngleDegrees = dEdgeAngle.ToDegrees();

            double dAngleToRotateDegrees;

            if (dEdgeAngleDegrees >= -30.0 && dEdgeAngleDegrees < 90.0)
                dAngleToRotateDegrees = 30.0;
            else if (dEdgeAngleDegrees >= -150.0 && dEdgeAngleDegrees < -30.0)
                dAngleToRotateDegrees = 270.0;
            else
                dAngleToRotateDegrees = 150.0;

            var dWidth = 2.0 * mDHalfWidth;

            var oRotatedRectangle = new Rect(
                oVertexLocation.X,
                oVertexLocation.Y - mDHalfWidth,
                dWidth * MathHelper.Tangent30Degrees,
                dWidth
            );

            var oMatrix = GetRotatedMatrix(oVertexLocation, dAngleToRotateDegrees);
            var oRotatedOtherVertexLocation = oMatrix.Transform(otherEndpoint);
            var oRotatedEdgeEndpoint = GetEdgeEndpointOnRectangle(oVertexLocation, oRotatedRectangle, oRotatedOtherVertexLocation);

            oMatrix = GetRotatedMatrix(oVertexLocation, -dAngleToRotateDegrees);
            return oMatrix.Transform(oRotatedEdgeEndpoint);
        }

        public static Point GetEdgeEndpointOnDiamond(Point oVertexLocation, double mDHalfWidth, Point otherEndpoint)
        {
            var dHalfSquareWidth = mDHalfWidth / Math.Sqrt(2.0);

            var oRotatedDiamond = new Rect(
                oVertexLocation.X - dHalfSquareWidth,
                oVertexLocation.Y - dHalfSquareWidth,
                2.0 * dHalfSquareWidth,
                2.0 * dHalfSquareWidth
            );

            var oMatrix = GetRotatedMatrix(oVertexLocation, 45);
            var oRotatedOtherVertexLocation = oMatrix.Transform(otherEndpoint);
            var oRotatedEdgeEndpoint = GetEdgeEndpointOnRectangle(oVertexLocation, oRotatedDiamond, oRotatedOtherVertexLocation);

            oMatrix = GetRotatedMatrix(oVertexLocation, -45);
            return oMatrix.Transform(oRotatedEdgeEndpoint);
        }

        public static Point GetEdgeEndpointOnRectangle(Point sourcePos, Rect sourceBounds, Point targetPos, double angle = 0)
        {
            Func<Point, double, Point> rotate = (p, a) => angle == 0.0 ? p : MathHelper.RotatePoint(p, sourceBounds.Center(), a);

            var tgt_pt = rotate(targetPos, -angle);

            if (tgt_pt.X <= sourcePos.X)
            {
                var leftSide = Intersects(sourcePos.ToVector(), tgt_pt.ToVector(), sourceBounds.TopLeft().ToVector(), sourceBounds.BottomLeft().ToVector());
                if (leftSide.HasValue)
                    return rotate(new Point(leftSide.Value.X, leftSide.Value.Y), angle);
            }
            else
            {
                var rightSide = Intersects(sourcePos.ToVector(), tgt_pt.ToVector(), sourceBounds.TopRight().ToVector(), sourceBounds.BottomRight().ToVector());
                if (rightSide.HasValue)
                    return rotate(new Point(rightSide.Value.X, rightSide.Value.Y), angle);
            }

            if (tgt_pt.Y <= sourcePos.Y)
            {
                var topSide = Intersects(sourcePos.ToVector(), tgt_pt.ToVector(), sourceBounds.TopLeft().ToVector(), sourceBounds.TopRight().ToVector());
                if (topSide.HasValue)
                    return rotate(new Point(topSide.Value.X, topSide.Value.Y), angle);
            }
            else
            {
                var bottomSide = Intersects(sourcePos.ToVector(), tgt_pt.ToVector(), sourceBounds.BottomLeft().ToVector(), sourceBounds.BottomRight().ToVector());
                if (bottomSide.HasValue)
                    return rotate(new Point(bottomSide.Value.X, bottomSide.Value.Y), angle);
            }

            return rotate(new Point(sourcePos.X, sourcePos.Y), angle);
        }

        public static PathFigure GenerateOldArrow(Point ip1, Point ip2)
        {
            var p1 = new Vector(ip1.X, ip1.Y);
            var p2 = new Vector(ip2.X, ip2.Y);
            var v = p1 - p2;
            v = v / v.Length * 5;
            var n = new Vector(-v.Y, v.X) * 0.7;
            var ov1 = p2 + v - n;
            var ov2 = p2 + v + n;
            var fig = new PathFigure
            {
                StartPoint = ip2,
                Segments = new PathSegments
                {
                    new LineSegment { Point = new Point(ov1.X, ov1.Y) },
                    new LineSegment { Point = new Point(ov2.X, ov2.Y) }
                },
                IsClosed = true
            };
            return fig;
        }

        public static PathFigure GenerateArrow(Point oArrowTipLocation, Point start, Point end, double customAngle = 0.1)
        {
            var dArrowAngle = customAngle == 0.1 ? MathHelper.GetAngleBetweenPoints(start, end) : customAngle;
            var dArrowTipX = oArrowTipLocation.X;
            var dArrowTipY = oArrowTipLocation.Y;
            const double dArrowWidth = 3.0;
            const double dArrowHalfHeight = dArrowWidth / 2.0;
            var dX = dArrowTipX - dArrowWidth;

            var aoPoints = new[]
            {
                oArrowTipLocation,
                new Point(dX, dArrowTipY - dArrowHalfHeight),
                new Point(dX, dArrowTipY + dArrowHalfHeight),
                new Point(dX + 0.2, dArrowTipY)
            };

            var oMatrix = GetRotatedMatrix(oArrowTipLocation, -dArrowAngle.ToDegrees());
            for (int i = 0; i < aoPoints.Length; i++)
            {
                aoPoints[i] = oMatrix.Transform(aoPoints[i]);
            }

            return GetPathFigureFromPoints(aoPoints[0], aoPoints[1], aoPoints[2]);
        }

        /// <summary>
        /// Returns matrix rotated around specified point by angle in degrees.
        /// </summary>
        public static Matrix GetRotatedMatrix(Point centerOfRotation, double angleToRotateDegrees)
        {
            var oMatrix = Matrix.Identity;
            oMatrix = oMatrix.RotateAt(angleToRotateDegrees, centerOfRotation.X, centerOfRotation.Y);
            return oMatrix;
        }

        public static PathFigure GetPathFigureFromPoints(Point startPoint, params Point[] otherPoints)
        {
            var oPathFigure = new PathFigure { StartPoint = startPoint };
            var oPathSegmentCollection = new PathSegments();

            foreach (var item in otherPoints)
                oPathSegmentCollection.Add(new LineSegment { Point = item });

            oPathFigure.Segments = oPathSegmentCollection;
            oPathFigure.IsClosed = true;
            return oPathFigure;
        }

        public static PathGeometry GetPathGeometryFromPoints(Point startPoint, params Point[] otherPoints)
        {
            Debug.Assert(otherPoints != null);
            var iOtherPoints = otherPoints.Length;
            Debug.Assert(iOtherPoints > 0);

            var oPathFigure = new PathFigure { StartPoint = startPoint };
            var oPathSegmentCollection = new PathSegments();

            foreach (var item in otherPoints)
                oPathSegmentCollection.Add(new LineSegment { Point = item });

            oPathFigure.Segments = oPathSegmentCollection;
            oPathFigure.IsClosed = true;

            var oPathGeometry = new PathGeometry();
            oPathGeometry.Figures.Add(oPathFigure);

            return oPathGeometry;
        }
    }
}