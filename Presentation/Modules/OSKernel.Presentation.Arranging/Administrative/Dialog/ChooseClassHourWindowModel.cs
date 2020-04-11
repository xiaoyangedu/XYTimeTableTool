using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models;
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
    public class ChooseClassHourWindowModel : CommonViewModel, IInitilize
    {
        private List<UICourse> _courses;
        private List<UIClassHourCount> _classHours;
        private UIClassHourCount _selectClassHourCount;
        private bool _isCheckedAll;

        /// <summary>
        /// 是否全选
        /// </summary>
        public bool IsCheckedAll
        {
            get
            {
                return _isCheckedAll;
            }

            set
            {
                _isCheckedAll = value;
                RaisePropertyChanged(() => IsCheckedAll);

                this.Courses.ForEach(c =>
                {
                    c.IsChecked = _isCheckedAll;
                });
            }
        }


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

        /// <summary>
        /// 课时
        /// </summary>
        public List<UIClassHourCount> ClassHours
        {
            get
            {
                return _classHours;
            }

            set
            {
                _classHours = value;
                RaisePropertyChanged(() => ClassHours);
            }
        }

        /// <summary>
        /// 当前选择课时数
        /// </summary>
        public UIClassHourCount SelectClassHourCount
        {
            get
            {
                return _selectClassHourCount;
            }

            set
            {
                _selectClassHourCount = value;
                RaisePropertyChanged(() => SelectClassHourCount);

                if (_selectClassHourCount != null)
                {
                    this.Courses.ForEach(c =>
                    {
                        if (c.IsChecked) c.Lessons = _selectClassHourCount.Lessons;
                    });
                }

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

        public ChooseClassHourWindowModel()
        {
            this.Courses = new List<UICourse>();
            this.ClassHours = new List<UIClassHourCount>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            var cl = CommonDataManager.GetCLCase(base.LocalID);

            this.ClassHours = new List<UIClassHourCount>
            {
                new UIClassHourCount(){ Lessons=1 },
                new UIClassHourCount(){ Lessons=2 },
                new UIClassHourCount(){ Lessons=3 },
                new UIClassHourCount(){ Lessons=4 },
                new UIClassHourCount(){ Lessons=5 },
                new UIClassHourCount(){ Lessons=6 },
                new UIClassHourCount(){ Lessons=7 },
                new UIClassHourCount(){ Lessons=8 },
                new UIClassHourCount(){ Lessons=9 },
                new UIClassHourCount(){ Lessons=10 },
                new UIClassHourCount(){ Lessons=11 },
                new UIClassHourCount(){ Lessons=12 },
            };
            this.SelectClassHourCount = ClassHours[4];

            var model = CommonDataManager.GetCPCase(base.LocalID);

            this.Courses = model.Courses?.Select(s =>
            {
                return new UICourse()
                {
                    ID = s.ID,
                    Name = s.Name,
                    ClassHours = this.ClassHours.ToList(),
                    Lessons = 5,
                };
            })?.ToList();
        }

        void save(object obj)
        {
            ChooseClassHourWindow window = obj as ChooseClassHourWindow;
            foreach (var c in this.Courses)
            {
                if (c.IsChecked)
                {
                    if (window.Courses == null) { window.Courses = new List<UICourse>(); }
                    if (window.Courses.Any(cc => cc.ID == c.ID)) {
                        window.Courses.Remove(window.Courses.Find(cc => cc.ID == c.ID));
                    }
                    window.Courses.Add(c);
                }
            }
            window.IsSave = true;

            // 清除选中状态
            this.Courses.ForEach(c => c.IsChecked = false);

            this.ShowDialog("提示信息", "保存成功", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
        }

        void cancel(object obj)
        {
            ChooseClassHourWindow window = obj as ChooseClassHourWindow;
     
            if (this.Courses.Count(c => c.IsChecked) > 0)
            {
                if (window.Courses == null)
                {
                    window.Courses = new List<UICourse>();
                }

                foreach (var c in this.Courses)
                {
                    if (c.IsChecked)
                    {
                        if (window.Courses.Any(cc => cc.ID == c.ID))
                        {
                            window.Courses.Remove(window.Courses.Find(cc => cc.ID == c.ID));
                        }
                        window.Courses.Add(c);
                    }
                }
            }
            window.DialogResult = window.IsSave;
        }
    }
}
