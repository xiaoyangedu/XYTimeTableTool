using Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models;
using XYKernel.OS.Common.Models.Mixed.Result;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.DataManager;
using OSKernel.Presentation.Arranging.Utilities;

namespace OSKernel.Presentation.Arranging.Mixed.Result
{
    /// <summary>
    /// 调整课表验证逻辑
    /// </summary>
    public class AdjustLogic
    {
        /// <summary>
        /// 当前本地结果
        /// </summary>
        public static ResultModel CurrentLocalResult;

        /// <summary>
        /// 验证当前调整课程
        /// </summary>
        /// <param name="localID">本地方案ID</param>
        /// <param name="items">验证项</param>
        /// <param name="resultModel">结果模型</param>
        /// <returns>返回可拖拽区域</returns>
        public static List<DayPeriodModel> CheckCanAdjustPosition(string localID, List<ResultDetailModel> items, ResultModel resultModel)
        {
            var positions = GetMovablePositons(localID, items, resultModel);
            //单向检查结果
            List<DayPeriodModel> oneWayPositions = positions.Item1;
            //双向校验
            List<DayPeriodModel> twoWayPositions = new List<DayPeriodModel>();
            oneWayPositions?.ForEach(x =>
            {
                List<ResultDetailModel> targetItems = resultModel.ResultClasses.SelectMany(rc => rc.ResultDetails)?
                    .Where(rd => rd.DayPeriod.Day == x.Day && rd.DayPeriod.PeriodName == x.PeriodName)?.ToList();

                if ((targetItems?.Count() ?? 0) == 0)
                {
                    twoWayPositions.Add(x);
                }
                else
                {
                    Tuple<bool, string> checkresult = CanReplacePosition(localID, items, targetItems, resultModel);
                    if (checkresult.Item1)
                    {
                        twoWayPositions.Add(x);
                    }
                }
            });

            #region 在返回结果中增加当前课位
            DayPeriodModel currentPosition = items?.FirstOrDefault()?.DayPeriod;
            if (currentPosition != null)
            {
                twoWayPositions = TimeOperation.TimeSlotUnion(twoWayPositions, new List<DayPeriodModel>() { currentPosition });
            }
            #endregion

            return twoWayPositions;
        }

        /// <summary>
        /// 是否可以拖拽替换
        /// </summary>
        /// <param name="localID">本地方案ID</param>
        /// <param name="sourceItems">原始拖拽源</param>
        /// <param name="targetItems">目标拖拽源</param>
        /// <param name="resultModel">结果模型</param>
        /// <returns></returns>
        public static Tuple<bool, string> CanReplacePosition(string localID, List<ResultDetailModel> sourceItems, List<ResultDetailModel> targetItems, ResultModel resultModel)
        {
            ICommonDataManager commonDataManager = CacheManager.Instance.UnityContainer.Resolve<ICommonDataManager>();
            var cl = commonDataManager.GetCLCase(localID);
            var rule = commonDataManager.GetMixedRule(localID);
            var algo = commonDataManager.GetMixedAlgoRule(localID);

            var positionsSource = GetMovablePositons(localID, sourceItems, resultModel);
            var positionsTarget = GetMovablePositons(localID, targetItems, resultModel);

            DayPeriodModel dpSource = sourceItems.FirstOrDefault().DayPeriod;
            DayPeriodModel dpTarget = targetItems.FirstOrDefault().DayPeriod;

            if (dpTarget != null && positionsSource.Item1.Exists(dp => dp.Day == dpTarget.Day && dp.PeriodName == dpTarget.PeriodName) &&
                dpSource != null && positionsTarget.Item1.Exists(dp => dp.Day == dpSource.Day && dp.PeriodName == dpSource.PeriodName))
            {
                return Tuple.Create(true, string.Empty);
            }
            else
            {
                var sourceToTargetWarning = positionsSource.Item2.Where(dp => dp.DayPeriod.Day == dpTarget.Day && dp.DayPeriod.PeriodName == dpTarget.PeriodName).ToList();
                var targetToSourceWarning = positionsTarget.Item2.Where(dp => dp.DayPeriod.Day == dpSource.Day && dp.DayPeriod.PeriodName == dpSource.PeriodName).ToList();

                StringBuilder sb = new StringBuilder(100);

                sourceToTargetWarning?.ForEach(w =>
                {
                    sb.AppendLine(w.WaringMessage);
                });

                targetToSourceWarning?.ForEach(w =>
                {
                    sb.AppendLine(w.WaringMessage);
                });

                return Tuple.Create(false, sb.ToString());
            }
        }

        private static Tuple<List<DayPeriodModel>, List<PostionWithWarningInfo>> GetMovablePositons(string localID, List<ResultDetailModel> item, ResultModel resultModel)
        {
            //可用课位，最后取反返回不可用课位
            var dayPeriods = new List<DayPeriodModel>();
            var notReachable = new List<PostionWithWarningInfo>();

            ICommonDataManager commonDataManager = CacheManager.Instance.UnityContainer.Resolve<ICommonDataManager>();
            var cl = commonDataManager.GetCLCase(localID);
            var rule = commonDataManager.GetMixedRule(localID);
            var algo = commonDataManager.GetMixedAlgoRule(localID);

            //获取方案可用时间
            dayPeriods = cl?.Positions?.Where(p => p.IsSelected
                            && p.Position != XYKernel.OS.Common.Enums.Position.AB
                            && p.Position != XYKernel.OS.Common.Enums.Position.Noon
                            && p.Position != XYKernel.OS.Common.Enums.Position.PB)
                            ?.Select(d => d.DayPeriod)?.ToList() ?? new List<DayPeriodModel>();

            //TODO: 是否基于现有模型更新结果模型中的教师信息?
            item?.ForEach(it =>
            {

                var classHourInfo = cl.ClassHours.FirstOrDefault(c => c.ID == it.ClassHourId);
                string classID = classHourInfo?.ClassID ?? string.Empty;
                string courseID = classHourInfo?.CourseID ?? string.Empty;

                List<string> teachers = new List<string>();
                teachers = it?.Teachers == null ? new List<string>() : it.Teachers.ToList();

                //2.0 检查批量规则 - 仅查权重为高级的规则
                rule?.TeacherTimes?.Where(t => t.Weight == 1 && teachers.Contains(t.TeacherID))?.ToList()?.ForEach(x =>
                {
                    if (x.ForbidTimes != null)
                    {
                        dayPeriods = TimeOperation.TimeSlotDiff(dayPeriods, x.ForbidTimes);

                        x.ForbidTimes.ForEach(fb =>
                        {
                            notReachable.Add(new PostionWithWarningInfo() { DayPeriod = fb, WaringMessage = $"教师在课位({TimeOperation.GetDateInfo(fb)})有禁止规则!" });
                        });
                    }

                    if (x.MustTimes != null)
                    {
                        //必须时间暂不必查
                    }
                });

                rule?.CourseTimes?.Where(c => c.Weight == 1 && c.ClassID == classID)?.ToList()?.ForEach(x =>
                {
                    if (x.ForbidTimes != null)
                    {
                        dayPeriods = TimeOperation.TimeSlotDiff(dayPeriods, x.ForbidTimes);

                        x.ForbidTimes.ForEach(fb =>
                        {
                            notReachable.Add(new PostionWithWarningInfo() { DayPeriod = fb, WaringMessage = $"课程在课位({TimeOperation.GetDateInfo(fb)})有禁止规则!" });
                        });
                    }

                    if (x.MustTimes != null)
                    {
                        int classHourNumber = cl.ClassHours.Where(ch => ch.ClassID == classID && ch.CourseID == courseID).Count();

                        if (x.MustTimes.Count >= classHourNumber)
                        {
                            dayPeriods.ForEach(dp =>
                            {
                                if (!x.MustTimes.Exists(mt => mt.Day == dp.Day && mt.PeriodName == dp.PeriodName))
                                {
                                    notReachable.Add(new PostionWithWarningInfo() { DayPeriod = dp, WaringMessage = "课程应先满足排到必须规则指定课位内!" });
                                }
                            });

                            dayPeriods = TimeOperation.TimeSlotInterSect(dayPeriods, x.MustTimes);
                        }
                        else
                        {
                            List<DayPeriodModel> classHourTimes = resultModel.ResultClasses.Where(c => c.ClassID == classID)
                                                                  ?.SelectMany(c => c.ResultDetails)
                                                                  ?.Select(c => c.DayPeriod).ToList() ?? new List<DayPeriodModel>() { };

                            List<DayPeriodModel> classHoursInMust = TimeOperation.TimeSlotInterSect(x.MustTimes, classHourTimes);
                            if (classHoursInMust.Count < x.MustTimes.Count)
                            {
                                dayPeriods.ForEach(dp =>
                                {
                                    if (!x.MustTimes.Exists(mt => mt.Day == dp.Day && mt.PeriodName == dp.PeriodName))
                                    {
                                        notReachable.Add(new PostionWithWarningInfo() { DayPeriod = dp, WaringMessage = "课程应先满足排到必须规则指定课位内!" });
                                    }
                                });

                                dayPeriods = TimeOperation.TimeSlotInterSect(dayPeriods, x.MustTimes);
                            }
                            else
                            {
                                //如果课位在必须时间内，则只能和本班课时互换以保障课时优先在必须时间内
                                List<DayPeriodModel> mustTempTimes = TimeOperation.TimeSlotInterSect(x.MustTimes, new List<DayPeriodModel>() { it.DayPeriod });
                                if (mustTempTimes.Count == 1)
                                {
                                    dayPeriods.ForEach(dp =>
                                    {
                                        if (!classHourTimes.Exists(mt => mt.Day == dp.Day && mt.PeriodName == dp.PeriodName))
                                        {
                                            notReachable.Add(new PostionWithWarningInfo() { DayPeriod = dp, WaringMessage = "课程应先满足排到必须规则指定课位内!" });
                                        }
                                    });

                                    dayPeriods = TimeOperation.TimeSlotInterSect(dayPeriods, classHourTimes);
                                }
                            }
                        }
                    }
                });

                rule?.AmPmClassHours?.Where(a => a.Weight == 1 && a.ClassID == classID && a.CourseID == courseID)?.ToList()?.ForEach(x =>
                {

                    int pmNumber = 0;
                    int amNumber = 0;
                    var timePosition = cl.Positions.Where(p => p.DayPeriod.Day == it.DayPeriod.Day && p.DayPeriod.PeriodName == it.DayPeriod.PeriodName).FirstOrDefault();
                    var classHours = resultModel.ResultClasses?.Where(c => c.ClassID == classID)
                             ?.SelectMany(c => c.ResultDetails)?.ToList();

                    classHours?.ForEach(c =>
                    {
                        var tPosition = cl.Positions.Where(p => p.DayPeriod.Day == c.DayPeriod.Day && p.DayPeriod.PeriodName == c.DayPeriod.PeriodName).FirstOrDefault();
                        if (tPosition != null)
                        {
                            if (tPosition.Position == XYKernel.OS.Common.Enums.Position.AM)
                            {
                                amNumber = amNumber + 1;
                            }

                            if (tPosition.Position == XYKernel.OS.Common.Enums.Position.PM)
                            {
                                pmNumber = pmNumber + 1;
                            }
                        }
                    });

                    //If current time slot is AM, And PMMax is full, Disable PM
                    if (timePosition.Position == XYKernel.OS.Common.Enums.Position.AM)
                    {
                        if (x.PmMax > 0 && pmNumber >= x.PmMax)
                        {
                            //Disable PM
                            var pmTimes = cl.Positions.Where(p => p.IsSelected && p.Position == XYKernel.OS.Common.Enums.Position.PM).Select(p => p.DayPeriod).ToList();
                            dayPeriods = TimeOperation.TimeSlotDiff(dayPeriods, pmTimes);

                            pmTimes.ForEach(pt =>
                            {
                                notReachable.Add(new PostionWithWarningInfo() { DayPeriod = pt, WaringMessage = "违反班级课程的下午最大课时规则!" });
                            });
                        }

                        if (x.AmMax > 0 && amNumber > x.AmMax)
                        {
                            //Disable AM
                            var amTimes = cl.Positions.Where(p => p.IsSelected && p.Position == XYKernel.OS.Common.Enums.Position.AM).Select(p => p.DayPeriod).ToList();
                            dayPeriods = TimeOperation.TimeSlotDiff(dayPeriods, amTimes);

                            amTimes.ForEach(pt =>
                            {
                                notReachable.Add(new PostionWithWarningInfo() { DayPeriod = pt, WaringMessage = "违反班级课程的上午最大课时规则!" });
                            });
                        }
                    }

                    //If current time slot is PM, And AMMax is full, Disable AM
                    if (timePosition.Position == XYKernel.OS.Common.Enums.Position.PM)
                    {
                        if (x.AmMax > 0 && amNumber >= x.AmMax)
                        {
                            //Disable AM
                            var amTimes = cl.Positions.Where(p => p.IsSelected && p.Position == XYKernel.OS.Common.Enums.Position.AM).Select(p => p.DayPeriod).ToList();
                            dayPeriods = TimeOperation.TimeSlotDiff(dayPeriods, amTimes);

                            amTimes.ForEach(at =>
                            {
                                notReachable.Add(new PostionWithWarningInfo() { DayPeriod = at, WaringMessage = "违反班级课程的上午最大课时规则!" });
                            });
                        }

                        if (x.PmMax > 0 && pmNumber > x.PmMax)
                        {
                            //Disable PM
                            var pmTimes = cl.Positions.Where(p => p.IsSelected && p.Position == XYKernel.OS.Common.Enums.Position.PM).Select(p => p.DayPeriod).ToList();
                            dayPeriods = TimeOperation.TimeSlotDiff(dayPeriods, pmTimes);

                            pmTimes.ForEach(at =>
                            {
                                notReachable.Add(new PostionWithWarningInfo() { DayPeriod = at, WaringMessage = "违反班级课程的下午最大课时规则!" });
                            });
                        }
                    }
                });
            });

            return Tuple.Create(dayPeriods, notReachable);
        }
    }

    public class PostionWithWarningInfo
    {
        public DayPeriodModel DayPeriod { get; set; }
        public string WaringMessage { get; set; }
    }
}
