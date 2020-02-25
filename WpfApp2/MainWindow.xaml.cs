using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CurvePlayground
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum Interpolation
        {
            LINEAR,
            SPLINE
        }
        enum Splines
        {
            CATMULL_ROM,
            BEZIER,
            NATURAL_CUBIC,
            BSPLINE,
            NURB,
            LAGRANGE
        }

        private Interpolation interpolation;
        private Splines spline;

        private int controlPoints = 2;
        SolidColorBrush aliceBlueBrush = new SolidColorBrush();
        SolidColorBrush cornflowerBlueBrush = new SolidColorBrush();
        SolidColorBrush lightPinkBrush = new SolidColorBrush();
        SolidColorBrush deepPinkBrush = new SolidColorBrush();
        List<ControlPointInfo> controlPointInfos = new List<ControlPointInfo>();


        private object movingObject;
        private double firstXPos, firstYPos;
        public MainWindow()
        {
            InitializeComponent();
            aliceBlueBrush.Color = Colors.AliceBlue;
            cornflowerBlueBrush.Color = Colors.CornflowerBlue;
            lightPinkBrush.Color = Colors.LightPink;
            deepPinkBrush.Color = Colors.DeepPink;
            ellipseStart.MouseEnter += Ellipse_MouseEnter;
            ellipseStart.MouseLeave += Ellipse_MouseLeave;
            ellipseStart.MouseMove += Ellipse_MouseMove;
            ellipseStart.MouseLeftButtonUp += Ellipse_MouseLeftButtonUp;
            ellipseStart.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;
            ellipseEnd.MouseEnter += Ellipse_MouseEnter;
            ellipseEnd.MouseLeave += Ellipse_MouseLeave;
            ellipseEnd.MouseMove += Ellipse_MouseMove;
            ellipseEnd.MouseLeftButtonUp += Ellipse_MouseLeftButtonUp;
            ellipseEnd.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;

            canvas.MouseMove += Ellipse_MouseMove;

            controlPointInfos.Add(new ControlPointInfo(ellipseStart, true));
            controlPointInfos.Add(new ControlPointInfo(ellipseEnd, false));
            ellipseStart.SetValue(Ellipse.FillProperty, lightPinkBrush);
            ellipseStart.SetValue(Ellipse.StrokeProperty, deepPinkBrush);

            interpolation = Interpolation.LINEAR;

            radioLinearInterpolation.Click += Radio_Click;
            radioSplineInterpolation.Click += Radio_Click;
            radioCatmullromSplines.Click += Radio_Click;
            radioBezierSplines.Click += Radio_Click;
            radioBSplines.Click += Radio_Click;
            radioNCSplines.Click += Radio_Click;
            radioNURBS.Click += Radio_Click;
            radioLagrange.Click += Radio_Click;
        }

        private void Radio_Click(object sender, RoutedEventArgs e)
        {
            if (sender == radioLinearInterpolation)
            {
                interpolation = Interpolation.LINEAR;
                groupBoxSplines.IsEnabled = false;
            }
            else if (sender == radioSplineInterpolation)
            {
                groupBoxSplines.IsEnabled = true;
                interpolation = Interpolation.SPLINE;
            }
            else if (sender == radioCatmullromSplines)
            {
                spline = Splines.CATMULL_ROM;
            }
            else if (sender == radioBezierSplines)
            {
                spline = Splines.BEZIER;
            }
            else if (sender == radioBSplines)
            {
                spline = Splines.BSPLINE;
            }
            else if (sender == radioNCSplines)
            {
                spline = Splines.NATURAL_CUBIC;
            }
            else if (sender == radioNURBS)
            {
                spline = Splines.NURB;
            }
            else if (sender == radioLagrange)
            {
                spline = Splines.LAGRANGE;
            }
            Redrawn();
        }

        private void Redrawn()
        {
            canvas.Children.Clear();
            foreach (var info in controlPointInfos)
            {
                if (info.selected)
                {
                    info.ellipse.SetValue(Ellipse.FillProperty, lightPinkBrush);
                    info.ellipse.SetValue(Ellipse.StrokeProperty, deepPinkBrush);
                }
                else
                {
                    info.ellipse.SetValue(Ellipse.FillProperty, aliceBlueBrush);
                    info.ellipse.SetValue(Ellipse.StrokeProperty, cornflowerBlueBrush);
                }
                canvas.Children.Add(info.ellipse);
            }
            if (interpolation == Interpolation.LINEAR) // LINEAR
            {

                List<Point> points = new List<Point>();
                for (int i = 0; i < controlPoints; i++)
                {
                    points.Add(new Point((double)controlPointInfos[i].ellipse.GetValue(Canvas.LeftProperty) + 3,
                        (double)controlPointInfos[i].ellipse.GetValue(Canvas.TopProperty) + 3));
                }

                var b = Linear.GetLinearApproximation(points.ToArray());

                PathFigure pf = new PathFigure(b.Points[0], new[] { b }, false);

                PathFigureCollection pfc = new PathFigureCollection();
                pfc.Add(pf);

                var pge = new PathGeometry();
                pge.Figures = pfc;

                Path p = new Path();
                p.Data = pge;
                p.Stroke = cornflowerBlueBrush;

                canvas.Children.Add(p);

            }
            else // SPLINE
            {
                List<Point> points = new List<Point>();
                for (int i = 0; i < controlPoints; i++)
                {
                    points.Add(new Point((double)controlPointInfos[i].ellipse.GetValue(Canvas.LeftProperty) + 3,
                        (double)controlPointInfos[i].ellipse.GetValue(Canvas.TopProperty) + 3));
                }
                PolyLineSegment b = null;



                if (spline == Splines.BEZIER)
                {
                    b = Bezier.GetBezierApproximation(points.ToArray(), 256);
                }
                else if (spline == Splines.LAGRANGE)
                {
                    b = Lagrange.GetLagrangeApproximation(points.ToArray(), 256);
                }
                else if (spline == Splines.CATMULL_ROM)
                {
                    b = CatmullRom.GetCatmullRomApproximation(points.ToArray(), 256);
                }


                PathFigure pf = new PathFigure(b.Points[0], new[] { b }, false);

                PathFigureCollection pfc = new PathFigureCollection();
                pfc.Add(pf);

                var pge = new PathGeometry();
                pge.Figures = pfc;

                Path p = new Path();
                p.Data = pge;
                p.Stroke = cornflowerBlueBrush;

                canvas.Children.Add(p);
            }
        }
        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine(e.LeftButton);
            if (e.LeftButton == MouseButtonState.Pressed && (movingObject != null))
            {
                Ellipse ellipse = movingObject as Ellipse;
                double newLeft = e.GetPosition(canvas).X - firstXPos - canvas.Margin.Left;
                // newLeft inside canvas right-border?
                if (newLeft > canvas.Margin.Left + canvas.ActualWidth - ellipse.ActualWidth)
                    newLeft = canvas.Margin.Left + canvas.ActualWidth - ellipse.ActualWidth;
                // newLeft inside canvas left-border?
                else if (newLeft < canvas.Margin.Left)
                    newLeft = canvas.Margin.Left;
                ellipse.SetValue(Canvas.LeftProperty, newLeft);

                double newTop = e.GetPosition(canvas).Y - firstYPos - canvas.Margin.Top;
                // newTop inside canvas bottom-border?
                if (newTop > canvas.Margin.Top + canvas.ActualHeight - ellipse.ActualHeight)
                    newTop = canvas.Margin.Top + canvas.ActualHeight - ellipse.ActualHeight;
                // newTop inside canvas top-border?
                else if (newTop < canvas.Margin.Top)
                    newTop = canvas.Margin.Top;
                ellipse.SetValue(Canvas.TopProperty, newTop);
            }
            Redrawn();
        }

        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("Ellipse_MouseLeftButtonDown");
            Ellipse ellipse = sender as Ellipse;
            firstXPos = e.GetPosition(ellipse).X;
            firstYPos = e.GetPosition(ellipse).Y;
            movingObject = sender;
        }

        private void Ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("Ellipse_MouseLeftButtonUp");

            movingObject = null;

            foreach (var info in controlPointInfos)
            {
                info.selected = false;
                info.ellipse.SetValue(Ellipse.FillProperty, aliceBlueBrush);
                info.ellipse.SetValue(Ellipse.StrokeProperty, cornflowerBlueBrush);
                if (info.ellipse == sender)
                {
                    info.selected = true;
                    info.ellipse.SetValue(Ellipse.FillProperty, lightPinkBrush);
                    info.ellipse.SetValue(Ellipse.StrokeProperty, deepPinkBrush);
                }
            }
        }


        private void canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ellipseStart.SetValue(Canvas.TopProperty, canvas.ActualHeight / 2);
            ellipseStart.SetValue(Canvas.LeftProperty, (double)0);
            ellipseEnd.SetValue(Canvas.TopProperty, canvas.ActualHeight / 2);
            ellipseEnd.SetValue(Canvas.LeftProperty, canvas.ActualWidth - ellipseEnd.ActualWidth);
        }

        private void buttonAddInterpolation_Click(object sender, RoutedEventArgs e)
        {
            if (controlPoints < 10)
            {
                controlPoints++;
                labelControlPoints.Content = "Control Points: " + controlPoints;
                buttonRemoveInterpolation.IsEnabled = true;
                if (controlPoints == 10)
                {
                    buttonAddInterpolation.IsEnabled = false;
                }
            }

            Ellipse ellipse = new Ellipse();
            ellipse.Height = 7;
            ellipse.Width = 7;
            ellipse.SetValue(Ellipse.FillProperty, aliceBlueBrush);
            ellipse.SetValue(Ellipse.StrokeProperty, cornflowerBlueBrush);
            ellipse.MouseEnter += Ellipse_MouseEnter;
            ellipse.MouseLeave += Ellipse_MouseLeave;
            ellipse.MouseMove += Ellipse_MouseMove;
            ellipse.MouseLeftButtonUp += Ellipse_MouseLeftButtonUp;
            ellipse.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;


            for (int i = 0; i < controlPointInfos.Count; i++)
            {
                if (controlPointInfos[i].selected)
                {
                    double left = (double)controlPointInfos[i].ellipse.GetValue(Canvas.LeftProperty);
                    double top = (double)controlPointInfos[i].ellipse.GetValue(Canvas.TopProperty);
                    double right;
                    double bottom;
                    if (i + 1 < controlPointInfos.Count)
                    {
                        right = (double)controlPointInfos[i + 1].ellipse.GetValue(Canvas.LeftProperty);
                        bottom = (double)controlPointInfos[i + 1].ellipse.GetValue(Canvas.TopProperty);
                    }
                    else
                    {
                        right = canvas.ActualWidth;
                        bottom = canvas.ActualHeight / 2;
                    }

                    ellipse.SetValue(Canvas.TopProperty, (top + bottom) / 2);
                    ellipse.SetValue(Canvas.LeftProperty, (left + right) / 2);

                    controlPointInfos[i].selected = false;
                    controlPointInfos.Insert(i + 1, new ControlPointInfo(ellipse, true));

                    Redrawn();
                    break;
                }
            }
        }

        private void Ellipse_MouseLeave(object sender, MouseEventArgs e)
        {
            Redrawn();
            
        }

        private void Ellipse_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Ellipse)sender).SetValue(Ellipse.FillProperty, lightPinkBrush);
            ((Ellipse)sender).SetValue(Ellipse.StrokeProperty, deepPinkBrush);
        }

        private void buttonDraw_Click(object sender, RoutedEventArgs e)
        {
            // 获取坐标后面+3是因为控制点的宽高都是7
            Redrawn();
            
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            
        }

        private void buttonRemoveInterpolation_Click(object sender, RoutedEventArgs e)
        {
            if (controlPoints > 2)
            {
                foreach (var info in controlPointInfos)
                {
                    if (info.selected)
                    {
                        controlPointInfos.Remove(info);
                        break;
                    }
                }
                controlPointInfos[0].selected = true;
                controlPoints--;
                labelControlPoints.Content = "Control Points: " + controlPoints;
                buttonAddInterpolation.IsEnabled = true;
                if (controlPoints == 2)
                {
                    buttonRemoveInterpolation.IsEnabled = false;
                }
            }
            Redrawn();
        }
    }
}
