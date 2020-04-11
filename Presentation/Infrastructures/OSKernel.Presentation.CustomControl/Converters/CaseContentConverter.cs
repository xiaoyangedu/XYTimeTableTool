using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace OSKernel.Presentation.CustomControl
{
    public class CaseContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var resources = Application.Current.Resources;

            if (value != null)
            {
                var isAuto = (bool)value;
                if (isAuto)
                {
                    return resources["CaseRunAutoProcess"];
                }
                else
                {
                    return resources["CaseRunProcess"];
                }
            }
            else
                return resources["CaseRunProcess"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
