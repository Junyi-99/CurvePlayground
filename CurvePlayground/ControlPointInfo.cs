using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace CurvePlayground
{
    class ControlPointInfo
    {
        public Ellipse ellipse; // 所属的 Ellipse
        public bool selected;
        public double x;
        public double y;
        public ControlPointInfo(Ellipse ellipse)
        {
            this.x = (double)ellipse.GetValue(Canvas.LeftProperty);
            this.y = (double)ellipse.GetValue(Canvas.TopProperty);
            this.ellipse = ellipse;
            this.selected = false;
        }
        public ControlPointInfo(Ellipse ellipse,bool selected)
        {
            this.x = (double)ellipse.GetValue(Canvas.LeftProperty);
            this.y = (double)ellipse.GetValue(Canvas.TopProperty);
            this.ellipse = ellipse;
            this.selected = selected;
        }
    }
}
