using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using XYKernel.OS.Common.Models.Mixed;
using OSKernel.Presentation.Models.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;
using OSKernel.Presentation.Arranging.Dialog;
using OSKernel.Presentation.CustomControl;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using OSKernel.Presentation.Arranging.Mixed.Dialog;
using OSKernel.Presentation.Utilities;
using System.Dynamic;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Views
{
    public class StudentViewModel : CommonViewModel, IInitilize, IRefresh
    {
        /// <summary>
        /// 用来存储临时数据(DataGrid)
        /// </summary>
        private DataGrid _dg;

        private StudentView _view;

        private string _searchStudent;

        private ListCollectionView _studentCollectionView;

        private ObservableCollection<UIStudent> _students;

        /// <summary>
        /// 学生
        /// </summary>
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

        public ICommand CreateCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(createCommand);
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(saveCommand);
            }
        }

        public ICommand WindowCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(windowCommand);
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(deleteCommand);
            }
        }

        public ICommand ImportStudentSelectionsCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(importStudentSelectionsCommand);
            }
        }

        public ICommand ExportStudentSelectionsCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(exportStudentSelectionsCommand);
            }
        }

        public ICommand StatisticsCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(statisticsCommand);
            }
        }

        /// <summary>
        /// 搜索学生
        /// </summary>
        public string SearchStudent
        {
            get
            {
                return _searchStudent;
            }

            set
            {
                _searchStudent = value;
                RaisePropertyChanged(() => SearchStudent);

                _studentCollectionView.Refresh();
            }
        }

        public StudentViewModel()
        {
            this.Students = new ObservableCollection<UIStudent>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            var cl = base.GetClCase(base.LocalID);

            // 学生列表
            int no = 0;

            Task.Run(() =>
            {
                var students = (from s in cl.Students
                                select new UIStudent()
                                {
                                    NO = ++no,
                                    ID = s.ID,
                                    Name = s.Name,
                                    Preselections = s.Preselections.Select(p =>
                                    {
                                        UIPreselection preselection = new UIPreselection();
                                        preselection.CourseID = p.CourseID;
                                        preselection.LevelID = p.LevelID;

                                        var course = cl.Courses.FirstOrDefault(c => c.ID.Equals(p.CourseID));
                                        if (course != null)
                                        {
                                            var level = course.Levels?.FirstOrDefault(cll => cll.ID.Equals(p.LevelID));
                                            preselection.Course = course.Name;
                                            preselection.Level = level?.Name;
                                        }

                                        return preselection;

                                    })?.ToList()
                                })?.ToList();

                if (students != null)
                {
                    GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        this.Students = new ObservableCollection<UIStudent>(students);
                        _studentCollectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(this.Students);
                        _studentCollectionView.Filter = StudentContains;
                    });
                }
            });

        }

        bool StudentContains(object contain)
        {
            UIStudent student = contain as UIStudent;
            if (string.IsNullOrEmpty(this.SearchStudent))
                return true;

            if (student.Name.IndexOf(this.SearchStudent) != -1)
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 课程复选框选择事件
        /// </summary>
        /// <param name="sender">触发者</param>
        /// <param name="e">事件参数</param>
        private void courseTextBlockLoaded(object sender, RoutedEventArgs e)
        {
            var textBlock = sender as Label;
            if (!(textBlock.DataContext is UIStudent))
                return;

            var student = textBlock.DataContext as UIStudent;
            var key = textBlock.Tag.ToString();

            var studentRule = this.Students.FirstOrDefault(s => s.ID.Equals(student.ID));
            if (studentRule != null)
            {
                var selection = studentRule.Preselections.FirstOrDefault(p => p.Course.Equals(key));
                if (selection != null)
                {
                    if (string.IsNullOrEmpty(selection.Level))
                    {
                        textBlock.Content = selection.Course;
                    }
                    else
                    {
                        textBlock.Content = selection.Level;
                    }
                }
            }
        }

        void createCommand()
        {
            CreateStudentWindow window = new CreateStudentWindow();
            window.Closed += (s, args) =>
            {
                if (window.DialogResult.Value)
                {
                    var has = this.Students.Any(c =>
                    {
                        return window.Students.Any(cc => cc.Equals(c.Name));
                    });

                    if (has)
                    {
                        var result = this.ShowDialog("提示信息", "存在相同学生,是否继续添加", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
                        if (result != CustomControl.Enums.DialogResultType.OK)
                        {
                            return;
                        }
                    }

                    var cl = base.GetClCase(base.LocalID);

                    var last = this.Students.LastOrDefault();
                    int no = last == null ? 0 : last.NO;

                    foreach (var t in window.Students)
                    {
                        var studentID = this.Students.Count == 0 ? 0 : this.Students.Max(st => Convert.ToInt64(st.ID));
                        this.Students.Add(new UIStudent
                        {
                            NO = ++no,
                            ID = (studentID + 1).ToString(),
                            Name = t,
                        });
                    }
                }
            };
            window.ShowDialog();
        }

        void batchDeleteCommand()
        {
            var cl = CommonDataManager.GetCLCase(base.LocalID);

            var dialog = this.ShowDialog("提示信息", "确认删除?", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
            if (dialog == CustomControl.Enums.DialogResultType.OK)
            {

            }
        }

        void windowCommand(object obj)
        {
            StudentView view = obj as StudentView;
            _dg = view.dg;
            _view = view;

            if (_dg != null)
            {
                if (_dg.Columns.Count > 3)
                {
                    var number = _dg.Columns.Count;

                    for (int i = number; i > 3; i--)
                    {
                        _dg.Columns.Remove(_dg.Columns[i - 1]);
                    }
                }

                var cl = CommonDataManager.GetCLCase(base.LocalID);

                cl.Courses.ForEach(c =>
                {
                    DataGridTextColumn dgtc = new DataGridTextColumn();
                    dgtc.Header = c.Name;
                    dgtc.Binding = new Binding($"ExpandoObject.{c.Name}");

                    //DataTemplate dt = new DataTemplate();
                    //FrameworkElementFactory factory = new FrameworkElementFactory(typeof(Label));
                    //factory.SetValue(Label.TagProperty, c.Name);
                    //factory.AddHandler(Label.LoadedEvent, new RoutedEventHandler(courseTextBlockLoaded));
                    //dt.VisualTree = factory;

                    //dgtc.CellTemplate = dt;

                    GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        _dg.Columns.Add(dgtc);
                    });
                });

                this.RefreshStudentPreselections();

                _dg.ItemsSource = this.Students;
            }
        }

        void deleteCommand(object obj)
        {
            UIStudent student = obj as UIStudent;
            var result = this.ShowDialog("提示信息", "确认删除?", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
            if (result == CustomControl.Enums.DialogResultType.OK)
            {
                this.Students.Remove(student);
            }
        }

        void saveCommand()
        {
            var noChooseSameCourseOnly = this.Students.Any(s =>
              {
                  var courseGroups = s.Preselections.Where(p => p.IsChecked).GroupBy(p => p.CourseID);
                  if (courseGroups.Count() > 1)
                      return true;
                  else
                      return false;
              });

            if (noChooseSameCourseOnly)
            {
                this.ShowDialog("提示信息", "请检查学生志愿,同一科目不同分层只能选择一个!", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.Warning);
                return;
            }


            var cl = base.GetClCase(base.LocalID);

            // 清空学生志愿
            cl.Students = this.Students.Select(s =>
           {
               return new StudentModel()
               {
                   ID = s.ID,
                   Name = s.Name,
                   Preselections = s.Preselections?.Select(p =>
                   {
                       return new PreselectionModel()
                       {
                           CourseID = p.CourseID,
                           LevelID = p.LevelID
                       };
                   })?.ToList()
               };
           })?.ToList();

            // 3.保存
            base.Serialize(cl, LocalID);

            this.ShowDialog("提示信息", "保存成功!", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
        }

        void importStudentSelectionsCommand()
        {
            var cl = CommonDataManager.GetCLCase(base.LocalID);

            var preselections = (from c in cl.Courses
                                 from cc in c.Levels
                                 select new UIPreselection()
                                 {
                                     CourseID = c.ID,
                                     Course = c.Name,
                                     LevelID = cc.ID,
                                     Level = cc.Name,
                                 })?.ToList();

            ImportStudentSelectionWindow window = new ImportStudentSelectionWindow();
            window.Closed += (s, args) =>
            {
                if (window.DialogResult.Value)
                {
                    // 清空当前志愿
                    //foreach (var st in this.Students)
                    //{
                    //    st.Preselections?.Clear();
                    //}

                    //清空所有学生
                    this.Students.Clear();

                    var tables = window.DT;
                    var columnsCount = tables.Columns.Count;

                    // 内容
                    for (int i = 1; i < tables.Rows.Count; i++)
                    {
                        var name = tables.Rows[i][0].ToString();

                        if (string.IsNullOrEmpty(name))
                        {
                            continue;
                        }

                        var student = this.Students.FirstOrDefault(st => st.Name.Equals(name));

                        if (student == null)
                        {
                            int number = this.Students.Count == 0 ? 0 : this.Students.Max(st => Convert.ToInt32(st.ID));
                            int no = this.Students.Count == 0 ? 0 : this.Students.Max(st => st.NO);

                            student = new UIStudent()
                            {
                                ID = (number + 1).ToString(),
                                NO = no + 1,
                                Name = name,
                            };

                            this.Students.Add(student);
                        }

                        // 遍历其它志愿
                        for (int h = 1; h < columnsCount; h++)
                        {
                            var value = tables.Rows[i][h].ToString();
                            if (!string.IsNullOrEmpty(value))
                            {
                                var columnName = tables.Rows[0][h].ToString();

                                // 如果有层没有输入层的名字,直接输入科目的名字。
                                var firstCourse = cl.Courses.FirstOrDefault(clc => clc.Name.Equals(value));
                                if (firstCourse?.Levels?.Count > 1)
                                {
                                    continue;
                                }

                                UIPreselection selection;
                                var selections = preselections.Where(p => p.Course.Equals(columnName))?.ToList();

                                if (!columnName.Equals(value))
                                {
                                    selection = selections?.FirstOrDefault(ss => ss.Level.Equals(value));
                                }
                                else
                                {
                                    selection = selections.FirstOrDefault();
                                }

                                if (selection != null)
                                {
                                    var has = student.Preselections.Any(p => p.Course.Equals(selection.Course));
                                    if (!has)
                                    {
                                        student.Preselections.Add(selection);
                                    }
                                }
                            }
                        }
                    }

                    // 重新绑定界面
                    windowCommand(_view);
                }
            };
            window.ShowDialog();
        }

        void exportStudentSelectionsCommand()
        {
            var local = CommonDataManager.GetLocalCase(base.LocalID);

            System.Windows.Forms.SaveFileDialog saveDialog = new System.Windows.Forms.SaveFileDialog();
            saveDialog.Filter = "Microsoft Excel files(*.xls)|*.xls;*.xlsx";
            saveDialog.FileName = $"{local.Name}(学生志愿)";
            var result = saveDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                #region 获取所有学生志愿

                var cl = CommonDataManager.GetCLCase(base.LocalID);

                var preselections = (from c in cl.Courses
                                     from cc in c.Levels
                                     select new UIPreselection()
                                     {
                                         CourseID = c.ID,
                                         Course = c.Name,
                                         LevelID = cc.ID,
                                         Level = cc.Name,
                                     })?.ToList();

                #endregion

                string filePath = saveDialog.FileName;

                System.Data.DataTable dt = new System.Data.DataTable();

                #region 学生姓名
                System.Data.DataColumn studentColumn = new System.Data.DataColumn()
                {
                    ColumnName = "姓名"
                };
                dt.Columns.Add(studentColumn);
                #endregion

                #region 动态添加其它列

                foreach (var c in cl.Courses)
                {
                    dt.Columns.Add(
                        new System.Data.DataColumn()
                        {
                            ColumnName = c.Name
                        });
                }

                #endregion

                #region 填充行内容

                foreach (var student in this.Students)
                {
                    System.Data.DataRow newRow = dt.NewRow();
                    newRow["姓名"] = student.Name;

                    student.Preselections.ForEach(sp =>
                    {
                        var ps = preselections.FirstOrDefault(p => p.CourseID.Equals(sp.CourseID) && p.LevelID.Equals(sp.LevelID));
                        if (ps != null)
                        {
                            if (string.IsNullOrEmpty(ps.Level))
                            {
                                newRow[ps.Course] = ps.Course;
                            }
                            else
                            {
                                newRow[ps.Course] = ps.Level;
                            }

                        }
                    });

                    dt.Rows.Add(newRow);
                }

                #endregion

                var excelResult = NPOIClass.DataTableToExcel(dt, filePath);
                if (excelResult.Item1)
                {
                    this.ShowDialog("提示信息", "导出成功", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
                }
                else
                {
                    this.ShowDialog("提示信息", excelResult.Item2, CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Error);
                    return;
                }
            }
        }

        void statisticsCommand()
        {
            var cl = CommonDataManager.GetCLCase(base.LocalID);

            var preselections = (from c in cl.Courses
                                 from cc in c.Levels
                                 select new UIPreselection()
                                 {
                                     CourseID = c.ID,
                                     Course = c.Name,
                                     LevelID = cc.ID,
                                     Level = cc.Name,
                                 })?.ToList();

            List<string> combinations = new List<string>();
            foreach (var p in preselections)
            {
                int count = this.Students.Count(s => s.Preselections.Any(sp => sp.LevelID.Equals(p.LevelID) && sp.CourseID.Equals(p.CourseID)));
                if (count == 0)
                {
                    combinations.Add($"{p.Display} 无人");
                }
                else
                {
                    combinations.Add($"{p.Display} {count}人");
                }
            }

            StatisticStudentSelectionWindow window = new StatisticStudentSelectionWindow(combinations);
            window.ShowDialog();
        }

        public void Refresh()
        {
            this.Initilize();
            this.RefreshStudentPreselections();
        }

        /// <summary>
        /// 刷新学生志愿UI
        /// </summary>
        public void RefreshStudentPreselections()
        {
            var cl = CommonDataManager.GetCLCase(base.LocalID);

            foreach (var student in Students)
            {
                IDictionary<string, object> dics = new ExpandoObject();
                cl.Courses.ForEach(c =>
                {
                    dics.Add(c.Name, string.Empty);
                });

                student.Preselections.ForEach(p =>
                {
                    // 列头显示情况。
                    var hasColumn = dics.ContainsKey(p.Course);
                    if (hasColumn)
                    {
                        if (!p.LevelID.Equals("0"))
                        {
                            dics[p.Course] = p.Level;
                        }
                        else
                        {
                            dics[p.Course] = p.Course;
                        }
                    }
                });

                student.ExpandoObject = dics;
            }
        }

    }
}
