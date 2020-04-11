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

namespace OSKernel.Presentation.Arranging.Mixed.Dialog
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
                 new UICourse(){ IsChecked=true, Name="物理学考",ColorString="#1C86EE" },
                 new UICourse(){ IsChecked=true, Name="物理选考",ColorString="#000EB4"},

                 new UICourse(){ IsChecked=true, Name="化学学考",ColorString="#8B3E2F"},
                 new UICourse(){ IsChecked=true, Name="化学选考",ColorString="#FF5D7A"},

                 new UICourse(){ IsChecked=true, Name="生物学考",ColorString="#F22AD5" },
                 new UICourse(){ IsChecked=true, Name="生物选考",ColorString="#A85C9D" },

                 new UICourse(){ IsChecked=true, Name="历史学考",ColorString="#664AFF" },
                 new UICourse(){ IsChecked=true, Name="历史选考",ColorString="#8896FF" },

                 new UICourse(){ IsChecked=true, Name="地理学考",ColorString="#6FA896" },
                 new UICourse(){ IsChecked=true, Name="地理选考",ColorString="#00B179" },

                 new UICourse(){ IsChecked=true, Name="政治学考",ColorString="#44D820" },
                 new UICourse(){ IsChecked=true, Name="政治选考",ColorString="#2AB907" },

                 new UICourse(){ IsChecked=true, Name="技术学考",ColorString="#ED5D1C" },
                 new UICourse(){ IsChecked=true, Name="技术选考",ColorString="#C63E00" },
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
