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
        // 选考
        private List<UICourse> _selectives;
        // 学考
        private List<UICourse> _academics;

        private bool _isAllChecked;

        private bool _isAcademicCheckedAll;

        private bool _isSelectiveCheckedAll;

        public bool IsAllChecked
        {
            get
            {
                return _isAllChecked;
            }

            set
            {
                _isAllChecked = value;
                RaisePropertyChanged(() => IsAllChecked);

                this.IsAcademicCheckedAll = _isAllChecked;
                this.IsSelectiveCheckedAll = _isAllChecked;
            }
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

        /// <summary>
        /// 选考
        /// </summary>
        public List<UICourse> Selectives
        {
            get
            {
                return _selectives;
            }

            set
            {
                _selectives = value;
                RaisePropertyChanged(() => Selectives);
            }
        }

        /// <summary>
        /// 学考
        /// </summary>
        public List<UICourse> Academics
        {
            get
            {
                return _academics;
            }

            set
            {
                _academics = value;
                RaisePropertyChanged(() => Academics);
            }
        }

        /// <summary>
        /// 学考
        /// </summary>
        public bool IsAcademicCheckedAll
        {
            get
            {
                return _isAcademicCheckedAll;
            }

            set
            {
                _isAcademicCheckedAll = value;
                RaisePropertyChanged(() => IsAcademicCheckedAll);

                this.Academics.ForEach(a => a.IsChecked = _isAcademicCheckedAll);
            }
        }

        /// <summary>
        /// 选考
        /// </summary>
        public bool IsSelectiveCheckedAll
        {
            get
            {
                return _isSelectiveCheckedAll;
            }

            set
            {
                _isSelectiveCheckedAll = value;
                RaisePropertyChanged(() => IsSelectiveCheckedAll);

                this.Selectives.ForEach(a => a.IsChecked = _isSelectiveCheckedAll);
            }
        }

        public SystemCourseWindowModel()
        {
            this.Academics = new List<UICourse>()
            {
                 new UICourse(){ IsChecked=false, Name="物理学考",ColorString="#1C86EE" },
                 new UICourse(){ IsChecked=false, Name="化学学考",ColorString="#8B3E2F"},
                 new UICourse(){ IsChecked=false, Name="生物学考",ColorString="#F22AD5" },
                 new UICourse(){ IsChecked=false, Name="历史学考",ColorString="#664AFF" },
                 new UICourse(){ IsChecked=false, Name="地理学考",ColorString="#6FA896" },
                 new UICourse(){ IsChecked=false, Name="政治学考",ColorString="#44D820" },
                 new UICourse(){ IsChecked=false, Name="技术学考",ColorString="#ED5D1C" },
            };

            this.Selectives = new List<UICourse>()
            {
                 new UICourse(){ IsChecked=false, Name="物理选考",ColorString="#000EB4"},
                 new UICourse(){ IsChecked=false, Name="化学选考",ColorString="#FF5D7A"},
                 new UICourse(){ IsChecked=false, Name="生物选考",ColorString="#A85C9D" },
                 new UICourse(){ IsChecked=false, Name="历史选考",ColorString="#8896FF" },
                 new UICourse(){ IsChecked=false, Name="地理选考",ColorString="#00B179" },
                 new UICourse(){ IsChecked=false, Name="政治选考",ColorString="#2AB907" },
                 new UICourse(){ IsChecked=false, Name="技术选考",ColorString="#C63E00" },
            };
        }

        [InjectionMethod]
        public void Initilize()
        {

        }

        void setColorCommand(object obj)
        {

        }

        void save(object obj)
        {
            SystemCourseWindow window = obj as SystemCourseWindow;

            List<UICourse> selectCourses = new List<UICourse>();

            var selects = this.Selectives.Where(sc => sc.IsChecked)?.ToList();
            selectCourses.AddRange(selects);

            var academics = this.Academics.Where(ad => ad.IsChecked)?.ToList();
            selectCourses.AddRange(academics);

            window.Courses = selectCourses;
            window.DialogResult = true;
        }

        void cancel(object obj)
        {
            SystemCourseWindow window = obj as SystemCourseWindow;
            window.DialogResult = false;
        }
    }
}
