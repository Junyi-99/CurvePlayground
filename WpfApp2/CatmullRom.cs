using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace CurvePlayground
{
    class CatmullRom
    {
        // Reference: https://en.wikipedia.org/wiki/Cubic_Hermite_spline#Catmull%E2%80%93Rom_spline
        private static Point[] p;
        private static double m(int k)
        {
            if (k < p.Length - 1)
                return (p[k + 1].Y - p[k - 1].Y) / (p[k + 1].X - p[k - 1].X);
            else
                return 0;
        }
      
        private static double h00(double t)
        {
            return (1 + 2 * t) * (1 - t) * (1 - t);
        }
        private static double h10(double t)
        {
            return t * (1 - t) * (1 - t);
        }
        private static double h01(double t)
        {
            return t * t * (3 - 2 * t);
        }
        private static double h11(double t)
        {
            return t * t * (t - 1);
        }
        public static double P(double x, Point k, Point k1, int pos, int n)
        {
            double t = (x - k.X) / (k1.X- k.X);
            if(pos >0 && pos <n-1 )
            {
                return h00(t) * k.Y + h10(t) * (k1.X - k.X) * m(pos) + h01(t) * k1.Y + h11(t) * (k1.X - k.X) * m(pos+1);
            }
            else
            {
                return h00(t) * k.Y + h10(t) * (k1.X - k.X) * 0 + h01(t) * k1.Y + h11(t) * (k1.X - k.X) * 0;
            }
        }
        public static PolyLineSegment GetCatmullRomApproximation(Point[] controlPoints, int outputSegmentCount)
        {
            p = controlPoints;
            double startingPoint = p[0].X;
            double range = p[p.Length - 1].X - startingPoint;
            Point[] points = new Point[outputSegmentCount + 1];
            int pos = 1;
            // startingPoint 到 startingPoint+range

            for (int i = 0; i <= outputSegmentCount; i++)
            {
                double t = (double)i / outputSegmentCount;
                double x = range * t + startingPoint;
                while (pos < controlPoints.Length && x > controlPoints[pos].X)
                {
                    pos++;
                }
                if (pos == controlPoints.Length)
                    break;


                points[i] = new Point(x, P(x, p[pos-1],p[pos], pos, p.Length));
            }
            return new PolyLineSegment(points, true);
        }
    }
}
