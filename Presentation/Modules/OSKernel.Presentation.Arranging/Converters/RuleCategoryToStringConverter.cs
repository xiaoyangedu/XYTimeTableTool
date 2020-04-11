using OSKernel.Presentation.Models.Enums;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace OSKernel.Presentation.Arranging.Converters
{
    /// <summary>
    /// 规则类型枚举转化成字符串
    /// </summary>
    public class RuleCategoryToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                RuleCategoryEnum categoryType = (RuleCategoryEnum)value;
                return categoryType.GetLocalDescription();
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
