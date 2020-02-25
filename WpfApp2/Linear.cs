using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace CurvePlayground
{
    class Linear
    {
        public static PolyLineSegment GetLinearApproximation(Point[] controlPoints)
        {
            return new PolyLineSegment(controlPoints, true);
        }
    }
}
