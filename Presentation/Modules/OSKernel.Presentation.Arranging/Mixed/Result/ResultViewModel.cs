using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.CustomControl.Enums;
using OSKernel.Presentation.Models.Result;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;
using OSKernel.Presentation.Utilities;
using System.Data;
using OSKernel.Presentation.Arranging.Administrative.Result;
using XYKernel.OS.Common.Models.Mixed.Result;
using OSKernel.Presentation.Core.Http;
using XYKernel.OS.Common.Models;
using OSKernel.Presentation.Models.Base;
using OSKernel.Presentation.Utilities.Models;

namespace OSKernel.Presentation.Arranging.Mixed.Result
{
    /// <summary>
    /// 结果界面
    /// </summary>
    public class ResultViewModel : CommonViewModel, IInitilize
    {
        private ObservableCollection<UIResult> _results;

        public ObservableCollection<UIResult> Results
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

        public ICommand ViewCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<UIResult>(View);
            }
        }

        public ICommand AdjustCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<UIResult>(Adjust);
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<UIResult>(Delete);
            }
        }

        /// <summary>
        /// 导出课表
        /// </summary>
        public ICommand ExportCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<UIResult>(exportTable);
            }
        }

        public ICommand UploadCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<UIResult>(upload);
            }
        }

        public ResultViewModel()
        {
            this.Results = new ObservableCollection<UIResult>();
        }

        public void View(UIResult result)
        {
            ResultDetailsView details = new ResultDetailsView(result);
            details.ShowDialog();
        }

        public void Adjust(UIResult result)
        {
            AdjustResult window = new AdjustResult(result);
            window.ShowDialog();
        }

        public void Delete(UIResult result)
        {
            var confirm = this.ShowDialog("提示信息", $"是否确认删除?\r\n {result.Name}", DialogSettingType.OkAndCancel, DialogType.Warning);

            if (confirm == DialogResultType.OK)
            {
                Results.Remove(result);
                var results = ResultDataManager.GetResults(base.LocalID);
                results.RemoveAll(r => r.TaskID == result.TaskID);

                var caseModel = CommonDataManager.GetLocalCase(base.LocalID);
                if (caseModel != null)
                {
                    caseModel.Serizlize(results);
                }
            }
        }

        [InjectionMethod]
        public void Initilize()
        {
            var results = ResultDataManager.GetResults(base.LocalID);
            if (results != null)
            {
                this.Results = new ObservableCollection<UIResult>(results);
            }
        }

        public void exportTable(UIResult result)
        {
            var adjustRecord = base.LocalID.DeSerializeAdjustRecord<ResultAdjustmentModel>(result.TaskID);
            if (adjustRecord != null)
            {
                if (!result.IsUploaded)
                {
                    this.ShowDialog("提示信息", "本地结果发生改变上传后才可以导出！", DialogSettingType.OnlyOkButton, DialogType.Warning);
                    return;
                }
            }

            // (1:班级 2:教师 3:学生)
            int exportType = 1;
            ExportTypeWindow window = new ExportTypeWindow(true);
            window.Closed += (s, arg) =>
            {
                if (window.DialogResult.Value)
                {
                    exportType = window.Type;

                    #region 导出

                    var cl = CommonDataManager.GetCLCase(base.LocalID);
                    var local = CommonDataManager.GetLocalCase(base.LocalID);

                    ResultModel resultModel = base.LocalID.DeSerializeLocalResult<ResultModel>(result.TaskID);

                    if (resultModel == null)
                    {
                        var value = OSHttpClient.Instance.GetResult(result.TaskID);
                        if (value.Item1)
                        {
                            resultModel = value.Item2;
                        }
                        else
                        {
                            this.ShowDialog("提示信息", "获取走班结果失败", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                            return;
                        }
                    }

                    string typeName = "班级课表";
                    if (exportType == 2)
                    {
                        typeName = "教师课表";
                    }
                    else if (exportType == 3)
                    {
                        typeName = "学生课表";
                    }

                    System.Windows.Forms.SaveFileDialog saveDialog = new System.Windows.Forms.SaveFileDialog();
                    saveDialog.Filter = "Microsoft Excel files(*.xls)|*.xls;*.xlsx";
                    saveDialog.FileName = $"{local.Name}-{result.TaskID}-({typeName})";
                    var confirm = saveDialog.ShowDialog();
                    if (confirm == System.Windows.Forms.DialogResult.OK)
                    {
                        Dictionary<string, DataTable> values = new Dictionary<string, DataTable>();
                        Dictionary<string, List<UIExcelExport>> studentPreselections = new Dictionary<string, List<UIExcelExport>>();

                        if (exportType == 1)
                        {
                            var dt = this.CreateTableFrame();
                            values.Add("课表", dt);

                            var details = (from c in resultModel.ResultClasses
                                           from rc in c.ResultDetails
                                           select new
                                           {
                                               c.ClassID,
                                               rc.ClassHourId,
                                               rc.DayPeriod,
                                               rc.Teachers,
                                               rc
                                           });

                            var groups = details.GroupBy(g => $"{g.DayPeriod.Day}|{g.DayPeriod.Period}|{g.DayPeriod.PeriodName}")?.ToList();
                            groups?.ForEach(g =>
                            {
                                var first = g.First();
                                var dayPeriod = first.DayPeriod;

                                StringBuilder sb = new StringBuilder();
                                g.ToList()?.ForEach(gi =>
                                {
                                    sb.AppendLine(cl.GetClassByID(gi.ClassID)?.Display);
                                    if (gi.Teachers != null)
                                    {
                                        var teachers = cl.GetTeachersByIds(gi.Teachers.ToList());
                                        sb.AppendLine(teachers.Select(t => t.Name).Parse());
                                        sb.AppendLine();
                                    }
                                });

                                //var setValue = g.ToList()?.Select(gi =>
                                //  {
                                //      return cl.GetClassByID(gi.ClassID)?.Display;
                                //  })?.Parse("\n");

                                if (sb.Length > 0)
                                    SetCellData(dt, sb.ToString(), dayPeriod);
                            });

                        }
                        else if (exportType == 2)
                        {
                            // 获取所有教师
                            var classHourIDs = from c in resultModel.ResultClasses from rd in c.ResultDetails select rd.ClassHourId;
                            var classHours = cl.GetClassHours(classHourIDs?.ToArray());
                            List<UITeacher> teachers = new List<UITeacher>();
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

                            if (teachers.Count == 0)
                            {
                                this.ShowDialog("提示信息", "没有教师信息无法导出教师课表!", DialogSettingType.OnlyOkButton, DialogType.Warning);
                                return;
                            }

                            teachers.ForEach(t =>
                            {
                                // 创建基础结构
                                var dt = this.CreateTableFrame();
                                values.Add(t.Name, dt);

                                var resultDetails = (from rc in resultModel.ResultClasses
                                                     from rd in rc.ResultDetails
                                                     select new
                                                     {
                                                         rc.ClassID,
                                                         rd.ClassHourId,
                                                         rd.DayPeriod,
                                                     });

                                var filters = (from ch in t.ClassHourIDs from rc in resultDetails where ch == rc.ClassHourId select rc)?.ToList();


                                var groups = filters?.GroupBy(g => $"{g.DayPeriod.Day}|{g.DayPeriod.Period}|{g.DayPeriod.PeriodName}")?.ToList();
                                groups?.ForEach(g =>
                                {
                                    var first = g.First();
                                    var dayPeriod = first.DayPeriod;

                                    var setValue = g.ToList()?.Select(gi =>
                                    {
                                        return cl.GetClassByID(gi.ClassID)?.Display;
                                    })?.Parse("\n");

                                    SetCellData(dt, setValue, dayPeriod);
                                });
                            });

                        }
                        else if (exportType == 3)
                        {
                            var preselectionsGroups = (from stu in cl.Students
                                                       from sp in stu.Preselections
                                                       where stu.Preselections?.Count > 0
                                                       select new
                                                       {
                                                           stu.ID,
                                                           stu.Name,
                                                           stu.Preselections,
                                                           groupString = stu.Preselections.OrderBy(o => o.CourseID).OrderBy(o => o.LevelID).Select(pss => $"{pss.CourseID}|{pss.LevelID}")?.Parse()
                                                       })?.GroupBy(g => g.groupString)?.ToList();


                            preselectionsGroups?.ForEach(g =>
                            {
                                var first = g.First();
                                var sheetString = first.Preselections.Select(p =>
                                  {
                                      var courseInfo = cl.Courses.FirstOrDefault(c => c.ID.Equals(p.CourseID));
                                      if (courseInfo != null)
                                      {
                                          var levelInfo = courseInfo.Levels.FirstOrDefault(l => l.ID.Equals(p.LevelID));
                                          if (levelInfo != null)
                                          {
                                              return $"{courseInfo.Name}{levelInfo.Name}";
                                          }
                                          else
                                              return $"{courseInfo.Name}";
                                      }
                                      else
                                          return string.Empty;
                                  })?.Parse();

                                List<UIExcelExport> exports = new List<UIExcelExport>();

                                var students = g.Distinct()?.ToList();
                                students.ForEach(gi =>
                                {
                                    // 创建表格
                                    var dt = this.CreateTableFrame();

                                    UIExcelExport export = new UIExcelExport();
                                    export.Table = dt;
                                    export.CreateTime = DateTime.Now;
                                    export.StudentID = gi.ID;
                                    export.Student = gi.Name;

                                    // 获取结果数据
                                    var resultDetails = (from rc in resultModel.ResultClasses
                                                         from rd in rc.ResultDetails
                                                         where rc.ResultStudents.Contains(gi.ID)
                                                         select new
                                                         {
                                                             rc.ClassID,
                                                             rd.ClassHourId,
                                                             rd.DayPeriod,
                                                         });
                                    // 根据位置分组
                                    var groups = resultDetails?.Where(rd =>
                                    {
                                        var classHour = cl.GetClassHours(new int[] { rd.ClassHourId })?.FirstOrDefault();
                                        if (classHour == null)
                                            return false;
                                        else
                                        {
                                            if (gi.Preselections == null)
                                                return false;
                                            else
                                            {
                                                return gi.Preselections.Any(p => p.CourseID.Equals(classHour.CourseID) && p.LevelID.Equals(classHour.LevelID));
                                            }
                                        }
                                    })?.GroupBy(gg => $"{gg.DayPeriod.Day}{gg.DayPeriod.Period}")?.ToList();

                                    // 填充表格数据
                                    groups?.ForEach(gg =>
                                    {
                                        var firstgg = gg.First();
                                        var dayPeriod = firstgg.DayPeriod;


                                        StringBuilder sb = new StringBuilder();
                                        gg.ToList()?.ForEach(ggi =>
                                        {
                                            sb.AppendLine(cl.GetClassByID(ggi.ClassID)?.Display);

                                            var classInfo = cl.GetClassByID(ggi.ClassID);
                                            if (classInfo != null)
                                            {
                                                if (classInfo.TeacherIDs?.Count > 0)
                                                {
                                                    var teachers = cl.GetTeachersByIds(classInfo.TeacherIDs);
                                                    var teacherString = teachers?.Select(t => t.Name)?.Parse();
                                                    sb.AppendLine(teacherString);
                                                    sb.AppendLine();
                                                }
                                            }
                                        });

                                        //var setValue = gg.ToList()?.Select(ggi =>
                                        //{
                                        //    return cl.GetClassByID(ggi.ClassID)?.Display;
                                        //})?.Parse("\n");

                                        if (sb.Length > 0)
                                            SetCellData(dt, sb.ToString(), dayPeriod);
                                    });

                                    // 向集合中添加
                                    exports.Add(export);
                                });

                                studentPreselections.Add(sheetString, exports);

                            });
                        }

                        List<int> enableIndex = new List<int>();
                        var abIndex = cl.Positions.FirstOrDefault(p => p.Position == XYKernel.OS.Common.Enums.Position.AB);
                        if (abIndex != null)
                        {
                            enableIndex.Add(abIndex.DayPeriod.Period);
                        }

                        var noonIndex = cl.Positions.FirstOrDefault(p => p.Position == XYKernel.OS.Common.Enums.Position.Noon);
                        if (noonIndex != null)
                        {
                            enableIndex.Add(noonIndex.DayPeriod.Period);
                        }

                        var pbIndex = cl.Positions.FirstOrDefault(p => p.Position == XYKernel.OS.Common.Enums.Position.PB);
                        if (pbIndex != null)
                        {
                            enableIndex.Add(pbIndex.DayPeriod.Period);
                        }

                        if (exportType == 3)
                        {
                            var table = NPOIClass.DataTableToExcel(studentPreselections, saveDialog.FileName, enableIndex);
                            if (table.Item1)
                            {
                                this.ShowDialog("提示信息", "导出成功!", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
                            }
                            else
                            {
                                this.ShowDialog("提示信息", table.Item2, CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                            }
                        }
                        else
                        {
                            var table = NPOIClass.DataTableToExcel(values, saveDialog.FileName, enableIndex);
                            if (table.Item1)
                            {
                                this.ShowDialog("提示信息", "导出成功!", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
                            }
                            else
                            {
                                this.ShowDialog("提示信息", table.Item2, CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                            }
                        }
                    }

                    #endregion
                }
            };
            window.ShowDialog();
        }

        public void upload(UIResult result)
        {
            var adjustRecord = base.LocalID.DeSerializeAdjustRecord<ResultAdjustmentModel>(result.TaskID);
            if (adjustRecord == null)
            {
                this.ShowDialog("提示信息", "没有调整记录,无法上传！", DialogSettingType.OnlyOkButton, DialogType.Warning);
                return;
            }

            var confirm = this.ShowDialog("提示信息", "确认上传方案?", DialogSettingType.OkAndCancel, DialogType.Warning);
            if (confirm == DialogResultType.OK)
            {
                var operation = OSHttpClient.Instance.WriteBackResult(result.TaskID, adjustRecord);
                if (operation.Item1)
                {
                    result.IsUploaded = true;
                    var local = CommonDataManager.GetLocalCase(base.LocalID);
                    var results = ResultDataManager.GetResults(base.LocalID);
                    local.Serizlize(results);
                    this.ShowDialog("提示信息", "上传成功!", DialogSettingType.NoButton, DialogType.None);
                }
                else
                {
                    this.ShowDialog("提示信息", operation.Item2, DialogSettingType.OnlyOkButton, DialogType.Error);
                }
            }
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

            var cl = CommonDataManager.GetCLCase(base.LocalID);

            var groups = cl.Positions.OrderBy(p => p.DayPeriod.Period).GroupBy(p => p.DayPeriod.PeriodName);
            foreach (var g in groups)
            {
                var newRow = dt.NewRow();
                newRow["课节"] = g.Key;
                dt.Rows.Add(newRow);
            }

            return dt;
        }

        private void SetCellData(DataTable setDataTable, string setValue, DayPeriodModel dayPeriod)
        {
            switch (dayPeriod.Day)
            {
                case DayOfWeek.Monday:
                    setDataTable.Rows[dayPeriod.Period]["星期一"] = setValue;
                    break;

                case DayOfWeek.Tuesday:
                    setDataTable.Rows[dayPeriod.Period]["星期二"] = setValue;
                    break;

                case DayOfWeek.Wednesday:
                    setDataTable.Rows[dayPeriod.Period]["星期三"] = setValue;
                    break;

                case DayOfWeek.Thursday:
                    setDataTable.Rows[dayPeriod.Period]["星期四"] = setValue;
                    break;

                case DayOfWeek.Friday:
                    setDataTable.Rows[dayPeriod.Period]["星期五"] = setValue;
                    break;

                case DayOfWeek.Saturday:
                    setDataTable.Rows[dayPeriod.Period]["星期六"] = setValue;
                    break;

                case DayOfWeek.Sunday:
                    setDataTable.Rows[dayPeriod.Period]["星期日"] = setValue;
                    break;

            }
        }
    }
}
