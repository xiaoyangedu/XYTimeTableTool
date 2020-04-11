using MahApps.Metro.Controls;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.Http;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Base;
using OSKernel.Presentation.Models.Result;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Unity;
using XYKernel.OS.Common.Models.Administrative;
using XYKernel.OS.Common.Models.Administrative.Result;

namespace OSKernel.Presentation.Arranging.Administrative.Result
{
    public class ResultDetailsViewModel : CommonViewModel, IInitilize
    {
        private List<UIClass> _classes;

        private List<UITeacher> _teachers;

        private UITeacher _selectTeacher;

        private UIClass _selectClass;

        private List<UIResultWeek> _results;

        private List<UIResultWeek> _teacherResults;

        public List<UIResultWeek> Results
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
                    this.Results.ForEach(r =>
                    {
                        r.Mondays.Clear();
                        r.Tuesdays.Clear();
                        r.Wednesdays.Clear();
                        r.Thursdays.Clear();
                        r.Fridays.Clear();
                        r.Saturdays.Clear();
                        r.Sundays.Clear();
                    });

                    var firstClass = _resultClasses.FirstOrDefault(f => f.ClassID.Equals(_selectClass.ID));
                    if (firstClass != null)
                    {
                        var normals = firstClass.ResultDetails?.Where(rd => rd.ResultType == XYKernel.OS.Common.Enums.ClassHourResultType.Normal)?.ToList();

                        normals?.ForEach(n =>
                        {
                            var solidColorBrush = this.GetRandomColor(n.CourseID);

                            StackPanel sp = new StackPanel();
                            sp.Drop += Sp_Drop;
                            sp.AllowDrop = true;
                            sp.Margin = new Thickness(5);
                            sp.Background = solidColorBrush;

                            Label text = new Label();
                            text.HorizontalContentAlignment = HorizontalAlignment.Center;
                            text.VerticalContentAlignment = VerticalAlignment.Center;
                            text.Margin = new Thickness(2);
                            text.Foreground = new SolidColorBrush(Colors.White);
                            text.FontWeight = FontWeights.Bold;
                            text.FontSize = 20;
                            text.MinWidth = 80;
                            //text.MouseMove += Text_MouseMove;
                            text.DataContext = n;
                            sp.Children.Add(text);

                            var cp = CommonDataManager.GetCPCase(base.LocalID);

                            var hourInfo = cp.ClassHours.FirstOrDefault(c => c.ID == n.ClassHourId);
                            if (hourInfo != null)
                            {
                                var teachers = cp.GetTeachersByIds(hourInfo.TeacherIDs);

                                Label teacherText = new Label();
                                teacherText.HorizontalContentAlignment = HorizontalAlignment.Center;
                                teacherText.VerticalContentAlignment = VerticalAlignment.Center;
                                teacherText.Content = teachers.Select(t => t.Name)?.Parse();
                                teacherText.Foreground = new SolidColorBrush(Colors.White);
                                //teacherText.FontWeight = FontWeights.Bold;
                                teacherText.FontSize = 13;
                                teacherText.ToolTip = teacherText.Content;
                                sp.Children.Add(teacherText);
                            }

                            var course = _courses.FirstOrDefault(c => c.ID.Equals(n.CourseID));
                            if (course != null)
                            {
                                text.Content = course.Name;
                            }

                            var position = this.Results.FirstOrDefault(r => r.Period.Period == n.DayPeriod.Period);
                            if (position != null)
                            {
                                ContentControl content = new ContentControl()
                                {
                                    Content = sp
                                };

                                if (n.DayPeriod.Day == DayOfWeek.Sunday)
                                {
                                    position.Sundays.Add(content);
                                }
                                else if (n.DayPeriod.Day == DayOfWeek.Saturday)
                                {
                                    position.Saturdays.Add(content);
                                }
                                else if (n.DayPeriod.Day == DayOfWeek.Friday)
                                {
                                    position.Fridays.Add(content);
                                }
                                else if (n.DayPeriod.Day == DayOfWeek.Thursday)
                                {
                                    position.Thursdays.Add(content);
                                }
                                else if (n.DayPeriod.Day == DayOfWeek.Wednesday)
                                {
                                    position.Wednesdays.Add(content);
                                }
                                else if (n.DayPeriod.Day == DayOfWeek.Tuesday)
                                {
                                    position.Tuesdays.Add(content);
                                }
                                else if (n.DayPeriod.Day == DayOfWeek.Monday)
                                {
                                    position.Mondays.Add(content);
                                }
                            }
                        });

                        var mulitplys = firstClass.ResultDetails?.Where(rd => rd.ResultType != XYKernel.OS.Common.Enums.ClassHourResultType.Normal)?.ToList();

                        mulitplys?.ForEach(m =>
                        {
                            var solidColorBrush = this.GetRandomColor(m.CourseID);

                            StackPanel sp = new StackPanel();
                            sp.Margin = new Thickness(5);
                            sp.Background = solidColorBrush;

                            Label text = new Label();
                            text.HorizontalContentAlignment = HorizontalAlignment.Center;
                            text.VerticalContentAlignment = VerticalAlignment.Center;
                            text.Margin = new Thickness(2);
                            text.Content = m.CourseID;
                            text.Foreground = new SolidColorBrush(Colors.White);
                            text.FontWeight = FontWeights.Bold;
                            text.FontSize = 20;
                            text.MouseMove += Text_MouseMove;
                            text.DataContext = m;
                            sp.Children.Add(text);

                            var cp = CommonDataManager.GetCPCase(base.LocalID);

                            var hourInfo = cp.ClassHours.FirstOrDefault(c => c.ID == m.ClassHourId);
                            if (hourInfo != null)
                            {
                                var teachers = cp.GetTeachersByIds(hourInfo.TeacherIDs);

                                Label teacherText = new Label();
                                teacherText.HorizontalContentAlignment = HorizontalAlignment.Center;
                                teacherText.VerticalContentAlignment = VerticalAlignment.Center;
                                teacherText.Margin = new Thickness(2);
                                teacherText.Content = teachers.Select(t => t.Name)?.Parse();
                                teacherText.Foreground = new SolidColorBrush(Colors.White);
                                //teacherText.FontWeight = FontWeights.Bold;
                                teacherText.FontSize = 13;
                                teacherText.ToolTip = teacherText.Content;
                                sp.Children.Add(teacherText);
                            }

                            var course = _courses.FirstOrDefault(c => c.ID.Equals(m.CourseID));
                            if (course != null)
                            {
                                text.Content = course.Name;
                            }

                            var position = this.Results.FirstOrDefault(r => r.Period.Period == m.DayPeriod.Period);
                            if (position != null)
                            {
                                ContentControl content = new ContentControl()
                                {
                                    Content = sp
                                };

                                if (m.DayPeriod.Day == DayOfWeek.Sunday)
                                {
                                    position.Sundays.Add(content);
                                }
                                else if (m.DayPeriod.Day == DayOfWeek.Saturday)
                                {
                                    position.Saturdays.Add(content);
                                }
                                else if (m.DayPeriod.Day == DayOfWeek.Friday)
                                {
                                    position.Fridays.Add(content);
                                }
                                else if (m.DayPeriod.Day == DayOfWeek.Thursday)
                                {
                                    position.Thursdays.Add(content);
                                }
                                else if (m.DayPeriod.Day == DayOfWeek.Wednesday)
                                {
                                    position.Wednesdays.Add(content);
                                }
                                else if (m.DayPeriod.Day == DayOfWeek.Tuesday)
                                {
                                    position.Tuesdays.Add(content);
                                }
                                else if (m.DayPeriod.Day == DayOfWeek.Monday)
                                {
                                    position.Mondays.Add(content);
                                }
                            }
                        });
                    }
                }
            }
        }

        private void Text_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Label label = sender as Label;
            if (label != null && e.LeftButton == MouseButtonState.Pressed)
            {
                // 拖拽控件
                DragDrop.DoDragDrop(label, label.DataContext, DragDropEffects.Copy);
            }
        }

        private void Sp_Drop(object sender, DragEventArgs e)
        {
            var result = e.Data.GetData(typeof(ResultDetailModel));
            if (result != null)
            {
                // 获取数据
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

                    filters.ForEach(rc =>
                    {
                        var solidColorBrush = this.GetRandomColor(rc.CourseID);

                        StackPanel sp = new StackPanel();
                        sp.HorizontalAlignment = HorizontalAlignment.Stretch;
                        sp.VerticalAlignment = VerticalAlignment.Stretch;
                        sp.Margin = new Thickness(5);
                        sp.Background = solidColorBrush;

                        Label text = new Label();
                        text.HorizontalContentAlignment = HorizontalAlignment.Center;
                        text.VerticalContentAlignment = VerticalAlignment.Center;
                        text.Margin = new Thickness(2);
                        text.Foreground = new SolidColorBrush(Colors.White);
                        text.FontWeight = FontWeights.Bold;
                        text.FontSize = 20;
                        sp.Children.Add(text);

                        var course = _courses.FirstOrDefault(c => c.ID.Equals(rc.CourseID));
                        var classInfo = this.Classes.FirstOrDefault(c => c.ID.Equals(rc.ClassID));
                        text.Content = $"{course?.Name}-{classInfo?.Name}";

                        var position = this.TeacherResults.FirstOrDefault(r => r.Period.Period == rc.DayPeriod.Period);
                        if (position != null)
                        {
                            ContentControl content = new ContentControl()
                            {
                                Content = sp
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

        public ResultDetailsViewModel()
        {
            this.Classes = new List<UIClass>();
            this.Teachers = new List<UITeacher>();
            this.Results = new List<UIResultWeek>();
            this.TeacherResults = new List<UIResultWeek>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            var cp = CommonDataManager.GetCPCase(base.LocalID);
            var local = CommonDataManager.GetLocalCase(base.LocalID);
            _colors = local.CourseColors;

            #region 班级课位

            List<UIResultWeek> resultWeeks = new List<UIResultWeek>();
            var groups = cp.Positions.OrderBy(p => p.DayPeriod.Period).GroupBy(p => p.DayPeriod.Period);
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
                    resultWeeks.Add(weekItem);
                }
            }

            this.Results = resultWeeks;

            #endregion

            #region 教师课位

            List<UIResultWeek> teacherWeeks = new List<UIResultWeek>();
            var teacherGroups = cp.Positions.OrderBy(p => p.DayPeriod.Period).GroupBy(p => p.DayPeriod.Period);
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
        }

        MetroWindow _resultDetailsWindow;
        List<ResultClassModel> _resultClasses;
        List<CourseModel> _courses;

        public void GetData(UIResult result, MetroWindow window)
        {
            _resultDetailsWindow = window;
            _resultDetailsWindow.Title = $"{result.Name} 结果";

            var cp = CommonDataManager.GetCPCase(base.LocalID);
            _courses = cp.Courses;

            var value = OSHttpClient.Instance.GetAdminResult(result.TaskID);

            if (!value.Item1)
            {
                if (value.Item3.IndexOf("签名不正确") != -1)
                {
                    if (SignLogic.SignCheck())
                    {
                        value = OSHttpClient.Instance.GetAdminResult(result.TaskID);
                    }
                }
            }

            if (value.Item1)
            {
                _resultClasses = value.Item2.ResultClasses?.ToList();

                List<UIClass> classes = new List<UIClass>();
                List<UITeacher> teachers = new List<UITeacher>();

                if (_resultClasses != null)
                {
                    // 1.根据结果获取班级
                    foreach (var rc in _resultClasses)
                    {
                        var classInfo = cp.Classes.FirstOrDefault(c => c.ID.Equals(rc.ClassID));
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

                    // 2.根据班级获取教师
                    var classHourIDs = from c in _resultClasses from rd in c.ResultDetails select rd.ClassHourId;
                    var classHours = cp.GetClassHours(classHourIDs?.ToArray());
                    classHours?.ForEach(ch =>
                    {
                        ch.Teachers.ForEach(t =>
                        {
                            var teacher = teachers.FirstOrDefault(tt => tt.ID.Equals(t.ID));
                            if (teacher == null)
                            {
                                teachers.Add(new UITeacher()
                                {
                                    ID = t.ID,
                                    Name = t.Name,
                                    ClassHourIDs = new List<int> { ch.ID }
                                });
                            }
                            else
                            {
                                teacher.ClassHourIDs.Add(ch.ID);
                            }
                        });
                    });
                }

                this.Classes = classes;
                this.SelectClass = this.Classes.FirstOrDefault();

                this.Teachers = teachers;
                this.SelectTeacher = teachers?.FirstOrDefault();
            }
        }
        Dictionary<string, string> _colors = new Dictionary<string, string>();

       

        public SolidColorBrush GetRandomColor(string classKey)
        {
            if (_colors.ContainsKey(classKey))
            {
                return new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString(_colors[classKey]));
            }
            else
                return new SolidColorBrush(Colors.Black);
        }
    }
}
