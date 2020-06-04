using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.Http;
using OSKernel.Presentation.Core.Http.Table;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.CustomControl.Enums;
using OSKernel.Presentation.Models.Base;
using OSKernel.Presentation.Models.Result;
using OSKernel.Presentation.Utilities;
using OSKernel.Presentation.Utilities.Excels;
using OSKernel.Presentation.Utilities.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;
using XYKernel.OS.Common.Models;
using XYKernel.OS.Common.Models.Administrative.Result;

namespace OSKernel.Presentation.Arranging.Administrative.Result
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

        public ICommand AnalysisCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<UIResult>(analysis);
            }
        }

        public ICommand PrechargeCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<UIResult>(precharge);
            }
        }

        public ResultViewModel()
        {
            this.Results = new ObservableCollection<UIResult>();
        }

        public void View(UIResult result)
        {
            ResultDetailsView details = new ResultDetailsView(result);
            details.Owner = System.Windows.Application.Current.MainWindow;
            details.ShowDialog();
        }

        public void Adjust(UIResult result)
        {
            AdjustResult window = new AdjustResult(result);
            window.Closed += (s, arg) =>
            {
                if (window.DialogResult.Value)
                {

                }
            };
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

        public void exportTable(UIResult result)
        {
            var adjustRecord = base.LocalID.DeSerializeAdjustRecord<ResultAdjustmentModel>(result.TaskID);
            if (adjustRecord != null)
            {
                if (!result.IsUploaded)
                {
                    this.ShowDialog("提示信息", "本地结果发生改变请先上传结果！", DialogSettingType.OnlyOkButton, DialogType.Warning);
                    return;
                }
            }

            // (1:班级 2:教师 4 年级)
            int exportType = 1;
            ExportTypeWindow window = new ExportTypeWindow(false);
            window.Closed += (s, arg) =>
            {
                if (window.DialogResult.Value)
                {
                    exportType = window.Type;

                    #region 导出

                    var cp = CommonDataManager.GetCPCase(base.LocalID);
                    var local = CommonDataManager.GetLocalCase(base.LocalID);

                    ResultModel resultModel = base.LocalID.DeSerializeLocalResult<ResultModel>(result.TaskID);
                    if (resultModel == null)
                    {
                        var value = WebAPI.Instance.GetAdminResult(result.TaskID);
                        if (value.Item1)
                        {
                            resultModel = value.Item2;
                        }
                        else
                        {
                            this.ShowDialog("提示信息", "获取行政班结果失败", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                            return;
                        }
                    }

                    string typeName = string.Empty;
                    if (exportType == 1)
                    {
                        typeName = "班级课表";
                    }
                    else if (exportType == 2)
                    {
                        typeName = "教师课表";
                    }
                    else if (exportType == 4)
                    {
                        typeName = "年级课表";
                    }

                    System.Windows.Forms.SaveFileDialog saveDialog = new System.Windows.Forms.SaveFileDialog();
                    saveDialog.Filter = "Microsoft Excel files(*.xls)|*.xls;*.xlsx";
                    saveDialog.FileName = $"{local.Name}-{result.TaskID}-({typeName})";
                    var confirm = saveDialog.ShowDialog();
                    if (confirm == System.Windows.Forms.DialogResult.OK)
                    {
                        if (exportType == 1)
                        {
                            Dictionary<string, DataTable> values = new Dictionary<string, DataTable>();
                            foreach (var rc in resultModel.ResultClasses)
                            {
                                // 创建基础结构
                                var dt = this.CreateTableFrame();
                                // 创建sheet
                                var firstClass = cp.Classes.FirstOrDefault(c => rc.ClassID.Equals(c.ID));
                                values.Add(firstClass.Name, dt);
                                // 常规
                                var normals = rc.ResultDetails.Where(rd => rd.ResultType == XYKernel.OS.Common.Enums.ClassHourResultType.Normal)?.ToList();
                                normals?.ForEach(n =>
                                {
                                    var classHourInfo = cp.GetClassHours(new int[] { n.ClassHourId })?.FirstOrDefault();
                                    SetCellData(dt, classHourInfo.Course + "\n" + classHourInfo.TeacherString, n.DayPeriod);
                                });
                                // 单双周
                                var mulitplys = rc.ResultDetails.Where(rd => rd.ResultType != XYKernel.OS.Common.Enums.ClassHourResultType.Normal)?.ToList();
                                var groups = mulitplys?.GroupBy(m => $"{m.DayPeriod.Day}{m.DayPeriod.Period}");
                                if (groups != null)
                                {
                                    foreach (var g in groups)
                                    {
                                        var first = g.FirstOrDefault();
                                        var courseName = g.Select(gi =>
                                        {
                                            return cp.Courses.FirstOrDefault(c => c.ID.Equals(gi.CourseID))?.Name;
                                        })?.Parse("|");
                                        var teacherName = g.Select(gi =>
                                        {
                                            return cp.GetTeachersByIds(gi.Teachers.ToList()).Select(a => a.Name).ToArray().Parse(",");
                                        })?.Parse("|");
                                        SetCellData(dt, courseName + "\n" + teacherName, first.DayPeriod);
                                    }
                                }


                            }

                            #region 导出

                            List<int> enableIndex = new List<int>();

                            var abIndex = cp.Positions.FirstOrDefault(p => p.Position == XYKernel.OS.Common.Enums.Position.AB);
                            if (abIndex != null)
                            {
                                enableIndex.Add(abIndex.DayPeriod.Period);
                            }

                            var noonIndex = cp.Positions.FirstOrDefault(p => p.Position == XYKernel.OS.Common.Enums.Position.Noon);
                            if (noonIndex != null)
                            {
                                enableIndex.Add(noonIndex.DayPeriod.Period);
                            }

                            var pbIndex = cp.Positions.FirstOrDefault(p => p.Position == XYKernel.OS.Common.Enums.Position.PB);
                            if (pbIndex != null)
                            {
                                enableIndex.Add(pbIndex.DayPeriod.Period);
                            }

                            var table = NPOIClass.DataTableToExcel(values, saveDialog.FileName, enableIndex);
                            if (table.Item1)
                            {
                                this.ShowDialog("提示信息", "导出成功!", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
                                FileHelper.OpenFilePath(saveDialog.FileName);
                            }
                            else
                            {
                                this.ShowDialog("提示信息", table.Item2, CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                            }

                            #endregion
                        }
                        else if (exportType == 4)
                        {
                            GradeExcelModel excelModel = new GradeExcelModel();

                            // 检查是否有周六周日
                            List<string> weeks = new List<string>()
                            {
                                "星期一",
                                "星期二",
                                "星期三",
                                "星期四",
                                "星期五",
                            };

                            // 是否有周末
                            var hasWeekend = resultModel.ResultClasses.Any(rc => rc.ResultDetails.Any(rd => (rd.DayPeriod.Day == DayOfWeek.Saturday || rd.DayPeriod.Day == DayOfWeek.Sunday)));

                            if (hasWeekend)
                            {
                                weeks.Add("星期六");
                                weeks.Add("星期日");
                            }

                            // 节次
                            List<string> periods = new List<string>();
                            cp.Positions.GroupBy(p => p.DayPeriod.PeriodName)?.ToList()?.ForEach(p =>
                              {
                                  periods.Add(p.Key);
                              });

                            // 设置节次
                            excelModel.SetPeriods(periods);

                            // 设置星期
                            excelModel.SetWeeks(weeks);

                            foreach (var rc in resultModel.ResultClasses)
                            {
                                var classInfo = resultModel.Classes.FirstOrDefault(c => rc.ClassID.Equals(c.ID));

                                // 添加结果
                                List<string> values = new List<string>();
                                excelModel.AddClassesDictionary(classInfo.Name, values);

                                var mondays = rc.ResultDetails.Where(rd => rd.DayPeriod.Day == DayOfWeek.Monday)?.ToList();
                                var tuesdays = rc.ResultDetails.Where(rd => rd.DayPeriod.Day == DayOfWeek.Tuesday)?.ToList();
                                var wednesdays = rc.ResultDetails.Where(rd => rd.DayPeriod.Day == DayOfWeek.Wednesday)?.ToList();
                                var thursdays = rc.ResultDetails.Where(rd => rd.DayPeriod.Day == DayOfWeek.Thursday)?.ToList();
                                var fridays = rc.ResultDetails.Where(rd => rd.DayPeriod.Day == DayOfWeek.Friday)?.ToList();
                                var saturdays = rc.ResultDetails.Where(rd => rd.DayPeriod.Day == DayOfWeek.Saturday)?.ToList();
                                var sundays = rc.ResultDetails.Where(rd => rd.DayPeriod.Day == DayOfWeek.Sunday)?.ToList();

                                values.AddRange(FillGradeExportValues(mondays, periods));
                                values.AddRange(FillGradeExportValues(tuesdays, periods));
                                values.AddRange(FillGradeExportValues(wednesdays, periods));
                                values.AddRange(FillGradeExportValues(thursdays, periods));
                                values.AddRange(FillGradeExportValues(fridays, periods));

                                if (saturdays?.Count > 0)
                                {
                                    values.AddRange(FillGradeExportValues(saturdays, periods));
                                }

                                if (sundays?.Count > 0)
                                {
                                    values.AddRange(FillGradeExportValues(sundays, periods));
                                }
                            }

                            var table = GradeExcel.Export(excelModel, saveDialog.FileName);
                            if (table.Item1)
                            {
                                this.ShowDialog("提示信息", "导出成功!", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
                                FileHelper.OpenFilePath(saveDialog.FileName);
                            }
                            else
                            {
                                this.ShowDialog("提示信息", table.Item2, CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                            }
                        }
                        else if (exportType == 2)
                        {
                            Dictionary<string, DataTable> values = new Dictionary<string, DataTable>();

                            // 获取所有教师
                            var classHourIDs = from c in resultModel.ResultClasses from rd in c.ResultDetails select rd.ClassHourId;
                            var classHours = cp.GetClassHours(classHourIDs?.ToArray());
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
                                                         rd.CourseID,
                                                         rd.DayPeriod,
                                                         rd.ResultType
                                                     });

                                var filters = (from ch in t.ClassHourIDs from rc in resultDetails where ch == rc.ClassHourId select rc)?.ToList();

                                var groups = filters?.GroupBy(g => $"{g.DayPeriod.Day}{g.DayPeriod.Period}");
                                if (groups != null)
                                {
                                    foreach (var g in groups)
                                    {
                                        var dayPeriod = g.FirstOrDefault().DayPeriod;

                                        var ids = g.Select(gs =>
                                          {
                                              return gs.ClassHourId;
                                          })?.ToArray();

                                        var setValue = cp.GetClassHours(ids)?.Select(ss => $"{ss.Course}-{ss.Class}")?.Parse("|");

                                        SetCellData(dt, setValue, dayPeriod);
                                    }
                                }
                            });

                            #region 导出

                            List<int> enableIndex = new List<int>();

                            var abIndex = cp.Positions.FirstOrDefault(p => p.Position == XYKernel.OS.Common.Enums.Position.AB);
                            if (abIndex != null)
                            {
                                enableIndex.Add(abIndex.DayPeriod.Period);
                            }

                            var noonIndex = cp.Positions.FirstOrDefault(p => p.Position == XYKernel.OS.Common.Enums.Position.Noon);
                            if (noonIndex != null)
                            {
                                enableIndex.Add(noonIndex.DayPeriod.Period);
                            }

                            var pbIndex = cp.Positions.FirstOrDefault(p => p.Position == XYKernel.OS.Common.Enums.Position.PB);
                            if (pbIndex != null)
                            {
                                enableIndex.Add(pbIndex.DayPeriod.Period);
                            }

                            var table = NPOIClass.DataTableToExcel(values, saveDialog.FileName, enableIndex);
                            if (table.Item1)
                            {
                                this.ShowDialog("提示信息", "导出成功!", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
                                FileHelper.OpenFilePath(saveDialog.FileName);
                            }
                            else
                            {
                                this.ShowDialog("提示信息", table.Item2, CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                            }

                            #endregion
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
                var operation = WebAPI.Instance.WriteBackResult(result.TaskID, adjustRecord);
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
                    this.ShowDialog("提示信息", operation.Item3, DialogSettingType.OnlyOkButton, DialogType.Warning);
                }
            }
        }

        public void analysis(UIResult result)
        {
            ResultDataManager.CurrentResult = result;

            var local = CommonDataManager.GetLocalCase(base.LocalID);
            Analysis.Result.Administrative.HostWindow host = new Analysis.Result.Administrative.HostWindow(result.Name);
            host.ShowDialog();
        }

        public void precharge(UIResult result)
        {
            var sample = WebAPI.Instance.SampleTask(result.TaskID);

            if (sample.Item1)
            {
                // 预览
                PrechargeResultWindow precharge = new PrechargeResultWindow(result.Name, sample.Item2);
                precharge.Closed += (s, arg) =>
                {
                    // 如果预览结果为True.
                    if (precharge.DialogResult == true)
                    {
                        var results = ResultDataManager.GetResults(base.LocalID);
                        var local = CommonDataManager.GetLocalCase(base.LocalID);

                        if (precharge.IsUseResult)
                        {
                            var value = WebAPI.Instance.ConfirmTask(result.TaskID);
                            if (value.Item1)
                            {
                                result.IsUsed = true;
                                local.Serizlize(results);

                                // 更新界面
                                var uiTask = this.Results.FirstOrDefault(r => r.TaskID.Equals(result.TaskID));
                                if (uiTask != null)
                                {
                                    uiTask.IsUsed = true;
                                    uiTask.RaiseChanged();
                                }

                                this.ShowDialog("提示信息", "操作成功！", DialogSettingType.NoButton, DialogType.None);
                            }
                            else
                            {
                                this.ShowDialog("提示信息", "操作失败！", DialogSettingType.OnlyOkButton, DialogType.Warning);
                            }
                        }
                        else
                        {
                            var value = WebAPI.Instance.AbandonTask(result.TaskID);
                            if (value.Item1)
                            {
                                this.Results.Remove(result);
                                results.RemoveAll(r=>r.TaskID== result.TaskID);
                                local.Serizlize(results);

                                this.ShowDialog("提示信息", "操作成功！", DialogSettingType.NoButton, DialogType.None);
                            }
                            else
                            {
                                this.ShowDialog("提示信息", "操作失败！", DialogSettingType.OnlyOkButton, DialogType.Warning);
                            }
                        }
                    }
                };
                precharge.ShowDialog();
            }
            else
            {
                this.ShowDialog("提示信息", sample.Item3, DialogSettingType.OnlyOkButton, DialogType.Warning);
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

            var cp = CommonDataManager.GetCPCase(base.LocalID);

            var groups = cp.Positions.OrderBy(p => p.DayPeriod.Period).GroupBy(p => p.DayPeriod.PeriodName);
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

        List<string> FillGradeExportValues(List<ResultDetailModel> details, List<string> periods)
        {
            var cp = CommonDataManager.GetCPCase(base.LocalID);

            List<string> values = new List<string>();
            if (details != null)
            {
                // 单周
                var normals = details.Where(rd => rd.ResultType == XYKernel.OS.Common.Enums.ClassHourResultType.Normal)?.ToList();

                // 双周
                var mulitplys = details.Where(rd => rd.ResultType != XYKernel.OS.Common.Enums.ClassHourResultType.Normal)?.ToList();

                periods.ForEach(p =>
                {
                    // 双周
                    var multiply = details.FirstOrDefault(m => m.DayPeriod.PeriodName.Equals(p));

                    // 单周
                    var normal = details.FirstOrDefault(m => m.DayPeriod.PeriodName.Equals(p));

                    if (multiply == null && normal == null)
                    {
                        values.Add(string.Empty);
                    }
                    else
                    {
                        if (normal != null)
                        {
                            var classHourInfo = cp.GetClassHours(new int[] { normal.ClassHourId })?.FirstOrDefault();
                            values.Add(classHourInfo.Course + "\n" + classHourInfo.TeacherString);
                        }
                        else
                        {
                            var groups = mulitplys?.GroupBy(m => $"{m.DayPeriod.Day}{m.DayPeriod.Period}");
                            if (groups != null)
                            {
                                foreach (var g in groups)
                                {
                                    var first = g.FirstOrDefault();

                                    var courseName = g.Select(gi =>
                                    {
                                        return cp.Courses.FirstOrDefault(c => c.ID.Equals(gi.CourseID))?.Name;
                                    })?.Parse("|");

                                    var teacherName = g.Select(gi =>
                                    {
                                        return cp.GetTeachersByIds(gi.Teachers.ToList()).Select(a => a.Name).ToArray().Parse(",");
                                    })?.Parse("|");

                                    values.Add(courseName + "\n" + teacherName);
                                }
                            }
                        }
                    }

                });
            }

            return values;
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
    }
}
