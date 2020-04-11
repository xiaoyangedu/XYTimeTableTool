using OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Course.Model;
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

namespace OSKernel.Presentation.Arranging.Mixed.Dialog
{
    public class ChooseClassHourWindowModel : CommonViewModel, IInitilize
    {
        private List<UIClassHourCount> _classHours;
        private UIClassHourCount _selectClassHourCount;
        private List<UICourseLevelTree> _courseLevels;
        private bool _isCheckedAll;

        /// <summary>
        /// 课程班级列表
        /// </summary>
        public List<UICourseLevelTree> CourseLevels
        {
            get
            {
                return _courseLevels;
            }

            set
            {
                _courseLevels = value;
                RaisePropertyChanged(() => CourseLevels);
            }
        }

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

                this.CourseLevels.ForEach(c =>
                {
                    c.IsChecked = _isCheckedAll;
                });
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
                    this.CourseLevels.ForEach(cl =>
                    {
                        if (cl.IsChecked)
                            cl.Lessons = _selectClassHourCount.Lessons;
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
            this.CourseLevels = new List<UICourseLevelTree>();
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

            #region 获取所有科目

            // 1.绑定列表
            var courses = new List<UICourseLevelTree>();
            cl.Courses.ForEach(c =>
            {
                c.Levels?.ForEach(l =>
                {
                    UICourseLevelTree level = new UICourseLevelTree()
                    {
                        CourseID = c.ID,
                        Name = c.Name,
                        LevelID = l.ID,
                        LevelName = l.Name,
                        ClassHours = this.ClassHours.ToList(),
                        Lessons = 5
                    };
                    courses.Add(level);
                });
            });

            this.CourseLevels = courses;

            #endregion
        }

        void save(object obj)
        {
            ChooseClassHourWindow window = obj as ChooseClassHourWindow;
            //window.CourseLevels = this.CourseLevels;
            //window.IsSave = true;

            foreach (var cl in this.CourseLevels)
            {
                if (cl.IsChecked)
                {
                    if (window.CourseLevels == null) { window.CourseLevels = new List<UICourseLevelTree>(); }
                    if (window.CourseLevels.Any(ccl => ccl.LevelID == cl.LevelID && ccl.CourseID == cl.CourseID))
                    {
                        window.CourseLevels.Remove(window.CourseLevels.Find(cc => cc.LevelID == cl.LevelID && cc.CourseID == cl.CourseID));
                    }
                    window.CourseLevels.Add(cl);
                }
            }
            window.IsSave = true;

            // 清除选中状态
            this.CourseLevels.ForEach(c => c.IsChecked = false);

            this.ShowDialog("提示信息", "保存成功", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);

        }

        void cancel(object obj)
        {
            ChooseClassHourWindow window = obj as ChooseClassHourWindow;

            if (this.CourseLevels.Count(c => c.IsChecked) > 0)
            {
                if (window.CourseLevels == null) { window.CourseLevels = new List<UICourseLevelTree>(); }
                foreach (var cl in this.CourseLevels)
                {
                    if (cl.IsChecked)
                    {
                        if (window.CourseLevels.Any(ccl => ccl.LevelID == cl.LevelID && ccl.CourseID == cl.CourseID))
                        {
                            window.CourseLevels.Remove(window.CourseLevels.Find(cc => cc.LevelID == cl.LevelID && cc.CourseID == cl.CourseID));
                        }
                        window.CourseLevels.Add(cl);
                    }
                }
            }
            if (window.CourseLevels != null && window.CourseLevels.Count > 0)
            {
                window.CourseLevels.ForEach(cl => cl.IsChecked = true);
            } 

            window.DialogResult = window.IsSave;
        }
    }

    public class UICourseLevelTree : GalaSoft.MvvmLight.ObservableObject
    {
        private bool isChecked;

        private int _lessons;

        /// <summary>
        /// 选中状态
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                isChecked = value;
                RaisePropertyChanged(() => IsChecked);
            }
        }

        /// <summary>
        /// 课程ID
        /// </summary>
        public string CourseID { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 层名
        /// </summary>
        public string LevelName { get; set; }

        /// <summary>
        /// 层ID
        /// </summary>
        public string LevelID { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string Display
        {
            get
            {
                if (string.IsNullOrEmpty(LevelName))
                    return Name;
                else
                    return $"{Name}-{LevelName}";
            }
        }

        #region 集中设置课时使用

        /// <summary>
        /// 课时
        /// </summary>
        public List<UIClassHourCount> ClassHours { get; set; }

        /// <summary>
        /// 课时
        /// </summary>
        public int Lessons
        {
            get
            {
                return _lessons;
            }

            set
            {
                _lessons = value;
                RaisePropertyChanged(() => Lessons);
            }
        }

        #endregion
    }

}
