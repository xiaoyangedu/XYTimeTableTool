using OSKernel.Presentation.Arranging.Dialog;
using OSKernel.Presentation.Arranging.Mixed.Dialog;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Mixed;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Unity;
using XYKernel.OS.Common.Models.Mixed;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Views
{
    public class CourseViewModel : CommonViewModel, IInitilize, IRefresh
    {
        private ObservableCollection<UICourse> _courses;

        public ObservableCollection<UICourse> Courses
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

        public bool ShowCreateLevel
        {
            get
            {
                return this.Courses?.Count > 0;
            }
        }

        /// <summary>
        /// 创建科目
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

        public ICommand CreateLevelCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(createLevelCommand);
            }
        }

        public ICommand DeleteLevelCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(deleteLevelCommand);
            }
        }

        public ICommand CreateUniformLevelCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(createUniformLevelCommand);
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(saveCommand);
            }
        }

        public ICommand SetColorCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(setColorCommand);
            }
        }

        public ICommand ModifyLevelCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(modifyLevelCommand);
            }
        }

        public ICommand SystemCourseCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(systemCourseCommand);
            }
        }

        public CourseViewModel()
        {
            this.Courses = new ObservableCollection<UICourse>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            this.Courses.Clear();

            var cl = base.GetClCase(base.LocalID);
            var local = CommonDataManager.GetLocalCase(base.LocalID);

            cl.Courses.ForEach(c =>
            {
                UICourse course = new UICourse()
                {
                    ID = c.ID,
                    Name = c.Name,
                    Levels = new ObservableCollection<UILevel>()
                };

                var has = local.CourseColors.ContainsKey(c.ID);
                if (has)
                {
                    course.ColorString = local.CourseColors[c.ID];
                }

                if (c.Levels != null)
                {
                    // 查找空层
                    if (c.Levels.Any(l => l.ID.Equals("0")))
                    {
                        // 空层
                        course.DefaultLevel = new UILevel()
                        {
                            CourseID = course.ID,
                            ID = "0",
                            Name = string.Empty,
                            Lessons = 5
                        };
                    }
                    else
                    {
                        var levels = c.Levels.Select(l =>
                        {
                            return new UILevel()
                            {
                                CourseID = c.ID,
                                ID = l.ID,
                                Name = l.Name,
                                Lessons = l.Lessons,
                            };
                        });

                        if (levels != null)
                            course.Levels = new ObservableCollection<UILevel>(levels);
                    }
                }

                this.Courses.Add(course);
            });
        }

        void createCommand()
        {
            CreateCourseWindow window = new CreateCourseWindow();
            window.Closed += (s, arg) =>
            {
                if (window.DialogResult.Value)
                {
                    var has = this.Courses.Any(c =>
                    {
                        return window.Courses.Any(cc => cc.Equals(c.Name));
                    });

                    if (has)
                    {
                        var result = this.ShowDialog("提示信息", "存在相同科目,是否继续添加", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
                        if (result != CustomControl.Enums.DialogResultType.OK)
                        {
                            return;
                        }
                    }

                    var local = CommonDataManager.GetLocalCase(base.LocalID);
                    window.Courses.ForEach(c =>
                    {
                        var any = this.Courses.Any(cc => cc.Name.Equals(c));
                        if (!any)
                        {
                            var courseID = this.Courses.Count == 0 ? 0 : this.Courses.Max(cc => Convert.ToInt64(cc.ID));

                            var course = new UICourse
                            {
                                ID = (courseID + 1).ToString(),
                                Name = c,
                                DefaultLevel = new UILevel()
                                {
                                    ID = "0",
                                    Name = string.Empty,
                                    Lessons = 5
                                },
                                ColorString = "#000000"
                            };

                            this.Courses.Add(course);

                            var hasColor = local.CourseColors.ContainsKey(c);
                            if (!hasColor)
                            {
                                local.CourseColors.Add(course.ID, "#000000");
                            }
                            else
                            {
                                local.CourseColors[course.ID] = "#000000";
                            }
                        }

                    });
                    this.RaisePropertyChanged(() => ShowCreateLevel);
                }
            };
            window.ShowDialog();
        }

        List<UICourse> _toDeleteCourses = new List<UICourse>();
        void deleteCommand(object obj)
        {
            var result = this.ShowDialog("提示信息", "确认删除?", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
            if (result == CustomControl.Enums.DialogResultType.OK)
            {
                var local = CommonDataManager.GetLocalCase(base.LocalID);

                UICourse uiCourse = obj as UICourse;
                this.Courses.Remove(uiCourse);
                local.CourseColors.Remove(uiCourse.ID);
                local.Serialize();

                _toDeleteCourses.Add(uiCourse);
                this.RaisePropertyChanged(() => ShowCreateLevel);
            }
        }

        void createLevelCommand(object obj)
        {
            UICourse course = obj as UICourse;

            CreateLevelWindow levelWin = new CreateLevelWindow();
            levelWin.Closed += (s, arg) =>
            {
                if (levelWin.DialogResult.Value)
                {
                    var has = course.Levels.Any(c =>
                    {
                        return levelWin.Levels.Any(cc => cc.Equals(c.Name));
                    });

                    if (has)
                    {
                        var result = this.ShowDialog("提示信息", "存在相同层,是否继续添加", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
                        if (result != CustomControl.Enums.DialogResultType.OK)
                        {
                            return;
                        }
                    }

                    if (course.DefaultLevel != null)
                    {
                        // 删除默认层
                        course.DefaultLevel = null;
                    }

                    foreach (var l in levelWin.Levels)
                    {
                        var levelID = course.Levels.Count == 0 ? 0 : course.Levels.Max(m => Convert.ToInt64(m.ID));

                        var theSame = course.Levels.Any(ll => ll.Name.Equals(l));
                        if (!theSame)
                        {
                            course.Levels.Add(new UILevel
                            {
                                ID = (levelID + 1).ToString(),
                                CourseID = course.ID,
                                Name = l,
                                Lessons = 5
                            });
                        }
                    }
                }
            };
            levelWin.ShowDialog();
        }

        List<UILevel> _toDeleteLevels = new List<UILevel>();
        void deleteLevelCommand(object obj)
        {
            var dialog = this.ShowDialog("提示信息", "确认删除当前层?", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
            if (dialog == CustomControl.Enums.DialogResultType.OK)
            {
                UILevel level = obj as UILevel;
                var course = this.Courses.FirstOrDefault(c => c.ID.Equals(level.CourseID));
                if (course != null)
                {
                    var first = course.Levels.FirstOrDefault(l => l.ID.Equals(level.ID));
                    if (first != null)
                    {
                        course.Levels.Remove(first);

                        _toDeleteLevels.Add(first);

                        if (course.Levels.Count == 0)
                        {
                            // 恢复默认层
                            course.DefaultLevel = new UILevel()
                            {
                                CourseID = course.ID,
                                ID = "0",
                                Name = string.Empty,
                                Lessons = 5
                            };
                        }
                    }
                }
            }
        }

        void saveCommand()
        {
            if (_toDeleteCourses.Count > 0)
            {
                MixedDataHelper.CourseChanged(_toDeleteCourses, base.LocalID, CommonDataManager);
                _toDeleteCourses.Clear();
            }
            if (_toDeleteLevels.Count > 0)
            {
                MixedDataHelper.LevelChanged(_toDeleteLevels, base.LocalID, CommonDataManager);
                _toDeleteLevels.Clear();
            }

            var has = this.Courses.Any(c =>
              {
                  var groups = c.Levels.GroupBy(l => l.Name);

                  if (groups != null)
                  {
                      if (groups.Any(g => g.Count() > 1))
                          return true;
                      else
                          return false;
                  }
                  else
                  {
                      return false;
                  }
              });

            if (has)
            {
                var result = this.ShowDialog("提示信息", "同科目下存在相同的名称的层,是否确认继续?", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
                if (result != CustomControl.Enums.DialogResultType.OK)
                {
                    return;
                }
            }

            var cl = base.GetClCase(base.LocalID);
            cl.Courses.Clear();

            foreach (var c in this.Courses)
            {
                var model = new XYKernel.OS.Common.Models.Mixed.CourseModel()
                {
                    ID = c.ID,
                    Name = c.Name,
                    Levels = new List<XYKernel.OS.Common.Models.Mixed.LevelModel>()
                };

                if (c.Levels.Count > 0)
                {
                    model.Levels = c.Levels.Select(sl =>
                    {
                        return new XYKernel.OS.Common.Models.Mixed.LevelModel()
                        {
                            ID = sl.ID,
                            Lessons = sl.Lessons,
                            Name = sl.Name
                        };

                    })?.ToList();
                }
                else
                {
                    var classCount = cl.Classes.Count(cc => cc.CourseID.Equals(model.ID) && cc.LevelID.Equals("0"));
                    var classHourCount = classCount == 0 ? 0 : cl.GetClassHoursByCouresAndLevel(model.ID, "0").Count / classCount;

                    model.Levels.Add(new XYKernel.OS.Common.Models.Mixed.LevelModel()
                    {
                        // 插入空层
                        ID = "0",
                        Name = string.Empty,
                        Lessons = classHourCount == 0 ? 5 : classHourCount,
                    });
                }

                cl.Courses.Add(model);
            }

            // 3.保存
            base.Serialize(cl, LocalID);

            // 4.保存本地方案
            var local = CommonDataManager.GetLocalCase(base.LocalID);
            local.Serialize();

            this.ShowDialog("提示信息", "保存成功!", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
        }

        void createUniformLevelCommand()
        {
            CreateLevelWindow levelWin = new CreateLevelWindow();
            levelWin.Closed += (s, arg) =>
            {
                if (levelWin.DialogResult.Value)
                {
                    var has = this.Courses.Any(c =>
                    {
                        return c.Levels.Any(cl =>
                         {
                             return levelWin.Levels.Any(cc => cc.Equals(cl.Name));
                         });
                    });

                    if (has)
                    {
                        var result = this.ShowDialog("提示信息", "存在相同层,是否继续添加", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
                        if (result != CustomControl.Enums.DialogResultType.OK)
                        {
                            return;
                        }
                    }

                    foreach (var c in this.Courses)
                    {
                        if (c.DefaultLevel != null)
                        {
                            // 删除默认层
                            c.DefaultLevel = null;
                        }

                        foreach (var l in levelWin.Levels)
                        {
                            var levelID = c.Levels.Count == 0 ? 0 : c.Levels.Max(m => Convert.ToInt64(m.ID));

                            var theSame = c.Levels.Any(cl => cl.Name.Equals(l));
                            if (!theSame)
                            {
                                c.Levels.Add(new UILevel
                                {
                                    ID = (levelID + 1).ToString(),
                                    CourseID = c.ID,
                                    Name = l,
                                    Lessons = 5
                                });

                            }
                        }
                    }

                }
            };
            levelWin.ShowDialog();
        }

        void setColorCommand(object obj)
        {
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

        void modifyLevelCommand(object obj)
        {
            UILevel level = obj as UILevel;

            ModifyLevelWindow window = new ModifyLevelWindow(level.Name);
            window.Closed += (s, args) =>
            {
                if (window.DialogResult.Value)
                {
                    var course = this.Courses.FirstOrDefault(c => c.ID.Equals(level.CourseID));

                    var has = course.Levels.Any(cl => cl.Name.Equals(window.Level));
                    if (has)
                    {
                        this.ShowDialog("提示信息", "当前科目下存在该层!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                        return;
                    }
                    else
                    {
                        level.Name = window.Level;
                    }
                }
            };
            window.ShowDialog();
        }

        void systemCourseCommand()
        {
            SystemCourseWindow window = new SystemCourseWindow();
            window.Closed += (s, arg) =>
            {
                if (window.DialogResult.Value)
                {
                    var cl = CommonDataManager.GetCLCase(base.LocalID);

                    var courses = window.Courses.Where(sc => sc.IsChecked)?.ToList();
                    var has = this.Courses.Any(c =>
                    {
                        return courses.Any(cc => cc.Name.Equals(c.Name));
                    });

                    if (has)
                    {
                        var result = this.ShowDialog("提示信息", "存在相同科目,是否继续添加", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
                        if (result != CustomControl.Enums.DialogResultType.OK)
                        {
                            return;
                        }
                    }
                    var local = CommonDataManager.GetLocalCase(base.LocalID);
                    courses.ForEach(c =>
                    {
                        var any = this.Courses.Any(cc => cc.Name.Equals(c.Name));
                        if (!any)
                        {
                            var courseID = this.Courses.Count == 0 ? 0 : this.Courses.Max(cs => Convert.ToInt64(cs.ID));
                            string id = (courseID + 1).ToString();

                            // 更新UI
                            this.Courses.Add(new UICourse
                            {
                                ID = id,
                                Name = c.Name,
                                ColorString = c.ColorString,
                                DefaultLevel = new UILevel()
                                {
                                    ID = "0",
                                    Name = string.Empty,
                                    Lessons = 5
                                }
                            });

                            var hasColor = local.CourseColors.ContainsKey(id);
                            if (!hasColor)
                            {
                                local.CourseColors.Add(id, c.ColorString);
                            }
                            else
                            {
                                local.CourseColors[id] = c.ColorString;
                            }
                        }
                    });

                    local.Serialize();

                    this.RaisePropertyChanged(() => ShowCreateLevel);
                }
            };
            window.ShowDialog();
        }

        public void Refresh()
        {
            this.Initilize();
        }
    }
}
