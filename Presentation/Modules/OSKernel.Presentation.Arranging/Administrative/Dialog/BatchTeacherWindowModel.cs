using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Base;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;

namespace OSKernel.Presentation.Arranging.Administrative.Dialog
{
    /// <summary>
    /// 批量设置教师
    /// </summary>
    public class BatchTeacherWindowModel : CommonViewModel, IInitilize
    {
        private List<UITeacher> _teachers;
        private List<UICourse> _courses;
        private List<UIClass> _classes;
        private List<UIClass> values;

        private UICourse _selectCourse;
        private UITeacher _selectTeacher;

        public List<Models.Base.UITeacher> Teachers
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
        public Models.Base.UITeacher SelectTeacher
        {
            get
            {
                return _selectTeacher;
            }

            set
            {
                _selectTeacher = value;
                RaisePropertyChanged(() => SelectTeacher);

                if (SelectTeacher != null)
                {
                    this.SelectCourse = null;

                    this.RaisePropertyChanged(() => this.ShowEmpty);
                    this.Courses.ForEach(c =>
                    {
                        // 2.绑定当前教师的选中情况
                        c.SelectClasses = values.Count(v =>
                        {
                            var course = v.Courses.FirstOrDefault(vc => vc.CourseID.Equals(c.ID));
                            if (course != null)
                            {
                                return course.Teachers.Any(t => t.ID.Equals(this.SelectTeacher.ID));
                            }
                            else
                            {
                                return false;
                            }
                        });

                        // 清空选择课程
                        this.SelectCourse = null;
                    });
                }
            }
        }

        /// <summary>
        /// 课程
        /// </summary>
        public List<UICourse> Courses
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

        /// <summary>
        /// 选中科目
        /// </summary>
        public UICourse SelectCourse
        {
            get
            {
                return _selectCourse;
            }

            set
            {
                _selectCourse = value;
                RaisePropertyChanged(() => SelectCourse);

                if (_selectCourse != null)
                {
                    var classes = this.values.Where(v => v.Courses.Any(vc => vc.CourseID.Equals(_selectCourse.ID)));
                    if (classes != null)
                    {
                        // 1.取消之前的订阅
                        this.Classes.ForEach(c =>
                        {
                            c.PropertyChanged -= NewClass_PropertyChanged;
                        });

                        var cp = CommonDataManager.GetCPCase(base.LocalID);

                        // 2.班级列表
                        this.Classes = classes.Select(sc =>
                          {
                              UIClass newClass = new UIClass()
                              {
                                  ID = sc.ID,
                                  Name = sc.Name,
                                  CourseID = _selectCourse.ID,
                                  Course = _selectCourse.Name
                              };

                              // 3.绑定结果
                              var course = sc.Courses.FirstOrDefault(scc => scc.CourseID.Equals(_selectCourse.ID));
                              if (course != null)
                              {
                                  var has = course.Teachers.Any(t => t.ID.Equals(this.SelectTeacher.ID));
                                  newClass.IsChecked = has;

                                  newClass.TeacherString = course.Teachers?.Select(t => t.Name)?.Parse();
                              }

                              newClass.PropertyChanged += NewClass_PropertyChanged;
                              return newClass;

                          })?.ToList();
                    }

                    // 绑定选中状态
                }
                else
                {
                    this.Classes.ForEach(c =>
                    {
                        c.PropertyChanged -= NewClass_PropertyChanged;
                        c.IsChecked = false;
                        c.PropertyChanged += NewClass_PropertyChanged;
                    });

                    this.Classes.Clear();

                    this.RaisePropertyChanged(() => Classes);
                }
            }
        }

        private void NewClass_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UIClass model = sender as UIClass;
            if (e.PropertyName.Equals(nameof(model.IsChecked)))
            {
                if (model.IsChecked)
                {
                    // 1.更新界面显示
                    this.SelectCourse.SelectClasses = this.Classes.Count(c => c.IsChecked);
                    if (!this.SelectTeacher.HasOperation)
                        this.SelectTeacher.HasOperation = true;

                    // 原始班级
                    var sc = values.FirstOrDefault(v => v.ID.Equals(model.ID));

                    // 更新原始班级
                    if (sc != null)
                    {
                        // 原始科目
                        var scc = sc.Courses.FirstOrDefault(c => c.CourseID.Equals(this.SelectCourse.ID));

                        // 是否有教师
                        var teacher = scc.Teachers.FirstOrDefault(t => t.ID.Equals(this.SelectTeacher.ID));
                        if (teacher == null)
                        {
                            scc.Teachers.Add(new XYKernel.OS.Common.Models.Administrative.TeacherModel
                            {
                                ID = this.SelectTeacher.ID,
                                Name = this.SelectTeacher.Name
                            });
                        }

                        var uiClass = this.Classes.FirstOrDefault(c => c.ID.Equals(sc.ID) && c.CourseID.Equals(scc.CourseID));
                        if (uiClass != null)
                        {
                            uiClass.TeacherString = scc.Teachers?.Select(t => t.Name)?.Parse();
                        }
                    }
                }
                else
                {
                    // 1.更新界面显示
                    this.SelectCourse.SelectClasses = this.Classes.Count(c => c.IsChecked);
                    this.SelectTeacher.HasOperation = this.Courses.Count(c => c.ShowSelectClassCountArea) > 0 ? true : false;

                    // 原始班级
                    var sc = values.FirstOrDefault(v => v.ID.Equals(model.ID));

                    // 更新原始班级
                    if (sc != null)
                    {
                        // 原始科目
                        var scc = sc.Courses.FirstOrDefault(c => c.CourseID.Equals(this.SelectCourse.ID));

                        // 移除教师
                        var teacher = scc.Teachers.FirstOrDefault(t => t.ID.Equals(this.SelectTeacher.ID));
                        if (teacher != null)
                        {
                            scc.Teachers.Remove(teacher);
                        }

                        var uiClass = this.Classes.FirstOrDefault(c => c.ID.Equals(sc.ID) && c.CourseID.Equals(scc.CourseID));
                        if (uiClass != null)
                        {
                            uiClass.TeacherString = scc.Teachers?.Select(t => t.Name)?.Parse();
                        }
                    }
                }
            }
        }

        void save(object obj)
        {
            BatchTeacherWindow window = obj as BatchTeacherWindow;
            window.Classes = this.values;
            window.IsSave = true;

            this.ShowDialog("提示信息", "保存成功", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
        }

        void cancel(object obj)
        {
            BatchTeacherWindow window = obj as BatchTeacherWindow;
            window.Classes = this.values;
            window.IsSave = true;
            window.DialogResult = window.IsSave;
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

        public ICommand WindowCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<string>(windowCommand);
            }
        }

        public ICommand ImportCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(Import);
            }
        }

        public ICommand ExportCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(Export);
            }
        }

        public ICommand TeacherModifyCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<Models.Base.UITeacher>(TeacherModify);
            }
        }

        /// <summary>
        /// 显示空
        /// </summary>
        public bool ShowEmpty
        {
            get
            {
                if (SelectTeacher == null)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 班级
        /// </summary>
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

        public BatchTeacherWindowModel()
        {
            this.Teachers = new List<Models.Base.UITeacher>();
            this.Classes = new List<UIClass>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            var cp = CommonDataManager.GetCPCase(base.LocalID);

            // 教师列表
            this.Teachers = cp.Teachers.Select(s =>
              {
                  return new Models.Base.UITeacher()
                  {
                      ID = s.ID,
                      Name = s.Name
                  };
              })?.ToList();

            // 绑定科目
            this.Courses = cp.Courses.Select(c =>
            {
                return new UICourse()
                {
                    ID = c.ID,
                    Name = c.Name,
                };

            })?.ToList();
        }

        public void BindData(List<UIClass> datas)
        {
            this.values = datas;

            this.Teachers.ForEach(t =>
            {
                t.HasOperation = values.Any(v => v.Courses.Any(vc => vc.Teachers.Any(vt => vt.ID.Equals(t.ID))));
            });
        }

        void windowCommand(string parms)
        {
            if (parms.Equals("unloaded"))
            {
                this.Classes.ForEach(c =>
                {
                    c.PropertyChanged -= NewClass_PropertyChanged;
                });
            }
        }

        void Export()
        {
            var local = CommonDataManager.GetLocalCase(base.LocalID);
            var cp = CommonDataManager.GetCPCase(base.LocalID);

            System.Windows.Forms.SaveFileDialog saveDialog = new System.Windows.Forms.SaveFileDialog();
            saveDialog.Filter = "Microsoft Excel files(*.xls)|*.xls;*.xlsx";
            saveDialog.FileName = $"{local.Name}(教师批量设置)";
            var result = saveDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                System.Data.DataColumn classColumn = new System.Data.DataColumn()
                {
                    ColumnName = "班级"
                };
                dt.Columns.Add(classColumn);

                cp.Courses.ForEach(c =>
                {
                    var newColumn = new System.Data.DataColumn()
                    {
                        ColumnName = c.Name
                    };
                    dt.Columns.Add(newColumn);
                });

                this.values.ForEach(v =>
                {
                    System.Data.DataRow newRow = dt.NewRow();
                    newRow["班级"] = v.Name;

                    if (v.Courses != null)
                    {
                        foreach (var course in v.Courses)
                        {
                            newRow[course.Course] = course.TeacherString;
                        }
                    }

                    dt.Rows.Add(newRow);
                });

                var excelResult = NPOIClass.DataTableToExcel(dt, saveDialog.FileName);
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

        void TeacherModify(Models.Base.UITeacher teacher)
        {
            if (teacher != null)
            {
                this.SelectTeacher = teacher;

                var firstCourse = this.Courses.FirstOrDefault(c => c.ShowSelectClassCountArea);
                if (firstCourse != null)
                {
                    this.SelectCourse = firstCourse;
                }
            }
        }

        void Import()
        {
            System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();
            open.Filter = "Microsoft Excel files(*.xls)|*.xls;*.xlsx";
            open.FilterIndex = 1;
            open.RestoreDirectory = true;
            open.AddExtension = true;
            open.CheckFileExists = true;
            open.CheckPathExists = true;
            open.ShowHelp = true;

            if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var excelResult = NPOIClass.ExcelToDataTable(open.FileName, true);
                if (excelResult.Item1 == null)
                {
                    this.ShowDialog("提示信息", excelResult.Item2, CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Error);
                    return;
                }
                else
                {
                    var data = excelResult.Item1;

                    var cp = CommonDataManager.GetCPCase(base.LocalID);

                    // 列数
                    var columnCount = data.Columns.Count;

                    List<string> columns = new List<string>();
                    foreach (DataColumn col in data.Columns)
                    {
                        columns.Add(col.ColumnName);
                    }

                    // 生成临时集合
                    var sources = this.values.ToList();
                    sources.ForEach(v =>
                    {
                        v.HasOperation = false;
                        v.IsChecked = false;
                        foreach (var c in v.Courses)
                        {
                            c.Teachers?.Clear();
                        }
                    });

                    for (int i = 0; i < data.Rows.Count; i++)
                    {
                        var className = data.Rows[i][0].ToString();
                        if (!string.IsNullOrEmpty(className))
                        {
                            var firstClass = sources.FirstOrDefault(c => c.Name.Equals(className));
                            if (firstClass != null)
                            {
                                var coureses = (from cc in firstClass.Courses
                                                from tcc in cp.Courses
                                                where cc.CourseID.Equals(tcc.ID)
                                                select new
                                                {
                                                    tcc.Name,
                                                    cc
                                                })?.ToList();


                                coureses?.ForEach(c =>
                                {
                                    var has = columns.Contains(c.Name);
                                    if (has)
                                    {
                                        c.cc.Teachers = new List<XYKernel.OS.Common.Models.Administrative.TeacherModel>();

                                        var teacherString = data.Rows[i][c.Name]?.ToString();
                                        if (!string.IsNullOrEmpty(teacherString))
                                        {
                                            var teachers = teacherString.Split(',')?.ToList();
                                            teachers?.ForEach(t =>
                                            {
                                                var firstTeacher = cp.Teachers.FirstOrDefault(ct => ct.Name.Equals(t));
                                                if (firstTeacher != null)
                                                    c.cc.Teachers.Add(firstTeacher);
                                            });
                                        }
                                    }
                                });
                            }
                        }
                    }

                    this.BindData(sources);
                }
            }
        }
    }
}
