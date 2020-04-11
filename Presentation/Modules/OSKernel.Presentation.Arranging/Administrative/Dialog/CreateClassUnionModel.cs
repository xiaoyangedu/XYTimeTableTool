using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;

namespace OSKernel.Presentation.Arranging.Administrative.Dialog
{
    public class CreateClassUnionModel : CommonViewModel, IInitilize
    {
        private List<UICourse> _courses;

        private List<UIClass> _classes;

        private UICourse _selectCourse;

        /// <summary>
        /// 课程
        /// </summary>
        public List<UICourse> Courses
        {
            get
            {
                return _courses;
            }

            set
            {
                _courses = value;
                RaisePropertyChanged(() => Courses);
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

        public List<UIClass> Classes
        {
            get
            {
                return _classes;
            }

            set
            {
                _classes = value;
                RaisePropertyChanged(() => Classes);
            }
        }

        public UICourse SelectCourse
        {
            get
            {
                return _selectCourse;
            }

            set
            {
                _selectCourse = value;
                RaisePropertyChanged(() => SelectCourse);

                // 过滤班级
                if (value != null)
                {
                    this.changeCourse(value);
                }
            }
        }

        public CreateClassUnionModel()
        {
            this.Courses = new List<UICourse>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            var cp = CommonDataManager.GetCPCase(base.LocalID);
            if (cp == null)
                return;

            this.Courses = cp.Courses.Select(s =>
              {
                  return new UICourse()
                  {
                      ID = s.ID,
                      Name = s.Name,
                  };
              })?.ToList();
        }

        void changeCourse(UICourse course)
        {
            var cp = CommonDataManager.GetCPCase(base.LocalID);

            this.Classes = cp.GetClasses(course.ID);
        }

        void save(object obj)
        {
            var count = this.Classes.Count(c => c.IsChecked);
            if (count == 0)
            {
                this.ShowDialog("提示信息", "没有选择的班级", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }
            else if (count == 1)
            {
                this.ShowDialog("提示信息", "至少选择两个班级", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

            CreateClassUnion win = obj as CreateClassUnion;
            //win.IsSave = true;
            win.SelectCourse = this.SelectCourse;
            win.SelectClasses = this.Classes.Where(c => c.IsChecked)?.ToList();
            //this.ShowDialog("提示信息", "保存成功!", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);

            win.DialogResult = true;
        }

        void cancel(object obj)
        {
            CreateClassUnion win = obj as CreateClassUnion;
            win.DialogResult = false;
            //win.DialogResult = win.IsSave;
        }
    }

}
