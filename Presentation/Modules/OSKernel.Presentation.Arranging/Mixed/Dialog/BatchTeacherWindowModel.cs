using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Mixed;
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

namespace OSKernel.Presentation.Arranging.Mixed.Dialog
{
    public class BatchTeacherWindowModel : CommonViewModel, IInitilize
    {
        private List<Models.Base.UITeacher> _teachers;
        private List<UICourseLevel> _courses;
        private List<UICourseLevel> values;

        private Models.Base.UITeacher _selectTeacher;
        private UICourseLevel _selectCourse;

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

                    var cl = CommonDataManager.GetCLCase(base.LocalID);

                    this.Courses.ForEach(c =>
                    {
                        // 清除状态
                        foreach (var cc in c.Classes)
                        {
                            cc.PropertyChanged -= C_PropertyChanged;
                            cc.IsChecked = false;
                            cc.TeacherIDs.Clear();
                        }

                        // 绑定当前状态
                        var course = values.FirstOrDefault(v => v.CourseID.Equals(c.CourseID) && v.LevelID.Equals(c.LevelID));
                        if (course != null)
                        {
                            var classes = course.Classes.Where(cc => cc.TeacherIDs.Contains(this.SelectTeacher.ID))?.ToList();
                            var classCount = classes?.Count;
                            c.SelectClasses = classCount ?? 0;

                            classes?.ForEach(cc =>
                            {
                                var firstClass = c.Classes.FirstOrDefault(ccc => ccc.ID.Equals(cc.ID));
                                if (firstClass != null)
                                {
                                    firstClass.IsChecked = true;
                                    firstClass.TeacherString = cl.GetTeachersByIds(cc.TeacherIDs)?.Select(t => t.Name)?.Parse();
                                }
                            });

                        }
                        else
                            c.SelectClasses = 0;

                        // 绑定状态
                        foreach (var cc in c.Classes)
                        {
                            cc.PropertyChanged += C_PropertyChanged;
                        }
                    });
                }
            }
        }

        /// <summary>
        /// 课程
        /// </summary>
        public List<UICourseLevel> Courses
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
        public UICourseLevel SelectCourse
        {
            get
            {
                return _selectCourse;
            }

            set
            {
                _selectCourse = value;
                RaisePropertyChanged(() => SelectCourse);

                if (SelectCourse != null)
                {
                    if (SelectCourse.Classes != null)
                    {
                        var cl = CommonDataManager.GetCLCase(base.LocalID);

                        var selectCourse = values.FirstOrDefault(vc => vc.CourseID.Equals(SelectCourse.CourseID));
                        if (selectCourse != null)
                        {
                            var filters = (from sc in SelectCourse.Classes
                                           from tc in selectCourse.Classes
                                           where sc.ID.Equals(tc.ID)
                                           select new
                                           {
                                               sc,
                                               tc
                                           })?.ToList();

                            foreach (var f in filters)
                            {
                                f.sc.TeacherString = cl.GetTeachersByIds(f.tc.TeacherIDs)?.Select(t => t.Name)?.Parse();
                            }
                        }
                    }
                }

            }
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

        void C_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UIClass classModel = sender as UIClass;
            if (e.PropertyName.Equals(nameof(classModel.IsChecked)))
            {
                if (classModel.IsChecked)
                {
                    var courseLevel = values.FirstOrDefault(v => v.CourseID.Equals(classModel.CourseID) && v.LevelID.Equals(classModel.LevelID));
                    if (courseLevel != null)
                    {
                        var uicourse = this.Courses.FirstOrDefault(c => c.CourseID.Equals(classModel.CourseID) && c.LevelID.Equals(classModel.LevelID));
                        uicourse.SelectClasses = uicourse.Classes.Count(c => c.IsChecked);

                        this.SelectTeacher.HasOperation = true;

                        var targetClass = courseLevel.Classes.FirstOrDefault(c => c.ID.Equals(classModel.ID));
                        if (targetClass != null)
                        {
                            var cl = CommonDataManager.GetCLCase(base.LocalID);

                            if (!targetClass.TeacherIDs.Contains(this.SelectTeacher.ID))
                            {
                                targetClass.TeacherIDs.Add(this.SelectTeacher.ID);
                                classModel.TeacherString = cl.GetTeachersByIds(targetClass.TeacherIDs)?.Select(t => t.Name)?.Parse();
                            }
                        }
                    }
                }
                else
                {
                    var courseLevel = values.FirstOrDefault(v => v.CourseID.Equals(classModel.CourseID) && v.LevelID.Equals(classModel.LevelID));
                    if (courseLevel != null)
                    {
                        var uicourse = this.Courses.FirstOrDefault(c => c.CourseID.Equals(classModel.CourseID) && c.LevelID.Equals(classModel.LevelID));
                        uicourse.SelectClasses = uicourse.Classes.Count(c => c.IsChecked);

                        if (this.Courses.All(cc => cc.SelectClasses == 0))
                        {
                            this.SelectTeacher.HasOperation = false;
                        }

                        var targetClass = courseLevel.Classes.FirstOrDefault(c => c.ID.Equals(classModel.ID));
                        if (targetClass != null)
                        {
                            var cl = CommonDataManager.GetCLCase(base.LocalID);

                            targetClass.TeacherIDs.Remove(this.SelectTeacher.ID);
                            classModel.TeacherString = cl.GetTeachersByIds(targetClass.TeacherIDs)?.Select(t => t.Name)?.Parse();
                        }
                    }
                }
            }
        }

        void save(object obj)
        {
            BatchTeacherWindow window = obj as BatchTeacherWindow;
            window.CourseLevels = values;
            window.IsSave = true;

            this.ShowDialog("提示信息", "保存成功", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);

        }

        void cancel(object obj)
        {
            BatchTeacherWindow window = obj as BatchTeacherWindow;
            window.CourseLevels = values;
            window.IsSave = true;
            window.DialogResult = window.IsSave;
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

        public BatchTeacherWindowModel()
        {
            this.Teachers = new List<Models.Base.UITeacher>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            var cl = CommonDataManager.GetCLCase(base.LocalID);

            // 教师列表
            this.Teachers = cl.Teachers.Select(s =>
              {
                  return new Models.Base.UITeacher()
                  {
                      ID = s.ID,
                      Name = s.Name
                  };
              })?.ToList();
        }

        public void BindData(List<UICourseLevel> sources, List<Models.Mixed.UICourseLevel> binds)
        {
            values = sources;

            // 1.绑定课程
            this.Courses = binds;
            // 2.绑定教师状态
            this.Teachers.ForEach(t =>
            {
                t.HasOperation = values.Any(v => v.Classes.Any(tc => tc.TeacherIDs.Contains(t.ID)));
            });
            // 3.绑定界面承载区域
            this.Courses.ForEach(d =>
            {
                foreach (var c in d.Classes)
                {
                    c.PropertyChanged += C_PropertyChanged;
                }
            });
        }

        public void ImportData()
        {
            this.SelectTeacher = null;

            // 2.绑定教师状态
            this.Teachers.ForEach(t =>
            {
                t.HasOperation = values.Any(v => v.Classes.Any(tc => tc.TeacherIDs.Contains(t.ID)));
            });
        }

        void windowCommand(string parms)
        {
            if (parms.Equals("unloaded"))
            {
                this.Courses.ForEach(c =>
                {
                    foreach (var cc in c.Classes)
                    {
                        cc.PropertyChanged -= C_PropertyChanged;
                    }
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

                System.Data.DataColumn courseColumn = new System.Data.DataColumn()
                {
                    ColumnName = "课程"
                };
                dt.Columns.Add(courseColumn);

                System.Data.DataColumn levelColumn = new System.Data.DataColumn()
                {
                    ColumnName = "层"
                };
                dt.Columns.Add(levelColumn);

                System.Data.DataColumn classColumn = new System.Data.DataColumn()
                {
                    ColumnName = "班级"
                };
                dt.Columns.Add(classColumn);

                System.Data.DataColumn teacherColumn = new System.Data.DataColumn()
                {
                    ColumnName = "教师"
                };
                dt.Columns.Add(teacherColumn);

                this.values.ForEach(v =>
                {
                    if (v.Classes != null)
                    {
                        foreach (var classInfo in v.Classes)
                        {
                            System.Data.DataRow newRow = dt.NewRow();
                            newRow["课程"] = v.Course;
                            newRow["层"] = v.Level;
                            newRow["班级"] = classInfo.Name;

                            newRow["教师"] = classInfo.TeacherIDs?.Select(tid =>
                            {
                                return this.Teachers.FirstOrDefault(t => t.ID.Equals(tid))?.Name;
                            })?.Parse();

                            dt.Rows.Add(newRow);
                        }
                    }
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

                    // 列数
                    var columnCount = data.Columns.Count;

                    List<string> columns = new List<string>();
                    foreach (DataColumn col in data.Columns)
                    {
                        columns.Add(col.ColumnName);
                    }

                    var hasCourse = columns.Contains("课程");
                    var hasLevel = columns.Contains("层");
                    var hasClass = columns.Contains("班级");
                    var hasTeacher = columns.Contains("教师");

                    if (!(hasCourse && hasLevel && hasClass && hasTeacher))
                    {
                        this.ShowDialog("提示信息", "文档格式不正确! 导入失败!");
                        return;
                    }

                    // 生成临时集合
                    this.values.ForEach(v =>
                    {
                        v.SelectClasses = 0;
                        v.IsChecked = false;
                        foreach (var c in v.Classes)
                        {
                            c.TeacherIDs = new List<string>();
                        }
                    });

                    for (int i = 0; i < data.Rows.Count; i++)
                    {
                        var courseName = data.Rows[i]["课程"].ToString();
                        var levelName = data.Rows[i]["层"].ToString();
                        var className = data.Rows[i]["班级"].ToString();
                        var teacherName = data.Rows[i]["教师"].ToString();

                        if (!string.IsNullOrEmpty(courseName))
                        {
                            var courseLevel = this.values.FirstOrDefault(v => v.Course.Equals(courseName) && v.Level.Equals(levelName));
                            if (courseLevel != null)
                            {
                                var firstClass = courseLevel.Classes.FirstOrDefault(c => c.Name.Equals(className));
                                if (firstClass != null)
                                {
                                    if (!string.IsNullOrEmpty(teacherName))
                                    {
                                        var teachers = teacherName.Split(',');

                                        if (teachers != null)
                                        {
                                            var teacherIDs = (from t in teachers from st in this.Teachers where t.Equals(st.Name) select st.ID)?.ToList();
                                            if (teacherIDs != null)
                                                firstClass.TeacherIDs?.AddRange(teacherIDs);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    this.ImportData();
                }
            }
        }
    }
}
