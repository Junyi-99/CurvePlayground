using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace CurvePlayground
{
    class BSpline
    {
        private static double C(double u)
        {
            return 0;
        }
        public static PolyLineSegment GetLagrangeApproximation(Point[] controlPoints, int outputSegmentCount)
        {
            double startingPoint = controlPoints[0].X;
            double range = controlPoints[controlPoints.Length - 1].X - startingPoint;
            Point[] points = new Point[outputSegmentCount + 1];
            for (int i = 0; i <= outputSegmentCount; i++)
            {
                double t = (double)i / outputSegmentCount;
                double x = t * range;
                points[i] = new Point(x + startingPoint, C(x + startingPoint));
            }
            return new PolyLineSegment(points, true);
        }
    }
}
