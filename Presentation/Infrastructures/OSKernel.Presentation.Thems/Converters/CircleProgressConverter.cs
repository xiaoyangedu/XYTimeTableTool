using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;

namespace OSKernel.Presentation.Thems.Converters
{
    public class CircleProgressCenterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double Height = System.Convert.ToDouble(values[0]);
            double Width = System.Convert.ToDouble(values[1]);
            return new Point(Width / 2, Height / 2);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CircleProgressRadiusConverter : IValueConverter, IMultiValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double Length = System.Convert.ToDouble(value);
            return Length / 2-3;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double Length = System.Convert.ToDouble(values[0]);
            double Padding = System.Convert.ToDouble(values[1]);
            return Length / 2- Padding;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CircleProgressValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double Value = System.Convert.ToDouble(values[0]);
            double Minimum = System.Convert.ToDouble(values[1]);
            double Maximum = System.Convert.ToDouble(values[2]);
            double Height = System.Convert.ToDouble(values[3]);
            double Width = System.Convert.ToDouble(values[4]);
            double Radius = Math.Min(Height, Width) / 2;
            double Padding = System.Convert.ToDouble(values[5]);

            if (Value < Maximum)
            {
                PathGeometry PathGeometry = new PathGeometry();
                bool IsLargeArc = false;
                double Angel = (Value - Minimum) / (Maximum - Minimum) * 360D;
                if (Angel > 180) IsLargeArc = true;
                double BigR = Radius;
                double SmallR = Radius - Padding;
                Point CenterPoint = new Point(Radius, Radius);
                Point Firstpoint = GetPointByAngel(CenterPoint, BigR, 0);
                Point Secondpoint = GetPointByAngel(CenterPoint, BigR, Angel);
                Point Thirdpoint = GetPointByAngel(CenterPoint, SmallR, 0);
                Point Fourthpoint = GetPointByAngel(CenterPoint, SmallR, Angel);

                PathFigure PathFigure = new PathFigure
                {
                    //IsClosed = true,
                    StartPoint = Firstpoint
                };
                PathFigure.Segments.Add(
                    new ArcSegment
                    {
                        Point = Secondpoint,
                        IsLargeArc = IsLargeArc,
                        Size = new Size(BigR, BigR),
                        SweepDirection = SweepDirection.Clockwise
                    });
                PathFigure.Segments.Add(new LineSegment { Point = Fourthpoint });
                PathFigure.Segments.Add(
                    new ArcSegment
                    {
                        Point = Thirdpoint,
                        IsLargeArc = IsLargeArc,
                        Size = new Size(SmallR, SmallR),
                        SweepDirection = SweepDirection.Counterclockwise
                    });
                PathFigure.Segments.Add(new LineSegment { Point = Firstpoint });
                PathGeometry.Figures.Add(PathFigure);

                PathGeometry.Figures.Add(new PathFigure()
                {
                    StartPoint = new Point(0, 0),
                    Segments = { new LineSegment { Point = new Point(Width, Height) } },
                    IsClosed = true
                });

                return PathGeometry;
            }
            else
            {
                return new GeometryGroup()
                {
                    Children =
                    {
                        new EllipseGeometry(new Point(Radius, Radius), Radius, Radius),
                        new EllipseGeometry(new Point(Radius, Radius), Radius - Padding, Radius - Padding)
                    }
                };
            }
        }

        private Point GetPointByAngel(Point CenterPoint, double r, double angel)
        {
            return new Point
            {
                X = Math.Sin(angel * Math.PI / 180) * r + CenterPoint.X,
                Y = CenterPoint.Y - Math.Cos(angel * Math.PI / 180) * r
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
