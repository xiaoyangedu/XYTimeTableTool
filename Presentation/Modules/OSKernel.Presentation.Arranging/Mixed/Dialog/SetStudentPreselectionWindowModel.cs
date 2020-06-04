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

namespace OSKernel.Presentation.Arranging.Mixed.Dialog
{
    public class SetStudentPreselectionWindowModel : CommonViewModel, IInitilize
    {
        List<UIPreselection> _preselections;

        /// <summary>
        /// 志愿
        /// </summary>
        public List<UIPreselection> Preselections
        {
            get
            {
                return _preselections;
            }

            set
            {
                _preselections = value;
                RaisePropertyChanged(() => Preselections);
            }
        }

        public SetStudentPreselectionWindowModel()
        {
            this.Preselections = new List<UIPreselection>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            var cl = CommonDataManager.GetCLCase(base.LocalID);

            var preselections = new List<UIPreselection>();
            cl.Courses.ForEach(course =>
            {
                if (course.Levels.Count > 0)
                {
                    course.Levels.ForEach(l =>
                    {
                        UIPreselection preselection = new UIPreselection()
                        {
                            LevelID = l.ID,
                            Level = l.Name,
                            CourseID = course.ID,
                            Course = course.Name,
                            IsChecked = false
                        };
                        preselections.Add(preselection);
                    });
                }
                else
                {
                    UIPreselection preselection = new UIPreselection()
                    {
                        CourseID = course.ID,
                        Course = course.Name,
                        IsChecked = false
                    };
                    preselections.Add(preselection);
                }
            });

            this.Preselections = preselections;
        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        public void BindData(UIStudent student)
        {
            student.Preselections?.ForEach(sp =>
            {
                var preselection = this.Preselections.FirstOrDefault(p => p.CourseID.Equals(sp.CourseID) && p.LevelID.Equals(sp.LevelID));
                if (preselection != null)
                {
                    preselection.IsChecked = true;
                }
            });
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

        void save(object obj)
        {
            var group = this.Preselections.Where(p => p.IsChecked).GroupBy(p => p.CourseID);

            var isSelectMoreLevel = group.Any(g => g.Count() > 1);
            if (isSelectMoreLevel)
            {
                this.ShowDialog("提示信息", "每个科目只能选择一个层!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

            SetStudentPreselectionWindow window = obj as SetStudentPreselectionWindow;

            // 清除
            window.Preselections = new List<UIPreselection>();

            // 添加
            this.Preselections.Where(p => p.IsChecked)?.ToList()?.ForEach(p =>
              {
                  UIPreselection uiPreselection = new UIPreselection()
                  {
                      Course = p.Course,
                      CourseID = p.CourseID,
                      IsChecked = p.IsChecked,
                      Level = p.Level,
                      LevelID = p.LevelID

                  };
                  window.Preselections.Add(uiPreselection);
              });

            window.DialogResult = true;
        }

        void cancel(object obj)
        {
            SetStudentPreselectionWindow window = obj as SetStudentPreselectionWindow;
            window.DialogResult = false;
        }
    }
}
