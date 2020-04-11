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
using XYKernel.OS.Common.Models.Mixed.Result;
using XYKernel.OS.Common.Models.Mixed;
using System.Windows.Controls;
using System.Windows;
using OSKernel.Presentation.Utilities;
using System.Windows.Input;
using OSKernel.Presentation.CustomControl;
using XYKernel.OS.Common.Models;
using System.Windows.Documents;
using System.Collections.ObjectModel;
using OSKernel.Presentation.Models.Result.Mixed;
using System.Windows.Shapes;
using System.Windows.Data;

namespace OSKernel.Presentation.Arranging.Mixed.Result
{
    /// <summary>
    /// 调整结果模型
    /// </summary>
    public class AdjustResultViewModel : CommonViewModel, IInitilize
    {
        private List<UIResultWeek> _teacherResults;
        private List<UITeacher> _teachers;
        private UITeacher _selectTeacher;
        private List<UIAdjustResultWeek> _results;
        private bool _showCanNotDrag;
        private bool _checkedAllStudent;

        private ObservableCollection<ClassHourAdjustmentModel> _adjustmentRecords;
        private ObservableCollection<UIDragItem> _courseFrames;

        List<Models.Result.Mixed.UIClass> sourceClasses;
        List<Models.Result.Mixed.UIClass> _classes;
        List<Models.Result.Mixed.UIClass> _adjustClasses;

        private Models.Result.Mixed.UIClass _selectClass;
        private Models.Result.Mixed.UIClass _selectAdjustClass;

        ObservableCollection<Models.Base.UIStudent> _students;
        List<ResultClassModel> _resultClasses;

        MetroWindow _window;

        private UIResult _result;
        private ResultAdjustmentModel _localAdjust;
        private ResultModel _localResult;

        private long _taskID
        {
            get
            {
                return _result.TaskID;
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
        /// 选中的教师
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
                                             rd.DayPeriod,
                                         });

                    var filters = (from ch in _selectTeacher.ClassHourIDs from rc in resultDetails where ch == rc.ClassHourId select rc)?.ToList();
                    filters.ForEach(rc =>
                    {
                        var classModel = _localResult.GetClassByClassHourID(rc.ClassHourId);


                        Label label = new Label();
                        label.Style = (Style)_window.FindResource("ItemStyle");
                        //rb.Background = this.GetRandomColor(classModel.CourseID);
                        //var teacherString = _localResult.GetTeachersByTeacherIDs(classModel.TeacherIDs)?.Select(t => t.Name)?.Parse();
                        label.Content = classModel.Display; //+ (string.IsNullOrEmpty(teacherString) == true ? "" : "\n" + teacherString);
                        label.ToolTip = label.Content;
                        label.SetValue(TextBlock.TextWrappingProperty, TextWrapping.Wrap);
                        label.Opacity = 0.9;
                        label.FontSize = 13;
                        label.Margin = new System.Windows.Thickness(2);

                        var position = this.TeacherResults.FirstOrDefault(r => r.Period.Period == rc.DayPeriod.Period);
                        if (position != null)
                        {
                            if (rc.DayPeriod.Day == DayOfWeek.Friday)
                            {
                                position.Fridays.Add(label);
                            }
                            else if (rc.DayPeriod.Day == DayOfWeek.Thursday)
                            {
                                position.Thursdays.Add(label);
                            }
                            else if (rc.DayPeriod.Day == DayOfWeek.Wednesday)
                            {
                                position.Wednesdays.Add(label);
                            }
                            else if (rc.DayPeriod.Day == DayOfWeek.Tuesday)
                            {
                                position.Tuesdays.Add(label);
                            }
                            else if (rc.DayPeriod.Day == DayOfWeek.Monday)
                            {
                                position.Mondays.Add(label);
                            }
                            else if (rc.DayPeriod.Day == DayOfWeek.Saturday)
                            {
                                position.Saturdays.Add(label);
                            }
                            else if (rc.DayPeriod.Day == DayOfWeek.Sunday)
                            {
                                position.Sundays.Add(label);
                            }
                        }
                    });

                }
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

        public ICommand RefreshCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(refresh);
            }
        }

        public ICommand AssignCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(assign);
            }
        }

        /// <summary>
        /// 显示是否能够拖拽
        /// </summary>
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
        /// 班级集合
        /// </summary>
        public List<Models.Result.Mixed.UIClass> Classes
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
        /// 选择班级
        /// </summary>
        public Models.Result.Mixed.UIClass SelectClass
        {
            get
            {
                return _selectClass;
            }

            set
            {
                _selectClass = value;
                RaisePropertyChanged(() => SelectClass);

                if (SelectClass.Students != null)
                {
                    SelectClass.Students.ForEach(s =>
                    {
                        s.IsChecked = false;
                        s.PropertyChanged -= S_PropertyChanged;
                    });

                    this.Students = new ObservableCollection<UIStudent>(SelectClass.Students);

                    SelectClass.Students.ForEach(s =>
                    {
                        s.PropertyChanged += S_PropertyChanged;
                    });
                }
            }
        }

        private void S_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UIStudent model = sender as UIStudent;

            if (e.PropertyName.Equals(nameof(model.IsChecked)))
            {
                this.ShowCanAssignClass();
            }
        }

        public ObservableCollection<UIStudent> Students
        {
            get
            {
                return _students;
            }

            set
            {
                _students = value;
                RaisePropertyChanged(() => Students);
            }
        }

        /// <summary>
        /// 选中所有学生
        /// </summary>
        public bool CheckedAllStudent
        {
            get
            {
                return _checkedAllStudent;
            }

            set
            {
                _checkedAllStudent = value;

                foreach (var student in this.Students)
                {
                    student.IsChecked = value;
                }
            }
        }

        /// <summary>
        /// 调整班级列表
        /// </summary>
        public List<Models.Result.Mixed.UIClass> AdjustClasses
        {
            get
            {
                return _adjustClasses;
            }

            set
            {
                _adjustClasses = value;
                RaisePropertyChanged(() => AdjustClasses);
            }
        }

        public Models.Result.Mixed.UIClass SelectAdjustClass
        {
            get
            {
                return _selectAdjustClass;
            }

            set
            {
                _selectAdjustClass = value;
                RaisePropertyChanged(() => SelectAdjustClass);
            }
        }

        public AdjustResultViewModel()
        {
            this.TeacherResults = new List<UIResultWeek>();
            this.Teachers = new List<UITeacher>();
            this.Results = new List<UIAdjustResultWeek>();

            this.AdjustmentRecords = new ObservableCollection<ClassHourAdjustmentModel>();
            this.CourseFrames = new ObservableCollection<UIDragItem>();

            this.Classes = new List<Models.Result.Mixed.UIClass>();
            this.Students = new ObservableCollection<UIStudent>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            var local = CommonDataManager.GetLocalCase(base.LocalID);
            _colors = local.CourseColors;
        }

        Dictionary<string, string> _colors = new Dictionary<string, string>();
        public SolidColorBrush GetRandomColor(string classKey)
        {
            if (_colors.ContainsKey(classKey))
            {
                return new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString(_colors[classKey]));
            }
            else
            {
                return (SolidColorBrush)Application.Current.FindResource("main_background");
            }
        }

        public void GetData(UIResult result, MetroWindow window)
        {
            _result = result;
            _window = window;

            _localAdjust = base.LocalID.DeSerializeAdjustRecord<ResultAdjustmentModel>(_taskID);
            _localResult = base.LocalID.DeSerializeLocalResult<ResultModel>(_taskID);

            AdjustLogic.CurrentLocalResult = _localResult;
            this.RaisePropertyChanged(() => ShowRefreshButton);

            // 课程框
            var courseFrames = base.LocalID.DeSerializeCourseFrame<UIDragItem>(_taskID);
            courseFrames?.ForEach(cf =>
            {
                this.CourseFrames.Add(cf);
            });
            this.RaisePropertyChanged(() => ShowRecord);
            this.RaisePropertyChanged(() => ShowCourseFrame);

            if (_localResult != null)
            {
                _resultClasses = _localResult.ResultClasses.ToList();
            }
            else
            {
                var value = OSHttpClient.Instance.GetResult(_taskID);
                if (!value.Item1)
                {
                    if (value.Item3.IndexOf("签名不正确") != -1)
                    {
                        if (SignLogic.SignCheck())
                        {
                            value = OSHttpClient.Instance.GetResult(_taskID);
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
                    _resultClasses = _localResult.ResultClasses?.ToList();
                }
            }

            this.ShowCanNotDrag = !CanAdjust(_localResult);

            // 课时详细
            var details = (from c in _resultClasses
                           from rc in c.ResultDetails
                           select new
                           {
                               c.ClassID,
                               rc.ClassHourId,
                               rc.DayPeriod,
                               rc.Teachers,
                               c.ResultStudents,
                               rc
                           });

            #region 绑定结果

            #region 年级课位
            List<UIAdjustResultWeek> resultWeeks = new List<UIAdjustResultWeek>();
            var groupYears = _localResult.Positions.OrderBy(p => p.DayPeriod.Period).GroupBy(p => p.DayPeriod.Period);
            if (groupYears != null)
            {
                foreach (var g in groupYears)
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

            List<UIResultWeek> teacherResults = new List<UIResultWeek>();
            var teacherGroups = _localResult.Positions.OrderBy(p => p.DayPeriod.Period).GroupBy(p => p.DayPeriod.Period);
            if (teacherGroups != null)
            {
                foreach (var g in teacherGroups)
                {
                    var item = g.First();
                    UIResultWeek weekItem = new UIResultWeek()
                    {
                        Period = item.DayPeriod,
                        PositionType = item.Position,
                    };
                    teacherResults.Add(weekItem);
                }
            }
            this.TeacherResults = teacherResults;

            #endregion

            // 1.清除之间的控件
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

            #region 创建空白区域

            this.Results.ForEach(r =>
            {
                r.Monday.Content = CreateGridPanel(CreateWrapPanel(
                    new DayPeriodModel()
                    {
                        Day = DayOfWeek.Monday,
                        Period = r.Period.Period,
                        PeriodName = r.Period.PeriodName
                    }));

                r.Tuesday.Content = CreateGridPanel(CreateWrapPanel(
                    new DayPeriodModel()
                    {
                        Day = DayOfWeek.Tuesday,
                        Period = r.Period.Period,
                        PeriodName = r.Period.PeriodName
                    }));

                r.Wednesday.Content = CreateGridPanel(CreateWrapPanel(
                    new DayPeriodModel()
                    {
                        Day = DayOfWeek.Wednesday,
                        Period = r.Period.Period,
                        PeriodName = r.Period.PeriodName
                    }));

                r.Thursday.Content = CreateGridPanel(CreateWrapPanel(
                   new DayPeriodModel()
                   {
                       Day = DayOfWeek.Thursday,
                       Period = r.Period.Period,
                       PeriodName = r.Period.PeriodName
                   }));

                r.Friday.Content = CreateGridPanel(CreateWrapPanel(
                  new DayPeriodModel()
                  {
                      Day = DayOfWeek.Friday,
                      Period = r.Period.Period,
                      PeriodName = r.Period.PeriodName
                  }));

                r.Saturday.Content = CreateGridPanel(CreateWrapPanel(
                  new DayPeriodModel()
                  {
                      Day = DayOfWeek.Saturday,
                      Period = r.Period.Period,
                      PeriodName = r.Period.PeriodName
                  }));

                r.Sunday.Content = CreateGridPanel(CreateWrapPanel(
                  new DayPeriodModel()
                  {
                      Day = DayOfWeek.Sunday,
                      Period = r.Period.Period,
                      PeriodName = r.Period.PeriodName
                  }));
            });

            #endregion

            var courseFrameIDs = (from cf in this.CourseFrames from d in cf.Details select d.ClassHourId);

            var filters = details.Where(d => !courseFrameIDs.Any(c => c.Equals(d.ClassHourId)));

            // 2.绑定课时
            var groups = filters.GroupBy(g => $"{g.DayPeriod.Day}|{g.DayPeriod.Period}|{g.DayPeriod.PeriodName}");
            if (groups != null)
            {
                foreach (var g in groups)
                {
                    var first = g.First();
                    var dp = first.DayPeriod;
                    WrapPanel wp = null;

                    var items = g.ToList();

                    items?.ForEach(i =>
                    {
                        var classModel = _localResult.GetClassByClassHourID(i.ClassHourId);
                        Label label = new Label();
                        label.Background = this.GetRandomColor(classModel.CourseID);
                        label.Foreground = new SolidColorBrush(Colors.White);
                        //rb.Click += Rb_Click;
                        var teacherString = _localResult.GetTeachersByTeacherIDs(classModel.TeacherIDs)?.Select(t => t.Name)?.Parse();
                        label.Content = classModel.Display + (string.IsNullOrEmpty(teacherString) == true ? "" : "\n" + teacherString);
                        label.ToolTip = label.Content;
                        label.Opacity = 0.9;
                        label.FontSize = 13;
                        label.Margin = new System.Windows.Thickness(2);

                        var position = this.Results.FirstOrDefault(r => r.Period.Period == dp.Period);
                        if (position != null)
                        {
                            if (dp.Day == DayOfWeek.Sunday)
                            {
                                wp = ((Grid)position.Sunday.Content).Children[0] as WrapPanel;
                            }
                            else if (dp.Day == DayOfWeek.Saturday)
                            {
                                wp = ((Grid)position.Saturday.Content).Children[0] as WrapPanel;
                            }
                            else if (dp.Day == DayOfWeek.Friday)
                            {
                                wp = ((Grid)position.Friday.Content).Children[0] as WrapPanel;
                            }
                            else if (dp.Day == DayOfWeek.Thursday)
                            {
                                wp = ((Grid)position.Thursday.Content).Children[0] as WrapPanel;
                            }
                            else if (dp.Day == DayOfWeek.Wednesday)
                            {
                                wp = ((Grid)position.Wednesday.Content).Children[0] as WrapPanel;
                            }
                            else if (dp.Day == DayOfWeek.Tuesday)
                            {
                                wp = ((Grid)position.Tuesday.Content).Children[0] as WrapPanel;
                            }
                            else if (dp.Day == DayOfWeek.Monday)
                            {
                                wp = ((Grid)position.Monday.Content).Children[0] as WrapPanel;
                            }
                            wp.Children.Add(label);
                        }
                    });

                    wp.AllowDrop = true;

                    UIDragItem dragItem = wp.DataContext as UIDragItem;
                    dragItem.D_Height = wp.Height;
                    dragItem.D_Width = wp.Width;
                    dragItem.CanDrag = true;
                    dragItem.CanDrop = true;
                    dragItem.Details = items?.Select(gi =>
                    {
                        return gi.rc;
                    })?.ToList();
                }
            }

            #endregion

            #region 获取教师

            // 绑定教师
            this.Teachers = _localResult.GetTeachers();
            this.SelectTeacher = this.Teachers.FirstOrDefault();

            #endregion

            #region 班级

            List<Models.Result.Mixed.UIClass> classes = new List<Models.Result.Mixed.UIClass>();
            if (_resultClasses != null)
            {
                foreach (var d in _resultClasses)
                {
                    var classModel = _localResult.GetClassByID(d.ClassID);

                    Models.Result.Mixed.UIClass model = new Models.Result.Mixed.UIClass();
                    model.ID = d.ClassID;
                    model.Name = classModel.Name;
                    model.Display = $"{classModel.Course}{classModel.Level}{classModel.Name}";
                    model.Students = _localResult.getStudentByStudentIDs(d.ResultStudents?.ToList());
                    model.Positions = d.ResultDetails?.Select(rd => rd.DayPeriod)?.ToList();
                    classes.Add(model);
                }
            }

            this.sourceClasses = classes;
            this.Classes = classes;

            #endregion
        }

        public void CourseFrameDrop(WrapPanel sourceWP)
        {
            var dragItem = (UIDragItem)sourceWP.DataContext;
            if (dragItem.IsFromCourseFrame)
                return;

            this.RefreshStatus();

            dragItem.IsFromCourseFrame = true;

            this.CourseFrames.Add(dragItem);
            this.RaisePropertyChanged(() => ShowCourseFrame);
            this.CourseFrames?.ToList()?.SerializeCourseFrame(base.LocalID, _taskID);

            this.AdjustRecordToCourseFrame(dragItem);

            // 在结果中移除当前项
            this.RemoveDayPeriod(dragItem);
        }

        private Grid CreateGridPanel(WrapPanel wp)
        {
            var grid = new Grid();
            grid.Children.Add(wp);
            grid.DataContext = wp.DataContext;

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

        private WrapPanel CreateWrapPanel(DayPeriodModel dayPeriod)
        {
            var wp = new WrapPanel()
            {
                Name = "sp",
                Margin = new Thickness(5),
                Cursor = Cursors.Hand,
                AllowDrop = true,
                Background = new SolidColorBrush(Colors.Transparent)
            };
            wp.Drop += Wp_Drop;
            wp.MouseLeftButtonDown += Wp_MouseLeftButtonDown;
            wp.MouseMove += Wp_MouseMove;
            wp.QueryContinueDrag += Wp_QueryContinueDrag;

            wp.DataContext = new UIDragItem()
            {
                DayPeriod = dayPeriod,
                Details = new List<ResultDetailModel>()
            };

            return wp;
        }

        private void RefreshStatus()
        {
            this.Results.ForEach(r =>
            {
                if (r.Monday.Content != null)
                {
                    var wp = (r.Monday.Content as Grid).Children[0] as WrapPanel;
                    clearWrapPanelStatus(wp);
                }
                if (r.Tuesday.Content != null)
                {
                    var wp = (r.Tuesday.Content as Grid).Children[0] as WrapPanel;
                    clearWrapPanelStatus(wp);
                }

                if (r.Wednesday.Content != null)
                {
                    var wp = (r.Wednesday.Content as Grid).Children[0] as WrapPanel;
                    clearWrapPanelStatus(wp);
                }

                if (r.Thursday.Content != null)
                {
                    var wp = (r.Thursday.Content as Grid).Children[0] as WrapPanel;
                    clearWrapPanelStatus(wp);
                }

                if (r.Friday.Content != null)
                {
                    var wp = (r.Friday.Content as Grid).Children[0] as WrapPanel;
                    clearWrapPanelStatus(wp);
                }

                if (r.Saturday.Content != null)
                {
                    var wp = (r.Saturday.Content as Grid).Children[0] as WrapPanel;
                    clearWrapPanelStatus(wp);
                }

                if (r.Sunday.Content != null)
                {
                    var wp = (r.Sunday.Content as Grid).Children[0] as WrapPanel;
                    clearWrapPanelStatus(wp);
                }

            });
        }

        private void clearWrapPanelStatus(WrapPanel wp)
        {
            wp.AllowDrop = true;
            wp.Opacity = 1;
            wp.Background = new SolidColorBrush(Colors.Transparent);

            var dragItem = wp.DataContext as UIDragItem;
            dragItem.CanDrop = true;
            dragItem.RaiseStatus();
        }

        private void Wp_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {

        }

        private void Wp_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                WrapPanel wp = sender as WrapPanel;
                UIDragItem model = wp.DataContext as UIDragItem;

                if (wp == null)
                    return;

                if (model.CanDrag)
                {
                    DragDrop.DoDragDrop(wp, wp, DragDropEffects.Copy);
                }
            }
        }

        public void Wp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState != MouseButtonState.Pressed)
            {
                return;
            }

            this.RefreshStatus();

            FrameworkElement wp = sender as FrameworkElement;
            var model = wp.DataContext as UIDragItem;

            if (!model.CanDrag)
            {
                e.Handled = false;
                return;
            }

            List<DayPeriodModel> dayPeriods = new List<DayPeriodModel>();
            var brush = (SolidColorBrush)Application.Current.FindResource("main_lightgroud");
            var enable = (SolidColorBrush)Application.Current.FindResource("week_forbid");

            if (!ShowCanNotDrag)
            {
                dayPeriods = AdjustLogic.CheckCanAdjustPosition(base.LocalID, model.Details, _localResult);

                dayPeriods.ForEach(p =>
                {
                    var result = this.Results.FirstOrDefault(r => r.Period.Period == p.Period);
                    if (result != null)
                    {
                        WrapPanel panel = null;

                        switch (p.Day)
                        {
                            case DayOfWeek.Monday:
                                panel = ((Grid)result.Monday.Content).Children[0] as WrapPanel;
                                break;

                            case DayOfWeek.Tuesday:
                                panel = ((Grid)result.Tuesday.Content).Children[0] as WrapPanel;
                                break;

                            case DayOfWeek.Wednesday:
                                panel = ((Grid)result.Wednesday.Content).Children[0] as WrapPanel;
                                break;

                            case DayOfWeek.Thursday:
                                panel = ((Grid)result.Thursday.Content).Children[0] as WrapPanel;
                                break;

                            case DayOfWeek.Friday:
                                panel = ((Grid)result.Friday.Content).Children[0] as WrapPanel;
                                break;

                            case DayOfWeek.Saturday:
                                panel = ((Grid)result.Saturday.Content).Children[0] as WrapPanel;
                                break;

                            case DayOfWeek.Sunday:
                                panel = ((Grid)result.Sunday.Content).Children[0] as WrapPanel;
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

                var unAvaliableDayPeriods = this.getUnDayPeriods(dayPeriods);
                unAvaliableDayPeriods.ForEach(p =>
                {
                    var result = this.Results.FirstOrDefault(r => r.Period.Period == p.Period);
                    if (result != null)
                    {
                        WrapPanel panel = null;

                        switch (p.Day)
                        {
                            case DayOfWeek.Monday:
                                panel = ((Grid)result.Monday.Content).Children[0] as WrapPanel;
                                break;

                            case DayOfWeek.Tuesday:
                                panel = ((Grid)result.Tuesday.Content).Children[0] as WrapPanel;
                                break;

                            case DayOfWeek.Wednesday:
                                panel = ((Grid)result.Wednesday.Content).Children[0] as WrapPanel;
                                break;

                            case DayOfWeek.Thursday:
                                panel = ((Grid)result.Thursday.Content).Children[0] as WrapPanel;
                                break;

                            case DayOfWeek.Friday:
                                panel = ((Grid)result.Friday.Content).Children[0] as WrapPanel;
                                break;

                            case DayOfWeek.Saturday:
                                panel = ((Grid)result.Saturday.Content).Children[0] as WrapPanel;
                                break;

                            case DayOfWeek.Sunday:
                                panel = ((Grid)result.Sunday.Content).Children[0] as WrapPanel;
                                break;
                        }

                        panel.Background = enable;
                        panel.Opacity = 0.1;

                        UIDragItem dragItem = panel.DataContext as UIDragItem;
                        dragItem.CanDrop = false;
                        dragItem.RaiseStatus();
                    }
                });
            }
        }

        private void Wp_Drop(object sender, DragEventArgs e)
        {
            this.RefreshStatus();

            var targetWP = sender as WrapPanel;

            var rect = e.Data.GetData(typeof(Rectangle)) as Rectangle;

            var sourceWP = e.Data.GetData(typeof(WrapPanel)) as WrapPanel;

            if (rect == null)
            {
                // 如果拖拽,拖放为自己.
                if (sourceWP.Equals(targetWP))
                    return;

                var canDrag = (sourceWP.DataContext as UIDragItem).CanDrag;
                if (!canDrag)
                    return;

                this.course_Drop(sourceWP, targetWP);
            }
            else
            {
                this.courseFrame_Drop(rect, targetWP);

            }
        }

        private void refresh()
        {
            // 删除调整记录和结果
            PathHelper.DeleteTaskAdjustRecord(base.LocalID, _taskID, ".adjust");
            PathHelper.DeleteTaskAdjustRecord(base.LocalID, _taskID, ".localResult");
            PathHelper.DeleteTaskAdjustRecord(base.LocalID, _taskID, ".adjustCourseFrame");

            // 刷新按钮状态
            _localAdjust = null;
            _localResult = null;

            this.RaisePropertyChanged(() => ShowRefreshButton);

            // 清除调整记录
            this.AdjustmentRecords.Clear();
            this.RaisePropertyChanged(() => ShowRecord);

            // 清除课程框
            this.CourseFrames.Clear();
            this.RaisePropertyChanged(() => ShowCourseFrame);

            // 获取数据
            this.GetData(_result, _window);

            // 保存结果状态
            _result.IsUploaded = false;
            var local = CommonDataManager.GetLocalCase(base.LocalID);
            var result = ResultDataManager.GetResults(base.LocalID);
            local.Serizlize(result);
        }

        private void assign()
        {
            if (this.Students.All(s => !s.IsChecked))
                return;

            if (this.SelectAdjustClass == null)
            {
                this.ShowDialog("提示信息", "没有选择目标班级!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

            //1.移入目标
            this.Students.Where(s => s.IsChecked)?.ToList()?.ForEach(rs =>
              {
                  // 更新界面
                  this.SelectAdjustClass.Students.Add(rs);
                  this.SelectClass.Students.RemoveAll(r => r.ID.Equals(rs.ID));
                  this.Students.Remove(rs);

                  // 移除原先的
                  var sourceClass = _localResult.ResultClasses.FirstOrDefault(rc => rc.ClassID.Equals(this.SelectClass.ID));
                  sourceClass.ResultStudents.Remove(rs.ID);

                  // 向目标中添加
                  var targetClass = _localResult.ResultClasses.FirstOrDefault(rc => rc.ClassID.Equals(this.SelectAdjustClass.ID));
                  targetClass.ResultStudents.Add(rs.ID);

                  // 生成调整记录
                  ClassMemberAdjustmentModel classMemberAdjustment = new ClassMemberAdjustmentModel();
                  classMemberAdjustment.AdjustmentTime = DateTime.Now;
                  classMemberAdjustment.FromClassID = this.SelectClass.ID;
                  classMemberAdjustment.ToClassID = this.SelectAdjustClass.ID;
                  classMemberAdjustment.StudentID = rs.ID;
                  this.CreateAjustRecord(classMemberAdjustment);
              });

            // 刷新界面
            this.SelectAdjustClass.RaiseProperty();
            this.SelectClass.RaiseProperty();

            _localResult.SerializeLocalResult(base.LocalID, _taskID);
        }

        private void courseFrame_Drop(Rectangle source, WrapPanel target)
        {
            // 原始数据
            var sourceData = (UIDragItem)source.DataContext;

            // 目标数据
            var targetData = (UIDragItem)target.DataContext;

            if (targetData.CanDrag)
            {
                this.ShowDialog("提示信息", "从课程框拖拽必须放在空白处!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

            var targetDP = new DayPeriodModel
            {
                Day = targetData.DayPeriod.Day,
                Period = targetData.DayPeriod.Period,
                PeriodName = targetData.DayPeriod.PeriodName
            };

            #region 课程框中向回拖拽

            var wp = this.CreateWrapPanel(targetDP);
            wp.AllowDrop = true;


            sourceData.Details.ForEach(d =>
            {
                d.DayPeriod = targetDP;

                var classInfo = _localResult.GetClassByClassHourID(d.ClassHourId);

                Label label = new Label();
                label.Background = this.GetRandomColor(classInfo.CourseID);
                label.Foreground = new SolidColorBrush(Colors.White);

                var teacherString = _localResult.GetTeachersByTeacherIDs(classInfo.TeacherIDs)?.Select(t => t.Name)?.Parse();
                label.Content = classInfo.Display + (string.IsNullOrEmpty(teacherString) == true ? "" : "\n" + teacherString);
                label.ToolTip = label.Content;
                label.Opacity = 0.9;
                label.FontSize = 13;
                label.Margin = new System.Windows.Thickness(2);

                var position = this.Results.FirstOrDefault(r => r.Period.Period == targetDP.Period);
                if (position != null)
                {
                    if (targetDP.Day == DayOfWeek.Sunday)
                    {
                        wp = ((Grid)position.Sunday.Content).Children[0] as WrapPanel;
                    }
                    else if (targetDP.Day == DayOfWeek.Saturday)
                    {
                        wp = ((Grid)position.Saturday.Content).Children[0] as WrapPanel;
                    }
                    else if (targetDP.Day == DayOfWeek.Friday)
                    {
                        wp = ((Grid)position.Friday.Content).Children[0] as WrapPanel;
                    }
                    else if (targetDP.Day == DayOfWeek.Thursday)
                    {
                        wp = ((Grid)position.Thursday.Content).Children[0] as WrapPanel;
                    }
                    else if (targetDP.Day == DayOfWeek.Wednesday)
                    {
                        wp = ((Grid)position.Wednesday.Content).Children[0] as WrapPanel;
                    }
                    else if (targetDP.Day == DayOfWeek.Tuesday)
                    {
                        wp = ((Grid)position.Tuesday.Content).Children[0] as WrapPanel;
                    }
                    else if (targetDP.Day == DayOfWeek.Monday)
                    {
                        wp = ((Grid)position.Monday.Content).Children[0] as WrapPanel;
                    }
                    wp.Children.Add(label);
                }
            });

            sourceData.CanDrag = true;
            sourceData.CanDrop = true;
            sourceData.RaiseStatus();
            sourceData.IsFromCourseFrame = false;
            sourceData.D_Height = wp.Height;
            sourceData.D_Width = wp.Width;
            sourceData.DayPeriod = targetDP;

            wp.DataContext = sourceData;

            #endregion

            #region 移除课程框对影项,结果中增加该项

            this.CourseFrames.Remove(sourceData);
            this.RaisePropertyChanged(() => ShowCourseFrame);
            this.CourseFrames?.ToList()?.SerializeCourseFrame(base.LocalID, _taskID);

            // 向结果中增加
            //sourceData.Details.ForEach(d =>
            //{
            //    var classInfo = _localResult.GetClassByClassHourID(d.ClassHourId);
            //    var firstClass = this._localResult.ResultClasses.FirstOrDefault(f => f.ClassID.Equals(classInfo.ID));

            //    if (firstClass != null)
            //    {
            //        var has = firstClass.ResultDetails.Any(rd => rd.ClassHourId.Equals(d.ClassHourId));
            //        if (!has)
            //        {
            //            firstClass.ResultDetails.Add(d);
            //        }
            //    }
            //});
            //_localResult.SerializeLocalResult(base.LocalID, _taskID);

            #endregion

            AdjustRecordToCourseFrame(sourceData);

        }

        private void course_Drop(WrapPanel source, WrapPanel target)
        {
            Tuple<bool, string> result = Tuple.Create<bool, string>(true, string.Empty);

            var sourceModel = (UIDragItem)source.DataContext;
            var targetModel = (UIDragItem)target.DataContext;

            if (targetModel.Details?.Count() > 0)
            {
                if (!ShowCanNotDrag)
                    result = AdjustLogic.CanReplacePosition(base.LocalID, sourceModel.Details, targetModel.Details, _localResult);
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

        private void SetStackPanel(DayPeriodModel dayPeriod, WrapPanel wp)
        {
            var target = this.Results.FirstOrDefault(r => r.Period.Period == dayPeriod.Period);

            var dragItem = (UIDragItem)wp.DataContext;
            dragItem.DayPeriod = new DayPeriodModel()
            {
                Day = dayPeriod.Day,
                Period = dayPeriod.Period,
                PeriodName = dayPeriod.PeriodName
            };

            // 更新目标状态
            dragItem.Details?.ForEach(di =>
            {
                di.DayPeriod = dragItem.DayPeriod;
            });

            switch (dayPeriod.Day)
            {
                case DayOfWeek.Monday:
                    this.copyContentControl(target.Monday, wp);
                    break;
                case DayOfWeek.Tuesday:
                    this.copyContentControl(target.Tuesday, wp);
                    break;
                case DayOfWeek.Wednesday:
                    this.copyContentControl(target.Wednesday, wp);
                    break;
                case DayOfWeek.Thursday:
                    this.copyContentControl(target.Thursday, wp);
                    break;
                case DayOfWeek.Friday:
                    this.copyContentControl(target.Friday, wp);
                    break;
                case DayOfWeek.Saturday:
                    this.copyContentControl(target.Saturday, wp);
                    break;
                case DayOfWeek.Sunday:
                    this.copyContentControl(target.Sunday, wp);
                    break;
            }
        }

        private void copyContentControl(ContentControl content, WrapPanel wp)
        {
            // 移除与原有父级关系
            if (wp.Parent != null)
            {
                var father = wp.Parent as Grid;
                father.Children.Remove(wp);
            }

            content.Content = this.CreateGridPanel(wp);
        }

        /// <summary>
        /// 移除制定课位信息
        /// </summary>
        private void RemoveDayPeriod(UIDragItem removeItem)
        {
            var first = this.Results.FirstOrDefault(r => r.Period.Period == removeItem.DayPeriod.Period);
            if (first != null)
            {
                Grid grid = CreateGridPanel(CreateWrapPanel(removeItem.DayPeriod));
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

            //if (!isToCourseFrame)
            //{
            //    removeItem.Details.ForEach(td =>
            //    {
            //        var defaultRC = _localResult.ResultClasses.FirstOrDefault(rc => rc.ResultDetails.Any(rd => rd.ClassHourId.Equals(td.ClassHourId)));
            //        if (defaultRC != null)
            //        {
            //            defaultRC.ResultDetails.Remove(td);
            //        }
            //        _localResult.SerializeLocalResult(base.LocalID, _taskID);
            //    });
            //}


            #endregion
        }

        #region 生成调整记录

        private void AdjustRecordToEmpty(UIDragItem dragItem, UIDragItem targetItem)
        {
            var record = new ClassHourAdjustmentModel();
            record.AdjustmentTime = DateTime.Now;
            record.AdjustType = XYKernel.OS.Common.Enums.AdjustTypeEnum.Empty;
            record.Start = new List<AdjustmentDetailModel>();

            dragItem.Details.ForEach(d =>
            {
                var classInfo = _localResult.GetClassByClassHourID(d.ClassHourId);
                var teachers = _localResult.GetTeachersByTeacherIDs(d.Teachers?.ToList());

                var start = new AdjustmentDetailModel()
                {
                    ClassName = classInfo.Name,
                    CourseName = classInfo.Course,
                    LevelName = classInfo.Level,
                    TeacherNames = teachers?.Select(t => t.Name)?.ToList(),
                    TimeSlot = d.DayPeriod
                };
                record.Start.Add(start);
            });

            record.End = new List<AdjustmentDetailModel>()
            {
                new AdjustmentDetailModel()
                {
                     TimeSlot=targetItem.DayPeriod
                }
            };

            CreateAdjustRecord(record);
        }

        private void AdjustRecordToCourseFrame(UIDragItem dragItem)
        {
            var record = new ClassHourAdjustmentModel();
            record.AdjustmentTime = DateTime.Now;
            record.AdjustType = XYKernel.OS.Common.Enums.AdjustTypeEnum.Empty;
            record.Start = new List<AdjustmentDetailModel>();

            dragItem.Details.ForEach(d =>
            {
                var classInfo = _localResult.GetClassByClassHourID(d.ClassHourId);
                var teachers = _localResult.GetTeachersByTeacherIDs(d.Teachers?.ToList());

                var start = new AdjustmentDetailModel()
                {
                    ClassName = classInfo.Name,
                    CourseName = classInfo.Course,
                    LevelName = classInfo.Level,
                    TeacherNames = teachers?.Select(t => t.Name)?.ToList(),
                    TimeSlot = d.DayPeriod
                };
                record.Start.Add(start);
            });

            CreateAdjustRecord(record);
        }

        private void AdjustRecordToReplace(UIDragItem dragItem, UIDragItem targetItem)
        {
            var record = new ClassHourAdjustmentModel();
            record.AdjustmentTime = DateTime.Now;
            record.AdjustType = XYKernel.OS.Common.Enums.AdjustTypeEnum.Empty;
            record.Start = new List<AdjustmentDetailModel>();
            record.End = new List<AdjustmentDetailModel>();

            dragItem.Details.ForEach(d =>
            {
                var classInfo = _localResult.GetClassByClassHourID(d.ClassHourId);
                var teachers = _localResult.GetTeachersByTeacherIDs(d.Teachers?.ToList());

                var start = new AdjustmentDetailModel()
                {
                    ClassName = classInfo.Name,
                    CourseName = classInfo.Course,
                    LevelName = classInfo.Level,
                    TeacherNames = teachers?.Select(t => t.Name)?.ToList(),
                    TimeSlot = d.DayPeriod
                };
                record.Start.Add(start);
            });

            targetItem.Details.ForEach(d =>
            {
                var classInfo = _localResult.GetClassByClassHourID(d.ClassHourId);
                var teachers = _localResult.GetTeachersByTeacherIDs(d.Teachers?.ToList());

                var end = new AdjustmentDetailModel()
                {
                    ClassName = classInfo.Name,
                    CourseName = classInfo.Course,
                    LevelName = classInfo.Level,
                    TeacherNames = teachers?.Select(t => t.Name)?.ToList(),
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

            if (_localAdjust.ClassHourAdjustmentDetails == null)
            {
                _localAdjust.ClassHourAdjustmentDetails = new List<ClassHourAdjustmentModel>();
            }
            _localAdjust.ClassHourAdjustmentDetails.Add(record);
            _localAdjust.Serialize(base.LocalID, _taskID);
            _localResult.SerializeLocalResult(base.LocalID, _taskID);

            this.RaisePropertyChanged(() => ShowRefreshButton);
        }

        private void CreateAjustRecord(ClassMemberAdjustmentModel record)
        {
            if (!ShowRefreshButton)
            {
                _localAdjust = new ResultAdjustmentModel();
                _localAdjust.ClassMemberAdjustmentDetails = new List<ClassMemberAdjustmentModel>();
                _localAdjust.CurrentResult = _localResult;
                _localAdjust.ResultId = _result.TaskID.ToString();
            }

            if (_localAdjust.ClassMemberAdjustmentDetails == null)
            {
                _localAdjust.ClassMemberAdjustmentDetails = new List<ClassMemberAdjustmentModel>();
            }
            _localAdjust.ClassMemberAdjustmentDetails.Add(record);
            _localAdjust.Serialize(base.LocalID, _taskID);

            this.RaisePropertyChanged(() => ShowRefreshButton);
        }

        #endregion

        private bool CanAdjust(ResultModel result)
        {
            var cl = CommonDataManager.GetCLCase(base.LocalID);

            if (result.Classes.Count != cl.Classes.Count)
                return false;

            if (result.Teachers.Count != cl.Teachers.Count)
                return false;

            if (result.Courses.Count != cl.Courses.Count)
                return false;

            var allClass = result.Classes.All(c => cl.Classes.Any(cc => cc.Name.Equals(c.Name)));
            if (!allClass)
                return false;

            var allTeacher = result.Teachers.All(c => cl.Teachers.Any(cc => cc.Name.Equals(c.Name)));
            if (!allTeacher)
                return false;

            var allCourse = result.Courses.All(c => cl.Courses.Any(cc => cc.Name.Equals(c.Name)));
            if (!allCourse)
                return false;

            var classHourIDs = (from ch in result.ResultClasses from dt in ch.ResultDetails select dt.ClassHourId)?.ToList();
            var allSameClassHour = classHourIDs.All(ch => cl.ClassHours.Any(sch => sch.ID.Equals(ch)));
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

        void ShowCanAssignClass()
        {
            var cl = CommonDataManager.GetCLCase(base.LocalID);

            // 1.获取学生列表
            var students = (from s in this.Students where s.IsChecked == true select s)?.ToList();
            if (students.Count() == 0)
            {
                this.AdjustClasses = sourceClasses;
                return;
            }

            // 学生志愿班级
            List<DayPeriodModel> peselections = new List<DayPeriodModel>();
            students.ForEach(s =>
            {
                // 学生所在班级
                var studentInClass = sourceClasses.Where(sc => sc.Students.Any(ss => ss.ID.Equals(s.ID)))?.ToList();
                // 移除当前选中班级
                studentInClass.RemoveAll(sc => sc.ID.Equals(this.SelectClass.ID));
                // 学生在班级
                studentInClass?.ForEach(sic =>
                {
                    sic.Positions.ForEach(p =>
                    {
                        var has = peselections.Any(pp => pp.Day == p.Day && pp.Period == p.Period);
                        if (!has)
                        {
                            peselections.Add(p);
                        }
                    });
                });

            });

            var tempClasses = sourceClasses.ToList();
            tempClasses.RemoveAll(c => c.ID.Equals(this.SelectClass.ID));

            this.AdjustClasses = tempClasses.Where(c =>
            {
                return !c.Positions.Any(cp => peselections.Any(p => p.Day == cp.Day && p.Period == cp.Period));
            })?.ToList();
        }
    }
}
