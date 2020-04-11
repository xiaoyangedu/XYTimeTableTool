using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using Unity;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using OSKernel.Presentation.Models.Result;
using MahApps.Metro.Controls;
using OSKernel.Presentation.Models.Base;
using OSKernel.Presentation.Core.Http;
using System.Windows.Media;
using XYKernel.OS.Common.Models.Administrative.Result;
using XYKernel.OS.Common.Models.Administrative;
using System.Windows.Controls;
using System.Windows;
using OSKernel.Presentation.Utilities;
using System.Windows.Input;
using OSKernel.Presentation.CustomControl;
using XYKernel.OS.Common.Models;
using System.Windows.Documents;
using System.Collections.ObjectModel;
using OSKernel.Presentation.Models.Result.Administrative;
using System.Data;
using System.Windows.Shapes;
using System.Windows.Data;

namespace OSKernel.Presentation.Arranging.Administrative.Result
{
    /// <summary>
    /// 调整结果模型
    /// </summary>
    public class AdjustResultViewModel : CommonViewModel, IInitilize
    {
        private List<UIClass> _classes;
        private List<UITeacher> _teachers;
        private List<ResultCourseInfo> _courses;
        private List<UIAdjustResultWeek> _results;
        private List<UIResultWeek> _teacherResults;
        private List<ResultClassModel> _resultClasses;

        private bool _showCanNotDrag;

        private ObservableCollection<ClassHourAdjustmentModel> _adjustmentRecords;
        private ObservableCollection<UIDragItem> _courseFrames;

        private AdjustResult _resultDetailsWindow;
        private UITeacher _selectTeacher;
        private UIClass _selectClass;
        private Dictionary<string, string> _colors = new Dictionary<string, string>();

        private UIResult _result;
        private ResultAdjustmentModel _localAdjust;
        private ResultModel _localResult;


        private string _teacherCourseString;

        private long _taskID
        {
            get
            {
                return _result.TaskID;
            }
        }

        /// <summary>
        /// 显示刷新按钮
        /// </summary>
        public bool ShowRefreshButton
        {
            get
            {
                if (_localAdjust == null)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// 显示记录
        /// </summary>
        public bool ShowRecord
        {
            get
            {
                if (_adjustmentRecords.Count > 0)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// 显示课程框
        /// </summary>
        public bool ShowCourseFrame
        {
            get
            {
                if (_courseFrames.Count > 0)
                    return false;
                else
                    return true;
            }
        }

        public List<UIAdjustResultWeek> Results
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

        /// <summary>
        /// 教师结果
        /// </summary>
        public List<UIResultWeek> TeacherResults
        {
            get
            {
                return _teacherResults;
            }
            set
            {
                _teacherResults = value;
                RaisePropertyChanged(() => TeacherResults);
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
        /// 教师
        /// </summary>
        public List<UITeacher> Teachers
        {
            get
            {
                return _teachers;
            }

            set
            {
                _teachers = value;
                RaisePropertyChanged(() => Teachers);
            }
        }

        /// <summary>
        /// 选择教师
        /// </summary>
        public UITeacher SelectTeacher
        {
            get
            {
                return _selectTeacher;
            }

            set
            {
                _selectTeacher = value;
                RaisePropertyChanged(() => SelectTeacher);

                if (_selectTeacher != null)
                {
                    this.TeacherResults.ForEach(r =>
                    {
                        r.Mondays.Clear();
                        r.Tuesdays.Clear();
                        r.Wednesdays.Clear();
                        r.Thursdays.Clear();
                        r.Fridays.Clear();
                        r.Saturdays.Clear();
                        r.Sundays.Clear();
                    });

                    var resultDetails = (from rc in _resultClasses
                                         from rd in rc.ResultDetails
                                         select new
                                         {
                                             rc.ClassID,
                                             rd.ClassHourId,
                                             rd.CourseID,
                                             rd.DayPeriod,
                                             rd.ResultType
                                         });

                    var filters = (from ch in _selectTeacher.ClassHourIDs from rc in resultDetails where ch == rc.ClassHourId select rc)?.ToList();
                    this.TeacherCourseString = _localResult.GetTeacherCourse(_selectTeacher.ID)?.Parse();


                    filters.ForEach(rc =>
                    {
                        var solidColorBrush = this.GetRandomColor(rc.CourseID);

                        //System.Windows.Shapes.Ellipse ellipse = new System.Windows.Shapes.Ellipse();
                        //ellipse.Width = 25;
                        //ellipse.Height = 25;
                        //ellipse.HorizontalAlignment = HorizontalAlignment.Center;
                        //ellipse.VerticalAlignment = VerticalAlignment.Center;
                        //ellipse.Fill = solidColorBrush;

                        TextBlock text = new TextBlock();
                        text.HorizontalAlignment = HorizontalAlignment.Center;
                        text.VerticalAlignment = VerticalAlignment.Center;
                        text.Margin = new Thickness(2);
                        text.Opacity = 0.7;

                        var course = _courses.FirstOrDefault(c => c.ID.Equals(rc.CourseID));
                        var classInfo = this.Classes.FirstOrDefault(c => c.ID.Equals(rc.ClassID));

                        text.Text = $"{classInfo?.Name}";
                        text.ToolTip = $"{course?.Name}{classInfo?.Name}";

                        var position = this.TeacherResults.FirstOrDefault(r => r.Period.Period == rc.DayPeriod.Period);
                        if (position != null)
                        {
                            ContentControl content = new ContentControl()
                            {
                                Content = text
                            };

                            if (rc.DayPeriod.Day == DayOfWeek.Sunday)
                            {
                                position.Sundays.Add(content);
                            }
                            else if (rc.DayPeriod.Day == DayOfWeek.Saturday)
                            {
                                position.Saturdays.Add(content);
                            }
                            else if (rc.DayPeriod.Day == DayOfWeek.Friday)
                            {
                                position.Fridays.Add(content);
                            }
                            else if (rc.DayPeriod.Day == DayOfWeek.Thursday)
                            {
                                position.Thursdays.Add(content);
                            }
                            else if (rc.DayPeriod.Day == DayOfWeek.Wednesday)
                            {
                                position.Wednesdays.Add(content);
                            }
                            else if (rc.DayPeriod.Day == DayOfWeek.Tuesday)
                            {
                                position.Tuesdays.Add(content);
                            }
                            else if (rc.DayPeriod.Day == DayOfWeek.Monday)
                            {
                                position.Mondays.Add(content);
                            }
                        }
                    });

                }
            }
        }

        /// <summary>
        /// 选择班级
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

                if (_selectClass != null)
                {
                    // 清除之间的控件
                    this.Results.ForEach(r =>
                    {
                        r.Monday.Content = null;
                        r.Tuesday.Content = null;
                        r.Wednesday.Content = null;
                        r.Thursday.Content = null;
                        r.Friday.Content = null;
                        r.Saturday.Content = null;
                        r.Sunday.Content = null;
                    });

                    // 创建空白区域
                    this.Results.ForEach(r =>
                    {
                        r.Monday.Content = CreateGridPanel(CreateStackPanel(new DayPeriodModel()
                        {
                            Day = DayOfWeek.Monday,
                            Period = r.Period.Period,
                            PeriodName = r.Period.PeriodName
                        }));

                        r.Tuesday.Content = CreateGridPanel(CreateStackPanel(
                            new DayPeriodModel()
                            {
                                Day = DayOfWeek.Tuesday,
                                Period = r.Period.Period,
                                PeriodName = r.Period.PeriodName
                            }));

                        r.Wednesday.Content = CreateGridPanel(CreateStackPanel(
                            new DayPeriodModel()
                            {
                                Day = DayOfWeek.Wednesday,
                                Period = r.Period.Period,
                                PeriodName = r.Period.PeriodName
                            }));

                        r.Thursday.Content = CreateGridPanel(CreateStackPanel(
                           new DayPeriodModel()
                           {
                               Day = DayOfWeek.Thursday,
                               Period = r.Period.Period,
                               PeriodName = r.Period.PeriodName
                           }));

                        r.Friday.Content = CreateGridPanel(CreateStackPanel(
                          new DayPeriodModel()
                          {
                              Day = DayOfWeek.Friday,
                              Period = r.Period.Period,
                              PeriodName = r.Period.PeriodName
                          }));

                        r.Saturday.Content = CreateGridPanel(CreateStackPanel(
                          new DayPeriodModel()
                          {
                              Day = DayOfWeek.Saturday,
                              Period = r.Period.Period,
                              PeriodName = r.Period.PeriodName
                          }));

                        r.Sunday.Content = CreateGridPanel(CreateStackPanel(
                          new DayPeriodModel()
                          {
                              Day = DayOfWeek.Sunday,
                              Period = r.Period.Period,
                              PeriodName = r.Period.PeriodName
                          }));
                    });

                    // 绑定当前班级结果
                    var firstClass = _resultClasses.FirstOrDefault(f => f.ClassID.Equals(_selectClass.ID));
                    if (firstClass != null)
                    {
                        // 绑定课程框
                        var courseFrames = base.LocalID.DeSerializeCourseFrame<UIDragItem>(_taskID, _selectClass.Name);
                        if (courseFrames != null)
                        {
                            this.CourseFrames = new ObservableCollection<UIDragItem>(courseFrames);
                        }
                        else
                        {
                            this.CourseFrames.Clear();
                        }
                        this.RaisePropertyChanged(() => ShowCourseFrame);


                        // 获取常规的结果
                        var normals = firstClass.ResultDetails?.Where(rd => rd.ResultType == XYKernel.OS.Common.Enums.ClassHourResultType.Normal)?.ToList();
                        normals?.ForEach(n =>
                        {
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

                            Label teacherText = new Label();
                            teacherText.HorizontalContentAlignment = HorizontalAlignment.Center;
                            teacherText.VerticalContentAlignment = VerticalAlignment.Center;
                            teacherText.VerticalAlignment = VerticalAlignment.Center;
                            teacherText.Foreground = new SolidColorBrush(Colors.White);

                            // 绑定教师名称
                            var teachers = this._localResult.GetTeachersByTeacherIDs(n.Teachers?.ToList());
                            string teacherString = teachers.Select(t => t.Name)?.Parse();

                            teacherText.Content = teacherString;
                            teacherText.ToolTip = teacherString;

                            var position = this.Results.FirstOrDefault(r => r.Period.Period == n.DayPeriod.Period);
                            if (position != null)
                            {
                                StackPanel sp = null;
                                if (n.DayPeriod.Day == DayOfWeek.Sunday)
                                {
                                    sp = ((Grid)position.Sunday.Content).Children[0] as StackPanel;
                                }
                                else if (n.DayPeriod.Day == DayOfWeek.Saturday)
                                {
                                    sp = ((Grid)position.Saturday.Content).Children[0] as StackPanel;
                                }
                                else if (n.DayPeriod.Day == DayOfWeek.Friday)
                                {
                                    sp = ((Grid)position.Friday.Content).Children[0] as StackPanel;
                                }
                                else if (n.DayPeriod.Day == DayOfWeek.Thursday)
                                {
                                    sp = ((Grid)position.Thursday.Content).Children[0] as StackPanel;
                                }
                                else if (n.DayPeriod.Day == DayOfWeek.Wednesday)
                                {
                                    sp = ((Grid)position.Wednesday.Content).Children[0] as StackPanel;
                                }
                                else if (n.DayPeriod.Day == DayOfWeek.Tuesday)
                                {
                                    sp = ((Grid)position.Tuesday.Content).Children[0] as StackPanel;
                                }
                                else if (n.DayPeriod.Day == DayOfWeek.Monday)
                                {
                                    sp = ((Grid)position.Monday.Content).Children[0] as StackPanel;
                                }

                                UIDragItem dragItem = sp.DataContext as UIDragItem;

                                if (_colors.ContainsKey(n.CourseID))
                                {
                                    dragItem.D_Color = _colors[n.CourseID];
                                }
                                else
                                {
                                    //如果课程不存在则给黑色
                                    dragItem.D_Color = "#000000";
                                }
                                dragItem.D_Height = sp.Height;
                                dragItem.D_Width = sp.Width;
                                dragItem.D_Teacher = teacherString;
                                dragItem.D_Courses = new List<string> { course.Name };
                                dragItem.CanDrag = true;
                                dragItem.CanDrop = true;
                                dragItem.IsNormal = true;
                                dragItem.Details = new List<ResultDetailModel> { n };

                                var second = createSecondStackPanel(n.CourseID, dragItem);
                                second.Children.Add(text);
                                if (!string.IsNullOrEmpty(teacherString))
                                {
                                    second.Children.Add(teacherText);
                                }

                                sp.AllowDrop = true;
                                sp.Children.Add(second);

                            }
                        });

                        var mulitplys = firstClass.ResultDetails?.Where(rd => rd.ResultType != XYKernel.OS.Common.Enums.ClassHourResultType.Normal)?.ToList();
                        var groups = mulitplys?.GroupBy(m => $"{m.DayPeriod.Day}{m.DayPeriod.Period}");
                        if (groups != null)
                        {
                            foreach (var g in groups)
                            {
                                var n = g.First();

                                var position = this.Results.FirstOrDefault(r => r.Period.Period == n.DayPeriod.Period);
                                if (position != null)
                                {
                                    StackPanel sp = null;

                                    if (n.DayPeriod.Day == DayOfWeek.Sunday)
                                    {
                                        sp = ((Grid)position.Sunday.Content).Children[0] as StackPanel;
                                    }
                                    else if (n.DayPeriod.Day == DayOfWeek.Saturday)
                                    {
                                        sp = ((Grid)position.Saturday.Content).Children[0] as StackPanel;
                                    }
                                    else if (n.DayPeriod.Day == DayOfWeek.Friday)
                                    {
                                        sp = ((Grid)position.Friday.Content).Children[0] as StackPanel;
                                    }
                                    else if (n.DayPeriod.Day == DayOfWeek.Thursday)
                                    {
                                        sp = ((Grid)position.Thursday.Content).Children[0] as StackPanel;
                                    }
                                    else if (n.DayPeriod.Day == DayOfWeek.Wednesday)
                                    {
                                        sp = ((Grid)position.Wednesday.Content).Children[0] as StackPanel;
                                    }
                                    else if (n.DayPeriod.Day == DayOfWeek.Tuesday)
                                    {
                                        sp = ((Grid)position.Tuesday.Content).Children[0] as StackPanel;
                                    }
                                    else if (n.DayPeriod.Day == DayOfWeek.Monday)
                                    {
                                        sp = ((Grid)position.Monday.Content).Children[0] as StackPanel;
                                    }

                                    sp.AllowDrop = true;

                                    UIDragItem dragItem = sp.DataContext as UIDragItem;
                                    dragItem.D_Height = sp.Height;
                                    dragItem.D_Width = sp.Width;
                                    dragItem.CanDrag = true;
                                    dragItem.CanDrop = true;
                                    dragItem.IsNormal = false;
                                    dragItem.D_Courses = new List<string>();
                                    dragItem.Details = new List<ResultDetailModel>();

                                    g.ToList()?.ForEach(ig =>
                                    {
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

                                        // 绑定教师名称
                                        var teachers = this._localResult.GetTeachersByTeacherIDs(ig.Teachers?.ToList());
                                        string teacherString = teachers.Select(t => t.Name)?.Parse();

                                        Label teacherText = new Label();
                                        teacherText.HorizontalContentAlignment = HorizontalAlignment.Center;
                                        teacherText.VerticalContentAlignment = VerticalAlignment.Center;
                                        teacherText.VerticalAlignment = VerticalAlignment.Center;
                                        teacherText.Foreground = new SolidColorBrush(Colors.White);

                                        teacherText.Content = teacherString;
                                        teacherText.ToolTip = teacherString;

                                        var second = createSecondStackPanel(ig.CourseID, dragItem);
                                        second.Children.Add(text);

                                        if (!string.IsNullOrEmpty(teacherString))
                                        {
                                            second.Children.Add(teacherText);
                                        }

                                        sp.Children.Add(second);

                                        dragItem.D_Teacher = teacherString;
                                        dragItem.D_Courses.Add(course.Name);
                                        dragItem.Details.Add(ig);
                                    });


                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 刷新当前命令
        /// </summary>
        public ICommand RefreshCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(refresh);
            }
        }

        /// <summary>
        /// 调整记录
        /// </summary>
        public ObservableCollection<ClassHourAdjustmentModel> AdjustmentRecords
        {
            get
            {
                return _adjustmentRecords;
            }

            set
            {
                _adjustmentRecords = value;
            }
        }

        public ObservableCollection<UIDragItem> CourseFrames
        {
            get
            {
                return _courseFrames;
            }

            set
            {
                _courseFrames = value;
                RaisePropertyChanged(() => CourseFrames);
            }
        }

        public bool ShowCanNotDrag
        {
            get
            {
                return _showCanNotDrag;
            }

            set
            {
                _showCanNotDrag = value;
                RaisePropertyChanged(() => ShowCanNotDrag);
            }
        }

        /// <summary>
        /// 教师所教科目字符串
        /// </summary>
        public string TeacherCourseString
        {
            get
            {
                return _teacherCourseString;
            }

            set
            {
                _teacherCourseString = value;
                RaisePropertyChanged(() => TeacherCourseString);
            }
        }

        public AdjustResultViewModel()
        {
            this.Classes = new List<UIClass>();
            this.Teachers = new List<UITeacher>();
            this.Results = new List<UIAdjustResultWeek>();
            this.TeacherResults = new List<UIResultWeek>();
            this.AdjustmentRecords = new ObservableCollection<ClassHourAdjustmentModel>();
            this.CourseFrames = new ObservableCollection<UIDragItem>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            var local = CommonDataManager.GetLocalCase(base.LocalID);
            _colors = local.CourseColors;
            this.CourseFrames = new ObservableCollection<UIDragItem>();
        }

        public void CourseFrameDrop(StackPanel sourceSP)
        {
            var dragItem = (UIDragItem)sourceSP.DataContext;
            if (dragItem.IsFromCourseFrame)
                return;

            this.RefreshStatus();

            dragItem.IsFromCourseFrame = true;

            this.CourseFrames.Add(dragItem);
            this.RaisePropertyChanged(() => ShowCourseFrame);

            this.CourseFrames?.ToList()?.SerializeCourseFrame(base.LocalID, _taskID, this.SelectClass.Name);

            this.AdjustRecordToCourseFrame(dragItem);

            // 在结果中移除当前项
            this.RemoveDayPeriod(dragItem);
        }

        public void GetData(UIResult result, AdjustResult window)
        {
            _result = result;
            _resultDetailsWindow = window;

            // 获取调课记录
            _localAdjust = base.LocalID.DeSerializeAdjustRecord<ResultAdjustmentModel>(_taskID);
            _localResult = base.LocalID.DeSerializeLocalResult<ResultModel>(_taskID);
            AdjustLogic.CurrentLocalResult = _localResult;

            this.RaisePropertyChanged(() => ShowRefreshButton);

            if (_localResult != null)
            {
                _resultClasses = _localResult.ResultClasses?.ToList();
            }
            else
            {
                var value = OSHttpClient.Instance.GetAdminResult(result.TaskID);
                if (!value.Item1)
                {
                    if (value.Item3.IndexOf("签名不正确") != -1)
                    {
                        if (SignLogic.SignCheck())
                        {
                            value = OSHttpClient.Instance.GetAdminResult(_taskID);
                            if (value.Item1)
                            {
                                _localResult = value.Item2;
                                _resultClasses = _localResult.ResultClasses?.ToList();
                            }
                        }
                    }
                    else
                    {
                        this.ShowDialog("提示信息", "获取行政班结果失败", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                        return;
                    }

                }
                else
                {
                    _localResult = value.Item2;
                    _resultClasses = value.Item2.ResultClasses?.ToList();
                }
            }



            #region 班级课位

            List<UIAdjustResultWeek> resultWeeks = new List<UIAdjustResultWeek>();
            var groups = _localResult.Positions.OrderBy(p => p.DayPeriod.Period).GroupBy(p => p.DayPeriod.Period);
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

            this.Results = resultWeeks;

            #endregion

            #region 教师课位

            List<UIResultWeek> teacherWeeks = new List<UIResultWeek>();
            var teacherGroups = _localResult.Positions.OrderBy(p => p.DayPeriod.Period).GroupBy(p => p.DayPeriod.Period);

            if (groups != null)
            {
                foreach (var g in groups)
                {
                    var item = g.First();
                    UIResultWeek weekItem = new UIResultWeek()
                    {
                        Period = item.DayPeriod,
                        PositionType = item.Position,
                    };
                    teacherWeeks.Add(weekItem);
                }
            }
            this.TeacherResults = teacherWeeks;

            #endregion

            _courses = _localResult.Courses?.ToList();

            this.ShowCanNotDrag = !CanAdjust(_localResult);

            #region 绑定班级、绑定教师

            List<UIClass> classes = new List<UIClass>();
            if (_resultClasses != null)
            {
                // 1.根据结果获取班级
                foreach (var rc in _resultClasses)
                {
                    var classInfo = _localResult.Classes.FirstOrDefault(c => c.ID.Equals(rc.ClassID));
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

                this.Classes = classes;
                this.SelectClass = this.Classes.FirstOrDefault();

                // 2.绑定学生信息
                this.Teachers = _localResult.GetTeachers();
                this.SelectTeacher = this.Teachers?.FirstOrDefault();
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

        private Grid CreateGridPanel(StackPanel sp)
        {
            var grid = new Grid();
            grid.Children.Add(sp);
            grid.DataContext = sp.DataContext;

            // 添加禁止图标
            Path forbidPath = new Path();
            forbidPath.Width = 25;
            forbidPath.Height = 25;
            forbidPath.VerticalAlignment = VerticalAlignment.Center;
            forbidPath.HorizontalAlignment = HorizontalAlignment.Center;
            forbidPath.Stretch = Stretch.Fill;
            forbidPath.Opacity = 0.3;
            forbidPath.Data = (Geometry)Application.Current.Resources["geometry_forbid"];
            forbidPath.Fill = new SolidColorBrush(Colors.Gray);
            Binding binding = new Binding("ShowForbid")
            {
                Converter = new BooleanToVisibilityConverter()
            };
            forbidPath.SetBinding(Image.VisibilityProperty, binding);
            forbidPath.SetValue(Panel.ZIndexProperty, 1);
            grid.Children.Add(forbidPath);

            return grid;
        }

        private StackPanel CreateStackPanel(DayPeriodModel dayPeriod)
        {
            var sp = new StackPanel()
            {
                Margin = new Thickness(5),
                Cursor = Cursors.Hand,
                AllowDrop = true,
                Background = new SolidColorBrush(Colors.Transparent)
            };

            sp.Drop += Sp_Drop;
            sp.MouseLeftButtonDown += Sp_MouseLeftButtonDown;
            sp.MouseMove += SP_MouseMove;

            sp.DataContext = new UIDragItem()
            {
                D_ClassID = _selectClass.ID,
                D_ClassName = _selectClass.Name,
                DayPeriod = dayPeriod,
                Details = new List<ResultDetailModel>()
            };

            return sp;
        }

        private void ForbidPath_Loaded(object sender, RoutedEventArgs e)
        {
            Path content = sender as Path;

            StackPanel parent = content.Parent as StackPanel;

            var leftright = (parent.ActualWidth - content.ActualHeight) / 2;
            var updown = (parent.ActualHeight - content.ActualHeight) / 2;
            content.Margin = new Thickness(leftright, updown, leftright, updown);
        }

        private void refresh()
        {
            // 删除调整记录和结果
            PathHelper.DeleteTaskAdjustRecord(base.LocalID, _taskID, ".adjust");
            PathHelper.DeleteTaskAdjustRecord(base.LocalID, _taskID, ".localResult");

            this.Classes?.ForEach(c =>
            {
                PathHelper.DeleteTaskAdjustRecord(base.LocalID, _taskID, ".adjustCourseFrame", c.Name);
            });

            // 刷新按钮状态
            _localAdjust = null;
            _localResult = null;
            AdjustLogic.CurrentLocalResult = null;

            this.RaisePropertyChanged(() => ShowRefreshButton);

            // 清除调整记录
            //this.AdjustmentRecords.Clear();
            //this.RaisePropertyChanged(() => ShowRecord);

            // 清除课程框
            this.CourseFrames.Clear();
            this.RaisePropertyChanged(() => ShowCourseFrame);

            // 获取数据
            this.GetData(_result, _resultDetailsWindow);

            // 保存结果状态
            _result.IsUploaded = false;
            var local = CommonDataManager.GetLocalCase(base.LocalID);
            var result = ResultDataManager.GetResults(base.LocalID);
            local.Serizlize(result);
        }

        private void SP_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                StackPanel sp = sender as StackPanel;
                UIDragItem model = sp.DataContext as UIDragItem;

                if (sp == null)
                    return;

                if (model.CanDrag)
                {
                    DragDrop.DoDragDrop(sp, sp, DragDropEffects.Copy);
                }
            }
        }

        private void Sp_Drop(object sender, DragEventArgs e)
        {
            this.RefreshStatus();

            var targetSP = sender as StackPanel;

            var rect = e.Data.GetData(typeof(Rectangle)) as Rectangle;

            var sourceSP = e.Data.GetData(typeof(StackPanel)) as StackPanel;

            if (rect == null)
            {
                // 是否为自己
                if (sourceSP.Equals(targetSP))
                    return;

                // 是否为可拖拽对象
                var canDrag = (sourceSP.DataContext as UIDragItem).CanDrag;
                if (!canDrag)
                {
                    return;
                }


                this.course_Drop(sourceSP, targetSP);
            }
            else
            {
                this.courseFrame_Drop(rect, targetSP);

            }
        }

        private void courseFrame_Drop(Rectangle source, StackPanel target)
        {
            Tuple<bool, string> result = Tuple.Create<bool, string>(true, string.Empty);

            var sourceModel = (UIDragItem)source.DataContext;
            var targetModel = (UIDragItem)target.DataContext;

            if (targetModel.CanDrag)
            {
                this.ShowDialog("提示信息", "从课程框拖拽必须放在空白处!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

            var targetDP = new DayPeriodModel
            {
                Day = targetModel.DayPeriod.Day,
                Period = targetModel.DayPeriod.Period,
                PeriodName = targetModel.DayPeriod.PeriodName
            };

            var sourceDP = new DayPeriodModel()
            {
                Day = sourceModel.DayPeriod.Day,
                Period = sourceModel.DayPeriod.Period,
                PeriodName = sourceModel.DayPeriod.PeriodName
            };


            #region 课程框向回拖拽

            var sp = this.CreateStackPanel(targetDP);
            sp.AllowDrop = true;

            var targetResult = this.Results.FirstOrDefault(r => r.Period.Period == targetDP.Period);

            sourceModel.Details.ForEach(d =>
            {
                d.DayPeriod = targetDP;

                Label text = new Label();
                text.HorizontalContentAlignment = HorizontalAlignment.Center;
                text.VerticalContentAlignment = VerticalAlignment.Center;
                text.VerticalAlignment = VerticalAlignment.Center;
                text.Foreground = new SolidColorBrush(Colors.White);
                text.FontWeight = FontWeights.Bold;
                text.FontSize = 15;

                var course = _courses.FirstOrDefault(c => c.ID.Equals(d.CourseID));
                if (course != null)
                {
                    text.Content = course.Name;
                }


                Label teacherText = new Label();
                teacherText.HorizontalContentAlignment = HorizontalAlignment.Center;
                teacherText.VerticalContentAlignment = VerticalAlignment.Center;
                teacherText.VerticalAlignment = VerticalAlignment.Center;
                teacherText.Foreground = new SolidColorBrush(Colors.White);

                var teachers = this._localResult.GetTeachersByTeacherIDs(d.Teachers?.ToList());
                string teacherString = teachers.Select(t => t.Name)?.Parse();

                teacherText.Content = teacherString;
                teacherText.ToolTip = teacherString;

                var position = this.Results.FirstOrDefault(r => r.Period.Period == d.DayPeriod.Period);
                if (position != null)
                {
                    if (d.DayPeriod.Day == DayOfWeek.Sunday)
                    {
                        sp = ((Grid)position.Sunday.Content).Children[0] as StackPanel;
                    }
                    else if (d.DayPeriod.Day == DayOfWeek.Saturday)
                    {
                        sp = ((Grid)position.Saturday.Content).Children[0] as StackPanel;
                    }
                    else if (d.DayPeriod.Day == DayOfWeek.Friday)
                    {
                        sp = ((Grid)position.Friday.Content).Children[0] as StackPanel;
                    }
                    else if (d.DayPeriod.Day == DayOfWeek.Thursday)
                    {
                        sp = ((Grid)position.Thursday.Content).Children[0] as StackPanel;
                    }
                    else if (d.DayPeriod.Day == DayOfWeek.Wednesday)
                    {
                        sp = ((Grid)position.Wednesday.Content).Children[0] as StackPanel;
                    }
                    else if (d.DayPeriod.Day == DayOfWeek.Tuesday)
                    {
                        sp = ((Grid)position.Tuesday.Content).Children[0] as StackPanel;
                    }
                    else if (d.DayPeriod.Day == DayOfWeek.Monday)
                    {
                        sp = ((Grid)position.Monday.Content).Children[0] as StackPanel;
                    }

                    UIDragItem dragItem = sp.DataContext as UIDragItem;
                    //dragItem.D_Color = _colors[d.CourseID];
                    dragItem.D_Height = sp.Height;
                    dragItem.D_Width = sp.Width;
                    dragItem.D_Teacher = teacherString;
                    dragItem.D_Courses = new List<string> { course.Name };
                    dragItem.CanDrag = true;
                    dragItem.CanDrop = true;
                    dragItem.RaiseStatus();
                    dragItem.IsNormal = true;
                    dragItem.Details = new List<ResultDetailModel> { d };

                    var second = createSecondStackPanel(d.CourseID, dragItem);
                    second.Children.Add(text);
                    if (!string.IsNullOrEmpty(teacherString))
                    {
                        second.Children.Add(teacherText);
                    }

                    sp.AllowDrop = true;
                    sp.Children.Add(second);
                }
            });

            sourceModel.CanDrag = true;
            sourceModel.CanDrop = true;
            sourceModel.D_Height = sp.Height;
            sourceModel.D_Width = sp.Width;
            sourceModel.IsFromCourseFrame = false;
            sourceModel.DayPeriod = targetDP;

            sp.DataContext = sourceModel;

            #endregion

            #region 移除课程框对影项,结果中增加该项

            this.CourseFrames.Remove(sourceModel);
            this.RaisePropertyChanged(() => ShowCourseFrame);
            this.CourseFrames?.ToList()?.SerializeCourseFrame(base.LocalID, _taskID, this.SelectClass.Name);

            // 向结果中增加
            var firstResult = this._localResult.ResultClasses.FirstOrDefault(f => f.ClassID.Equals(sourceModel.D_ClassID));
            if (firstResult != null)
            {
                sourceModel.Details.ForEach(d =>
                {
                    var has = firstResult.ResultDetails.Any(rd => rd.ClassHourId.Equals(d.ClassHourId));
                    if (!has)
                    {
                        firstResult.ResultDetails.Add(d);
                    }
                });
                _localResult.SerializeLocalResult(base.LocalID, _taskID);
            }

            #endregion

            AdjustRecordToCourseFrame(sourceModel);
        }

        private void course_Drop(StackPanel source, StackPanel target)
        {
            Tuple<bool, string> result = Tuple.Create<bool, string>(true, string.Empty);

            var sourceModel = (UIDragItem)source.DataContext;
            var targetModel = (UIDragItem)target.DataContext;

            var classID = targetModel.D_ClassID;

            if (targetModel.Details?.Count() > 0)
            {
                if (!ShowCanNotDrag)
                    result = AdjustLogic.CanReplacePosition(base.LocalID, classID, sourceModel.Details, targetModel.Details, _localResult);
            }

            if (!result.Item1)
            {
                var confirm = this.ShowDialog("提示信息", result.Item2, CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
                if (confirm != CustomControl.Enums.DialogResultType.OK)
                {
                    return;
                }
            }

            var sourceDP = new DayPeriodModel()
            {
                Day = sourceModel.DayPeriod.Day,
                Period = sourceModel.DayPeriod.Period,
                PeriodName = sourceModel.DayPeriod.PeriodName
            };
            var targetDP = new DayPeriodModel
            {
                Day = targetModel.DayPeriod.Day,
                Period = targetModel.DayPeriod.Period,
                PeriodName = targetModel.DayPeriod.PeriodName
            };

            // 原始
            this.SetStackPanel(targetDP, source);

            // 目标
            this.SetStackPanel(sourceDP, target);

            if (targetModel.CanDrag)
            {
                AdjustRecordToReplace(sourceModel, targetModel);
            }
            else
            {
                AdjustRecordToEmpty(sourceModel, targetModel);
            }
        }

        public void Sp_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ButtonState != MouseButtonState.Pressed) { return; }

            this.RefreshStatus();

            // 容器
            FrameworkElement sp = sender as FrameworkElement;
            var model = sp.DataContext as UIDragItem;

            if (!model.CanDrag)
            {
                e.Handled = false;
                return;
            }

            // 联动设置教师
            if (!string.IsNullOrEmpty(model.D_Teacher))
            {
                var teachers = model.D_Teacher.Split(',');
                var firstTeacher = this.Teachers.FirstOrDefault(t => t.Name.Equals(teachers[0]));
                if (firstTeacher != null)
                {
                    this.SelectTeacher = firstTeacher;
                }
            }

            #region 可用课位

            List<DayPeriodModel> dayPeriods = new List<DayPeriodModel>();
            var brush = (SolidColorBrush)Application.Current.FindResource("main_lightgroud");
            var enable = (SolidColorBrush)Application.Current.FindResource("week_forbid");

            if (!ShowCanNotDrag)
            {
                // 可拖放区域
                dayPeriods = AdjustLogic.CheckCanAdjustPosition(base.LocalID, model.D_ClassID, model.Details, _localResult);
                // 可用课位
                dayPeriods.ForEach(p =>
                {
                    var result = this.Results.FirstOrDefault(r => r.Period.Period == p.Period);
                    if (result != null)
                    {
                        StackPanel panel = null;

                        switch (p.Day)
                        {
                            case DayOfWeek.Monday:
                                panel = ((Grid)result.Monday.Content).Children[0] as StackPanel;
                                break;

                            case DayOfWeek.Tuesday:
                                panel = ((Grid)result.Tuesday.Content).Children[0] as StackPanel;
                                break;

                            case DayOfWeek.Wednesday:
                                panel = ((Grid)result.Wednesday.Content).Children[0] as StackPanel;
                                break;

                            case DayOfWeek.Thursday:
                                panel = ((Grid)result.Thursday.Content).Children[0] as StackPanel;
                                break;

                            case DayOfWeek.Friday:
                                panel = ((Grid)result.Friday.Content).Children[0] as StackPanel;
                                break;

                            case DayOfWeek.Saturday:
                                panel = ((Grid)result.Saturday.Content).Children[0] as StackPanel;
                                break;

                            case DayOfWeek.Sunday:
                                panel = ((Grid)result.Sunday.Content).Children[0] as StackPanel;
                                break;
                        }

                        panel.Background = brush;
                        panel.Opacity = 1;

                        UIDragItem dragItem = panel.DataContext as UIDragItem;
                        panel.AllowDrop = true;
                        dragItem.CanDrop = true;
                        dragItem.RaiseStatus();
                    }
                });
            }

            if (!ShowCanNotDrag)
            {
                // 不可用课位
                var unAvaliableDayPeriods = this.getUnDayPeriods(dayPeriods);
                unAvaliableDayPeriods.ForEach(p =>
                {
                    var result = this.Results.FirstOrDefault(r => r.Period.Period == p.Period);
                    if (result != null)
                    {
                        StackPanel panel = null;
                        switch (p.Day)
                        {
                            case DayOfWeek.Monday:
                                panel = ((Grid)result.Monday.Content).Children[0] as StackPanel;
                                break;

                            case DayOfWeek.Tuesday:
                                panel = ((Grid)result.Tuesday.Content).Children[0] as StackPanel;
                                break;

                            case DayOfWeek.Wednesday:
                                panel = ((Grid)result.Wednesday.Content).Children[0] as StackPanel;
                                break;

                            case DayOfWeek.Thursday:
                                panel = ((Grid)result.Thursday.Content).Children[0] as StackPanel;
                                break;

                            case DayOfWeek.Friday:
                                panel = ((Grid)result.Friday.Content).Children[0] as StackPanel;
                                break;

                            case DayOfWeek.Saturday:
                                panel = ((Grid)result.Saturday.Content).Children[0] as StackPanel;
                                break;

                            case DayOfWeek.Sunday:
                                panel = ((Grid)result.Sunday.Content).Children[0] as StackPanel;
                                break;
                        }

                        panel.Background = enable;
                        panel.Opacity = 0.2;

                        UIDragItem dragItem = panel.DataContext as UIDragItem;
                        dragItem.CanDrop = false;
                        dragItem.RaiseStatus();
                    }
                });
            }

            #endregion
        }

        private void SetStackPanel(DayPeriodModel dayPeriod, StackPanel sp)
        {
            var target = this.Results.FirstOrDefault(r => r.Period.Period == dayPeriod.Period);

            var dragItem = (UIDragItem)sp.DataContext;
            dragItem.DayPeriod = new DayPeriodModel()
            {
                Day = dayPeriod.Day,
                Period = dayPeriod.Period,
                PeriodName = dayPeriod.PeriodName
            };

            if (dragItem.Details?.Count() > 0)
            {
                // 更新目标状态
                dragItem.Details.ForEach(di =>
                {
                    di.DayPeriod = dragItem.DayPeriod;
                });
            }

            switch (dayPeriod.Day)
            {
                case DayOfWeek.Monday:
                    this.copyContentControl(target.Monday, sp);
                    break;
                case DayOfWeek.Tuesday:
                    this.copyContentControl(target.Tuesday, sp);
                    break;
                case DayOfWeek.Wednesday:
                    this.copyContentControl(target.Wednesday, sp);
                    break;
                case DayOfWeek.Thursday:
                    this.copyContentControl(target.Thursday, sp);
                    break;
                case DayOfWeek.Friday:
                    this.copyContentControl(target.Friday, sp);
                    break;
                case DayOfWeek.Saturday:
                    this.copyContentControl(target.Saturday, sp);
                    break;
                case DayOfWeek.Sunday:
                    this.copyContentControl(target.Sunday, sp);
                    break;
            }
        }

        private void copyContentControl(ContentControl content, StackPanel sp)
        {
            // 移除与原有父级关系
            if (sp.Parent != null)
            {
                var father = sp.Parent as Grid;
                father.Children.Remove(sp);
            }

            content.Content = this.CreateGridPanel(sp);
        }

        private DataTable CreateTableFrame()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("课节");
            dt.Columns.Add("星期一");
            dt.Columns.Add("星期二");
            dt.Columns.Add("星期三");
            dt.Columns.Add("星期四");
            dt.Columns.Add("星期五");
            dt.Columns.Add("星期六");
            dt.Columns.Add("星期日");

            this.Results.ForEach(r =>
            {
                var newRow = dt.NewRow();
                newRow["课节"] = r.Period.PeriodName;
                dt.Rows.Add(newRow);
            });

            return dt;
        }

        /// <summary>
        /// 刷新所有状态
        /// </summary>
        private void RefreshStatus()
        {
            this.Results.ForEach(r =>
            {
                if (r.Monday.Content != null)
                {
                    var sp = (r.Monday.Content as Grid).Children[0] as StackPanel;
                    this.clearStackPanelStatus(sp);
                }
                if (r.Tuesday.Content != null)
                {
                    var sp = (r.Tuesday.Content as Grid).Children[0] as StackPanel;
                    this.clearStackPanelStatus(sp);
                }

                if (r.Wednesday.Content != null)
                {
                    var sp = (r.Wednesday.Content as Grid).Children[0] as StackPanel;
                    this.clearStackPanelStatus(sp);
                }

                if (r.Thursday.Content != null)
                {
                    var sp = (r.Thursday.Content as Grid).Children[0] as StackPanel;
                    this.clearStackPanelStatus(sp);
                }

                if (r.Friday.Content != null)
                {
                    var sp = (r.Friday.Content as Grid).Children[0] as StackPanel;
                    this.clearStackPanelStatus(sp);
                }

                if (r.Saturday.Content != null)
                {
                    var sp = (r.Saturday.Content as Grid).Children[0] as StackPanel;
                    this.clearStackPanelStatus(sp);
                }

                if (r.Sunday.Content != null)
                {
                    var sp = (r.Sunday.Content as Grid).Children[0] as StackPanel;
                    this.clearStackPanelStatus(sp);
                }

            });
        }

        private void clearStackPanelStatus(StackPanel sp)
        {
            sp.AllowDrop = true;
            sp.Background = new SolidColorBrush(Colors.Transparent);
            sp.Opacity = 1;

            var dragItem = sp.DataContext as UIDragItem;
            dragItem.CanDrop = true;
            dragItem.RaiseStatus();
        }

        /// <summary>
        /// 移除制定课位信息
        /// </summary>
        private void RemoveDayPeriod(UIDragItem removeItem)
        {
            var resultModel = removeItem.Details.First();

            var first = this.Results.FirstOrDefault(r => r.Period.Period == removeItem.DayPeriod.Period);
            if (first != null)
            {
                Grid grid = CreateGridPanel(this.CreateStackPanel(removeItem.DayPeriod));
                switch (removeItem.DayPeriod.Day)
                {
                    case DayOfWeek.Monday:
                        first.Monday.Content = grid;
                        break;
                    case DayOfWeek.Tuesday:
                        first.Tuesday.Content = grid;
                        break;
                    case DayOfWeek.Wednesday:
                        first.Wednesday.Content = grid;
                        break;
                    case DayOfWeek.Thursday:
                        first.Thursday.Content = grid;
                        break;
                    case DayOfWeek.Friday:
                        first.Friday.Content = grid;
                        break;
                    case DayOfWeek.Saturday:
                        first.Saturday.Content = grid;
                        break;
                    case DayOfWeek.Sunday:
                        first.Sunday.Content = grid;
                        break;
                }
            }

            #region 移除

            var firstResult = this._localResult.ResultClasses.FirstOrDefault(f => f.ClassID.Equals(removeItem.D_ClassID));
            if (firstResult != null)
            {
                removeItem.Details.ForEach(d =>
                {
                    var has = firstResult.ResultDetails.Any(rd => rd.ClassHourId.Equals(d.ClassHourId));
                    if (has)
                    {
                        firstResult.ResultDetails.Remove(d);
                    }
                });
                _localResult.SerializeLocalResult(base.LocalID, _taskID);
            }

            #endregion
        }

        private StackPanel createSecondStackPanel(string courseID, UIDragItem dragItem)
        {
            var solidColorBrush = this.GetRandomColor(courseID);
            StackPanel sp = new StackPanel();
            sp.Background = solidColorBrush;
            sp.Margin = new Thickness(15);
            sp.DataContext = dragItem;
            return sp;
        }

        #region 生成调整记录

        /// <summary>
        /// 向空白地方拖拽生成调整记录
        /// </summary>
        /// <param name="dragItem">拖拽对象</param>
        /// <param name="targetDP">目标位置</param>
        private void AdjustRecordToEmpty(UIDragItem dragItem, UIDragItem targetItem)
        {
            var record = new ClassHourAdjustmentModel();
            record.AdjustmentTime = DateTime.Now;
            record.ClassName = dragItem.D_ClassName;
            record.AdjustType = XYKernel.OS.Common.Enums.AdjustTypeEnum.CourseFrame;
            record.Start = new List<AdjustmentDetailModel>();
            record.End = new List<AdjustmentDetailModel>()
            {
                 new AdjustmentDetailModel()
                 {
                      TimeSlot=targetItem.DayPeriod
                 }
            };

            dragItem.Details.ForEach(d =>
            {
                var courseInfo = this._courses.FirstOrDefault(c => c.ID.Equals(d.CourseID));
                var start = new AdjustmentDetailModel()
                {
                    CourseName = courseInfo.Name,
                    TeacherNames = d.Teachers?.ToList(),
                    TimeSlot = d.DayPeriod
                };
                record.Start.Add(start);
            });
            CreateAdjustRecord(record);
        }

        /// <summary>
        /// 从课位向课程框拖拽
        /// </summary>
        /// <param name="dragItem">拖拽内容</param>
        private void AdjustRecordToCourseFrame(UIDragItem dragItem)
        {
            var record = new ClassHourAdjustmentModel();
            record.AdjustmentTime = DateTime.Now;
            record.ClassName = dragItem.D_ClassName;
            record.AdjustType = XYKernel.OS.Common.Enums.AdjustTypeEnum.CourseFrame;
            record.Start = new List<AdjustmentDetailModel>();
            record.End = new List<AdjustmentDetailModel>();

            dragItem.Details.ForEach(d =>
            {
                var courseInfo = this._courses.FirstOrDefault(c => c.ID.Equals(d.CourseID));
                var start = new AdjustmentDetailModel()
                {
                    CourseName = courseInfo.Name,
                    TeacherNames = d.Teachers?.ToList(),
                    TimeSlot = d.DayPeriod
                };
                record.Start.Add(start);
            });

            CreateAdjustRecord(record);
        }

        /// <summary>
        /// 两个课位相互替换
        /// </summary>
        /// <param name="dragItem">原始项</param>
        /// <param name="targetItem">目标项</param>
        private void AdjustRecordToReplace(UIDragItem dragItem, UIDragItem targetItem)
        {
            var record = new ClassHourAdjustmentModel();
            record.AdjustmentTime = DateTime.Now;
            record.ClassName = dragItem.D_ClassName;
            record.AdjustType = XYKernel.OS.Common.Enums.AdjustTypeEnum.CourseFrame;
            record.Start = new List<AdjustmentDetailModel>();
            record.End = new List<AdjustmentDetailModel>();

            dragItem.Details.ForEach(d =>
            {
                var courseInfo = this._courses.FirstOrDefault(c => c.ID.Equals(d.CourseID));
                var start = new AdjustmentDetailModel()
                {
                    CourseName = courseInfo.Name,
                    TeacherNames = d.Teachers?.ToList(),
                    TimeSlot = d.DayPeriod
                };
                record.Start.Add(start);
            });

            targetItem.Details.ForEach(d =>
            {
                var courseInfo = this._courses.FirstOrDefault(c => c.ID.Equals(d.CourseID));
                var end = new AdjustmentDetailModel()
                {
                    CourseName = courseInfo.Name,
                    TeacherNames = d.Teachers?.ToList(),
                    TimeSlot = d.DayPeriod
                };
                record.End.Add(end);
            });

            CreateAdjustRecord(record);
        }

        private void CreateAdjustRecord(ClassHourAdjustmentModel record)
        {
            if (!ShowRefreshButton)
            {
                _localAdjust = new ResultAdjustmentModel();
                _localAdjust.ClassHourAdjustmentDetails = new List<ClassHourAdjustmentModel>();
                _localAdjust.CurrentResult = _localResult;
                _localAdjust.ResultId = _result.TaskID.ToString();
            }

            _localAdjust.ClassHourAdjustmentDetails.Add(record);

            _localAdjust.Serialize(base.LocalID, _taskID);
            _localResult.SerializeLocalResult(base.LocalID, _taskID);
            AdjustLogic.CurrentLocalResult = _localResult;

            this.RaisePropertyChanged(() => ShowRefreshButton);
        }

        #endregion

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

            var classHourIDs = (from ch in result.ResultClasses from dt in ch.ResultDetails select dt.ClassHourId)?.ToList();
            var allSameClassHour = classHourIDs.All(ch => cp.ClassHours.Any(sch => sch.ID.Equals(ch)));
            if (!allSameClassHour)
                return false;

            return true;
        }

        /// <summary>
        /// 获取所有不可用的课位
        /// </summary>
        /// <param name="avaliableDayPeriods"></param>
        /// <returns></returns>
        private List<DayPeriodModel> getUnDayPeriods(List<DayPeriodModel> avaliableDayPeriods)
        {
            List<DayPeriodModel> allDayPeriods = new List<DayPeriodModel>();
            this.Results.ForEach(r =>
            {
                DayPeriodModel monday = new DayPeriodModel()
                {
                    Day = DayOfWeek.Monday,
                    Period = r.Period.Period,
                    PeriodName = r.Period.PeriodName
                };
                allDayPeriods.Add(monday);

                DayPeriodModel tuesday = new DayPeriodModel()
                {
                    Day = DayOfWeek.Tuesday,
                    Period = r.Period.Period,
                    PeriodName = r.Period.PeriodName
                };
                allDayPeriods.Add(tuesday);

                DayPeriodModel wednesday = new DayPeriodModel()
                {
                    Day = DayOfWeek.Wednesday,
                    Period = r.Period.Period,
                    PeriodName = r.Period.PeriodName
                };
                allDayPeriods.Add(wednesday);

                DayPeriodModel thursday = new DayPeriodModel()
                {
                    Day = DayOfWeek.Thursday,
                    Period = r.Period.Period,
                    PeriodName = r.Period.PeriodName
                };
                allDayPeriods.Add(thursday);

                DayPeriodModel friday = new DayPeriodModel()
                {
                    Day = DayOfWeek.Friday,
                    Period = r.Period.Period,
                    PeriodName = r.Period.PeriodName
                };
                allDayPeriods.Add(friday);

                DayPeriodModel satarday = new DayPeriodModel()
                {
                    Day = DayOfWeek.Saturday,
                    Period = r.Period.Period,
                    PeriodName = r.Period.PeriodName
                };
                allDayPeriods.Add(satarday);

                DayPeriodModel sunday = new DayPeriodModel()
                {
                    Day = DayOfWeek.Sunday,
                    Period = r.Period.Period,
                    PeriodName = r.Period.PeriodName
                };
                allDayPeriods.Add(sunday);
            });

            avaliableDayPeriods.ForEach(dp =>
            {
                allDayPeriods.RemoveAll(rd => rd.Day == dp.Day && rd.Period == dp.Period);
            });

            return allDayPeriods;
        }
    }
}
