using MahApps.Metro.Controls;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.Http;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Base;
using OSKernel.Presentation.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Unity;
using XYKernel.OS.Common.Models.Mixed.Result;

namespace OSKernel.Presentation.Arranging.Mixed.Result
{
    public class ResultDetailsViewModel : CommonViewModel, IInitilize
    {
        private List<UIResultWeek> _results;

        private List<UIResultWeek> _teacherResults;

        private List<UITeacher> _teachers;

        private UITeacher _selectTeacher;

        private List<Button> _buttons;

        private List<Button> _teacherButtons;

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

                    var cl = CommonDataManager.GetCLCase(base.LocalID);

                    filters.ForEach(rc =>
                    {
                        var classModel = cl.GetClassByID(rc.ClassID);

                        Button rb = new Button();
                        rb.Style = (Style)_window.FindResource("ItemStyle");
                        rb.Background = this.GetRandomColor(classModel.CourseID);
                        rb.Click += Rb_Click1;
                        rb.Content = classModel.Display;
                        rb.Opacity = 0.9;
                        rb.FontSize = 13;
                        rb.Margin = new System.Windows.Thickness(2);

                        var position = this.TeacherResults.FirstOrDefault(r => r.Period.Period == rc.DayPeriod.Period);
                        if (position != null)
                        {
                            if (rc.DayPeriod.Day == DayOfWeek.Friday)
                            {
                                position.Fridays.Add(rb);
                            }
                            else if (rc.DayPeriod.Day == DayOfWeek.Thursday)
                            {
                                position.Thursdays.Add(rb);
                            }
                            else if (rc.DayPeriod.Day == DayOfWeek.Wednesday)
                            {
                                position.Wednesdays.Add(rb);
                            }
                            else if (rc.DayPeriod.Day == DayOfWeek.Tuesday)
                            {
                                position.Tuesdays.Add(rb);
                            }
                            else if (rc.DayPeriod.Day == DayOfWeek.Monday)
                            {
                                position.Mondays.Add(rb);
                            }
                            else if (rc.DayPeriod.Day == DayOfWeek.Saturday)
                            {
                                position.Saturdays.Add(rb);
                            }
                            else if (rc.DayPeriod.Day == DayOfWeek.Sunday)
                            {
                                position.Sundays.Add(rb);
                            }
                        }

                        _teacherButtons.Add(rb);
                    });

                }
            }
        }

        private void Rb_Click1(object sender, RoutedEventArgs e)
        {
            Button select = sender as Button;

            var selects = _teacherButtons.Where(b => b.Content.ToString().Equals(select.Content.ToString()));
            var unselects = _teacherButtons.Where(b => !b.Content.ToString().Equals(select.Content.ToString()));

            //LinearGradientBrush linearGradientBrush = new LinearGradientBrush();

            //linearGradientBrush.GradientStops.Add(new GradientStop()
            //{
            //    Color = (Color)ColorConverter.ConvertFromString("#65CEFE"),
            //    Offset = 0.25
            //});

            //linearGradientBrush.GradientStops.Add(new GradientStop()
            //{
            //    Color = (Color)ColorConverter.ConvertFromString("#5BBFFA"),
            //    Offset = 0.45
            //});

            //linearGradientBrush.GradientStops.Add(new GradientStop()
            //{
            //    Color = (Color)ColorConverter.ConvertFromString("#49A5F5"),
            //    Offset = 0.85
            //});

            selects?.ToList()?.ForEach(b =>
            {
                b.FontSize = 25;
            });

            unselects?.ToList()?.ForEach(b =>
            {
                b.FontSize = 13;
            });
        }

        public ResultDetailsViewModel()
        {
            this.Results = new List<UIResultWeek>();
            this._buttons = new List<Button>();

            this.TeacherResults = new List<UIResultWeek>();
            this._teacherButtons = new List<Button>();
            this.Teachers = new List<UITeacher>();
        }

        List<ResultClassModel> _resultClasses;
        MetroWindow _window;

        private UIResult _result;
        private long _taskID
        {
            get
            {
                return _result.TaskID;
            }
        }

        public void GetData(UIResult result, MetroWindow window)
        {
            _window = window;
            _window.Title = $"{result.Name} 结果";

            var value = OSHttpClient.Instance.GetResult(result.TaskID);

            if (!value.Item1)
            {
                if (value.Item3.IndexOf("签名不正确") != -1)
                {
                    if (SignLogic.SignCheck())
                    {
                        value = OSHttpClient.Instance.GetResult(result.TaskID);
                    }
                }
            }

            if (value.Item1)
            {
                // 获取模型
                var model = value.Item2;

                // 结果班级
                _resultClasses = model.ResultClasses?.ToList();

                // 走班模型
                var cl = CommonDataManager.GetCLCase(base.LocalID);

                // 课时详细
                var details = (from c in model.ResultClasses
                               from rc in c.ResultDetails
                               select new
                               {
                                   c.ClassID,
                                   rc.ClassHourId,
                                   rc.DayPeriod
                               });

                List<UITeacher> teachers = new List<UITeacher>();

                foreach (var rc in model.ResultClasses)
                {
                    var classModel = cl.GetClassByID(rc.ClassID);

                    if (rc.ResultDetails != null)
                    {
                        foreach (var rd in rc.ResultDetails)
                        {
                            Button rb = new Button();
                            rb.Style = (Style)window.FindResource("ItemStyle");
                            rb.Background = this.GetRandomColor(classModel.CourseID);
                            rb.Click += Rb_Click;
                            rb.Content = classModel.Display;
                            rb.Opacity = 0.9;
                            rb.FontSize = 13;
                            rb.Margin = new System.Windows.Thickness(2);

                            var position = this.Results.FirstOrDefault(r => r.Period.Period == rd.DayPeriod.Period);
                            if (position != null)
                            {
                                if (rd.DayPeriod.Day == DayOfWeek.Friday)
                                {
                                    position.Fridays.Add(rb);
                                }
                                else if (rd.DayPeriod.Day == DayOfWeek.Thursday)
                                {
                                    position.Thursdays.Add(rb);
                                }
                                else if (rd.DayPeriod.Day == DayOfWeek.Wednesday)
                                {
                                    position.Wednesdays.Add(rb);
                                }
                                else if (rd.DayPeriod.Day == DayOfWeek.Tuesday)
                                {
                                    position.Tuesdays.Add(rb);
                                }
                                else if (rd.DayPeriod.Day == DayOfWeek.Monday)
                                {
                                    position.Mondays.Add(rb);
                                }
                                else if (rd.DayPeriod.Day == DayOfWeek.Saturday)
                                {
                                    position.Saturdays.Add(rb);
                                }
                                else if (rd.DayPeriod.Day == DayOfWeek.Sunday)
                                {
                                    position.Sundays.Add(rb);
                                }
                            }

                            _buttons.Add(rb);
                        }
                    }

                    #region 获取教师信息

                    var classHourIDs = from c in details select c.ClassHourId;
                    var classHours = cl.GetClassHours(classHourIDs?.ToArray());
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
                                var has = teacher.ClassHourIDs.Contains(ch.ID);
                                if (!has)
                                {
                                    teacher.ClassHourIDs.Add(ch.ID);
                                }
                            }
                        });
                    });

                    #endregion
                }

                this.Teachers = teachers;
                this.SelectTeacher = teachers.FirstOrDefault();
            }
        }

        private void Rb_Click(object sender, RoutedEventArgs e)
        {
            Button select = sender as Button;

            var parent = select.Parent;

            var selects = _buttons.Where(b => b.Content.ToString().Equals(select.Content.ToString()));
            var unselects = _buttons.Where(b => !b.Content.ToString().Equals(select.Content.ToString()));

            //LinearGradientBrush linearGradientBrush = new LinearGradientBrush();

            //linearGradientBrush.GradientStops.Add(new GradientStop()
            //{
            //    Color = (Color)ColorConverter.ConvertFromString("#65CEFE"),
            //    Offset = 0.25
            //});

            //linearGradientBrush.GradientStops.Add(new GradientStop()
            //{
            //    Color = (Color)ColorConverter.ConvertFromString("#5BBFFA"),
            //    Offset = 0.45
            //});

            //linearGradientBrush.GradientStops.Add(new GradientStop()
            //{
            //    Color = (Color)ColorConverter.ConvertFromString("#49A5F5"),
            //    Offset = 0.85
            //});

            selects?.ToList()?.ForEach(b =>
            {
                b.FontSize = 25;
            });

            unselects?.ToList()?.ForEach(b =>
            {
                b.FontSize = 13;
            });
        }

        [InjectionMethod]
        public void Initilize()
        {
            var cl = CommonDataManager.GetCLCase(base.LocalID);

            var local = CommonDataManager.GetLocalCase(base.LocalID);
            _colors = local.CourseColors;

            #region 班级课位

            List<UIResultWeek> resultWeeks = new List<UIResultWeek>();
            var groups = cl.Positions.OrderBy(p => p.DayPeriod.Period).GroupBy(p => p.DayPeriod.Period);
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

            List<UIResultWeek> teacherResults = new List<UIResultWeek>();

            var teacherGroups = cl.Positions.OrderBy(p => p.DayPeriod.Period).GroupBy(p => p.DayPeriod.Period);
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
