using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Unity;

namespace OSKernel.Presentation.Arranging.Administrative.Dialog
{
    /// <summary>
    /// 内置课程
    /// </summary>
    public class SystemCourseWindowModel : CommonViewModel, IInitilize
    {
        private List<UICourse> _systemCourses;

        /// <summary>
        /// 系统内置课程
        /// </summary>
        public List<UICourse> SystemCourses
        {
            get
            {
                return _systemCourses;
            }

            set
            {
                _systemCourses = value;
                RaisePropertyChanged(() => SystemCourses);
            }
        }

        public SystemCourseWindowModel()
        {
            this.SystemCourses = new List<UICourse>()
            {
                 new UICourse(){ IsChecked=true, Lessons=5, Name="语文", ColorString="#009ACD" },
                 new UICourse(){ IsChecked=true, Lessons=5, Name="数学",ColorString="#008B8B" },
                 new UICourse(){ IsChecked=true, Lessons=5, Name="英语",ColorString="#00CD00" },

                 new UICourse(){ IsChecked=true, Lessons=5, Name="物理",ColorString="#1C86EE" },
                 new UICourse(){ IsChecked=true, Lessons=5, Name="化学" ,ColorString="#8B3E2F"},
                 new UICourse(){ IsChecked=true, Lessons=5, Name="生物",ColorString="#8B475D" },

                 new UICourse(){ IsChecked=true, Lessons=5, Name="历史",ColorString="#CD3333" },
                 new UICourse(){ IsChecked=true, Lessons=5, Name="地理",ColorString="#EE9572" },
                 new UICourse(){ IsChecked=true, Lessons=5, Name="政治",ColorString="#F5238D" },

                 new UICourse(){ IsChecked=true, Lessons=5, Name="音乐",ColorString="#F52337" },
                 new UICourse(){ IsChecked=true, Lessons=5, Name="体育",ColorString="#3DA26C" },
                 new UICourse(){ IsChecked=true, Lessons=5, Name="美术",ColorString="#4E8B9C" },
            };
        }

        public ICommand SaveCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(save);
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(cancel);
            }
        }

        public ICommand SetColorCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(setColorCommand);
            }
        }

        [InjectionMethod]
        public void Initilize()
        {

        }

        void setColorCommand(object obj)
        {
            return;

            UICourse model = obj as UICourse;
            var local = CommonDataManager.GetLocalCase(base.LocalID);

            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SolidBrush sb = new SolidBrush(colorDialog.Color);
                SolidColorBrush solidColorBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(sb.Color.A, sb.Color.R, sb.Color.G, sb.Color.B));

                var colorString = solidColorBrush.ToString();

                model.ColorString = colorString;

                var has = local.CourseColors.ContainsKey(model.ID);
                if (!has)
                {
                    local.CourseColors.Add(model.ID, colorString);
                }
                else
                {
                    local.CourseColors[model.ID] = colorString;
                }

                // 保存方案
                local.Serialize();
            }
        }

        void save(object obj)
        {
            SystemCourseWindow window = obj as SystemCourseWindow;
            window.Courses = this.SystemCourses.Where(sc => sc.IsChecked)?.ToList();
            window.DialogResult = true;
        }

        void cancel(object obj)
        {
            SystemCourseWindow window = obj as SystemCourseWindow;
            window.DialogResult = false;
        }
    }
}
