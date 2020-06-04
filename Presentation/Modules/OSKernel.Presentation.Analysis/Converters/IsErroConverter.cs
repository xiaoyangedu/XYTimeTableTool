using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace OSKernel.Presentation.Analysis.Converters
{
    public class IsErroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var isErro = bool.Parse(value.ToString());
                if (isErro)
                {
                    return (SolidColorBrush)Application.Current.FindResource("local_red");
                }
                else
                    return null;
            }
            else
                return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
