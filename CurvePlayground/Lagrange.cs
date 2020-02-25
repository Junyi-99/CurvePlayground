using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace CurvePlayground
{
    class Lagrange
    {
        public static double p(double x, int k, Point[] points)
        {
            double sum = 1;
            for (int i = 0; i < points.Length; i++)
            {
                if (i != k)
                {
                    sum *= (x - points[i].X) / (points[k].X - points[i].X);
                }
            }
            return sum;
        }
        public static double Lnx(double x,Point[] points)
        {
            double sum = 0;
            for (int j = 0; j < points.Length; j++)
            {
                sum += p(x, j, points) * points[j].Y;
            }
            return sum;
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
                points[i] = new Point(x+ startingPoint, Lnx(x+startingPoint, controlPoints));
            }
            return new PolyLineSegment(points, true);
        }
    }
}
