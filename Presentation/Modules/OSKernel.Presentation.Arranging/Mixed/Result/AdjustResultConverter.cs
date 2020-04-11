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
using XYKernel.OS.Common.Models.Mixed.Result;

namespace OSKernel.Presentation.Arranging.Mixed.Result
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
                var cl = commonData.GetCLCase(commonData.LocalID);
                var classHourInfo = cl.GetClassHours(new int[] { model.ClassHourId })?.FirstOrDefault();

                var teacherString = classHourInfo.Teachers?.Select(t => t.Name)?.Parse();

                brush = this.GetRandomColor(classHourInfo.CourseID);
                content = classHourInfo.ShortDisplay + (string.IsNullOrEmpty(teacherString) == true ? "" : "\n" + teacherString);
            }
            else
            {
                var classInfo = AdjustLogic.CurrentLocalResult.GetClassByClassHourID(model.ClassHourId);
                var baseClassInfo = AdjustLogic.CurrentLocalResult.Classes.FirstOrDefault(c => c.ID.Equals(classInfo.ID));
                if (baseClassInfo != null)
                {
                    var courseInfo = AdjustLogic.CurrentLocalResult.Courses.FirstOrDefault(c => c.ID.Equals(baseClassInfo.CourseID));

                    if (courseInfo != null)
                    {
                        brush = this.GetRandomColor(courseInfo.ID);

                        Models.Mixed.UIClassHour classHour = new Models.Mixed.UIClassHour();
                        classHour.Class = baseClassInfo.Name;
                        classHour.Course = courseInfo.Name;
                        classHour.Level = courseInfo.Levels.FirstOrDefault(cl => cl.ID.Equals(baseClassInfo.LevelID))?.Name;

                        var teacherString = AdjustLogic.CurrentLocalResult.GetTeachersByTeacherIDs(classInfo.TeacherIDs)?.Select(t => t.Name)?.Parse();
                        content = classHour.ShortDisplay + (string.IsNullOrEmpty(teacherString) == true ? "" : "\n" + teacherString);
                    }
                }
            }

            Label label = new Label();
            label.Background = brush;

            label.Foreground = new SolidColorBrush(Colors.White);
            label.Content = content;
            label.Opacity = 0.9;
            label.FontSize = 13;
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
