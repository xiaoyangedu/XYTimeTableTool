using Unity;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using XYKernel.OS.Common.Models;
using XYKernel.OS.Common.Models.Administrative.Result;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.DataManager;
using OSKernel.Presentation.Arranging.Utilities;

namespace OSKernel.Presentation.Arranging.Administrative.Result
{
    /// <summary>
    /// 调整课表验证逻辑
    /// </summary>
    public class AdjustLogic
    {
        /// <summary>
        /// 用来临时记录当前本地结果
        /// </summary>
        public static ResultModel CurrentLocalResult;

        /// <summary>
        /// 验证当前调整课程-佟良远
        /// </summary>
        /// <param name="localID">本地方案ID</param>
        /// <param name="classID">班级ID</param>
        /// <param name="item">调整的项</param>
        /// <returns></returns>
        public static List<DayPeriodModel> CheckCanAdjustPosition(string localID, string classID, List<ResultDetailModel> item, ResultModel resultModel)
        {
            var positions = GetMovablePositons(localID, classID, item, resultModel);
            //单向检查结果
            List<DayPeriodModel> oneWayPositions = positions.Item1;
            //双向校验
            List<DayPeriodModel> twoWayPositions = new List<DayPeriodModel>();
            oneWayPositions?.ForEach(x =>
            {
                List<ResultDetailModel> targetItem = resultModel.ResultClasses.Where(rc => rc.ClassID == classID)?
                    .SelectMany(rc => rc.ResultDetails)?.ToList()?
                    .Where(rd => rd.DayPeriod.Day == x.Day && rd.DayPeriod.PeriodName == x.PeriodName)?.ToList();

                if ((targetItem?.Count() ?? 0) == 0)
                {
                    twoWayPositions.Add(x);
                }
                else
                {
                    Tuple<bool, string> checkresult = CanReplacePosition(localID, classID, item, targetItem, resultModel);
                    if (checkresult.Item1)
                    {
                        twoWayPositions.Add(x);
                    }
                }
            });

            #region 在返回结果中增加当前课位
            DayPeriodModel currentPosition = item?.FirstOrDefault()?.DayPeriod;
            if (currentPosition != null)
            {
                twoWayPositions = TimeOperation.TimeSlotUnion(twoWayPositions, new List<DayPeriodModel>() { currentPosition });
            }
            #endregion

            return twoWayPositions;
        }

        public static Tuple<bool, string> CanReplacePosition(string localID, string classID, List<ResultDetailModel> sourceItem, List<ResultDetailModel> targetItem, ResultModel resultModel)
        {
            ICommonDataManager commonDataManager = CacheManager.Instance.UnityContainer.Resolve<ICommonDataManager>();
            var cp = commonDataManager.GetCPCase(localID);
            var rule = commonDataManager.GetAminRule(localID);
            var algo = commonDataManager.GetAminAlgoRule(localID);

            var positionsSource = GetMovablePositons(localID, classID, sourceItem, resultModel);
            var positionsTarget = GetMovablePositons(localID, classID, targetItem, resultModel);

            DayPeriodModel dpSource = sourceItem.FirstOrDefault().DayPeriod;
            DayPeriodModel dpTarget = targetItem.FirstOrDefault().DayPeriod;

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

        private static Tuple<List<DayPeriodModel>, List<PostionWithWarningInfo>> GetMovablePositons(string localID, string classID, List<ResultDetailModel> item, ResultModel resultModel)
        {
            //可用课位，最后取反返回不可用课位
            var dayPeriods = new List<DayPeriodModel>();
            var notReachable = new List<PostionWithWarningInfo>();

            ICommonDataManager commonDataManager = CacheManager.Instance.UnityContainer.Resolve<ICommonDataManager>();
            var cp = commonDataManager.GetCPCase(localID);
            var rule = commonDataManager.GetAminRule(localID);
            var algo = commonDataManager.GetAminAlgoRule(localID);

            //获取方案可用时间
            dayPeriods = cp?.Positions?.Where(p => p.IsSelected
                            && p.Position != XYKernel.OS.Common.Enums.Position.AB
                            && p.Position != XYKernel.OS.Common.Enums.Position.Noon
                            && p.Position != XYKernel.OS.Common.Enums.Position.PB)
                            ?.Select(d => d.DayPeriod)?.ToList() ?? new List<DayPeriodModel>();

            //TODO: 是否基于现有模型更新结果模型中的教师信息?
            item?.ForEach(it =>
            {
                List<string> teachers = new List<string>();
                teachers = it?.Teachers == null ? new List<string>() : it.Teachers.ToList();

                //1.0 移除教师在其他班级的课位
                if (teachers.Count > 0)
                {
                    resultModel.ResultClasses?.Where(c => c.ClassID != classID)?.ToList()?.ForEach(c =>
                    {
                        c.ResultDetails?.Where(rd => rd.Teachers != null && rd.Teachers.Intersect(teachers).Count() > 0)?.ToList()?.ForEach(t =>
                        {
                            notReachable.Add(new PostionWithWarningInfo() { DayPeriod = t.DayPeriod, WaringMessage = $"教师在课位({TimeOperation.GetDateInfo(t.DayPeriod)})安排有其他班级课程!" });
                            dayPeriods.RemoveAll(p => p.Day == t.DayPeriod.Day && p.PeriodName == t.DayPeriod.PeriodName);
                        });
                    });
                }

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

                rule?.CourseTimes?.Where(c => c.Weight == 1 && c.ClassID == classID && c.CourseID == it.CourseID)?.ToList()?.ForEach(x =>
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
                        int classHourNumber = cp.ClassHours.Where(ch => ch.ClassID == classID && ch.CourseID == it.CourseID).Count();

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
                                                                  ?.Where(c => c.CourseID == it.CourseID)
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

                rule?.AmPmClassHours?.Where(a => a.Weight == 1 && a.ClassID == classID && a.CourseID == it.CourseID)?.ToList()?.ForEach(x =>
                {
                    int pmNumber = 0;
                    int amNumber = 0;
                    var timePosition = cp.Positions.Where(p => p.DayPeriod.Day == it.DayPeriod.Day && p.DayPeriod.PeriodName == it.DayPeriod.PeriodName).FirstOrDefault();
                    var classHours = resultModel.ResultClasses?.Where(c => c.ClassID == classID)
                             ?.SelectMany(c => c.ResultDetails)?.Where(c => c.CourseID == it.CourseID)?.ToList();

                    classHours?.ForEach(c =>
                    {
                        var tPosition = cp.Positions.Where(p => p.DayPeriod.Day == c.DayPeriod.Day && p.DayPeriod.PeriodName == c.DayPeriod.PeriodName).FirstOrDefault();
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
                            var pmTimes = cp.Positions.Where(p => p.IsSelected && p.Position == XYKernel.OS.Common.Enums.Position.PM).Select(p => p.DayPeriod).ToList();
                            dayPeriods = TimeOperation.TimeSlotDiff(dayPeriods, pmTimes);

                            pmTimes.ForEach(pt =>
                            {
                                notReachable.Add(new PostionWithWarningInfo() { DayPeriod = pt, WaringMessage = "违反班级课程的下午最大课时规则!" });
                            });
                        }

                        if (x.AmMax > 0 && amNumber > x.AmMax)
                        {
                            //Disable AM
                            var amTimes = cp.Positions.Where(p => p.IsSelected && p.Position == XYKernel.OS.Common.Enums.Position.AM).Select(p => p.DayPeriod).ToList();
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
                            var amTimes = cp.Positions.Where(p => p.IsSelected && p.Position == XYKernel.OS.Common.Enums.Position.AM).Select(p => p.DayPeriod).ToList();
                            dayPeriods = TimeOperation.TimeSlotDiff(dayPeriods, amTimes);

                            amTimes.ForEach(at =>
                            {
                                notReachable.Add(new PostionWithWarningInfo() { DayPeriod = at, WaringMessage = "违反班级课程的上午最大课时规则!" });
                            });
                        }

                        if (x.PmMax > 0 && pmNumber > x.PmMax)
                        {
                            //Disable PM
                            var pmTimes = cp.Positions.Where(p => p.IsSelected && p.Position == XYKernel.OS.Common.Enums.Position.PM).Select(p => p.DayPeriod).ToList();
                            dayPeriods = TimeOperation.TimeSlotDiff(dayPeriods, pmTimes);

                            pmTimes.ForEach(at =>
                            {
                                notReachable.Add(new PostionWithWarningInfo() { DayPeriod = at, WaringMessage = "违反班级课程的下午最大课时规则!" });
                            });
                        }
                    }
                });

                rule?.AmPmNoContinues.Where(a => a.Weight == 1 && teachers.Contains(a.TeacherID)).ToList()?.ForEach(x =>
                {
                    List<DayPeriodModel> currentTimeSlots = resultModel.ResultClasses?.SelectMany(c => c.ResultDetails)?.Where(c => c.Teachers != null && c.Teachers.Contains(x.TeacherID))?.ToList()?.Select(c => c.DayPeriod)?.ToList() ?? new List<DayPeriodModel>() { };
                    var amLast = cp.Positions.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.AM && p.DayPeriod.Day == DayOfWeek.Monday).OrderBy(p => p.PositionOrder).LastOrDefault();
                    var pmFirst = cp.Positions.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.PM && p.DayPeriod.Day == DayOfWeek.Monday).OrderBy(p => p.PositionOrder).FirstOrDefault();
                    List<DayPeriodModel> amLasts = new List<DayPeriodModel>();
                    List<DayPeriodModel> pmFirsts = new List<DayPeriodModel>();

                    if (amLast != null)
                    {
                        amLasts = cp.Positions.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.AM && p.DayPeriod.PeriodName == amLast.DayPeriod.PeriodName).Select(p => p.DayPeriod).ToList();
                    }

                    if (pmFirst != null)
                    {
                        pmFirsts = cp.Positions.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.PM && p.DayPeriod.PeriodName == pmFirst.DayPeriod.PeriodName).Select(p => p.DayPeriod).ToList();
                    }

                    //合并上午最后一节和下午第一节课位
                    List<DayPeriodModel> availableNoonTimes = TimeOperation.TimeSlotUnion(amLasts, pmFirsts);
                    //移除当前要移动的课位
                    currentTimeSlots = TimeOperation.TimeSlotDiff(currentTimeSlots, new List<DayPeriodModel>() { it.DayPeriod });
                    //教师在中午的上课信息
                    List<DayPeriodModel> teacherNoonTimes = TimeOperation.TimeSlotInterSect(currentTimeSlots, availableNoonTimes);
                    List<DayPeriodModel> teacherNotAvailableNoonTimes = new List<DayPeriodModel>() { };

                    availableNoonTimes.ForEach(an =>
                    {
                        if (teacherNoonTimes.Exists(t => t.Day == an.Day))
                        {
                            teacherNotAvailableNoonTimes.Add(an);
                        }
                    });

                    teacherNotAvailableNoonTimes = TimeOperation.TimeSlotDiff(teacherNotAvailableNoonTimes, teacherNoonTimes);
                    dayPeriods = TimeOperation.TimeSlotDiff(dayPeriods, teacherNotAvailableNoonTimes);

                    teacherNotAvailableNoonTimes.ForEach(tn =>
                    {
                        notReachable.Add(new PostionWithWarningInfo() { DayPeriod = tn, WaringMessage = "违反教师中午课时不连排规则!" });
                    });
                });

                rule?.MasterApprenttices.Where(m => m.Weight == 1 && m.CourseID == it.CourseID && (teachers.Contains(m.MasterID) || teachers.Intersect(m.ApprenticeIDs).Count() > 0))?.ToList()?.ForEach(x =>
                {
                    //默认处理逻辑为徒弟之间也不允许同时排课, 不然无法保证互听课
                    List<string> tempTeachers = new List<string>();
                    tempTeachers.AddRange(x.ApprenticeIDs);
                    tempTeachers.AddRange(teachers);
                    tempTeachers.Add(x.MasterID);
                    tempTeachers = tempTeachers.Distinct().ToList();

                    var classHoursTimes = resultModel.ResultClasses?.SelectMany(c => c.ResultDetails)?.Where(c => c.CourseID == it.CourseID && c.Teachers.Intersect(tempTeachers).Count() > 0)?.ToList()?.Select(c => c.DayPeriod);

                    classHoursTimes?.ToList()?.ForEach(c =>
                    {
                        dayPeriods.RemoveAll(d => d.Day == c.Day && d.PeriodName == c.PeriodName);
                        notReachable.Add(new PostionWithWarningInfo() { DayPeriod = c, WaringMessage = "违反师徒跟随规则!" });
                    });
                });

                rule?.Mutexes?.Where(m => m.Weight == 1 && m.CourseIDs != null && m.CourseIDs.Contains(it.CourseID))?.ToList()?.ForEach(x =>
                {
                    List<string> tempCourseIds = x.CourseIDs?.Select(xc => xc)?.ToList();
                    tempCourseIds.RemoveAll(t => t == it.CourseID);

                    var classHoursTimes = resultModel.ResultClasses
                    ?.Where(r => r.ClassID == classID)?.SelectMany(c => c.ResultDetails)
                    ?.Where(c => tempCourseIds.Contains(c.CourseID))
                    ?.ToList()?.Select(c => c.DayPeriod.Day);

                    classHoursTimes?.ToList()?.ForEach(c =>
                    {

                        dayPeriods.Where(d => d.Day == c)?.ToList()?.ForEach(dp =>
                        {
                            notReachable.Add(new PostionWithWarningInfo() { DayPeriod = dp, WaringMessage = "违反课程互斥规则!" });
                        });

                        dayPeriods.RemoveAll(d => d.Day == c);
                    });
                });

                #region 合班规则
                List<string> unionClassIDs = new List<string>();
                rule?.ClassUnions?.Where(cu => cu.ClassIDs != null && cu.ClassIDs.Contains(classID) && cu.CourseID == it.CourseID)?.ToList()?.ForEach(x => {
                    //Get All Union ClassID
                    x.ClassIDs.ForEach(cl => {
                        if (!unionClassIDs.Contains(cl))
                        {
                            unionClassIDs.Add(cl);
                        }
                    });
                });

                if (unionClassIDs.Count > 1)
                {
                    bool flag = true;
                    List<DayPeriodModel> timeslots = new List<DayPeriodModel>();

                    foreach (string classId in unionClassIDs)
                    {
                        var courseTimes = resultModel.ResultClasses?
                            .Where(rc => rc.ClassID == classId)?
                            .SelectMany(c => c.ResultDetails)?
                            .Where(c => c.CourseID == it.CourseID)?
                            .ToList()?.Select(c => c.DayPeriod)?.ToList() ?? new List<DayPeriodModel>();

                        if (timeslots.Count == 0)
                        {
                            timeslots = courseTimes;
                        }
                        else
                        {
                            List<DayPeriodModel> timesUnion = TimeOperation.TimeSlotUnion(timeslots.ToList(), courseTimes);
                            List<DayPeriodModel> timesDiff = TimeOperation.TimeSlotDiff(timeslots.ToList(), courseTimes);
                            if (timesUnion.Count != timeslots.Count || timesDiff.Count != 0)
                            {
                                flag = false;
                            }
                        }
                    }

                    if (flag)
                    {
                        List<DayPeriodModel> timesInterSect = TimeOperation.TimeSlotInterSect(timeslots.ToList(), dayPeriods.ToList());
                        List<DayPeriodModel> timesDiff = TimeOperation.TimeSlotDiff(dayPeriods.ToList(), timesInterSect.ToList());

                        timesDiff?.ForEach(c =>
                        {
                            dayPeriods.RemoveAll(d => d.Day == c.Day && d.PeriodName == c.PeriodName);
                            notReachable.Add(new PostionWithWarningInfo() { DayPeriod = c, WaringMessage = "违反合班规则!" });
                        });
                    }
                }
                #endregion
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
