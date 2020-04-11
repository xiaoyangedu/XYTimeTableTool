using OSKernel.Presentation.Arranging.Administrative.Dialog;
using OSKernel.Presentation.Arranging.Dialog;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models;
using OSKernel.Presentation.Models.Administrative;
using OSKernel.Presentation.Models.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;
using XYKernel.OS.Common.Models.Administrative;

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Views
{
    public class CourseSettingViewModel : CommonViewModel, IInitilize
    {
        private ObservableCollection<UIClass> _classes;

        private List<UIClassHourCount> _classHourCounts;

        /// <summary>
        /// 创建班级
        /// </summary>
        public ICommand CreateCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(createCommand);
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(deleteCommand);
            }
        }

        public ICommand CreateCourseCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(createCourseCommand);
            }
        }

        public ICommand DeleteCourseCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(deleteCourseCommand);
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(saveCommand);
            }
        }

        public ICommand ClassHourDetailsCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(classHourDetailsCommand);
            }
        }

        public ICommand SelectClassHourCountCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(selectClassHourCountCommand);
            }
        }

        public ICommand ChooseTeacherCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(chooseTeacherCommand);
            }
        }

        public ICommand UniformCreateCourseCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(uniformCreateCourseCommand);
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

        public ICommand UniformClearCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(uniformClearCommand);
            }
        }

        public ICommand StudentsCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(ViewStudents);
            }
        }

        public ObservableCollection<UIClass> Classes
        {
            get
            {
                return _classes;
            }

            set
            {
                _classes = value;
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

        /// <summary>
        /// 显示操作模板
        /// </summary>
        public bool ShowOperationPanel
        {
            get
            {
                if (this.Classes.Count > 0)
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
                return this.Classes.Any(c => c.Courses?.Count > 0);
            }
        }

        public CourseSettingViewModel()
        {
            this.Classes = new ObservableCollection<UIClass>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<UICourse>(this, Receive);

            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<UITeacher>(this, ReceiveTeacher);

            var cpCase = CommonDataManager.GetCPCase(base.LocalID);

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

            cpCase.Classes.ForEach(c =>
            {
                UIClass classModel = new UIClass()
                {
                    ID = c.ID,
                    Name = c.Name,
                };

                c.Settings.ForEach(set =>
                {
                    // 1.获取课程
                    var course = cpCase.Courses.FirstOrDefault(fc => fc.ID.Equals(set.CourseID));

                    // 2.获取教师
                    var teachers = (from t in cpCase.Teachers from st in set.TeacherIDs where st.Equals(t.ID) select t)?.ToList();

                    // 3.获取课时
                    var classHours = (from h in cpCase.ClassHours
                                      where h.ClassID == c.ID && h.CourseID == set.CourseID
                                      select new UIClassHour()
                                      {
                                          Class = c.Name,
                                          ClassID = c.ID,
                                          Course = course.Name,
                                          ID = h.ID,
                                          CourseID = course.ID,
                                          Teachers = cpCase.GetTeachersByIds(h.TeacherIDs),
                                      })?.ToList();

                    // 3.添加班级课程绑定
                    UIClassCourse classCourse = new UIClassCourse();
                    classCourse.ClassID = c.ID;
                    classCourse.ClassName = c.Name;
                    classCourse.CourseID = set.CourseID;
                    classCourse.Course = course?.Name;
                    classCourse.Lessons = set.Lessons;
                    classCourse.Teachers = teachers;
                    classCourse.ClassHours = classHours;


                    classModel.Courses.Add(classCourse);
                });

                this.Classes.Add(classModel);
            });
        }

        void createCommand()
        {
            CreateClassWindow window = new CreateClassWindow();
            window.Closed += (s, arg) =>
            {
                if (window.DialogResult.Value)
                {
                    var cp = CommonDataManager.GetCPCase(base.LocalID);

                    var has = this.Classes.Any(c =>
                    {
                        return window.Classes.Any(cc => cc.Equals(c.Name));
                    });

                    if (has)
                    {
                        var result = this.ShowDialog("提示信息", "存在相同班级,是否继续添加?", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
                        if (result != CustomControl.Enums.DialogResultType.OK)
                        {
                            return;
                        }
                    }

                    foreach (var c in window.Classes)
                    {
                        var alreadyIn = this.Classes.Any(sc => sc.Name.Equals(c));
                        if (!alreadyIn)
                        {
                            var classID = this.Classes.Count == 0 ? 0 : this.Classes.Max(cc => Convert.ToInt64(cc.ID));
                            string id = (classID + 1).ToString();
                            this.Classes.Add(new UIClass
                            {
                                ID = id,
                                Name = c
                            });

                            if (!cp.Classes.Any(cc => cc.Name.Equals(c)))
                            {
                                cp.Classes.Add(new ClassModel()
                                {
                                    ID = id,
                                    Name = c,
                                    Students = new List<StudentModel>()
                                });
                            }
                        }
                    }

                    this.RaisePropertyChanged(() => ShowOperationPanel);

                }
            };
            window.ShowDialog();
        }

        void ViewStudents(object obj)
        {
            UIClass classModel = obj as UIClass;

            StudentWindow window = new StudentWindow(classModel);
            window.ShowDialog();
        }

        void Receive(UICourse course)
        {
            foreach (var c in this.Classes)
            {
                var removeItem = c.Courses?.FirstOrDefault(cc => cc.CourseID.Equals(course.ID));
                if (removeItem != null)
                    c.Courses.Remove(removeItem);
            }
        }

        void ReceiveTeacher(UITeacher teacher)
        {
            foreach (var c in this.Classes)
            {
                if (c.Courses != null)
                {
                    foreach (var course in c.Courses)
                    {
                        course.Teachers?.RemoveAll(t => t.ID.Equals(teacher.ID));
                        course.RaiseChanged();
                    }
                }
            }
        }

        List<UIClass> removeClasses = new List<UIClass>();
        void deleteCommand(object obj)
        {
            var confirm = this.ShowDialog("提示信息", "确认删除?", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
            if (confirm == CustomControl.Enums.DialogResultType.OK)
            {
                UIClass uiClass = obj as UIClass;
                this.Classes.Remove(uiClass);
                removeClasses.Add(uiClass);

                this.RaisePropertyChanged(() => ShowOperationPanel);
            }
        }

        void createCourseCommand(object obj)
        {
            UIClass uiClass = obj as UIClass;

            var currentCourses = uiClass.Courses.Select(sc =>
              {
                  return new UICourse()
                  {
                      ID = sc.CourseID,
                      Name = sc.Course
                  };
              })?.ToList();

            // 获取所有课程
            var cpCase = CommonDataManager.GetCPCase(base.LocalID);
            var uiCourses = cpCase.Courses?.Select(s =>
            {
                return new UICourse()
                {
                    ID = s.ID,
                    Name = s.Name
                };
            })?.ToList();

            ChooseCourseWindow chooseWindow = new ChooseCourseWindow(currentCourses, uiCourses);
            chooseWindow.Closed += (s, arg) =>
            {
                if (chooseWindow.DialogResult.Value)
                {
                    chooseWindow.Courses.ForEach(c =>
                    {
                        var has = uiClass.Courses.Any(uc => uc.Course.Equals(c.Name));
                        if (!has)
                        {
                            var classCourse = new Models.Administrative.UIClassCourse
                            {
                                ClassID = uiClass.ID,
                                ClassName = uiClass.Name,
                                CourseID = c.ID,
                                Course = c.Name,
                            };

                            this.createClassHours(classCourse);
                            uiClass.Courses.Add(classCourse);
                        }
                    });

                    this.RaisePropertyChanged(() => ShowUniform);
                }
            };
            chooseWindow.ShowDialog();
        }

        void deleteCourseCommand(object obj)
        {
            var confirm = this.ShowDialog("提示信息", "确认删除?", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
            if (confirm == CustomControl.Enums.DialogResultType.OK)
            {
                UIClassCourse model = obj as UIClassCourse;
                var classModel = this.Classes.FirstOrDefault(c => c.ID.Equals(model.ClassID));
                if (classModel != null)
                {
                    classModel.Courses.Remove(model);
                    this.RaisePropertyChanged(() => ShowUniform);
                }
            }
        }

        void classHourDetailsCommand(object obj)
        {
            UIClassCourse classCourse = obj as UIClassCourse;

            int no = 0;
            classCourse.ClassHours.ForEach(ch =>
            {
                ch.No = ++no;
            });

            ClassHourSettingWindow classsHourWindow = new ClassHourSettingWindow(classCourse.ClassHours);
            classsHourWindow.ShowDialog();
        }

        void selectClassHourCountCommand(object obj)
        {
            UIClassCourse classCourse = obj as UIClassCourse;
            this.createClassHours(classCourse);
        }

        void chooseTeacherCommand(object obj)
        {
            UIClassCourse classCourse = obj as UIClassCourse;

            var cpCase = CommonDataManager.GetCPCase(base.LocalID);

            var currentTeachers = classCourse.Teachers.Select(s =>
              {
                  return new UITeacher
                  {
                      ID = s.ID,
                      Name = s.Name
                  };
              })?.ToList();

            var allTeachers = cpCase.Teachers.Select(s =>
            {
                return new UITeacher
                {
                    ID = s.ID,
                    Name = s.Name
                };
            })?.ToList();

            ChooseTeacherWindow window = new ChooseTeacherWindow(currentTeachers, allTeachers);
            window.Closed += (s, arg) =>
            {
                if (window.DialogResult.Value)
                {
                    // 清空教师
                    classCourse.Teachers.Clear();

                    var selectedTeachers = window.Teachers.Select(st =>
                    {
                        return new XYKernel.OS.Common.Models.Administrative.TeacherModel()
                        {
                            ID = st.ID,
                            Name = st.Name
                        };
                    })?.ToList();
                    classCourse.Teachers = selectedTeachers;
                    classCourse.RaiseChanged();

                    this.createClassHours(classCourse);
                }
            };
            window.ShowDialog();
        }

        void uniformCreateCourseCommand()
        {
            // 获取所有课程
            var cpCase = CommonDataManager.GetCPCase(base.LocalID);
            var uiCourses = cpCase.Courses?.Select(s =>
            {
                return new UICourse()
                {
                    ID = s.ID,
                    Name = s.Name
                };
            })?.ToList();

            ChooseCourseWindow chooseWindow = new ChooseCourseWindow(new List<UICourse>(), uiCourses);
            chooseWindow.Closed += (s, arg) =>
            {
                if (chooseWindow.DialogResult.Value)
                {
                    chooseWindow.Courses.ForEach(c =>
                    {
                        foreach (var cc in this.Classes)
                        {
                            var has = cc.Courses.Any(uc => uc.Course.Equals(c.Name));
                            if (!has)
                            {
                                var classCourse = new Models.Administrative.UIClassCourse
                                {
                                    ClassID = cc.ID,
                                    ClassName = cc.Name,
                                    CourseID = c.ID,
                                    Course = c.Name,
                                };
                                this.createClassHours(classCourse);
                                cc.Courses.Add(classCourse);
                            }
                        }
                    });
                    this.RaisePropertyChanged(() => ShowUniform);
                }
            };
            chooseWindow.ShowDialog();
        }

        void uniformClassHourCommand()
        {
            Administrative.Dialog.ChooseClassHourWindow window = new Administrative.Dialog.ChooseClassHourWindow();
            window.Closed += (s, arg) =>
            {
                if (window.DialogResult.Value)
                {
                    window.Courses?.ForEach(c =>
                    {
                        var mappings = (from cc in this.Classes from ccc in cc.Courses where ccc.CourseID.Equals(c.ID) select ccc);
                        if (mappings != null)
                        {
                            foreach (var m in mappings)
                            {
                                m.Lessons = c.Lessons;
                            }
                        }
                    });
                }
            };
            window.ShowDialog();
        }

        void uniformTeacherCommand()
        {
            var cp = CommonDataManager.GetCPCase(base.LocalID);

            List<UIClass> newClasses = this.Classes.Select(c =>
            {
                UIClass newClass = new UIClass()
                {
                    ID = c.ID,
                    Name = c.Name,
                    Course = c.Course,
                    CourseID = c.CourseID,
                    HasOperation = c.HasOperation,
                    IsChecked = c.IsChecked,
                };

                var courses = c.Courses?.Select(cc =>
                  {
                      return new UIClassCourse()
                      {
                          Course = cc.Course,
                          CourseID = cc.CourseID,
                          ClassID = cc.ClassID,
                          ClassName = cc.ClassName,
                          Teachers = cc.Teachers.Select(t =>
                          {
                              return new TeacherModel()
                              {
                                  ID = t.ID,
                                  Name = t.Name
                              };
                          })?.ToList()
                      };
                  })?.ToList();

                newClass.Courses = new ObservableCollection<UIClassCourse>(courses);
                return newClass;

            })?.ToList();

            BatchTeacherWindow window = new BatchTeacherWindow(newClasses);
            window.Closed += (s, arg) =>
            {
                if (window.DialogResult.Value)
                {
                    window.Classes.ForEach(c =>
                    {
                        var firstClass = this.Classes.FirstOrDefault(sc => sc.ID.Equals(c.ID));
                        if (firstClass != null)
                        {
                            var newList = (from sc in firstClass.Courses
                                           from tc in c.Courses
                                           where tc.CourseID.Equals(sc.CourseID)
                                           select new
                                           {
                                               sc,
                                               tc
                                           });

                            if (newList != null)
                            {
                                foreach (var l in newList)
                                {
                                    l.sc.Teachers = l.tc.Teachers;
                                    l.sc.RaiseChanged();
                                    this.createClassHours(l.sc);
                                }
                            }
                        }
                    });
                }
            };

            window.ShowDialog();

            //ChooseTeacherWindow window = new ChooseTeacherWindow(new List<UITeacher>(), allTeachers);
            //window.Closed += (s, arg) =>
            //{
            //    if (window.DialogResult.Value)
            //    {
            //        var selectedTeachers = window.Teachers.Select(st =>
            //        {
            //            return new XYKernel.OS.Common.Models.Administrative.TeacherModel()
            //            {
            //                ID = st.ID,
            //                Name = st.Name
            //            };
            //        })?.ToList();
            //        foreach (var cc in this.Classes)
            //        {
            //            foreach (var ccc in cc.Courses)
            //            {
            //                ccc.Teachers = selectedTeachers;
            //                ccc.RaiseChanged();

            //                this.createClassHours(ccc);
            //            }
            //        }
            //    }
            //};
            //window.ShowDialog();
        }

        void uniformClearCommand()
        {
            var confirm = this.ShowDialog("提示信息", "确认清除?", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
            if (confirm == CustomControl.Enums.DialogResultType.OK)
            {
                if (this.Classes != null)
                {
                    foreach (var c in this.Classes)
                    {
                        if (c.Courses != null)
                        {
                            foreach (var course in c.Courses)
                            {
                                foreach (var classHour in course.ClassHours)
                                {
                                    classHour.Teachers?.Clear();
                                    classHour.TeacherString = string.Empty;
                                }
                                course.Lessons = 5;
                                course.Teachers?.Clear();
                                course.TeacherString = string.Empty;
                                course.RaiseChanged();
                            }
                        }
                    }
                }
            }
        }

        void saveCommand()
        {
            var showConfirm = IsClearRules();
            if (showConfirm)
            {
                StringBuilder information = new StringBuilder();
                information.AppendLine("数据发生改变,保存后会清除课时相关规则,是否确认修改?");
                var confirm = this.ShowDialog("提示信息", information.ToString(), CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
                if (confirm != CustomControl.Enums.DialogResultType.OK)
                {
                    return;
                }

                AdministrativeDataHelper.ClassHourChanged(base.LocalID, CommonDataManager);
            }

            var cp = CommonDataManager.GetCPCase(base.LocalID);

            // 删除班级
            removeClasses.ForEach(rc =>
            {
                cp.Classes.RemoveAll(dc => dc.ID.Equals(rc.ID));
            });
            removeClasses.Clear();

            foreach (var c in this.Classes)
            {
                // 1.班级
                var model = new XYKernel.OS.Common.Models.Administrative.ClassModel()
                {
                    ID = c.ID,
                    Name = c.Name,
                };

                var sourceClass = cp.Classes.FirstOrDefault(cc => cc.ID.Equals(c.ID));
                if (sourceClass != null)
                {
                    model.Students = sourceClass.Students;
                    cp.Classes.Remove(sourceClass);
                }

                // 2.课程
                foreach (var cc in c.Courses)
                {
                    var ccModel = new XYKernel.OS.Common.Models.Administrative.ClassCourseModel()
                    {
                        CourseID = cc.CourseID,
                        Lessons = cc.Lessons,
                        TeacherIDs = cc.Teachers.Select(s => s.ID)?.ToList(),
                    };
                    model.Settings.Add(ccModel);
                }

                cp.Classes.Add(model);
            }

            #region 生成课时

            var hours = (from c in this.Classes
                         from cc in c.Courses
                         from hour in cc.ClassHours
                         select new
                         {
                             ClassID = c.ID,
                             CourseID = cc.CourseID,
                             Teachers = hour.Teachers?.Select(t => t.ID)?.ToList()

                         })?.ToList();


            cp.ClassHours.Clear();
            int no = 0;
            hours.ForEach(h =>
            {
                cp.ClassHours.Add(new XYKernel.OS.Common.Models.Administrative.ClassHourModel
                {
                    ID = ++no,
                    ClassID = h.ClassID,
                    CourseID = h.CourseID,
                    TeacherIDs = h.Teachers
                });
            });

            #endregion

            // 3.保存
            cp.Serialize(base.LocalID);

            this.ShowDialog("提示信息", "保存成功!", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
        }

        /// <summary>
        /// 生成课时
        /// </summary>
        /// <param name="classCourse"></param>
        void createClassHours(UIClassCourse classCourse)
        {
            // 清除
            classCourse.ClassHours.Clear();

            // 课时
            var lessons = classCourse.Lessons;
            for (int i = 1; i <= lessons; i++)
            {
                // 课时
                UIClassHour classHour = new UIClassHour()
                {
                    // 生成的时候当做序号使用
                    ID = i,
                    ClassID = classCourse.ClassID,
                    Class = classCourse.ClassName,
                    CourseID = classCourse.CourseID,
                    Course = classCourse.Course,
                    Teachers = classCourse.Teachers,
                    Active = true,
                };
                classCourse.ClassHours.Add(classHour);
            }
        }

        bool IsClearRules()
        {
            var cp = CommonDataManager.GetCPCase(base.LocalID);
            if (cp.Classes.Count != this.Classes.Count)
            {
                return true;
            }

            // 班没有发生改变
            var all = cp.Classes.All(c => this.Classes.Any(cc => cc.ID.Equals(c.ID)));
            if (!all)
                return true;

            // 班级科目改变
            var allCourse = cp.Classes.All(c =>
              {
                  var target = this.Classes.FirstOrDefault(cc => cc.ID.Equals(c.ID));
                  if (target == null)
                      return false;
                  else
                  {
                      if (target.Courses?.Count != c.Settings?.Count)
                          return false;

                      var hasAll = c.Settings.All(s => target.Courses.Any(tc => tc.CourseID.Contains(s.CourseID)));
                      if (!hasAll)
                          return false;

                      var hasAllLession = c.Settings.All(s =>
                      {
                          var courseInfo = target.Courses.FirstOrDefault(tc => tc.CourseID.Equals(s.CourseID));
                          if (courseInfo.Lessons == s.Lessons)
                              return true;
                          else
                              return false;
                      });
                      if (!hasAllLession)
                          return false;

                      return true;
                  }
              });

            if (!allCourse)
                return true;

            return false;
        }
    }
}
