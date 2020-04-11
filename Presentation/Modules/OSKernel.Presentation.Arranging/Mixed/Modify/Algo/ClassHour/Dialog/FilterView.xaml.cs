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
using XYKernel.OS.Common.Models.Mixed;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Algo.ClassHour.Dialog
{
    /// <summary>
    /// FilterView.xaml 的交互逻辑
    /// </summary>
    public partial class FilterView : UserControl
    {
        ICommonDataManager _commonDataManager;
        IPatternDataManager _patternDataManager;

        public FilterView()
        {
            InitializeComponent();

            _commonDataManager = CacheManager.Instance.UnityContainer.Resolve<ICommonDataManager>();
            _patternDataManager = CacheManager.Instance.UnityContainer.Resolve<IPatternDataManager>();

            var local = _commonDataManager.GetLocalCase(_commonDataManager.LocalID);

            CLCase cl;
            if (local.Pattern == Models.Enums.PatternTypeEnum.None)
            {
                cl= _commonDataManager.GetCLCase(_commonDataManager.LocalID);
            }
            else
            {
                cl= _patternDataManager.GetCase(_commonDataManager.LocalID);
            }

            if (cl != null)
            {
                var classList = cl.GetClasses();
                classList.Insert(0, new Models.Mixed.UIClass()
                {
                    ID = string.Empty,
                    Name = string.Empty,
                });

                FilterHelper.Classes = classList;

                cmb_class.ItemsSource = classList;
                cmb_class.SelectedIndex = 0;


                var teachers = cl.Teachers.ToList();
                teachers.Insert(0, new XYKernel.OS.Common.Models.Mixed.TeacherModel
                {
                    ID = string.Empty,
                    Name = string.Empty
                });
                cmb_teacher.ItemsSource = teachers;
                cmb_teacher.SelectedIndex = 0;

                var courses = cl.Courses.ToList();
                courses.Insert(0, new XYKernel.OS.Common.Models.Mixed.CourseModel()
                {
                    ID = string.Empty,
                    Name = string.Empty
                });

                cmb_subject.ItemsSource = courses;
                cmb_subject.SelectedIndex = 0;

                var tagList = cl.Tags?.ToList();
                tagList.Insert(0, new XYKernel.OS.Common.Models.Mixed.TagModel()
                {
                    ID = string.Empty,
                    Name = string.Empty
                });

                cmb_tag.ItemsSource = tagList;
                cmb_tag.SelectedIndex = 0;
            }
        }
    }
}
