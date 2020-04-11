using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.DataManager;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Unity;
using XYKernel.OS.Common.Models.Administrative.Result;

namespace OSKernel.Presentation.Arranging.Administrative.Result
{
    /// <summary>
    /// 调整结果转换器
    /// </summary>
    public class AdjustResultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var model = value as ResultDetailModel;
            var commonData = CacheManager.Instance.UnityContainer.Resolve<ICommonDataManager>();
            var local = commonData.GetLocalCase(commonData.LocalID);
            _colors = local.CourseColors;

            SolidColorBrush brush = new SolidColorBrush();
            string content = string.Empty;
            if (AdjustLogic.CurrentLocalResult == null)
            {
                var cp = commonData.GetCPCase(commonData.LocalID);
                var classHourInfo = cp.GetClassHours(new int[] { model.ClassHourId })?.FirstOrDefault();

                brush = this.GetRandomColor(classHourInfo.CourseID);

                var teacherString = classHourInfo.Teachers?.Select(t => t.Name)?.Parse();

                content = string.IsNullOrEmpty(teacherString) ? classHourInfo.Course : classHourInfo.Course + "\n" + teacherString;
            }
            else
            {
                var course = AdjustLogic.CurrentLocalResult.Courses.FirstOrDefault(c => c.ID.Equals(model.CourseID));
                brush = this.GetRandomColor(course.ID);

                var teachers = AdjustLogic.CurrentLocalResult.GetTeachersByTeacherIDs(model.Teachers?.ToList());
                string teacherString = teachers.Select(t => t.Name)?.Parse();

                content = string.IsNullOrEmpty(teacherString) ? course.Name : course.Name + "\n" + teacherString;
            }

            Label label = new Label();
            label.Background = brush;
            label.Foreground = new SolidColorBrush(Colors.White);
            label.Content = content;
            label.Opacity = 0.9;
            label.FontSize = 15;
            label.Margin = new System.Windows.Thickness(2);
            return label;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        Dictionary<string, string> _colors = new Dictionary<string, string>();
        public SolidColorBrush GetRandomColor(string classKey)
        {
            if (_colors.ContainsKey(classKey))
            {
                return new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString(_colors[classKey]));
            }
            else
                return new SolidColorBrush(Colors.Black);
        }
    }
}
