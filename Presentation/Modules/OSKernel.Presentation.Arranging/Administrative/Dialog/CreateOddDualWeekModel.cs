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
    public class CreateOddDualWeekModel : CommonViewModel, IInitilize
    {
        private List<UIClass> _classes;

        private List<UICourse> _dualCourses;

        private List<UICourse> _oddCourses;

        private List<UICourse> _sourceCourses;

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

        public List<UICourse> DualCourses
        {
            get
            {
                return _dualCourses;
            }

            set
            {
                _dualCourses = value;
                RaisePropertyChanged(() => DualCourses);
            }
        }

        public List<UICourse> OddCourses
        {
            get
            {
                return _oddCourses;
            }

            set
            {
                _oddCourses = value;
                RaisePropertyChanged(() => OddCourses);
            }
        }

        public CreateOddDualWeekModel()
        {
            this.Classes = new List<UIClass>();
            this.OddCourses = new List<UICourse>();
            this.DualCourses = new List<UICourse>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            var cp = CommonDataManager.GetCPCase(base.LocalID);
            if (cp == null)
                return;

            this.Classes = cp.Classes.Select(c =>
            {
                var model = new UIClass()
                {
                    ID = c.ID,
                    Name = c.Name,
                };

                c.Settings?.ForEach(s =>
                {
                    var classCourse = new Models.Administrative.UIClassCourse()
                    {
                        ClassID = c.ID,
                        ClassName = c.Name,
                        Lessons = s.Lessons,
                        CourseID = s.CourseID,
                    };
                    model.Courses.Add(classCourse);
                });

                return model;
            })?.ToList();

            this.Classes.ForEach(c =>
            {
                c.PropertyChanged += C_PropertyChanged;
            });
        }

        private void C_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UIClass model = sender as UIClass;

            if (e.PropertyName.Equals(nameof(model.IsChecked)))
            {
                var cp = CommonDataManager.GetCPCase(base.LocalID);

                var selectClasses = this.Classes.Where(c => c.IsChecked);
                if (selectClasses != null)
                {
                    var groups = (from c in selectClasses
                                  from cc in c.Courses
                                  from sc in cp.Courses
                                  where sc.ID.Equals(cc.CourseID)
                                  select new UICourse()
                                  {
                                      ID = cc.CourseID,
                                      Lessons = cc.Lessons,
                                      Name = sc.Name,
                                  })?.GroupBy(c => c.ID);

                    if (groups != null)
                    {
                        List<UICourse> courses = new List<UICourse>();
                        foreach (var c in groups)
                        {
                            var course = c.FirstOrDefault();
                            courses.Add(course);
                        }

                        _sourceCourses = courses.ToList();

                        #region 取消订阅事件

                        this.OddCourses.ForEach(oc =>
                        {
                            oc.PropertyChanged -= Oc_PropertyChanged;
                        });

                        this.DualCourses.ForEach(dc =>
                        {
                            dc.PropertyChanged -= Dc_PropertyChanged;
                        });

                        #endregion

                        this.OddCourses = courses.Select(c => c.GetInstance())?.ToList();
                        this.DualCourses = courses.Select(c => c.GetInstance())?.ToList();

                        #region 订阅事件

                        this.OddCourses.ForEach(oc =>
                        {
                            oc.PropertyChanged += Oc_PropertyChanged;
                        });

                        this.DualCourses.ForEach(dc =>
                        {
                            dc.PropertyChanged += Dc_PropertyChanged;
                        });

                        #endregion
                    }
                }
            }
        }

        private void Dc_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UICourse course = sender as UICourse;

            if (e.PropertyName.Equals(nameof(course.IsChecked)))
            {
                if (course.IsChecked)
                {
                    // 取消事件
                    this.OddCourses.ForEach(oc =>
                    {
                        oc.PropertyChanged -= Oc_PropertyChanged;
                    });

                    var selectOadd = this.OddCourses.FirstOrDefault(f => f.IsChecked);

                    // 注册事件
                    this.OddCourses = _sourceCourses.Where(sc => sc.Lessons == course.Lessons && !sc.ID.Equals(course.ID))?.Select(t => t.GetInstance())?.ToList();
                    this.OddCourses.ForEach(oc =>
                    {
                        oc.PropertyChanged += Oc_PropertyChanged;
                    });

                    if (selectOadd != null)
                    {
                        var find = this.OddCourses.FirstOrDefault(oc => oc.ID.Equals(selectOadd.ID));
                        if (find != null)
                        {
                            find.PropertyChanged -= Oc_PropertyChanged;
                            find.IsChecked = true;
                            find.PropertyChanged += Oc_PropertyChanged;
                        }
                    }
                }
            }
        }

        private void Oc_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UICourse course = sender as UICourse;

            if (e.PropertyName.Equals(nameof(course.IsChecked)))
            {
                if (course.IsChecked)
                {
                    // 注册事件
                    this.DualCourses.ForEach(dc =>
                    {
                        dc.PropertyChanged -= Dc_PropertyChanged;
                    });

                    var selectDual = this.DualCourses.FirstOrDefault(f => f.IsChecked);

                    this.DualCourses = _sourceCourses.Where(sc => sc.Lessons == course.Lessons && !sc.ID.Equals(course.ID))?.Select(t => t.GetInstance())?.ToList();

                    // 注册事件
                    this.DualCourses.ForEach(dc =>
                    {
                        dc.PropertyChanged += Dc_PropertyChanged;
                    });

                    if (selectDual != null)
                    {
                        var find = this.DualCourses.FirstOrDefault(oc => oc.ID.Equals(selectDual.ID));
                        if (find != null)
                        {
                            find.PropertyChanged -= Dc_PropertyChanged;
                            find.IsChecked = true;
                            find.PropertyChanged += Dc_PropertyChanged;
                        }
                    }
                }
            }
        }

        void save(object obj)
        {
            var count = this.Classes.Count(c => c.IsChecked);
            if (count == 0)
            {
                this.ShowDialog("提示信息", "没有选择的班级", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

            var oddCourse = this.OddCourses.FirstOrDefault(fd => fd.IsChecked);
            var dualCourse = this.DualCourses.FirstOrDefault(fd => fd.IsChecked);

            CreateOddDualWeek win = obj as CreateOddDualWeek;
            //win.IsSave = true;
            win.OddCourse = oddCourse;
            win.DualCourse = dualCourse;
            win.SelectClasses = this.Classes.Where(c => c.IsChecked)?.ToList();
            win.DialogResult = true;

            //this.ShowDialog("提示信息", "保存成功!", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
        }

        void cancel(object obj)
        {
            CreateOddDualWeek win = obj as CreateOddDualWeek;
            win.DialogResult = false;
            //win.DialogResult = win.IsSave;
        }
    }

}
