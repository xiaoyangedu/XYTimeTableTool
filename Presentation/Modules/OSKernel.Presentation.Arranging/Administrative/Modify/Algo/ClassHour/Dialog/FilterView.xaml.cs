using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.DataManager;
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
using Unity;
namespace OSKernel.Presentation.Arranging.Administrative.Modify.Algo.ClassHour.Dialog
{
    /// <summary>
    /// FilterView.xaml 的交互逻辑
    /// </summary>
    public partial class FilterView : UserControl
    {
        ICommonDataManager _commonDataManager;

        public FilterView()
        {
            InitializeComponent();

            _commonDataManager = CacheManager.Instance.UnityContainer.Resolve<ICommonDataManager>();

            var cp = _commonDataManager.GetCPCase(_commonDataManager.LocalID);
            if (cp != null)
            {
                var teachers = cp.Teachers.ToList();
                var firstTeacher = new XYKernel.OS.Common.Models.Administrative.TeacherModel
                {
                    ID = string.Empty,
                    Name = string.Empty
                };
                teachers.Insert(0, firstTeacher);

                cmb_teacher.ItemsSource = teachers;
                cmb_teacher.SelectedIndex = 0;

                var courses = cp.Courses.ToList();
                var firstCourse = new XYKernel.OS.Common.Models.Administrative.CourseModel()
                {
                    ID = string.Empty,
                    Name = string.Empty
                };
                courses.Insert(0, firstCourse);
                cmb_subject.ItemsSource = courses;
                cmb_subject.SelectedIndex = 0;

                var classes = cp.Classes.ToList();
                var firstClass = new XYKernel.OS.Common.Models.Administrative.ClassModel()
                {
                    ID = string.Empty,
                    Name = string.Empty
                };

                classes.Insert(0, firstClass);
                cmb_class.ItemsSource = classes;
                cmb_class.SelectedIndex = 0;
            }
        }
    }
}
