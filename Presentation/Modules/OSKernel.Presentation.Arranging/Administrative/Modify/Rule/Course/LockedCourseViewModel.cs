using GalaSoft.MvvmLight.Messaging;
using OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Course.Model;
using OSKernel.Presentation.Arranging.Administrative.Result;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.Http;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Base;
using OSKernel.Presentation.Models.Enums;
using OSKernel.Presentation.Models.Result;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Unity;
using XYKernel.OS.Common.Models;
using XYKernel.OS.Common.Models.Administrative;
using XYKernel.OS.Common.Models.Administrative.Result;
using XYKernel.OS.Common.Models.Administrative.Rule;

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Course
{
    public class LockedCourseViewModel : CommonViewModel, IInitilize
    {
        private List<UIAdjustResultWeek> _adjusts;

        private List<UIClass> _classes;
        private UIClass _selectClass;

        private UIResult _selectResult;

        private ResultModel _resultModel;

        private List<UIResult> _results;
        public List<UIResult> Results
        {
            get
            {
                return _results;
            }

            set
            {
                _results = value;
                RaisePropertyChanged(() => Results);
            }
        }

        private Dictionary<string, string> _colors = new Dictionary<string, string>();

        private List<UILockedItem> _currentLockedItems;

        /// <summary>
        /// 选择结果
        /// </summary>
        public UIResult SelectResult
        {
            get
            {
                return _selectResult;
            }

            set
            {
                _selectResult = value;
                RaisePropertyChanged(() => SelectResult);
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(refreshCommand);
            }
        }

        public ICommand LockedCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(Locked);
            }
        }

        public ICommand UnLockedCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(UnLocked);
            }
        }

        public ICommand LockedCourseCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(LockedCourse);
            }
        }

        public ICommand UnLockedCourseCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(UnLockedCourse);
            }
        }

        public ICommand LockedTeacherCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(LockedTeacher);
            }
        }

        public ICommand UnLockedTeacherCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(UnLockedTeacher);
            }
        }

        public ICommand UserControlCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<string>(userControlCommand);
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

        /// <summary>
        /// 选中班级
        /// </summary>
        public UIClass SelectClass
        {
            get
            {
                return _selectClass;
            }

            set
            {
                _selectClass = value;
                RaisePropertyChanged(() => SelectClass);

                if (_selectClass == null)
                    return;

                this._currentLockedItems.Clear();

                // 清除调整记录
                this.Adjusts.ForEach(r =>
                {
                    r.Monday.Content = null;
                    r.Tuesday.Content = null;
                    r.Wednesday.Content = null;
                    r.Thursday.Content = null;
                    r.Friday.Content = null;
                    r.Saturday.Content = null;
                    r.Sunday.Content = null;
                });

                // 绑定当前班级结果
                var firstClass = _resultClasses.FirstOrDefault(f => f.ClassID.Equals(_selectClass.ID));
                if (firstClass != null)
                {
                    // 获取常规的结果
                    var normals = firstClass.ResultDetails?.Where(rd => rd.ResultType == XYKernel.OS.Common.Enums.ClassHourResultType.Normal)?.ToList();
                    normals?.ForEach(n =>
                    {
                        UILockedItem lockedItem = new UILockedItem()
                        {
                            DayPeriod = n.DayPeriod,
                            Class = this.SelectClass.Name,
                            Details = new List<ResultDetailModel>()
                            {
                                n
                            }
                        };

                        this._currentLockedItems.Add(lockedItem);

                        this.bindLockedState(lockedItem);

                        Label text = new Label();
                        text.HorizontalContentAlignment = HorizontalAlignment.Center;
                        text.VerticalContentAlignment = VerticalAlignment.Center;
                        text.VerticalAlignment = VerticalAlignment.Center;
                        text.Foreground = new SolidColorBrush(Colors.White);
                        text.FontWeight = FontWeights.Bold;
                        text.FontSize = 15;

                        var course = _courses.FirstOrDefault(c => c.ID.Equals(n.CourseID));
                        if (course != null)
                        {
                            text.Content = course.Name;
                        }

                        var teachers = this._resultModel.GetTeachersByTeacherIDs(n.Teachers?.ToList());
                        string teacherString = teachers.Select(t => t.Name)?.Parse();

                        Label teacherText = new Label();
                        teacherText.HorizontalContentAlignment = HorizontalAlignment.Center;
                        teacherText.VerticalContentAlignment = VerticalAlignment.Center;
                        teacherText.VerticalAlignment = VerticalAlignment.Center;
                        teacherText.Foreground = new SolidColorBrush(Colors.White);

                        if (string.IsNullOrEmpty(teacherString))
                        {
                            teacherText.Visibility = Visibility.Collapsed;
                        }

                        teacherText.Content = teacherString;
                        teacherText.ToolTip = teacherString;

                        var sp = createStackPanel(n.CourseID, lockedItem);
                        sp.Children.Add(text);
                        sp.Children.Add(teacherText);

                        StackPanel main = new StackPanel();
                        main.DataContext = lockedItem;
                        main.Children.Add(sp);

                        Image lockedImage = new Image();
                        lockedImage.Width = 32;
                        lockedImage.Height = 32;
                        lockedImage.Source = new BitmapImage(new Uri("pack://application:,,,/OSKernel.Presentation.Thems;Component/Images/locked.png", UriKind.RelativeOrAbsolute));
                        lockedImage.VerticalAlignment = VerticalAlignment.Bottom;
                        lockedImage.HorizontalAlignment = HorizontalAlignment.Right;
                        lockedImage.Margin = new Thickness(-32, -32, 5, 5);

                        Binding binding = new Binding("Locked")
                        {
                            Source = lockedItem,
                            Converter = new BooleanToVisibilityConverter()
                        };
                        lockedImage.SetBinding(Image.VisibilityProperty, binding);
                        main.Children.Add(lockedImage);

                        var position = this.Adjusts.FirstOrDefault(r => r.Period.Period == n.DayPeriod.Period);
                        if (position != null)
                        {
                            if (n.DayPeriod.Day == DayOfWeek.Sunday)
                            {
                                position.Sunday.Content = main;
                            }
                            else if (n.DayPeriod.Day == DayOfWeek.Saturday)
                            {
                                position.Saturday.Content = main;
                            }
                            else if (n.DayPeriod.Day == DayOfWeek.Friday)
                            {
                                position.Friday.Content = main;
                            }
                            else if (n.DayPeriod.Day == DayOfWeek.Thursday)
                            {
                                position.Thursday.Content = main;
                            }
                            else if (n.DayPeriod.Day == DayOfWeek.Wednesday)
                            {
                                position.Wednesday.Content = main;
                            }
                            else if (n.DayPeriod.Day == DayOfWeek.Tuesday)
                            {
                                position.Tuesday.Content = main;
                            }
                            else if (n.DayPeriod.Day == DayOfWeek.Monday)
                            {
                                position.Monday.Content = main;
                            }
                        }
                    });

                    var mulitplys = firstClass.ResultDetails?.Where(rd => rd.ResultType != XYKernel.OS.Common.Enums.ClassHourResultType.Normal)?.ToList();
                    var groups = mulitplys?.GroupBy(m => $"{m.DayPeriod.Day}{m.DayPeriod.Period}");
                    if (groups != null)
                    {
                        foreach (var g in groups)
                        {
                            var n = g.First();

                            var position = this.Adjusts.FirstOrDefault(r => r.Period.Period == n.DayPeriod.Period);
                            if (position != null)
                            {
                                StackPanel main = new StackPanel();
                                main.Margin = new Thickness(2);
                                main.Background = (SolidColorBrush)Application.Current.FindResource("main_lightgroud");

                                UILockedItem lockedItem = new UILockedItem()
                                {
                                    DayPeriod = n.DayPeriod,
                                    Class = this.SelectClass.Name,
                                };

                                this._currentLockedItems.Add(lockedItem);

                                if (n.DayPeriod.Day == DayOfWeek.Sunday)
                                {
                                    position.Sunday.Content = main;
                                }
                                else if (n.DayPeriod.Day == DayOfWeek.Saturday)
                                {
                                    position.Saturday.Content = main;
                                }
                                else if (n.DayPeriod.Day == DayOfWeek.Friday)
                                {
                                    position.Friday.Content = main;
                                }
                                else if (n.DayPeriod.Day == DayOfWeek.Thursday)
                                {
                                    position.Thursday.Content = main;
                                }
                                else if (n.DayPeriod.Day == DayOfWeek.Wednesday)
                                {
                                    position.Wednesday.Content = main;
                                }
                                else if (n.DayPeriod.Day == DayOfWeek.Tuesday)
                                {
                                    position.Tuesday.Content = main;
                                }
                                else if (n.DayPeriod.Day == DayOfWeek.Monday)
                                {
                                    position.Monday.Content = main;
                                }

                                g.ToList()?.ForEach(ig =>
                                {
                                    lockedItem.Details.Add(ig);

                                    Label text = new Label();
                                    text.HorizontalContentAlignment = HorizontalAlignment.Center;
                                    text.VerticalContentAlignment = VerticalAlignment.Center;
                                    text.VerticalAlignment = VerticalAlignment.Center;
                                    text.Foreground = new SolidColorBrush(Colors.White);
                                    text.FontWeight = FontWeights.Bold;
                                    text.FontSize = 15;

                                    var course = _courses.FirstOrDefault(c => c.ID.Equals(ig.CourseID));
                                    if (course != null)
                                    {
                                        text.Content = course.Name;
                                    }

                                    var teachers = this._resultModel.GetTeachersByTeacherIDs(n.Teachers?.ToList());
                                    string teacherString = teachers.Select(t => t.Name)?.Parse();

                                    Label teacherText = new Label();
                                    teacherText.HorizontalContentAlignment = HorizontalAlignment.Center;
                                    teacherText.VerticalContentAlignment = VerticalAlignment.Center;
                                    teacherText.VerticalAlignment = VerticalAlignment.Center;
                                    teacherText.Foreground = new SolidColorBrush(Colors.White);

                                    if (string.IsNullOrEmpty(teacherString))
                                    {
                                        teacherText.Visibility = Visibility.Collapsed;
                                    }

                                    teacherText.Content = teacherString;
                                    teacherText.ToolTip = teacherString;


                                    var sp = createStackPanel(ig.CourseID, lockedItem);
                                    sp.Children.Add(text);
                                    sp.Children.Add(teacherText);
                                    main.Children.Add(sp);
                                });

                                Image lockedImage = new Image();
                                lockedImage.Width = 32;
                                lockedImage.Height = 32;
                                lockedImage.Source = new BitmapImage(new Uri("pack://application:,,,/OSKernel.Presentation.Thems;Component/Images/locked.png", UriKind.RelativeOrAbsolute));
                                lockedImage.VerticalAlignment = VerticalAlignment.Bottom;
                                lockedImage.HorizontalAlignment = HorizontalAlignment.Right;
                                lockedImage.Margin = new Thickness(-32, -32, 5, 5);

                                Binding binding = new Binding("Locked")
                                {
                                    Source = lockedItem,
                                    Converter = new BooleanToVisibilityConverter()
                                };
                                lockedImage.SetBinding(Image.VisibilityProperty, binding);
                                main.Children.Add(lockedImage);

                                this.bindLockedState(lockedItem);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 调整记录
        /// </summary>
        public List<UIAdjustResultWeek> Adjusts
        {
            get
            {
                return _adjusts;
            }

            set
            {
                _adjusts = value;
                RaisePropertyChanged(() => Adjusts);
            }
        }

        public LockedCourseViewModel()
        {
            this.Results = new List<UIResult>();
            this.Classes = new List<UIClass>();
            this.Adjusts = new List<UIAdjustResultWeek>();

            this._currentLockedItems = new List<UILockedItem>();
        }

        StackPanel createStackPanel(string courseID, UILockedItem lockedItem)
        {
            var solidColorBrush = this.GetRandomColor(courseID);

            StackPanel sp = new StackPanel();
            sp.Background = solidColorBrush;
            sp.Margin = new Thickness(15);
            sp.DataContext = lockedItem;

            ContextMenu contextMenu = new ContextMenu();

            MenuItem lockMenuItem = new MenuItem()
            {
                Header = "锁定",
                Command = LockedCommand,
                CommandParameter = sp.DataContext
            };
            MenuItem unlockMenuItem = new MenuItem()
            {
                Header = "取消锁定",
                Command = UnLockedCommand,
                CommandParameter = sp.DataContext
            };

            MenuItem lockCourseMenuItem = new MenuItem()
            {
                Header = "锁定科目",
                Command = LockedCourseCommand,
                CommandParameter = sp.DataContext
            };
            MenuItem unlockCourseMenuItem = new MenuItem()
            {
                Header = "取消锁定科目",
                Command = UnLockedCourseCommand,
                CommandParameter = sp.DataContext
            };

            MenuItem lockTeacherMenuItem = new MenuItem()
            {
                Header = "锁定教师",
                Command = LockedTeacherCommand,
                CommandParameter = sp.DataContext
            };
            MenuItem unlockTeacherMenuItem = new MenuItem()
            {
                Header = "取消锁定教师",
                Command = UnLockedCourseCommand,
                CommandParameter = sp.DataContext
            };

            contextMenu.Items.Add(lockMenuItem);
            contextMenu.Items.Add(unlockMenuItem);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(lockCourseMenuItem);
            contextMenu.Items.Add(unlockCourseMenuItem);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(lockTeacherMenuItem);
            contextMenu.Items.Add(unlockTeacherMenuItem);

            sp.ContextMenu = contextMenu;
            return sp;
        }

        private ICollection<ResultClassModel> _resultClasses;
        private List<ResultCourseInfo> _courses;

        /// <summary>
        /// 绑定锁定状态
        /// </summary>
        /// <param name="lockedItem"></param>
        void bindLockedState(UILockedItem lockedItem)
        {
            var firstClass = _classes.FirstOrDefault(c => c.Name.Equals(lockedItem.Class));
            if (firstClass == null) return;

            var lockedClass = _lockedRule.LockedTimeTable?.FirstOrDefault(lt => lt.ClassID.Equals(firstClass.ID));
            if (lockedClass == null) return;

            if (lockedClass.LockedCourseTimeTable == null) return;

            var has = lockedClass.LockedCourseTimeTable.Any(lt => lt.LockedTimes.Any(t => t.Day == lockedItem.DayPeriod.Day && t.Period == lockedItem.DayPeriod.Period));
            lockedItem.Locked = has;
        }

        void refreshCommand()
        {
            var confirm = this.ShowDialog("提示信息", "确认刷新将清除已经设置的锁定内容?", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
            if (confirm != CustomControl.Enums.DialogResultType.OK)
                return;

            this._lockedRule.LockedTimeTable?.Clear();
            this.Refresh();
        }

        void Refresh()
        {
            if (this.SelectResult == null)
            {
                this.ShowDialog("提示信息", "结果不能为空!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

            var taskID = this.SelectResult.TaskID;

            _resultModel = base.LocalID.DeSerializeLocalResult<ResultModel>(taskID);
            if (_resultModel == null)
            {
                var value = OSHttpClient.Instance.GetAdminResult(taskID);
                if (value.Item1)
                {
                    _resultModel = value.Item2;
                    _resultClasses = value.Item2.ResultClasses?.ToList();
                }
                else
                {
                    this.ShowDialog("提示信息", "获取结果失败", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                    return;
                }
            }
            else
            {
                _resultClasses = _resultModel.ResultClasses;
            }

            //var canAdjust = CanAdjust(localResult);
            //if (!canAdjust)
            //{
            //    this.ShowDialog("提示信息", "基础数据发生改可能无法正确显示数据!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Error);
            //    return;
            //}

            #region 绑定课位

            List<UIAdjustResultWeek> resultWeeks = new List<UIAdjustResultWeek>();
            var groups = _resultModel.Positions.OrderBy(p => p.DayPeriod.Period).GroupBy(p => p.DayPeriod.Period);
            if (groups != null)
            {
                foreach (var g in groups)
                {
                    var item = g.First();
                    UIAdjustResultWeek weekItem = new UIAdjustResultWeek()
                    {
                        Period = item.DayPeriod,
                        PositionType = item.Position,
                    };
                    resultWeeks.Add(weekItem);
                }
            }
            this.Adjusts = resultWeeks;

            #endregion

            _courses = _resultModel.Courses?.ToList();

            List<UIClass> classes = new List<UIClass>();
            if (_resultClasses != null)
            {
                // 1.根据结果获取班级
                foreach (var rc in _resultClasses)
                {
                    var classInfo = _resultModel.Classes.FirstOrDefault(c => c.ID.Equals(rc.ClassID));
                    if (classInfo != null)
                    {
                        UIClass uiClass = new UIClass()
                        {
                            ID = classInfo.ID,
                            Name = classInfo.Name
                        };
                        classes.Add(uiClass);
                    }
                }

            }

            this.Classes = classes;
            this.SelectClass = this.Classes.FirstOrDefault();
        }

        void UnLocked(object obj)
        {
            var cp = CommonDataManager.GetCPCase(base.LocalID);

            // 取消锁定
            UILockedItem lockedItem = obj as UILockedItem;

            // 获取规则信息方式。
            var firstClass = _classes.FirstOrDefault(c => c.Name.Equals(lockedItem.Class));
            if (firstClass == null)
            {
                this.ShowDialog("提示信息", "没有找到班级信息!数据错误!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

            var lockedClass = _lockedRule.LockedTimeTable?.FirstOrDefault(l => l.ClassID.Equals(firstClass.ID));
            if (lockedClass != null)
            {
                lockedItem.Details.ForEach(c =>
                {
                    var courseModel = _resultModel.Courses.FirstOrDefault(cc => cc.ID.Equals(c.CourseID));
                    var localCourseModel = cp.Courses.FirstOrDefault(cc => cc.Name.Equals(courseModel.Name));

                    // 检查科目
                    if (localCourseModel == null)
                    {
                        this.ShowDialog("提示信息", "没有找到该科目,确认基础数据是否改变!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                        return;
                    }

                    var course = lockedClass.LockedCourseTimeTable?.FirstOrDefault(lc => lc.CourseID.Equals(c.CourseID));
                    if (course != null)
                    {
                        var has = course.LockedTimes.Any(lt => lt.Day == lockedItem.DayPeriod.Day && lt.Period == lockedItem.DayPeriod.Period);
                        if (has)
                        {
                            course.LockedTimes.RemoveAll(rt => rt.Day == lockedItem.DayPeriod.Day && rt.Period == lockedItem.DayPeriod.Period);

                            if (course.LockedTimes.Count == 0)
                            {
                                lockedClass.LockedCourseTimeTable.Remove(course);
                                if (lockedClass.LockedCourseTimeTable.Count == 0)
                                {
                                    _lockedRule.LockedTimeTable.Remove(lockedClass);
                                }
                            }
                        }
                    }
                });

                lockedItem.Locked = false;
            }
        }

        void Locked(object obj)
        {
            var cp = CommonDataManager.GetCPCase(base.LocalID);

            // 锁定
            UILockedItem lockedItem = obj as UILockedItem;
            lockedItem.Locked = true;

            // 获取规则信息方式。
            var firstClass = _classes.FirstOrDefault(c => c.Name.Equals(lockedItem.Class));
            if (firstClass == null)
            {
                this.ShowDialog("提示信息", "没有找到班级信息!数据错误!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

            var lockedClass = _lockedRule.LockedTimeTable?.FirstOrDefault(l => l.ClassID.Equals(firstClass.ID));
            if (lockedClass == null)
            {
                LockedClassTimeTable table = new LockedClassTimeTable();
                table.ClassID = firstClass.ID;
                table.LockedCourseTimeTable = new List<LockedClassCourseTimeTable>();

                lockedItem.Details.ForEach(d =>
                {
                    LockedClassCourseTimeTable courseTable = new LockedClassCourseTimeTable();
                    courseTable.CourseID = d.CourseID;
                    courseTable.LockedTimes = new List<DayPeriodModel>()
                    {
                         d.DayPeriod
                    };
                    table.LockedCourseTimeTable.Add(courseTable);
                });

                if (_lockedRule.LockedTimeTable == null)
                {
                    _lockedRule.LockedTimeTable = new List<LockedClassTimeTable>();
                }

                _lockedRule.LockedTimeTable.Add(table);
            }
            else
            {
                var classInfo = cp.Classes.FirstOrDefault(cc => cc.Name.Equals(lockedItem.Class));

                if (classInfo == null)
                {
                    this.ShowDialog("提示信息", "没有找到班级,确认基础数据是否改变!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                    return;
                }

                lockedItem.Details.ForEach(c =>
                {
                    var courseModel = _resultModel.Courses.FirstOrDefault(cc => cc.ID.Equals(c.CourseID));
                    var localCourseModel = cp.Courses.FirstOrDefault(cc => cc.Name.Equals(courseModel.Name));

                    // 检查科目
                    if (localCourseModel == null)
                    {
                        this.ShowDialog("提示信息", "没有找到该科目,确认基础数据是否改变!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                        return;
                    }

                    var course = lockedClass.LockedCourseTimeTable?.FirstOrDefault(lc => lc.CourseID.Equals(localCourseModel.ID));
                    if (course == null)
                    {
                        LockedClassCourseTimeTable courseTimeTable = new LockedClassCourseTimeTable()
                        {
                            CourseID = localCourseModel.ID,
                            LockedTimes = new List<DayPeriodModel>()
                              {
                                   new DayPeriodModel()
                                   {
                                        Day=lockedItem.DayPeriod.Day,
                                        Period=lockedItem.DayPeriod.Period,
                                        PeriodName=lockedItem.DayPeriod.PeriodName
                                   }
                              }
                        };
                        lockedClass.LockedCourseTimeTable.Add(courseTimeTable);
                    }
                    else
                    {
                        var has = course.LockedTimes.Any(lt => lt.Day == lockedItem.DayPeriod.Day && lt.Period == lockedItem.DayPeriod.Period);
                        if (!has)
                        {
                            course.LockedTimes.Add(new DayPeriodModel()
                            {
                                Day = lockedItem.DayPeriod.Day,
                                Period = lockedItem.DayPeriod.Period,
                                PeriodName = lockedItem.DayPeriod.PeriodName
                            });
                        }
                    }
                });
            }

        }

        void LockedCourse(object obj)
        {
            UILockedItem lockedItem = obj as UILockedItem;

            lockedItem.Details.ForEach(d =>
            {
                var items = this._currentLockedItems.Where(si => si.Details.Any(dt => dt.CourseID.Equals(d.CourseID)));
                if (items != null)
                {
                    foreach (var i in items)
                    {
                        Locked(i);
                    }
                }
            });

        }

        void UnLockedCourse(object obj)
        {
            UILockedItem lockedItem = obj as UILockedItem;

            // 锁定的详细界面。
            lockedItem.Details.ForEach(d =>
            {
                var items = this._currentLockedItems.Where(si => si.Details.Any(dt => dt.CourseID.Equals(d.CourseID)));
                if (items != null)
                {
                    foreach (var i in items)
                    {
                        UnLocked(i);
                    }
                }
            });
        }

        void LockedTeacher(object obj)
        {
            UILockedItem lockedItem = obj as UILockedItem;

            var teachers = (from d in lockedItem.Details from t in d.Teachers select t);
            if (teachers.Count() == 0)
            {
                this.ShowDialog("提示信息", "没有可用教师!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

            // 更新当前界面
            var filters = this._currentLockedItems.Where(i => i.Details.Any(d => d.Teachers.Any(dt => teachers.Contains(dt))));
            if (filters != null)
            {
                foreach (var f in filters)
                {
                    f.Locked = true;
                }
            }

            // 填充数据
            var results = (from c in _resultClasses
                           from rc in c.ResultDetails
                           select new
                           {
                               c.ClassID,
                               rc
                           }).Where(r => r.rc.Teachers.Any(t => teachers.Contains(t)));

            if (results != null)
            {
                foreach (var r in results)
                {
                    if (r.rc.Teachers.Count == 0)
                        continue;

                    var firstClass = _classes.FirstOrDefault(c => c.ID.Equals(r.ClassID));
                    if (firstClass != null)
                    {
                        UILockedItem item = new UILockedItem()
                        {
                            DayPeriod = r.rc.DayPeriod,
                            Class = firstClass?.Name,
                            Details = new List<ResultDetailModel>()
                            {
                                r.rc
                            }
                        };

                        this.Locked(item);
                    }
                }
            }
        }

        void UnLockedTeacher(object obj)
        {
            UILockedItem lockedItem = obj as UILockedItem;

            var teachers = (from d in lockedItem.Details from t in d.Teachers select t);
            if (teachers.Count() == 0)
            {
                this.ShowDialog("提示信息", "没有可用教师!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

            // 更新当前界面
            var filters = this._currentLockedItems.Where(i => i.Details.Any(d => d.Teachers.Any(dt => teachers.Contains(dt))));
            if (filters != null)
            {
                foreach (var f in filters)
                {
                    f.Locked = false;
                }
            }

            // 填充数据
            var results = (from c in _resultClasses
                           from rc in c.ResultDetails
                           select new
                           {
                               c.ClassID,
                               rc
                           }).Where(r => r.rc.Teachers.Any(t => teachers.Contains(t)));

            if (results != null)
            {
                foreach (var r in results)
                {
                    if (r.rc.Teachers.Count == 0)
                        continue;

                    var firstClass = _classes.FirstOrDefault(c => c.ID.Equals(r.ClassID));
                    if (firstClass != null)
                    {
                        UILockedItem item = new UILockedItem()
                        {
                            DayPeriod = r.rc.DayPeriod,
                            Class = firstClass?.Name,
                            Details = new List<ResultDetailModel>()
                            {
                                r.rc
                            }
                        };

                        this.UnLocked(item);
                    }
                }
            }
        }

        void userControlCommand(string parms)
        {
            if (parms.Equals("loaded"))
            {

            }
            else if (parms.Equals("unloaded"))
            {
                Messenger.Default.Unregister<HostView>(this, save);
            }
        }

        private TimeTableLockRule _lockedRule;

        [InjectionMethod]
        public void Initilize()
        {
            Messenger.Default.Register<HostView>(this, save);

            this.Comments = CommonDataManager.GetAdminRuleComments(AdministrativeRuleEnum.LockedCourse);

            this.Results = ResultDataManager.GetResults(base.LocalID);
            var local = CommonDataManager.GetLocalCase(base.LocalID);
            _colors = local.CourseColors;

            #region 获取规则

            var rule = CommonDataManager.GetAminRule(base.LocalID);
            if (rule.TimeTableLockedTimes == null)
            {
                _lockedRule = new TimeTableLockRule();
            }
            else
            {
                _lockedRule = new TimeTableLockRule()
                {
                    LockedTimeTable = rule.TimeTableLockedTimes.LockedTimeTable?.Select(t =>
                     {
                         return new LockedClassTimeTable()
                         {
                             ClassID = t.ClassID,
                             LockedCourseTimeTable = t.LockedCourseTimeTable?.Select(tt =>
                               {
                                   return new LockedClassCourseTimeTable()
                                   {
                                       CourseID = tt.CourseID,
                                       LockedTimes = tt.LockedTimes?.ToList()
                                   };
                               })?.ToList()
                         };
                     })?.ToList()
                };
            }

            #endregion

            #region 锁定任务ID

            if (local.LockedTaskID != 0)
            {
                this.SelectResult = this.Results.FirstOrDefault(r => r.TaskID.Equals(local.LockedTaskID));
                if (this.SelectResult != null)
                    Refresh();
            }

            #endregion
        }

        public SolidColorBrush GetRandomColor(string classKey)
        {
            if (_colors.ContainsKey(classKey))
            {
                return new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString(_colors[classKey]));
            }
            else
                return new SolidColorBrush(Colors.Black);
        }

        void save(HostView host)
        {
            if (this.SelectResult == null)
            {
                this.ShowDialog("提示信息", "选择结果为空!", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
                return;
            }

            var local = CommonDataManager.GetLocalCase(base.LocalID);
            local.LockedTaskID = this.SelectResult.TaskID;
            local.Serialize();

            var rule = CommonDataManager.GetAminRule(base.LocalID);

            if (_lockedRule.LockedTimeTable?.Count == 0)
            {
                rule.TimeTableLockedTimes = null;
            }
            else
            {
                rule.TimeTableLockedTimes = _lockedRule;
            }

            rule.Serialize(base.LocalID);

            this.ShowDialog("提示信息", "保存成功", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
        }

        private bool CanAdjust(ResultModel result)
        {
            var cp = CommonDataManager.GetCPCase(base.LocalID);

            if (result.Classes.Count != cp.Classes.Count)
                return false;

            if (result.Teachers.Count != cp.Teachers.Count)
                return false;

            if (result.Courses.Count != cp.Courses.Count)
                return false;

            var allClass = result.Classes.All(c => cp.Classes.Any(cc => cc.Name.Equals(c.Name)));
            if (!allClass)
                return false;

            var allTeacher = result.Teachers.All(c => cp.Teachers.Any(cc => cc.Name.Equals(c.Name)));
            if (!allTeacher)
                return false;

            var allCourse = result.Courses.All(c => cp.Courses.Any(cc => cc.Name.Equals(c.Name)));
            if (!allCourse)
                return false;

            return true;
        }
    }
}
