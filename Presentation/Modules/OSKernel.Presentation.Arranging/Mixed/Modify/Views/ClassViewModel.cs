using OSKernel.Presentation.Arranging.Dialog;
using OSKernel.Presentation.Arranging.Mixed.Dialog;
using OSKernel.Presentation.Arranging.Mixed.Modify.Views.Model;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models;
using OSKernel.Presentation.Models.Mixed;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;
using XYKernel.OS.Common.Models.Mixed;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Views
{
    public class ClassViewModel : CommonViewModel, IInitilize, IRefresh
    {
        ObservableCollection<UICourseLevel> _courseLevels;

        private List<UIClassHourCount> _classHourCounts;

        public ObservableCollection<UICourseLevel> CourseLevels
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

        public List<UIClassHourCount> ClassHourCounts
        {
            get
            {
                return _classHourCounts;
            }

            set
            {
                _classHourCounts = value;
                RaisePropertyChanged(() => ClassHourCounts);
            }
        }

        public ICommand CreateClassCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(createClassCommand);
            }
        }

        //public ICommand SelectClassHourCountCommand
        //{
        //    get
        //    {
        //        return new GalaSoft.MvvmLight.Command.RelayCommand<object>(selectClassHourCountCommand);
        //    }
        //}

        public ICommand ChooseTeacherCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(chooseTeacherCommand);
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(saveCommand);
            }
        }

        public ICommand DeleteCourseCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(deleteCourseCommand);
            }
        }

        public ICommand WindowCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<string>(windowCommand);
            }
        }

        public ICommand UniformClassCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(uniformClassCommand);
            }
        }

        public ICommand UniformClassHourCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(uniformClassHourCommand);
            }
        }

        public ICommand UniformTeacherCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(uniformTeacherCommand);
            }
        }

        public ICommand UniformCapacityCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(uniformCapacityCommand);
            }
        }

        /// <summary>
        /// 显示操作模板
        /// </summary>
        public bool ShowOperationPanel
        {
            get
            {
                if (this.CourseLevels.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 是否显示:
        /// </summary>
        public bool ShowUniform
        {
            get
            {
                return this.CourseLevels.Any(cl => cl.Classes?.Count > 0);
            }
        }

        public ClassViewModel()
        {
            this.ClassHourCounts = new List<UIClassHourCount>()
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

            this.CourseLevels = new ObservableCollection<UICourseLevel>();
        }

        public void Initilize()
        {
            var cl = base.GetClCase(base.LocalID);

            Task.Run(() =>
            {
                this.ShowLoading = true;

                GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    this.CourseLevels.Clear();
                });

                // 3.遍历课程
                cl.Courses.ForEach(c =>
                {
                    c.Levels.ForEach(l =>
                    {
                        var classCount = cl.Classes.Count(cc => cc.CourseID.Equals(c.ID) && cc.LevelID.Equals(l.ID));

                        var classHours = cl.GetClassHoursByCouresAndLevel(c.ID, l.ID);
                        var classHourCount = classCount == 0 ? 0 : classHours.Count;

                        UICourseLevel courseLevel = new UICourseLevel()
                        {
                            CourseID = c.ID,
                            Course = c.Name,
                            LevelID = l.ID,
                            Level = l.Name,
                            Lessons = classHourCount == 0 ? l.Lessons : classHourCount / classCount,
                        };

                        var filter = cl.Classes.Where(cc => cc.CourseID.Equals(c.ID) && cc.LevelID.Equals(l.ID))?.Select(cc =>
                        {
                            var uiClass = new UIClass()
                            {
                                ID = cc.ID,
                                LevelID = cc.LevelID,
                                Name = cc.Name,
                                Capacity = cc.Capacity,
                                CourseID = cc.CourseID,
                                StudentIDs = cc.StudentIDs,
                                TeacherIDs = cc.TeacherIDs
                            };

                            uiClass.TeacherString = (from t in cl.Teachers from tt in cc.TeacherIDs where tt.Equals(t.ID) select t.Name)?.Parse();
                            return uiClass;

                        })?.ToList();

                        // 过滤
                        if (filter != null)
                        {
                            courseLevel.Classes = new ObservableCollection<UIClass>(filter);
                        }

                        courseLevel.StudentCount = cl.Students.Where(sc => sc.Preselections.Any(p => p.CourseID == c.ID && p.LevelID == l.ID)).Count();

                        // 向层中添加
                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            this.CourseLevels.Add(courseLevel);
                        });
                    });
                });

                this.RaisePropertyChanged(() => ShowUniform);

                this.ShowLoading = false;
            });
        }

        void windowCommand(object obj)
        {
            var param = obj as string;

            if (param.Equals("loaded"))
            {
                Initilize();
            }
            else if (param.Equals("unloaded"))
            {

            }
            else if (param.Equals("closed"))
            {

            }
        }

        //void selectClassHourCountCommand(object obj)
        //{

        //}

        void chooseTeacherCommand(object obj)
        {
            UIClass model = obj as UIClass;

            var cl = base.GetClCase(base.LocalID);

            var allTeachers = cl.Teachers.Select(s =>
            {
                return new Models.Base.UITeacher()
                {
                    ID = s.ID,
                    Name = s.Name
                };
            })?.ToList();

            var teachers = (from ut in model.TeacherIDs
                            from t in cl.Teachers
                            where ut.Equals(t.ID)
                            select new Models.Base.UITeacher()
                            {
                                ID = t.ID,
                                Name = t.Name
                            })?.ToList();

            Arranging.Dialog.ChooseTeacherWindow window = new Arranging.Dialog.ChooseTeacherWindow(teachers, allTeachers);
            window.Closed += (s, arg) =>
            {
                if (window.DialogResult.Value)
                {
                    // 清空教师
                    model.TeacherIDs.Clear();
                    model.TeacherString = string.Empty;
                    if (window.Teachers.Count > 0)
                    {
                        // 选择名称
                        var selectedNames = window.Teachers.Select(st =>
                        {
                            return st.Name;
                        });
                        model.TeacherString = selectedNames?.Parse();

                        // 选择ID
                        var selectedIDs = window.Teachers.Select(st =>
                        {
                            return st.ID;
                        });
                        model.TeacherIDs = selectedIDs?.ToList();
                    }
                }
            };
            window.ShowDialog();
        }

        void createClassCommand(object obj)
        {
            UICourseLevel model = obj as UICourseLevel;

            Mixed.Dialog.CreateClassWindow win = new Mixed.Dialog.CreateClassWindow();
            win.Closed += (s, arg) =>
            {
                if (win.DialogResult.Value)
                {
                    var has = model.Classes.Any(c =>
                    {
                        return win.Classes.Any(cc => cc.Equals(c.Name));
                    });

                    if (has)
                    {
                        var result = this.ShowDialog("提示信息", "存在相同班级,是否继续添加", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
                        if (result != CustomControl.Enums.DialogResultType.OK)
                        {
                            return;
                        }
                    }
                    foreach (var c in win.Classes)
                    {
                        var courseLevels = (from l in this.CourseLevels from cl in l.Classes select cl)?.ToList();
                        var classID = courseLevels.Count == 0 ? 0 : courseLevels.Max(cc => Convert.ToInt64(cc.ID));

                        UIClass uiClass = new UIClass()
                        {
                            ID = (classID + 1).ToString(),
                            CourseID = model.CourseID,
                            Name = c,
                            Capacity = win.Capacity,
                            LevelID = model.LevelID
                        };
                        model.Classes.Add(uiClass);
                        model.RaiseChanged();
                    }

                    this.RaisePropertyChanged(() => ShowUniform);

                }
            };
            win.ShowDialog();
        }

        void deleteCourseCommand(object obj)
        {
            var result = this.ShowDialog("提示信息", "确认删除?", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
            if (result == CustomControl.Enums.DialogResultType.OK)
            {
                UIClass model = obj as UIClass;
                var courseLevel = this.CourseLevels.FirstOrDefault(cl => cl.CourseID.Equals(model.CourseID) && cl.LevelID.Equals(model.LevelID));
                if (courseLevel != null)
                {
                    courseLevel.Classes.Remove(model);
                    courseLevel.RaiseChanged();

                    this.RaisePropertyChanged(() => ShowUniform);
                }
            }
        }

        void uniformClassCommand()
        {
            // 1.检查是否存在学生志愿
            var clCase = CommonDataManager.GetCLCase(base.LocalID);
            var noSelection = clCase.Students.All(s => s.Preselections.Count == 0);
            if (noSelection)
            {
                this.ShowDialog("提示信息", "学生还没有志愿!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

            // 2.弹出班额
            Mixed.Dialog.AutoCreateClassWindow win = new Mixed.Dialog.AutoCreateClassWindow();
            win.Closed += (s, arg) =>
            {
                if (win.DialogResult.Value)
                {
                    // 班额
                    int capacity = win.Capacity;

                    var confirm = this.ShowDialog("提示信息", "自动分班会清除所有班级从新创建", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
                    if (confirm != CustomControl.Enums.DialogResultType.OK)
                    {
                        return;
                    }
                    foreach (var cl in this.CourseLevels)
                    {
                        cl.Classes.Clear();

                        //  获取选择该志愿的人数
                        int selectionCount = clCase.Students.Count(ss => ss.Preselections.Any(sp => sp.CourseID.Equals(cl.CourseID) && sp.LevelID.Equals(cl.LevelID)));

                        // 开班数
                        var classCount = (selectionCount + capacity - 1) / capacity;
                        if (classCount > 0)
                        {
                            for (int i = 1; i <= classCount; i++)
                            {
                                // 获取班级ID
                                var courseLevels = (from l in this.CourseLevels from ll in l.Classes select ll)?.ToList();
                                var classID = courseLevels.Count == 0 ? 0 : courseLevels.Max(cc => Convert.ToInt64(cc.ID));

                                UIClass uiClass = new UIClass()
                                {
                                    ID = (classID + 1).ToString(),
                                    CourseID = cl.CourseID,
                                    Name = i.ToString(),
                                    Capacity = capacity,
                                    LevelID = cl.LevelID
                                };
                                cl.Classes.Add(uiClass);
                                cl.RaiseChanged();
                            }
                        }

                    }

                    this.RaisePropertyChanged(() => ShowUniform);

                    //var has = this.CourseLevels.Any(cl => cl.Classes.Any(c =>
                    //  {
                    //      return win.Classes.Any(cc => cc.Equals(c.Name));
                    //  }));

                    //if (has)
                    //{
                    //    var result = this.ShowDialog("提示信息", "存在相同班级,是否继续添加", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
                    //    if (result != CustomControl.Enums.DialogResultType.OK)
                    //    {
                    //        return;
                    //    }
                    //}

                    //foreach (var c in win.Classes)
                    //{
                    //    foreach (var cl in this.CourseLevels)
                    //    {
                    //        var courseLevels = (from l in this.CourseLevels from ll in l.Classes select ll)?.ToList();
                    //        var classID = courseLevels.Count == 0 ? 0 : courseLevels.Max(cc => Convert.ToInt64(cc.ID));

                    //        UIClass uiClass = new UIClass()
                    //        {
                    //            ID = (classID + 1).ToString(),
                    //            CourseID = cl.CourseID,
                    //            Name = c,
                    //            Capacity = win.Capacity,
                    //            LevelID = cl.LevelID
                    //        };
                    //        cl.Classes.Add(uiClass);
                    //        cl.RaiseChanged();
                    //    }
                    //}

                    //this.RaisePropertyChanged(() => ShowUniform);
                }
            };
            win.ShowDialog();
        }

        void uniformClassHourCommand()
        {
            ChooseClassHourWindow window = new ChooseClassHourWindow();
            window.Closed += (s, arg) =>
            {
                if (window.DialogResult.Value)
                {
                    if (window.CourseLevels != null)
                    {
                        window.CourseLevels.ToList()?.ForEach(c =>
                        {
                            var courseLevel = this.CourseLevels.FirstOrDefault(cl => cl.CourseID.Equals(c.CourseID) && cl.LevelID.Equals(c.LevelID));
                            if (courseLevel != null)
                            {
                                courseLevel.Lessons = c.Lessons;
                            }
                        });
                    }
                }
            };
            window.ShowDialog();
        }

        void uniformTeacherCommand()
        {
            #region 原始

            var values = this.CourseLevels.Select(s =>
             {
                 UICourseLevel courseLevel = new UICourseLevel()
                 {
                     Course = s.Course,
                     CourseID = s.CourseID,
                     IsChecked = s.IsChecked,
                     Lessons = s.Lessons,
                     Level = s.Level,
                     LevelID = s.LevelID,
                     SelectClasses = s.SelectClasses,
                 };
                 var classes = s.Classes.Select(sc =>
                 {
                     return new UIClass()
                     {
                         Capacity = sc.Capacity,
                         Course = sc.Course,
                         CourseID = sc.CourseID,
                         Display = sc.Display,
                         HasOperation = sc.HasOperation,
                         ID = sc.ID,
                         IsChecked = sc.IsChecked,
                         Level = sc.Level,
                         LevelID = sc.LevelID,
                         Name = sc.LevelName,
                         NO = sc.NO,
                         TeacherIDs = sc.TeacherIDs.ToList()
                     };
                 });
                 courseLevel.Classes = new ObservableCollection<UIClass>(classes);
                 courseLevel.SelectClasses = courseLevel.Classes.Count;
                 return courseLevel;

             })?.ToList();

            #endregion

            #region 绑定

            var binds = this.CourseLevels.Select(s =>
            {
                UICourseLevel courseLevel = new UICourseLevel()
                {
                    Course = s.Course,
                    CourseID = s.CourseID,
                    IsChecked = s.IsChecked,
                    Lessons = s.Lessons,
                    Level = s.Level,
                    LevelID = s.LevelID,
                    SelectClasses = s.SelectClasses,
                };
                var classes = s.Classes.Select(sc =>
                {
                    return new UIClass()
                    {
                        Capacity = sc.Capacity,
                        Course = sc.Course,
                        CourseID = sc.CourseID,
                        Display = sc.Display,
                        HasOperation = sc.HasOperation,
                        ID = sc.ID,
                        IsChecked = sc.IsChecked,
                        Level = sc.Level,
                        LevelID = sc.LevelID,
                        Name = sc.LevelName,
                        NO = sc.NO,
                        TeacherIDs = sc.TeacherIDs.ToList()
                    };
                });
                courseLevel.Classes = new ObservableCollection<UIClass>(classes);
                courseLevel.SelectClasses = courseLevel.Classes.Count;
                return courseLevel;

            })?.ToList();

            #endregion

            BatchTeacherWindow window = new BatchTeacherWindow(values, binds);
            window.Closed += (s, arg) =>
            {
                if (window.DialogResult.Value)
                {
                    var cl = CommonDataManager.GetCLCase(base.LocalID);
                    var results = window.CourseLevels;

                    // 绑定结果
                    results.ForEach(r =>
                    {
                        var courseLevel = this.CourseLevels.FirstOrDefault(c => c.CourseID.Equals(r.CourseID) && c.LevelID.Equals(r.LevelID));
                        if (courseLevel != null)
                        {
                            foreach (var c in r.Classes)
                            {
                                var firstClass = courseLevel.Classes.FirstOrDefault(cc => cc.ID.Equals(c.ID));
                                firstClass.TeacherString = cl.GetTeachersByIds(c.TeacherIDs)?.Select(t => t.Name)?.Parse();
                                firstClass.TeacherIDs = c.TeacherIDs;
                            }
                        }
                    });
                }
            };
            window.ShowDialog();

            //var clModel = base.GetClCase(base.LocalID);
            //var allTeachers = clModel.Teachers.Select(s =>
            //{
            //    return new Models.Base.UITeacher()
            //    {
            //        ID = s.ID,
            //        Name = s.Name
            //    };
            //})?.ToList();

            //Arranging.Dialog.ChooseTeacherWindow window = new Arranging.Dialog.ChooseTeacherWindow(new List<Models.Base.UITeacher>(), allTeachers);
            //window.Closed += (s, arg) =>
            //{
            //    if (window.DialogResult.Value)
            //    {
            //        foreach (var cl in this.CourseLevels)
            //        {
            //            foreach (var cc in cl.Classes)
            //            {
            //                // 清空教师
            //                cc.TeacherIDs.Clear();
            //                if (window.Teachers.Count > 0)
            //                {
            //                    // 选择名称
            //                    var selectedNames = window.Teachers.Select(st =>
            //                    {
            //                        return st.Name;
            //                    });
            //                    cc.TeacherString = selectedNames?.Parse();

            //                    // 选择ID
            //                    var selectedIDs = window.Teachers.Select(st =>
            //                    {
            //                        return st.ID;
            //                    });
            //                    cc.TeacherIDs = selectedIDs?.ToList();
            //                }
            //            }
            //        }
            //    }
            //};
            //window.ShowDialog();
        }

        void uniformCapacityCommand(object obj)
        {
            // 班级
            UICourseLevel courseLevel = obj as UICourseLevel;

            SetCapacityWindow chooseClassHourWindow = new SetCapacityWindow();
            chooseClassHourWindow.Closed += (s, arg) =>
            {
                if (chooseClassHourWindow.DialogResult.Value)
                {
                    foreach (var c in courseLevel.Classes)
                    {
                        c.Capacity = chooseClassHourWindow.Capacity;
                    }
                }
            };
            chooseClassHourWindow.ShowDialog();
        }

        void saveCommand()
        {
            if (hasChanged())
            {
                MixedDataHelper.ClassHourChanged(base.LocalID, CommonDataManager);
            }

            // 层下是否存在重复班级
            var has = this.CourseLevels.Any(courseLevel =>
              {
                  var groups = courseLevel.Classes.GroupBy(c => c.Name);
                  if (groups.Any(g => g.Count() > 1))
                  {
                      return true;
                  }
                  else
                      return false;
              });

            if (has)
            {
                this.ShowDialog("提示信息", "同科同层下不能存在相同班级", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

            var cl = base.GetClCase(base.LocalID);

            var filters = this.CourseLevels.Where(c => c.Classes.Count > 0);

            if (filters != null)
            {
                // 清除班级信息
                cl.Classes.Clear();
                cl.ClassHours.Clear();

                int no = 0;
                foreach (var l in filters)
                {
                    if (l.Classes != null)
                    {
                        foreach (var c in l.Classes)
                        {
                            // 添加班级
                            cl.Classes.Add(new XYKernel.OS.Common.Models.Mixed.ClassModel()
                            {
                                Capacity = c.Capacity,
                                CourseID = c.CourseID,
                                ID = c.ID,
                                LevelID = c.LevelID,
                                Name = c.Name,
                                StudentIDs = c.StudentIDs,
                                TeacherIDs = c.TeacherIDs
                            });

                            for (int i = 0; i < l.Lessons; i++)
                            {
                                // 添加课时
                                var clasHour = new XYKernel.OS.Common.Models.Mixed.ClassHourModel()
                                {
                                    ID = ++no,
                                    CourseID = l.CourseID,
                                    ClassID = c.ID,
                                    LevelID = l.LevelID,
                                    TeacherIDs = c.TeacherIDs
                                };
                                cl.ClassHours.Add(clasHour);
                            }
                        }
                    }
                }

                // 4.序列化规则
                var serialize = this.CourseLevels.Select(l =>
                  {
                      return new SaveCourseLevel()
                      {
                          CourseID = l.CourseID,
                          LevelID = l.LevelID,
                          Lessons = l.Lessons
                      };
                  })?.ToList();

                //base.LocalID.MixedClassFile().SerializeObjectToJson(serialize);

                // 更改层下课时
                if (this.CourseLevels != null)
                {
                    foreach (var courseLevel in this.CourseLevels)
                    {
                        var scourse = cl.Courses.FirstOrDefault(cc => cc.ID.Equals(courseLevel.CourseID));
                        if (scourse != null)
                        {
                            var slevel = scourse.Levels.FirstOrDefault(ssl => ssl.ID.Equals(courseLevel.LevelID));
                            if (slevel != null)
                            {
                                slevel.Lessons = courseLevel.Lessons;
                            }
                        }
                    }
                }

                base.Serialize(cl, LocalID);
                this.ShowDialog("提示信息", "保存成功!", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
            }
        }

        bool hasChanged()
        {
            var cl = CommonDataManager.GetCLCase(base.LocalID);

            var has = this.CourseLevels.All(c =>
              {
                  return cl.Courses.Any(cc => cc.ID.Equals(c.CourseID)
                     && cc.Levels.Any(ll => ll.ID.Equals(c.LevelID) && c.Lessons == ll.Lessons));
              });

            if (!has)
                return true;

            var hasCourseLevel = this.CourseLevels.All(c =>
              {
                  var classes = cl.Classes.Where(clc => clc.CourseID.Equals(c.CourseID) && clc.LevelID.Equals(c.LevelID))?.ToList();

                  if (classes?.Count != c.Classes?.Count)
                      return false;

                  var allClass = c.Classes.All(clc =>
                    {
                        return classes.Any(cla => cla.Name.Equals(clc.Name));
                    });

                  if (!allClass)
                      return false;

                  return true;
              });
            if (!hasCourseLevel)
                return true;

            return false;
        }

        public void Refresh()
        {
            this.Initilize();
        }
    }
}
