using System;
using System.Collections.Generic;
using System.Linq;
using XYKernel.OS.Common.Models;
using XYKernel.OS.Common.Models.Administrative;
using XYKernel.OS.Common.Models.Administrative.Rule;
using XYKernel.OS.Common.Models.Administrative.AlgoRule;
using XYKernel.OS.Common.Models.Administrative.Result;
using OSKernel.Presentation.Analysis.Result.Models;
using OSKernel.Presentation.Models.Enums;
using OSKernel.Presentation.Analysis.Utilities;

namespace OSKernel.Presentation.Analysis.Result.Administrative
{
    public class ResultAnalysis
    {
        /// <summary>
        /// 排课结果规则匹配度检测
        /// </summary>
        /// <param name="cPCase"></param>
        /// <param name="rule"></param>
        /// <param name="algoRule"></param>
        /// <param name="resultModel"></param>
        /// <returns></returns>
        public static List<RuleAnalysisDetailResult> GetResultRuleFitDegreeAnalysis(CPCase cPCase, Rule rule, AlgoRule algoRule, ResultModel resultModel)
        {
            #region 公共变量
            List<RuleAnalysisDetailResult> drList = new List<RuleAnalysisDetailResult>() { };

            //方案可用课位
            List<DayPeriodModel> caseAvailableDayPeriod = cPCase?.Positions?.Where(p => p.IsSelected
                                      && p.Position != XYKernel.OS.Common.Enums.Position.AB
                                      && p.Position != XYKernel.OS.Common.Enums.Position.Noon
                                      && p.Position != XYKernel.OS.Common.Enums.Position.PB)?.Select(x => x.DayPeriod)?.ToList() ?? new List<DayPeriodModel>();

            //上午课位（含早自习）
            List<DayPeriodModel> amDayPeriod = cPCase?.Positions?.Where(p => p.IsSelected && (p.Position == XYKernel.OS.Common.Enums.Position.AM || p.Position == XYKernel.OS.Common.Enums.Position.MS))?
                                                .Select(p => p.DayPeriod)?.ToList() ?? new List<DayPeriodModel>() { };
            //下午课位（含晚自习）
            List<DayPeriodModel> pmDayPeriod = cPCase?.Positions?.Where(p => p.IsSelected && (p.Position == XYKernel.OS.Common.Enums.Position.PM || p.Position == XYKernel.OS.Common.Enums.Position.ES))?
                                                .Select(p => p.DayPeriod)?.ToList() ?? new List<DayPeriodModel>() { };

            //班级课程课时对应关系
            var classCourseMapping = from x in cPCase?.Classes
                                     from y in x.Settings
                                     select new { x.ID, y.CourseID, y.Lessons };

            //排课结果
            var allResultLessons = from rc in resultModel?.ResultClasses
                                 from rd in rc.ResultDetails
                                 select new { rc.ClassID, rd.CourseID, rd.DayPeriod, rd.Teachers };

            //全局课表位置信息
            var allCaseDayPeriods = resultModel?.Positions?.Where(p => p.Position != XYKernel.OS.Common.Enums.Position.AB
                                                && p.Position != XYKernel.OS.Common.Enums.Position.PB
                                                && p.Position != XYKernel.OS.Common.Enums.Position.Noon
                                                )?.Select(p => p.DayPeriod)?.ToList();

            //获取上下午大课间和午休关联的课位信息
            List<DayPeriodModel> aMBreakPreviousNextTimes = new List<DayPeriodModel>();
            List<DayPeriodModel> pMBreakPreviousNextTimes = new List<DayPeriodModel>();
            List<DayPeriodModel> noonBreakPreviousNextTimes = new List<DayPeriodModel>();
            Tuple<List<DayPeriodModel>, List<DayPeriodModel>, List<DayPeriodModel>> aMPMNoonBreakTimes = Miscellaneous.GetCPAMPMNoonBreak(cPCase?.Positions);
            aMBreakPreviousNextTimes = aMPMNoonBreakTimes.Item1;
            noonBreakPreviousNextTimes = aMPMNoonBreakTimes.Item2;
            pMBreakPreviousNextTimes = aMPMNoonBreakTimes.Item3;

            //获取全局课表信息
            Tuple<int[], int[], int[]> timeTableSlot = Miscellaneous.GetGlobalTimeTableSlot(allCaseDayPeriods);

            #endregion

            #region 上下午课时
            RuleAnalysisDetailResult drAmPmClassHours = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drAmPmClassHours.RuleName = Miscellaneous.GetLocalDescription(AdministrativeRuleEnum.AmPmClassHour);

            rule?.AmPmClassHours?.Where(ap => ap.Weight == 1)?.ToList()?.ForEach(ap => {

                string classCourseName = Miscellaneous.GetCPClassCourseInfo(cPCase, ap.ClassID, ap.CourseID);
                var courseClassInfo = resultModel?.ResultClasses?.Where(rc => rc.ClassID == ap.ClassID)?
                .FirstOrDefault()?.ResultDetails?
                .Where(rd => rd.CourseID == ap.CourseID)?
                .Select(rd => rd.DayPeriod)?.ToList();

                if (ap.AmMax > 0)
                {
                    var apLesson = TimeOperation.TimeSlotInterSect(amDayPeriod, courseClassInfo);
                    if (apLesson.Count > ap.AmMax)
                    {
                        drAmPmClassHours.DetailInfo.Add($"{ classCourseName } 的课表结果不满足上午最大{ ap.AmMax }课时的约束!");
                    }
                }

                if (ap.PmMax > 0)
                {
                    var apLesson = TimeOperation.TimeSlotInterSect(pmDayPeriod, courseClassInfo);
                    if (apLesson.Count > ap.PmMax)
                    {
                        drAmPmClassHours.DetailInfo.Add($"{ classCourseName } 的课表结果不满足下午最大{ ap.PmMax }课时的约束!");
                    }
                }
            });

            drList.Add(drAmPmClassHours);
            #endregion

            #region 教师上下午不连排
            RuleAnalysisDetailResult drAmPmNoContinues = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drAmPmNoContinues.RuleName = Miscellaneous.GetLocalDescription(AdministrativeRuleEnum.TeacherAmPmNoContinues);

            List<int> limitAmPositionValue = cPCase?.Positions?.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.AM)?.Select(p => p.PositionOrder)?.Distinct()?.ToList() ?? new List<int>();
            List<int> limitPmPositionValue = cPCase?.Positions?.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.PM)?.Select(p => p.PositionOrder)?.Distinct()?.ToList() ?? new List<int>();
            limitAmPositionValue = limitAmPositionValue.OrderBy(am => am).ToList();
            limitPmPositionValue = limitPmPositionValue.OrderBy(pm => pm).ToList();
            List<DayPeriodModel> amPmTimes = new List<DayPeriodModel>();
            
            if (limitAmPositionValue.Count > 0 && limitPmPositionValue.Count > 0)
            {
                //上午最后一节
                cPCase.Positions.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.AM && p.PositionOrder == limitAmPositionValue[limitAmPositionValue.Count - 1]).ToList().ForEach(p => {
                    amPmTimes.Add(p.DayPeriod);
                });

                //下午第一节
                cPCase.Positions.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.PM && p.PositionOrder == limitPmPositionValue[0]).ToList().ForEach(p => {
                    amPmTimes.Add(p.DayPeriod);
                });

                rule?.AmPmNoContinues?.Where(ap => ap.Weight == 1)?.ToList()?.ForEach(apnc => {
                    if (cPCase?.Teachers?.Exists(te => te.ID == apnc.TeacherID) ?? false)
                    {
                        string teacherName = cPCase.Teachers.FirstOrDefault(t => t.ID == apnc.TeacherID).Name;
                        List<int> classHours = cPCase?.ClassHours?.Where(ch => ch.TeacherIDs != null && ch.TeacherIDs.Contains(apnc.TeacherID))?.Select(ch => ch.ID)?.ToList() ?? new List<int>();

                        var classhours = from rc in resultModel?.ResultClasses
                                         from rd in rc.ResultDetails
                                         where classHours.Contains(rd.ClassHourId)
                                         select rd.DayPeriod;

                        //遍历每一天判断是否有违规
                        foreach (int day in Enum.GetValues(typeof(DayOfWeek)))
                        {
                            var oneDayAmPmTimes = amPmTimes.Where(ap => ap.Day == (DayOfWeek)day)?.ToList();
                            var interSectTimes = TimeOperation.TimeSlotInterSect(oneDayAmPmTimes, classhours.ToList()) ;

                            if (interSectTimes.Count == 2)
                            {
                                string weekName = TimeOperation.GetWeekName((DayOfWeek)day);
                                drAmPmNoContinues.DetailInfo.Add($"教师 { teacherName } 的课表在 {weekName} 中午有连排!");
                            }
                        }
                    }
                });
            }

            drList.Add(drAmPmNoContinues);

            #endregion

            #region 课时分散
            RuleAnalysisDetailResult drClassHourAverages = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drClassHourAverages.RuleName = Miscellaneous.GetLocalDescription(AdministrativeRuleEnum.ClassHourAverage);

            rule?.ClassHourAverages?.Where(cha => cha.Weight == 1)?.ToList()?.ForEach(cha => {
                int courseContinouCount = rule?.CourseArranges?.Where(ca => ca.CourseID == cha.CourseID && ca.ClassID == cha.ClassID)?.FirstOrDefault()?.Count ?? 0;
                int lesson = classCourseMapping?.FirstOrDefault(cc => cc.ID == cha.ClassID && cc.CourseID == cha.CourseID)?.Lessons ?? 0;
                int needDays = 0;
                
                if (courseContinouCount == 0)
                {
                    //如果没有连排
                    needDays = lesson;
                }
                else
                {
                    //如果有连排
                    needDays = lesson - courseContinouCount;
                }

                List<int> classHours = cPCase?.ClassHours?.Where(ch => ch.ClassID == cha.ClassID && ch.CourseID == cha.CourseID)?.Select(ch => ch.ID)?.ToList() ?? new List<int>();

                var classhours = from rc in resultModel?.ResultClasses
                                 from rd in rc.ResultDetails
                                 where classHours.Contains(rd.ClassHourId)
                                 select rd.DayPeriod;

                bool dayEnough = Miscellaneous.DaysEnoughForEvenlyDistributed(classhours?.ToList(), needDays, cha.MinDay);

                if (!dayEnough)
                {
                    string classCourseName = Miscellaneous.GetCPClassCourseInfo(cPCase, cha.ClassID, cha.CourseID);
                    drClassHourAverages.DetailInfo.Add($"{ classCourseName } 的课表不满足最小间隔为 {cha.MinDay} 天的课时分散!");
                }
            });

            drList.Add(drClassHourAverages);
            #endregion

            #region 教案平头
            RuleAnalysisDetailResult drClassHourPriorityBalance = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drClassHourPriorityBalance.RuleName = Miscellaneous.GetLocalDescription(AdministrativeRuleEnum.TeacherPriorityBalanceRule);

            rule?.ClassHourPriorityBalance?.Where(pb => pb.Weight == 1 && pb.ClassIDs != null && pb.ClassIDs.Count > 1)?.ToList()?.ForEach(pb => {

                string teacherName = cPCase?.Teachers?.FirstOrDefault(te => te.ID == pb.TeacherID)?.Name ?? string.Empty;
                string courseName = cPCase?.Courses?.FirstOrDefault(co => co.ID == pb.CourseID)?.Name ?? string.Empty;
                List<string> classNames = new List<string>();

                pb.ClassIDs.ForEach(cl => {
                    classNames.Add(cPCase?.Classes?.FirstOrDefault(cla => cla.ID == cl)?.Name ?? string.Empty);
                });

                var teacherLessons = from rc in resultModel?.ResultClasses
                                     from rd in rc.ResultDetails
                                     where rd.CourseID == rd.CourseID && pb.ClassIDs.Contains(rc.ClassID) && pb.CourseID == rd.CourseID 
                                     select new { rc.ClassID, rd.CourseID, rd.DayPeriod };

                var teacherLessonGroup = teacherLessons?.GroupBy(tl => new { tl.ClassID, tl.CourseID })?.Select(tl => new { tl.Key.ClassID, tl.Key.CourseID, LessonCount = tl.Count() });
                int maxLesson = teacherLessonGroup?.Max(lg => lg.LessonCount) ?? 0;
                int minLesson = teacherLessonGroup?.Min(lg => lg.LessonCount) ?? 0;

                if (maxLesson == minLesson && minLesson > 1)
                {
                    //只有在各班课时相同且课时都大于1时才考虑校验
                    var teacherTempData = from tl in teacherLessons
                                          select new { Day = tl.DayPeriod.Day == 0 ? 7 : (int)tl.DayPeriod.Day, tl.DayPeriod.Period, tl.ClassID };

                    //统计各班优先次数
                    teacherTempData = teacherTempData.OrderBy(t => t.Day).ThenBy(t => t.Period);
                    string[] classIDs = teacherTempData.Select(t => t.ClassID).Distinct().ToArray();
                    int[] classIDPriority = new int[classIDs.Length];
                    int lessonCount = teacherTempData.Count();

                    if (classIDs.Length > 0 && lessonCount == maxLesson * classIDs.Length)
                    {
                        for (int i = 0; i < maxLesson; i++)
                        {
                            string tempClassID = string.Empty;
                            int iIndexDP = 1000;

                            for (int j = 0; j < classIDs.Length; j++)
                            {
                                var lessonInfo = teacherTempData.Where(td => td.ClassID == classIDs[j])
                                                                .OrderBy(od => od.Day)
                                                                .ThenBy(od => od.Period)
                                                                .Skip(i).Take(1).FirstOrDefault();

                                int classDayPeriodIndex = Array.IndexOf(timeTableSlot.Item3, lessonInfo.Day * 100 + lessonInfo.Period);

                                if (classDayPeriodIndex < iIndexDP)
                                {
                                    tempClassID = classIDs[j];
                                    iIndexDP = classDayPeriodIndex;
                                }
                            }

                            int iIndex = Array.IndexOf(classIDs, tempClassID);
                            classIDPriority[iIndex] += 1;
                        }

                        float meanValue = (float)maxLesson / classIDs.Length;

                        for (int i = 0; i < classIDPriority.Length; i++)
                        {
                            if (Math.Abs(classIDPriority[i] - meanValue) >= 1.0)
                            {
                                drClassHourPriorityBalance.DetailInfo.Add($"教师 { teacherName } 在 {string.Join(",", classNames)} 的 {courseName} 课表教案不平头!");
                                break;
                            }
                        }
                    }
                }
            });

            drList.Add(drClassHourPriorityBalance);
            #endregion

            #region 同时开课
            RuleAnalysisDetailResult drClassHourSameOpens = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drClassHourSameOpens.RuleName = Miscellaneous.GetLocalDescription(AdministrativeRuleEnum.ClassHourSameOpen);

            rule?.ClassHourSameOpens?.Where(so => so.Weight == 1)?.ToList()?.ForEach(so => {

                List<CourseClassModel> ccmList = new List<CourseClassModel>();

                //获取全部教学班级
                so.Details.ForEach(de => {
                    de.Classes?.ForEach(cl => {
                        if (!ccmList.Exists(cc => cc.ClassID == cl.ClassID && cc.CourseID == cl.CourseID))
                        {
                            ccmList.Add(new CourseClassModel() {  ClassID = cl.ClassID, CourseID = cl.CourseID });
                        }
                    });
                });
                
                if (ccmList.Any())
                {
                    //获取全部班级课时ID
                    List<int> sameOpenRelatedLessons = new List<int>();

                    ccmList.ForEach(cc => {
                        List<int> singleClassLesson = cPCase?.ClassHours?.Where(ch => ch.ClassID == cc.ClassID && ch.CourseID == cc.CourseID)?.Select(ch => ch.ID)?.ToList() ?? new List<int>();
                        sameOpenRelatedLessons.AddRange(singleClassLesson);
                    });

                    sameOpenRelatedLessons = sameOpenRelatedLessons.Distinct().ToList();

                    //获取全部上课时间
                    var lessonResult = from rc in resultModel?.ResultClasses
                                     from rd in rc.ResultDetails
                                     where sameOpenRelatedLessons.Contains(rd.ClassHourId)
                                     select new {rd.DayPeriod, rd.CourseID, rc.ClassID } ;

                    var lessonResultGroup = lessonResult.GroupBy(lr => new { lr.DayPeriod.Day, lr.DayPeriod.Period })
                                            .Select(g => new { g.Key.Day, g.Key.Period, Count = g.Count() });

                    var lessonResultGroupDescending = lessonResultGroup.OrderByDescending(g => g.Count);

                    //班级课程上课时间分组
                    ClassHourSameOpenRule chsoResultRule = new ClassHourSameOpenRule() { Details = new List<SameOpenDetailsModel> { } };

                    lessonResultGroupDescending?.ToList()?.ForEach(g => {

                        SameOpenDetailsModel sodmResult = new SameOpenDetailsModel() { Classes = new List<CourseClassModel>() { } };

                        List<CourseClassModel> courseClassResult = new List<CourseClassModel>() { };
                        var lessonResultPerSlot = lessonResult.Where(lr => lr.DayPeriod.Day == g.Day && lr.DayPeriod.Period == g.Period);

                        lessonResultPerSlot?.ToList()?.ForEach(lrp => {
                            if (!courseClassResult.Exists(cc => cc.ClassID == lrp.ClassID && cc.CourseID == lrp.CourseID))
                            {
                                courseClassResult.Add(new CourseClassModel() { ClassID = lrp.ClassID, CourseID = lrp.CourseID });
                            }
                        });

                        sodmResult.Classes = courseClassResult;
                        sodmResult.Index = courseClassResult.Count;
                        chsoResultRule.Details.Add(sodmResult);
                    });

                    //验证排课结果能覆盖规则的要求
                    ClassHourSameOpenRule chsoSourceRule = new ClassHourSameOpenRule() { Details = new List<SameOpenDetailsModel>() { } };
                    chsoSourceRule.Details = so.Details.ToList();
                    chsoSourceRule.Details = chsoSourceRule.Details.OrderByDescending(de => de.Classes.Count).ToList();
                    chsoResultRule.Details = chsoResultRule.Details.OrderByDescending(de => de.Classes.Count).ToList();

                    int tempLesson = chsoSourceRule.Details.Count;
                    bool checkResult = true;
                    //计算规则设置的总课时
                    int sourceRuleLesson = chsoSourceRule.Details.SelectMany(de => de.Classes).Count();
                    //计算班级科目总课时
                    int classCourseLesson = 0;
                    ccmList.ForEach(cc => {
                        int lesson = classCourseMapping.FirstOrDefault(ccm => ccm.ID == cc.ClassID && ccm.CourseID == cc.CourseID)?.Lessons ?? 0;
                        classCourseLesson += lesson;
                    });
                    //获取班级科目名称
                    List<string> classCourseNameList = new List<string>();
                    ccmList.ForEach(cc => {
                        classCourseNameList.Add(Miscellaneous.GetCPClassCourseInfo(cPCase, cc.ClassID, cc.CourseID));
                    });
                    //仅检查所有课时都设定同时开课的情况
                    if (classCourseLesson == sourceRuleLesson)
                    {
                        int resultRuleLesson = chsoResultRule.Details.Take(tempLesson).SelectMany(de => de.Classes).Count();
                        if (resultRuleLesson < sourceRuleLesson)
                        {
                            checkResult = false;
                        }

                        //for (int i = 0; i < tempLesson && checkResult; i++)
                        //{
                        //    if (chsoResultRule.Details.Count > i)
                        //    {
                        //        SameOpenDetailsModel sourceDetail = chsoSourceRule.Details[i];
                        //        SameOpenDetailsModel resultDetail = chsoSourceRule.Details[i];

                        //        if (sourceDetail?.Classes != null && resultDetail?.Classes != null)
                        //        {
                        //            foreach (var item in sourceDetail.Classes)
                        //            {
                        //                if (!resultDetail.Classes.Exists(rd => rd.ClassID == item.ClassID && rd.CourseID == item.CourseID))
                        //                {
                        //                    checkResult = false;
                        //                    break;
                        //                }
                        //            }

                        //            foreach (var item in resultDetail.Classes)
                        //            {
                        //                if (!sourceDetail.Classes.Exists(sd => sd.ClassID == item.ClassID && sd.CourseID == item.CourseID))
                        //                {
                        //                    checkResult = false;
                        //                    break;
                        //                }
                        //            }
                        //        }
                        //    }
                        //}

                        if (!checkResult)
                        {
                            drClassHourSameOpens.DetailInfo.Add($"请检查 { string.Join(",", classCourseNameList) } 课表的同时开课规则!");
                        }
                    }
                    else
                    {
                        drClassHourSameOpens.DetailInfo.Add($"目前无法对部分课时同时开课的设置检查，请人为检查 { string.Join(",", classCourseNameList) } 课表的同时开课规则!");
                    }
                }
            });

            drList.Add(drClassHourSameOpens);
            #endregion

            #region 合班上课
            RuleAnalysisDetailResult drClassUnions = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drClassUnions.RuleName = Miscellaneous.GetLocalDescription(AdministrativeRuleEnum.ClassUnion);

            rule?.ClassUnions?.Where(cu => cu.ClassIDs != null)?.ToList()?.ForEach(cu =>
            {
                List<int> classHours = cPCase?.ClassHours?.Where(ch => ch.CourseID == cu.CourseID && cu.ClassIDs.Contains(ch.ClassID))?.Select(ch => ch.ID)?.ToList() ?? new List<int>();

                var classhours = from rc in resultModel?.ResultClasses
                                 from rd in rc.ResultDetails
                                 where classHours.Contains(rd.ClassHourId)
                                 select rd.DayPeriod;

                int lesson = classCourseMapping?.FirstOrDefault(cc => cc.ID == cu.ClassIDs[0] && cc.CourseID == cu.CourseID)?.Lessons ?? 0;

                var lessonGroup = classhours.Select(ch => new { ch.Day, ch.PeriodName })?
                                .GroupBy(g => new { g.Day, g.PeriodName })?
                                .Select( s => new { 
                                    s.Key.Day,
                                    s.Key.PeriodName,
                                    Count = s.Count()
                                });

                if (lessonGroup != null)
                {
                    if (lessonGroup.Count() != lesson || lessonGroup.Any(lg => lg.Count != cu.ClassIDs.Count()))
                    {
                        string classCourseName = Miscellaneous.GetCPClassCourseInfo(cPCase, cu.ClassIDs[0], cu.CourseID);
                        drClassUnions.DetailInfo.Add($"课表没有满足 { classCourseName } 的合班规则!");
                    }
                }
            });

            drList.Add(drClassUnions);
            #endregion

            #region 连排齐头
            RuleAnalysisDetailResult drContinuousPlanFlushes = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drContinuousPlanFlushes.RuleName = Miscellaneous.GetLocalDescription(AdministrativeRuleEnum.ArrangeContinuousPlanFlush);

            rule?.ContinuousPlanFlushes?.Where(pf => pf.Weight == 1 && (pf.ClassIDs?.Count ?? 0) > 1)?.ToList()?.ForEach(pf => {

                string courseName = cPCase?.Courses?.FirstOrDefault(co => co.ID == pf.CourseID)?.Name ?? string.Empty;
                List<string> classNames = new List<string>();

                pf.ClassIDs.ForEach(cl => {
                    classNames.Add(cPCase?.Classes?.FirstOrDefault(cla => cla.ID == cl)?.Name ?? string.Empty);
                });

                //验证这些班级有相同的课时和连排数设置
                var courseLessons = from cl in cPCase?.Classes
                          from co in cl.Settings
                          where pf.ClassIDs.Contains(cl.ID) && co.CourseID == pf.CourseID
                          select co.Lessons;

                int maxLesson = courseLessons?.ToList()?.Max() ?? 0;
                int minLesson = courseLessons?.ToList()?.Min() ?? 0;
                
                var courseContinous = from co in rule?.CourseArranges
                                      where co.CourseID == pf.CourseID && pf.ClassIDs.Contains(co.ClassID)
                                      select co.Count;

                int courseNumber = courseContinous?.Count() ?? 0;
                int maxContinousLesson = courseContinous?.ToList()?.Max() ?? 0;
                int minContinousLesson = courseContinous?.ToList()?.Min() ?? 0;

                //课时相同，连排数量相同，然后再检查连排齐头
                if (maxLesson == minLesson 
                    && courseNumber == pf.ClassIDs.Count 
                    && maxContinousLesson == minContinousLesson)
                {
                    Dictionary<string, List<int>> courseContinousDayInfo = new Dictionary<string, List<int>>();
                    List<int> courseContinousNumber = new List<int>();

                    pf.ClassIDs.ForEach(cl => {
                        var classCourseLesson = allResultLessons?.Where(rl => rl.ClassID == cl && rl.CourseID == pf.CourseID)?
                                                .Select(cc => cc.DayPeriod)?.ToList();
                        int[] classCourseTimeTableSlot = Miscellaneous.GetPartTimeTableSlot(timeTableSlot.Item3, classCourseLesson);
                        Tuple<int, List<int>> classCourseContinousInfo = Miscellaneous.ComputeContinousNumberByTimeTable(classCourseTimeTableSlot, timeTableSlot.Item1, timeTableSlot.Item2);
                        courseContinousNumber.Add(classCourseContinousInfo.Item1);
                        courseContinousDayInfo.Add(cl, classCourseContinousInfo.Item2);
                    });

                    if (courseContinousNumber.Max() == courseContinousNumber.Min() && courseContinousNumber.Count > 0)
                    {
                        List<int> continousMaxDays = new List<int>();
                        List<int> continousMinDays = new List<int>();

                        for (int i = 0; i < courseContinousNumber[0]; i++)
                        {
                            List<int> tempValue = new List<int>();
                            foreach (var item in courseContinousDayInfo)
                            {
                                tempValue.Add(item.Value.OrderBy(va => va).Skip(i).Take(1).FirstOrDefault());
                            }
                            continousMaxDays.Add(tempValue.Max());
                            continousMinDays.Add(tempValue.Min());
                        }

                        bool teachingPlan = true;
                        bool planInDays = true;

                        for (int i = 0; i < continousMaxDays.Count; i++)
                        {
                            //判断宽松齐头
                            if (i < continousMaxDays.Count - 1)
                            {
                                int currentMax = continousMaxDays[i];
                                int nextMin = continousMinDays[i+1];
                                if (currentMax >= nextMin)
                                {
                                    teachingPlan = false;
                                    break;
                                }
                            }

                            //检查是否在指定天内齐头
                            if (Math.Abs(continousMaxDays[i] - continousMinDays[i]) > pf.FlushDays)
                            {
                                planInDays = false;
                                break;
                            }
                        }

                        if (!teachingPlan || !planInDays)
                        {
                            drContinuousPlanFlushes.DetailInfo.Add($"{ string.Join(",", classNames) } 的 {courseName} 课表未能满足连排 {pf.FlushDays} 天内齐头规则!");
                        }
                    }
                }
            });

            drList.Add(drContinuousPlanFlushes);
            #endregion

            #region 课程连排
            RuleAnalysisDetailResult drCourseArranges = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drCourseArranges.RuleName = Miscellaneous.GetLocalDescription(AdministrativeRuleEnum.CourseArrangeContinuous);

            rule?.CourseArranges?.ForEach(ca => {
                //1. 计算连排数
                var classCourseLesson = allResultLessons?.Where(rl => rl.ClassID == ca.ClassID && rl.CourseID == ca.CourseID);
                var lessons = classCourseLesson?.Select(cl => cl.DayPeriod)?.ToList();
                int[] classCourseTimeTableSlot = Miscellaneous.GetPartTimeTableSlot(timeTableSlot.Item3, lessons);
                Tuple<int, List<int>> classCourseContinousInfo = Miscellaneous.ComputeContinousNumberByTimeTable(classCourseTimeTableSlot, timeTableSlot.Item1, timeTableSlot.Item2);
                int continousNumber = classCourseContinousInfo.Item1;
                string classCourseName = Miscellaneous.GetCPClassCourseInfo(cPCase, ca.ClassID, ca.CourseID);

                if (continousNumber < ca.Count)
                {
                    drCourseArranges.DetailInfo.Add($"{ classCourseName } 的课表连排数量为 {continousNumber} 没有满足连排数 {ca.Count} 的要求!");
                }

                //2. 检查时间
                if (ca.Times != null && ca.TimesWeight == 1 && continousNumber > 0)
                {
                    //计算必须时间能提供几个连排
                    int maxContinous = Math.Min(ca.Times.Count / 2, ca.Count);
                    //连排时间与连排必须时间交集
                    List<DayPeriodModel> continousTimeIntersect = TimeOperation.TimeSlotInterSect(ca.Times, lessons);
                    //求交集能提供几个连排
                    int[] intersectTimeTableSlot = Miscellaneous.GetPartTimeTableSlot(timeTableSlot.Item3, continousTimeIntersect);
                    Tuple<int, List<int>> intersectContinousInfo = Miscellaneous.ComputeContinousNumberByTimeTable(intersectTimeTableSlot, timeTableSlot.Item1, timeTableSlot.Item2);
                    int intersectContinousNumber = intersectContinousInfo.Item1;

                    if (intersectContinousNumber < maxContinous)
                    {
                        drCourseArranges.DetailInfo.Add($"{ classCourseName } 的课表连排结果课位不满足连排必须时间要求!");
                    }
                }

                //3. 检查隔天连排
                if (ca.IsIntervalDay && ca.IntervalDayWeight == 1 && continousNumber > 1)
                {
                    List<int> continousDays = classCourseContinousInfo.Item2;
                    for (int i = 0; i < continousDays.Count; i++)
                    {
                        for (int j = i + 1; j < continousDays.Count; j++)
                        {
                            //检查任意2个连排之间是否满足隔天
                            int intervalDay = Math.Abs(continousDays[i] - continousDays[j]);
                            if (intervalDay <= 1)
                            {
                                drCourseArranges.DetailInfo.Add($"{ classCourseName } 的课表连排结果不满足连排隔天要求!");
                            }
                        }
                    }
                }

                //4. 检查不跨上下午，大课间
                if (ca.NoCrossingBreak && ca.NoCrossingBreakWeight == 1 && continousNumber > 0)
                {
                    List<DayPeriodModel> amBreakInterSect = TimeOperation.TimeSlotInterSect(aMBreakPreviousNextTimes, lessons);
                    List<DayPeriodModel> noonBreakInterSect = TimeOperation.TimeSlotInterSect(noonBreakPreviousNextTimes, lessons);
                    List<DayPeriodModel> pmBreakInterSect = TimeOperation.TimeSlotInterSect(pMBreakPreviousNextTimes, lessons);

                    int[] amBreakInterSectTimeTableSlot = Miscellaneous.GetPartTimeTableSlot(timeTableSlot.Item3, amBreakInterSect);
                    Tuple<int, List<int>> amBreakInterSectContinousInfo = Miscellaneous.ComputeContinousNumberByTimeTable(amBreakInterSectTimeTableSlot, timeTableSlot.Item1, timeTableSlot.Item2);
                    if (amBreakInterSectContinousInfo.Item1 > 0)
                    {
                        drCourseArranges.DetailInfo.Add($"{ classCourseName } 的课表有跨上午大课间的连排!");
                    }

                    int[] noonBreakInterSectTimeTableSlot = Miscellaneous.GetPartTimeTableSlot(timeTableSlot.Item3, noonBreakInterSect);
                    Tuple<int, List<int>> noonBreakInterSectContinousInfo = Miscellaneous.ComputeContinousNumberByTimeTable(noonBreakInterSectTimeTableSlot, timeTableSlot.Item1, timeTableSlot.Item2);
                    if (noonBreakInterSectContinousInfo.Item1 > 0)
                    {
                        drCourseArranges.DetailInfo.Add($"{ classCourseName } 的课表有跨上下午连排!");
                    }

                    int[] pmBreakInterSectTimeTableSlot = Miscellaneous.GetPartTimeTableSlot(timeTableSlot.Item3, pmBreakInterSect);
                    Tuple<int, List<int>> pmBreakInterSectContinousInfo = Miscellaneous.ComputeContinousNumberByTimeTable(pmBreakInterSectTimeTableSlot, timeTableSlot.Item1, timeTableSlot.Item2);
                    if (pmBreakInterSectContinousInfo.Item1 > 0)
                    {
                        drCourseArranges.DetailInfo.Add($"{ classCourseName } 的课表有跨下午大课间的连排!");
                    }
                }
            });

            drList.Add(drCourseArranges);
            #endregion

            #region 同时开课限制
            RuleAnalysisDetailResult drCourseLimits = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drCourseLimits.RuleName = Miscellaneous.GetLocalDescription(AdministrativeRuleEnum.CourseLimit);

            rule?.CourseLimits?.Where(cl => cl.Weight == 1)?.ToList()?.ForEach(cl => {

                string cousreName = cPCase?.Courses?.FirstOrDefault(co => co.ID == cl.CourseID)?.Name ?? string.Empty;
                List<int> classHours = cPCase?.ClassHours?.Where(ch => ch.CourseID == cl.CourseID)?.Select(ch => ch.ID)?.ToList() ?? new List<int>();
                
                var lessonResult = from rc in resultModel?.ResultClasses
                                 from rd in rc.ResultDetails
                                 where classHours.Contains(rd.ClassHourId)
                                 select rd.DayPeriod;

                var lessonResultGroup = lessonResult?.GroupBy(lr => new { lr.Day, lr.Period })?
                                        .Select(g => new { g.Key.Day, g.Key.Period, Count = g.Count() });

                if (cl.Limit > 0 && lessonResultGroup != null)
                {
                    if (lessonResultGroup.Any(lr => lr.Count > cl.Limit))
                    {
                        List<string> detailInfo = new List<string>();
                        lessonResultGroup.Where(lr => lr.Count > cl.Limit).ToList().ForEach(lr => {
                            string weekName = TimeOperation.GetWeekName(lr.Day);
                            string periodName = cPCase.Positions.Where(p => p.DayPeriod.Day == lr.Day && p.DayPeriod.Period == lr.Period)?.FirstOrDefault()?.DayPeriod?.PeriodName ?? string.Empty;
                            detailInfo.Add($"{weekName}{periodName}");
                        });

                        drCourseLimits.DetailInfo.Add($"科目 { cousreName } 的课表超出同时开课限制 {string.Join(",", detailInfo)}!");
                    }
                }

                if (cl.PeriodLimits != null)
                {
                    cl.PeriodLimits.ForEach(pl => {
                        if (pl.Limit > 0)
                        {
                            var peroidLimit = lessonResultGroup?.FirstOrDefault(rg => rg.Day == pl.DayPeriod.Day && rg.Period == pl.DayPeriod.Period && rg.Count > pl.Limit);
                            if (peroidLimit != null)
                            {
                                string detailInfo = TimeOperation.GetDateInfo(pl.DayPeriod);
                                drCourseLimits.DetailInfo.Add($"科目 { cousreName } 的课表超出同时开课限制 {detailInfo}!");
                            }
                        }
                    });
                }
            });

            drList.Add(drCourseLimits);
            #endregion

            #region 课程排课时间
            RuleAnalysisDetailResult drCourseTimes = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drCourseTimes.RuleName = Miscellaneous.GetLocalDescription(AdministrativeRuleEnum.CourseTime);
            rule?.CourseTimes?.Where(ct => ct.Weight == 1)?.ToList()?.ForEach(ct => {

                List<int> classHours = cPCase?.ClassHours?.Where(ch => ch.CourseID == ct.CourseID && ch.ClassID == ct.ClassID)?.Select(ch => ch.ID)?.ToList() ?? new List<int>();
                string classCourseName = Miscellaneous.GetCPClassCourseInfo(cPCase, ct.ClassID, ct.CourseID);
                int lesson = classCourseMapping?.FirstOrDefault(cc => cc.ID == ct.ClassID && cc.CourseID == ct.CourseID)?.Lessons ?? 0;

                var classhours = from rc in resultModel?.ResultClasses
                                 from rd in rc.ResultDetails
                                 where classHours.Contains(rd.ClassHourId)
                                 select rd.DayPeriod;

                if (ct.ForbidTimes != null)
                {
                    var forbidInterSect = TimeOperation.TimeSlotInterSect(classhours?.ToList(), ct.ForbidTimes);

                    if (forbidInterSect != null && forbidInterSect.Count() > 0)
                    {
                        drCourseTimes.DetailInfo.Add($"课表没有满足 { classCourseName } 的课程禁止规则!");
                    }
                }

                if (ct.MustTimes != null)
                {
                    var mustInterSect = TimeOperation.TimeSlotInterSect(classhours?.ToList(), ct.MustTimes);

                    if (ct.MustTimes.Count >= lesson)
                    {
                        if (mustInterSect.Count() < lesson)
                        {
                            drCourseTimes.DetailInfo.Add($"课表没有满足 { classCourseName } 的课程必须规则!");
                        }
                    }
                    else
                    {
                        if (mustInterSect.Count != ct.MustTimes.Count)
                        {
                            drCourseTimes.DetailInfo.Add($"课表没有满足 { classCourseName } 的课程必须规则!");
                        }
                    }
                }
            });
            drList.Add(drCourseTimes);
            #endregion

            #region 教师半天上课
            RuleAnalysisDetailResult drHalfDayWork = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drHalfDayWork.RuleName = Miscellaneous.GetLocalDescription(AdministrativeRuleEnum.TeacherHalfDayWorkRule);

            rule?.HalfDayWork.Where(hd => hd.Weight == 1)?.ToList()?.ForEach(hd => {
                if (!string.IsNullOrEmpty(hd.TeacherID))
                {
                    List<int> classHours = cPCase?.ClassHours?.Where(ch => ch.TeacherIDs != null && ch.TeacherIDs.Contains(hd.TeacherID))?.Select(ch => ch.ID)?.ToList() ?? new List<int>();
                    string teacherName = cPCase?.Teachers?.FirstOrDefault(t => t.ID == hd.TeacherID)?.Name ?? string.Empty;
                    
                    var classhours = from rc in resultModel?.ResultClasses
                                     from rd in rc.ResultDetails
                                     where classHours.Contains(rd.ClassHourId)
                                     select rd.DayPeriod;

                    //遍历每一天判断是否有违规
                    foreach (int day in Enum.GetValues(typeof(DayOfWeek)))
                    {
                        var oneDayAmTimes = amDayPeriod.Where(ap => ap.Day == (DayOfWeek)day)?.ToList();
                        var oneDayPmTimes = pmDayPeriod.Where(ap => ap.Day == (DayOfWeek)day)?.ToList();

                        var amInterSectTimes = TimeOperation.TimeSlotInterSect(oneDayAmTimes, classhours.ToList());
                        var pmInterSectTimes = TimeOperation.TimeSlotInterSect(oneDayPmTimes, classhours.ToList());

                        if (amInterSectTimes.Count > 0 && pmInterSectTimes.Count > 0)
                        {
                            string weekName = TimeOperation.GetWeekName((DayOfWeek)day);
                            drAmPmNoContinues.DetailInfo.Add($"教师 { teacherName } 的课表在 {weekName} 不是半天上课!");
                        }
                    }
                }
            });
            drList.Add(drHalfDayWork);
            #endregion

            #region 师徒跟随
            RuleAnalysisDetailResult drMasterApprenttices = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drMasterApprenttices.RuleName = Miscellaneous.GetLocalDescription(AdministrativeRuleEnum.MasterApprenttice);

            rule?.MasterApprenttices?.Where(ma => ma.Weight == 1)?.ToList()?.ForEach(ma => {
                //目前师徒跟随逻辑为师徒课时不重复，徒弟之间课时是否重叠不关注

                //教师课表
                List<int> masterClassHours = cPCase?.ClassHours?.Where(ch => ch.TeacherIDs != null && ch.TeacherIDs.Contains(ma.MasterID) && ch.CourseID == ma.CourseID)?.Select(ch => ch.ID)?.ToList() ?? new List<int>();
                string masterName = cPCase?.Teachers?.FirstOrDefault(t => t.ID == ma.MasterID)?.Name ?? string.Empty;

                var masterResult = from rc in resultModel?.ResultClasses
                                 from rd in rc.ResultDetails
                                 where masterClassHours.Contains(rd.ClassHourId)
                                 select rd.DayPeriod;

                ma.ApprenticeIDs?.ForEach(ai => {
                    //学生课表
                    List<int> apprenticeClassHours = cPCase?.ClassHours?.Where(ch => ch.TeacherIDs != null && ch.TeacherIDs.Contains(ai) && ch.CourseID == ma.CourseID)?.Select(ch => ch.ID)?.ToList() ?? new List<int>();
                    string apprenticeName = cPCase?.Teachers?.FirstOrDefault(t => t.ID == ai)?.Name ?? string.Empty;

                    var apprenticeResult = from rc in resultModel?.ResultClasses
                                       from rd in rc.ResultDetails
                                       where apprenticeClassHours.Contains(rd.ClassHourId)
                                       select rd.DayPeriod;

                    //求教师课表与学生课表交集
                    var masterApprenticeInterSect = TimeOperation.TimeSlotInterSect(masterResult?.ToList(), apprenticeResult?.ToList());
                    if (masterApprenticeInterSect.Count > 0)
                    {
                        drMasterApprenttices.DetailInfo.Add($"教师 {masterName} 与 {apprenticeName} 的课表有重叠课位!");
                    }
                });
            });

            drList.Add(drMasterApprenttices);
            #endregion

            #region 教师每周最大工作天数
            RuleAnalysisDetailResult drMaxDaysPerWeek = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drMaxDaysPerWeek.RuleName = Miscellaneous.GetLocalDescription(AdministrativeRuleEnum.TeacherMaxDaysPerWeek);

            rule?.MaxDaysPerWeek?.Where(md => md.Weight == 1)?.ToList()?.ForEach(md => {
                if (!string.IsNullOrEmpty(md.TeacherID))
                {
                    List<int> teacherHours = cPCase?.ClassHours?.Where(ch => ch.TeacherIDs != null && ch.TeacherIDs.Contains(md.TeacherID))?.Select(ch => ch.ID)?.ToList() ?? new List<int>();
                    string teacherName = cPCase?.Teachers?.FirstOrDefault(t => t.ID == md.TeacherID)?.Name ?? string.Empty;

                    var teacherResult = from rc in resultModel?.ResultClasses
                                       from rd in rc.ResultDetails
                                       where teacherHours.Contains(rd.ClassHourId)
                                       select rd.DayPeriod;

                    int lessonUsedDay = teacherResult?.Select(ch => ch.Day)?.Distinct()?.Count() ?? 0;

                    if (lessonUsedDay > md.MaxDay)
                    {
                        drMaxDaysPerWeek.DetailInfo.Add($"教师 {teacherName} 的课表上课天数不应超过 {md.MaxDay} 天，但实际有 {lessonUsedDay} 天安排了课程!");
                    }
                }
            });

            drList.Add(drMaxDaysPerWeek);
            #endregion

            #region 教师每天最大课时间隔
            RuleAnalysisDetailResult drMaxGapsPerDay = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drMaxGapsPerDay.RuleName = Miscellaneous.GetLocalDescription(AdministrativeRuleEnum.TeacherMaxGapsPerDay);
            var periodMappingInfo = Miscellaneous.GetCPCasePeriodMapping(cPCase?.Positions);

            rule?.MaxGapsPerDay?.Where(mg => mg.Weight == 1)?.ToList()?.ForEach(mg => {
                if (!string.IsNullOrEmpty(mg.TeacherID))
                {
                    List<int> teacherHours = cPCase?.ClassHours?.Where(ch => ch.TeacherIDs != null && ch.TeacherIDs.Contains(mg.TeacherID))?.Select(ch => ch.ID)?.ToList() ?? new List<int>();
                    string teacherName = cPCase?.Teachers?.FirstOrDefault(t => t.ID == mg.TeacherID)?.Name ?? string.Empty;

                    var teacherResult = from rc in resultModel?.ResultClasses
                                        from rd in rc.ResultDetails
                                        where teacherHours.Contains(rd.ClassHourId)
                                        select rd.DayPeriod;

                    //遍历每一天判断是否有违规
                    foreach (int day in Enum.GetValues(typeof(DayOfWeek)))
                    {
                        var oneDayLessons = teacherResult.Where(ap => ap.Day == (DayOfWeek)day)?.ToList();
                        var newMappedPeriod = Miscellaneous.GetNewMappedPeriods(periodMappingInfo, oneDayLessons.Select(d => d.Period)?.ToList());
                        int maxGaps = Miscellaneous.MaxGaps(newMappedPeriod);

                        if (maxGaps > mg.MaxIntervel)
                        {
                            string weekName = TimeOperation.GetWeekName((DayOfWeek)day);
                            drMaxGapsPerDay.DetailInfo.Add($"教师 {teacherName} 的课表 在 {weekName} 最大间隔超过 {mg.MaxIntervel}!");
                        }
                    }
                }
            });

            drList.Add(drMaxGapsPerDay);
            #endregion

            #region 教师每天最大课时数
            RuleAnalysisDetailResult drMaxHoursDaily = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drMaxHoursDaily.RuleName = Miscellaneous.GetLocalDescription(AdministrativeRuleEnum.TeacherMaxHoursDaily);

            rule?.MaxHoursDaily?.Where(mh => mh.Weight == 1)?.ToList()?.ForEach(mh => {
                if (!string.IsNullOrEmpty(mh.TeacherID))
                {
                    List<int> teacherHours = cPCase?.ClassHours?.Where(ch => ch.TeacherIDs != null && ch.TeacherIDs.Contains(mh.TeacherID))?.Select(ch => ch.ID)?.ToList() ?? new List<int>();
                    string teacherName = cPCase?.Teachers?.FirstOrDefault(t => t.ID == mh.TeacherID)?.Name ?? string.Empty;

                    var teacherResult = from rc in resultModel?.ResultClasses
                                        from rd in rc.ResultDetails
                                        where teacherHours.Contains(rd.ClassHourId)
                                        select rd.DayPeriod;

                    var maxHours = teacherResult?.Select(ch => new { ch.Day, ch.Period })?
                                   .GroupBy(tr => new { tr.Day })?
                                   .Select(tr => new { tr.Key.Day, Count = tr.Count() });

                    var maxHoursConflict = maxHours?.Where(m => m.Count > mh.MaxHour)?.ToList();

                    if (maxHoursConflict != null && maxHoursConflict.Any())
                    {
                        drMaxHoursDaily.DetailInfo.Add($"教师 {teacherName} 的课表每天最大课时数不应超过 {mh.MaxHour}!");
                    }
                }
            });

            drList.Add(drMaxHoursDaily);
            #endregion

            #region 课程互斥
            RuleAnalysisDetailResult drMutexes = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drMutexes.RuleName = Miscellaneous.GetLocalDescription(AdministrativeRuleEnum.MutexGroup);

            rule?.Mutexes?.Where(mu => mu.Weight == 1)?.ToList()?.ForEach(mu => {

                if (mu.CourseIDs != null)
                {
                    for (int i = 0; i < mu.CourseIDs.Count; i++)
                    {
                        for (int j = i + 1; j < mu.CourseIDs.Count; j++)
                        {
                            string courseOneName = cPCase?.Courses?.FirstOrDefault(t => t.ID == mu.CourseIDs[i])?.Name ?? string.Empty;
                            string courseTwoName = cPCase?.Courses?.FirstOrDefault(t => t.ID == mu.CourseIDs[j])?.Name ?? string.Empty;

                            cPCase?.Classes?.ForEach(cl => {

                                string className = cl.Name ?? string.Empty;
                                List<int> courseOneHours = cPCase?.ClassHours?.Where(ch => ch.ClassID == cl.ID && ch.CourseID == mu.CourseIDs[i])?.Select(ch => ch.ID)?.ToList() ?? new List<int>();
                                List<int> courseTwoHours = cPCase?.ClassHours?.Where(ch => ch.ClassID == cl.ID && ch.CourseID == mu.CourseIDs[j])?.Select(ch => ch.ID)?.ToList() ?? new List<int>();

                                var courseOneResult = (from rc in resultModel?.ResultClasses
                                                   from rd in rc.ResultDetails
                                                   where courseOneHours.Contains(rd.ClassHourId)
                                                   select rd.DayPeriod.Day)?.Distinct();

                                var courseTwoResult = (from rc in resultModel?.ResultClasses
                                                      from rd in rc.ResultDetails
                                                      where courseTwoHours.Contains(rd.ClassHourId)
                                                      select rd.DayPeriod.Day)?.Distinct();

                                //求2种课程上课日期交集
                                if (courseOneResult != null && courseTwoResult != null)
                                {
                                    var courseInterSect = courseOneResult.Intersect(courseTwoResult);
                                    if (courseInterSect.Any())
                                    {
                                        drMutexes.DetailInfo.Add($"班级 {className} 的课程 {courseOneName} 与 {courseTwoName} 在同一天上课!");
                                    }
                                }
                            });
                        }
                    }
                }
            });

            drList.Add(drMutexes);
            #endregion

            #region 单双周
            RuleAnalysisDetailResult drOddDualWeeks = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drOddDualWeeks.RuleName = Miscellaneous.GetLocalDescription(AdministrativeRuleEnum.OddDualWeek);

            rule?.OddDualWeeks?.ForEach(od => {
                string courseOddName = cPCase?.Courses?.FirstOrDefault(t => t.ID == od.OddWeekCourseID)?.Name ?? string.Empty;
                string courseDualName = cPCase?.Courses?.FirstOrDefault(t => t.ID == od.DualWeekCourseID)?.Name ?? string.Empty;
                string className = cPCase?.Classes?.FirstOrDefault(cl => cl.ID == od.ClassID).Name ?? string.Empty;

                List<int> courseOddHours = cPCase?.ClassHours?.Where(ch => ch.ClassID == od.ClassID && ch.CourseID == od.OddWeekCourseID)?.Select(ch => ch.ID)?.ToList() ?? new List<int>();
                List<int> courseDualHours = cPCase?.ClassHours?.Where(ch => ch.ClassID == od.ClassID && ch.CourseID == od.DualWeekCourseID)?.Select(ch => ch.ID)?.ToList() ?? new List<int>();

                var courseOddResult = from rc in resultModel?.ResultClasses
                                       from rd in rc.ResultDetails
                                       where courseOddHours.Contains(rd.ClassHourId)
                                       select rd.DayPeriod;

                var courseDualResult = from rc in resultModel?.ResultClasses
                                       from rd in rc.ResultDetails
                                       where courseDualHours.Contains(rd.ClassHourId)
                                       select rd.DayPeriod;

                //求2种课程上课日期交集
                var courseInterSect = TimeOperation.TimeSlotInterSect(courseOddResult?.ToList(), courseDualResult?.ToList());
                if (courseInterSect.Count() != (courseOddResult?.Count() ?? 0))
                {
                    drOddDualWeeks.DetailInfo.Add($"班级 {className} 的课程 {courseOddName} 与 {courseDualName} 不满足单双周规则!");
                }
            });

            drList.Add(drOddDualWeeks);
            #endregion

            #region 教案齐头
            RuleAnalysisDetailResult drPlanFlushes = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drPlanFlushes.RuleName = Miscellaneous.GetLocalDescription(AdministrativeRuleEnum.TeachingPlanFlush);

            rule?.PlanFlushes?.Where(pf => pf.Weight == 1 && pf.ClassIDs != null && pf.ClassIDs.Count > 1 && !string.IsNullOrEmpty(pf.CourseID))?.ToList()?.ForEach(pf => {
                //课时的源信息
                var courseSource = from x in pf.ClassIDs
                                 from y in cPCase?.ClassHours?.Where(ch => ch.ClassID == x && ch.CourseID == pf.CourseID)
                                 select new { ClassID = x, y.CourseID, y.ID };

                //排课结果信息
                var courseResult = from rc in resultModel?.ResultClasses
                                   from rd in rc.ResultDetails
                                   from cs in courseSource
                                   where cs.ID == rd.ClassHourId
                                   select new { rd.DayPeriod, cs.ID, cs.ClassID };

                //对排课结果进行转换，强制星期日为7，而非0
                var convertedResult = courseResult?.Select(rc => new { Day = (rc.DayPeriod.Day == 0 ? 7 : (int)rc.DayPeriod.Day), rc.ClassID });
                bool resultFlag = true;
                List<string> classCourseNames = new List<string>() { };
                Dictionary<string, int[]> everyClassCourseLesson = new Dictionary<string, int[]> { };

                pf.ClassIDs.ForEach(cl => {
                    if (!string.IsNullOrEmpty(cl))
                    {
                        classCourseNames.Add(Miscellaneous.GetCPClassCourseInfo(cPCase, cl, pf.CourseID));
                        var teacherOneClassLessons = courseResult.Where(tl => tl.ClassID == cl)?.Select(tl => tl.DayPeriod)?.ToList();
                        int[] teacherSingleTimeSlot = Miscellaneous.GetPartTimeTableSlot(timeTableSlot.Item3, teacherOneClassLessons);

                        if (!everyClassCourseLesson.ContainsKey(cl))
                        {
                            everyClassCourseLesson.Add(cl, teacherSingleTimeSlot);
                        }
                    }
                });

                /* 判断教师课表是否教案宽松齐头 */
                resultFlag = Miscellaneous.CalculateTeacherTeachingPlansForSuggest(everyClassCourseLesson, timeTableSlot.Item2);

                /* 如果宽松齐头再判断是否严格指定天内齐头 */
                if (resultFlag)
                {
                    int lesson = classCourseMapping.FirstOrDefault(ma => ma.ID == pf.ClassIDs[0] && ma.CourseID == pf.CourseID)?.Lessons ?? 0;
                    for (int i = 0; i < lesson; i++)
                    {
                        List<int> lessonDays = new List<int>() { };
                        for (int j = 0; j < pf.ClassIDs.Count; j++)
                        {
                            int day = convertedResult.Where(cr => cr.ClassID == pf.ClassIDs[j]).OrderBy(cr => cr.Day).Skip(i).Take(1).FirstOrDefault().Day;
                            lessonDays.Add(day);
                        }

                        int maxDay = Math.Abs(lessonDays.Max() - lessonDays.Min());
                        if (maxDay > pf.FlushDays)
                        {
                            resultFlag = false;
                        }
                    }
                }

                if (!resultFlag)
                {
                    drPlanFlushes.DetailInfo.Add($"{string.Join(",", classCourseNames)} 不满足教案齐头规则！");
                }
            });

            drList.Add(drPlanFlushes);
            #endregion

            #region 教师排课时间
            RuleAnalysisDetailResult drTeacherTimes = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drTeacherTimes.RuleName = Miscellaneous.GetLocalDescription(AdministrativeRuleEnum.TeacherTime);

            rule?.TeacherTimes?.Where(tt => tt.Weight == 1)?.ToList()?.ForEach(tt => {

                List<int> teacherHours = cPCase?.ClassHours?.Where(ch => ch.TeacherIDs != null && ch.TeacherIDs.Contains(tt.TeacherID))?.Select(ch => ch.ID)?.ToList() ?? new List<int>();
                string teacherName = cPCase?.Teachers?.FirstOrDefault(t => t.ID == tt.TeacherID)?.Name ?? string.Empty;

                var teacherResult = from rc in resultModel?.ResultClasses
                                    from rd in rc.ResultDetails
                                    where teacherHours.Contains(rd.ClassHourId)
                                    select rd.DayPeriod;
                
                if (tt.ForbidTimes != null && tt.ForbidTimes.Any())
                {
                    var forbidInterSect = TimeOperation.TimeSlotInterSect(teacherResult?.ToList(), tt.ForbidTimes);

                    if (forbidInterSect != null && forbidInterSect.Any())
                    {
                        drTeacherTimes.DetailInfo.Add($"教师 {teacherName} 的课表打破了教师个人禁止时间规则!");
                    }
                }

                if (tt.MustTimes != null && tt.MustTimes.Any())
                {
                    var mustInterSect = TimeOperation.TimeSlotInterSect(teacherResult?.ToList(), tt.MustTimes);

                    if (tt.MustTimes.Count >= teacherHours.Count)
                    {
                        if (mustInterSect.Count != teacherHours.Count())
                        {
                            drTeacherTimes.DetailInfo.Add($"教师 {teacherName} 的课表打破了教师个人必须时间规则!");
                        }
                    }
                    else
                    {
                        if (mustInterSect.Count != tt.MustTimes.Count)
                        {
                            drTeacherTimes.DetailInfo.Add($"教师 {teacherName} 的课表打破了教师个人必须时间规则!");
                        }
                    }
                }
            });

            drList.Add(drTeacherTimes);
            #endregion

            #region 锁定课表
            RuleAnalysisDetailResult drTimeTableLockedTimes = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drTimeTableLockedTimes.RuleName = Miscellaneous.GetLocalDescription(AdministrativeRuleEnum.LockedCourse);

            rule?.TimeTableLockedTimes?.LockedTimeTable?.ForEach(ltt => {
                
                ltt.LockedCourseTimeTable?.ForEach(lctt => { 

                    List<int> classCourseHours = cPCase?.ClassHours?.Where(ch => ch.ClassID == ltt.ClassID && ch.CourseID == lctt.CourseID)?.Select(ch => ch.ID)?.ToList() ?? new List<int>();

                    var classCourseResult = from rc in resultModel?.ResultClasses
                                            from rd in rc.ResultDetails
                                            where classCourseHours.Contains(rd.ClassHourId)
                                            select rd.DayPeriod;

                    var interSect = TimeOperation.TimeSlotInterSect(lctt.LockedTimes, classCourseResult?.ToList());
                    if (interSect.Count < lctt.LockedTimes.Count)
                    {
                        string classCourseName = Miscellaneous.GetCPClassCourseInfo(cPCase, ltt.ClassID, lctt.CourseID);
                        drTimeTableLockedTimes.DetailInfo.Add($"{classCourseName} 的课表打破了课表锁定规则!");
                    }
                });
            });
            
            drList.Add(drTimeTableLockedTimes);
            #endregion

            return drList;
        }

        /// <summary>
        /// 针对排课结果的一般性评估（与设置规则无关）
        /// </summary>
        /// <param name="resultModel"></param>
        /// <returns></returns>
        public static List<RuleAnalysisDetailResult> GetSuggestResultRuleAnalysis(ResultModel resultModel)
        {
            List<RuleAnalysisDetailResult> drList = new List<RuleAnalysisDetailResult>() { };
            #region Common
            //方案可用课位
            List<DayPeriodModel> caseAvailableDayPeriod = resultModel?.Positions?.Where(p => p.IsSelected
                                      && p.Position != XYKernel.OS.Common.Enums.Position.AB
                                      && p.Position != XYKernel.OS.Common.Enums.Position.Noon
                                      && p.Position != XYKernel.OS.Common.Enums.Position.PB)?.Select(x => x.DayPeriod)?.ToList() ?? new List<DayPeriodModel>();

            //方案可用天数
            int availableDays = caseAvailableDayPeriod.Select(dp => dp.Day).Distinct().Count();

            //课位映射
            var periodMappingInfo = Miscellaneous.GetCPCasePeriodMapping(resultModel?.Positions?.ToList());

            //最大间隔3
            int maxGapsThree = 3;

            //最大连续课时3
            int maxContinousThree = 3;

            //所有排课结果课时（变形数据）
            var allLessons = resultModel?.ResultClasses?.SelectMany(rc => rc.ResultDetails);

            //方案上午可用课位
            List<DayPeriodModel> caseAMAvailableDayPeriod = resultModel?.Positions?.Where(p => p.IsSelected
                                      && p.Position == XYKernel.OS.Common.Enums.Position.AM)?.Select(x => x.DayPeriod)?.ToList() ?? new List<DayPeriodModel>();

            //方案下午可用课位
            List<DayPeriodModel> casePMAvailableDayPeriod = resultModel?.Positions?.Where(p => p.IsSelected
                                      && p.Position == XYKernel.OS.Common.Enums.Position.PM)?.Select(x => x.DayPeriod)?.ToList() ?? new List<DayPeriodModel>();

            //全局课表位置信息
            var allCaseDayPeriods = resultModel?.Positions?.Where(p => p.Position != XYKernel.OS.Common.Enums.Position.AB
                                                && p.Position != XYKernel.OS.Common.Enums.Position.PB
                                                && p.Position != XYKernel.OS.Common.Enums.Position.Noon
                                                )?.Select(p => p.DayPeriod)?.ToList();

            Tuple<int[], int[], int[]> timeTableSlot = Miscellaneous.GetGlobalTimeTableSlot(allCaseDayPeriods);

            int[] globalWeekDays = timeTableSlot.Item1;
            int[] globalDayPeriods = timeTableSlot.Item2;
            int[] globalTimeTableSlot = timeTableSlot.Item3;
            #endregion

            #region 课时分散
            RuleAnalysisDetailResult drClassHourAverages = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drClassHourAverages.RuleName = "课时分散";

            resultModel?.Classes?.ToList()?.ForEach(cl => {
                var classCourse = resultModel.ResultClasses?.Where(rc => rc.ClassID == cl.ID);
                string className = cl.Name ?? string.Empty;
                var classCourseID = classCourse?.SelectMany(cc => cc.ResultDetails)?.Select(rd => rd.CourseID)?.Distinct()?.ToList();

                classCourseID?.ForEach(co => {
                    string courseName = resultModel?.Courses?.FirstOrDefault(c => c.ID == co)?.Name ?? string.Empty;
                    var singleClassLessons = classCourse?.FirstOrDefault(c => c.ClassID == cl.ID)?.ResultDetails?.Where(rd => rd.CourseID == co)?.Select(rd => rd.DayPeriod)?.ToList();
                    int[] singleTimeSlot = Miscellaneous.GetPartTimeTableSlot(timeTableSlot.Item3, singleClassLessons);
                    bool resultFlag = Miscellaneous.CalculateLessonsEvenlyDistributedForSuggest(singleTimeSlot, globalDayPeriods);

                    if (!resultFlag)
                    {
                        drClassHourAverages.DetailInfo.Add($"{ className } {courseName} 的课表不满足课时分散!");
                    }
                });
            });

            drList.Add(drClassHourAverages);
            #endregion

            #region 音乐、体育、美术不排在上午前两节
            RuleAnalysisDetailResult drMusicPEArtFirstTwoLessons = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drMusicPEArtFirstTwoLessons.RuleName = "音乐、体育、美术不排在上午前两节";

            resultModel?.Classes?.ToList()?.ForEach(cl => {
                var classCourse = resultModel.ResultClasses?.Where(rc => rc.ClassID == cl.ID);
                string className = cl.Name ?? string.Empty;
                var classCourseID = classCourse?.SelectMany(cc => cc.ResultDetails)?.Select(rd => rd.CourseID)?.Distinct()?.ToList();
                List<string> musicPEArtID = new List<string>();
                List<string> musicPEArtName = new List<string>();

                classCourseID?.ForEach(co => {
                    string courseName = resultModel?.Courses?.FirstOrDefault(c => c.ID == co)?.Name ?? string.Empty;
                    if (courseName == "音乐" || courseName == "体育" || courseName == "美术")
                    {
                        musicPEArtID.Add(co);
                        musicPEArtName.Add(courseName);
                    }
                });

                if (musicPEArtID.Count > 0)
                {
                    musicPEArtName = musicPEArtName.OrderBy(m => m).ToList();

                    var classCourseLesson = classCourse?.FirstOrDefault(c => c.ClassID == cl.ID)?.ResultDetails?.Where(rd => musicPEArtID.Contains(rd.CourseID));
                    var classCourseDay = classCourseLesson?.Select(rd => rd.DayPeriod)?.ToList();
                    var amPostions = resultModel?.Positions?.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.AM)?.Select(p => p.PositionOrder)?.Distinct();

                    if (amPostions != null && amPostions.Count() > 1)
                    {
                        var firstTwoPositon = amPostions.OrderBy(p => p).Take(2);
                        var amFirstTwo = resultModel.Positions.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.AM && firstTwoPositon.Contains(p.PositionOrder)).Select(p => p.DayPeriod);

                        var timeInterSect = TimeOperation.TimeSlotInterSect(amFirstTwo?.ToList(), classCourseDay);
                        if (timeInterSect != null && timeInterSect.Count() > 0)
                        {
                            drMusicPEArtFirstTwoLessons.DetailInfo.Add($"{ className } {string.Join(",", musicPEArtName)} 的课表占用了上午前2节课!");
                        }
                    }
                }
            });

            drList.Add(drMusicPEArtFirstTwoLessons);
            #endregion

            #region 音乐、体育、美术尽量分散
            RuleAnalysisDetailResult drMusicPEArtMutex = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drMusicPEArtMutex.RuleName = "音乐、体育、美术尽量分散";

            resultModel?.Classes?.ToList()?.ForEach(cl => {
                var classCourse = resultModel.ResultClasses?.Where(rc => rc.ClassID == cl.ID);
                string className = cl.Name ?? string.Empty;
                var classCourseID = classCourse?.SelectMany(cc => cc.ResultDetails)?.Select(rd => rd.CourseID)?.Distinct()?.ToList();
                List<string> musicPEArtID = new List<string>();
                List<string> musicPEArtName = new List<string>();

                classCourseID?.ForEach(co => {
                    string courseName = resultModel?.Courses?.FirstOrDefault(c => c.ID == co)?.Name ?? string.Empty;
                    if (courseName == "音乐" || courseName == "体育" || courseName == "美术")
                    {
                        musicPEArtID.Add(co);
                        musicPEArtName.Add(courseName);
                    }
                });

                if (musicPEArtID.Count > 0)
                {
                    musicPEArtName = musicPEArtName.OrderBy(m => m).ToList();

                    var classCourseLesson = classCourse?.FirstOrDefault(c => c.ClassID == cl.ID)?.ResultDetails?.Where(rd => musicPEArtID.Contains(rd.CourseID));
                    var classCourseDay = classCourseLesson?.Select(rd => rd.DayPeriod)?.ToList();

                    var dayGroup = classCourseDay?.Select(cc => new { cc.Day })?.ToList()?
                             .GroupBy(g => g.Day)?
                             .Select(s => new { Day = s , Count = s.Count()});

                    if (dayGroup != null && dayGroup.ToList().Exists(g => g.Count > 1))
                    { 
                        drMusicPEArtMutex.DetailInfo.Add($"{ className } {string.Join(",", musicPEArtName)} 有一天多节的情况!");
                    }
                }
            });

            drList.Add(drMusicPEArtMutex);
            #endregion

            #region 2、3课时科目尽量分散
            RuleAnalysisDetailResult drDisperseAMAP = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drDisperseAMAP.RuleName = "2、3课时科目尽量分散";

            resultModel?.Classes?.ToList()?.ForEach(cl => {
                var classCourse = resultModel.ResultClasses?.Where(rc => rc.ClassID == cl.ID);
                string className = cl.Name ?? string.Empty;
                /* 查找2/3课时课程 */
                var classCourseID = classCourse?.SelectMany(cc => cc.ResultDetails)?.GroupBy(rd => rd.CourseID)?.Where(rd => rd.Count() <= 3 && rd.Count() > 1)?.Select(rd => rd.Key)?.Distinct()?.ToList();
                List<string> lessLessonCourseID = new List<string>();

                if (classCourseID != null)
                {
                    classCourseID.ForEach(cc => {
                        bool resultFlag = true;
                        string courseName = resultModel?.Courses?.FirstOrDefault(c => c.ID == cc)?.Name ?? string.Empty;
                        var classCourseLesson = classCourse?.FirstOrDefault(c => c.ClassID == cl.ID)?.ResultDetails?.Where(rd => rd.CourseID == cc);
                        int lesson = classCourseLesson.Count();

                        var classCourseDay = classCourseLesson.Select(rd => (int) rd.DayPeriod.Day).Distinct().ToList();
                        classCourseDay = classCourseDay?.Select(cd => cd == 0 ? 7 : cd)?.ToList() ?? new List<int> { };/* 避免周六日被判断为非连续 */

                        if (lesson > classCourseDay.Count)
                        {
                            /*存在一天多节课*/
                            resultFlag = false;
                        }
                        else
                        {
                            /* 各天紧密连在一起 */
                            int maxGaps = Miscellaneous.MaxGaps(classCourseDay);
                            if (maxGaps == 0)
                            {
                                resultFlag = false;
                            }
                        }
                  
                        if (!resultFlag)
                        {
                            drDisperseAMAP.DetailInfo.Add($"{ className } {courseName} 课时应尽量分散!");
                        }
                    });
                }
            });

            drList.Add(drDisperseAMAP);
            #endregion

            #region 教师课程间隔不超过3节/教师连续上课不超过3节
            RuleAnalysisDetailResult drTeacherMax3Gaps = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drTeacherMax3Gaps.RuleName = "教师课程间隔不超过3节";
            RuleAnalysisDetailResult drTeacherMax3Continous = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drTeacherMax3Continous.RuleName = "教师连续上课不超过3节";

            resultModel?.Teachers?.ToList()?.ForEach(te => {
                var teacherLessons = allLessons?.Where(tl => tl.Teachers != null && tl.Teachers.Contains(te.ID))?.Select(tl => new { tl.DayPeriod.Day, tl.DayPeriod.Period });

                //遍历每一天判断是否有违规
                foreach (int day in Enum.GetValues(typeof(DayOfWeek)))
                {
                    var dayLessons = teacherLessons.Where(ap => ap.Day == (DayOfWeek)day)?.ToList();
                    var newMappedPeriod = Miscellaneous.GetNewMappedPeriods(periodMappingInfo, dayLessons.Select(d => d.Period)?.ToList());
                    string weekName = TimeOperation.GetWeekName((DayOfWeek)day);
                    int maxGaps = Miscellaneous.MaxGaps(newMappedPeriod);
                    int maxContinous = Miscellaneous.MaxContinous(newMappedPeriod);

                    if (maxGaps > maxGapsThree)
                    {
                        drTeacherMax3Gaps.DetailInfo.Add($"教师 {te.Name} 的课表在 {weekName} 最大间隔超过 {maxGapsThree}!");
                    }

                    if (maxContinous > maxContinousThree)
                    {
                        drTeacherMax3Continous.DetailInfo.Add($"教师 {te.Name} 的课表在 {weekName} 最大连续课时超过 {maxContinousThree}!");
                    }
                }
            });

            drList.Add(drTeacherMax3Gaps);
            drList.Add(drTeacherMax3Continous);
            #endregion

            #region 各科目下午课时占比均衡（下午比上午多-不考虑早晚自己-大于2课时，且上午可用不大于下午总课时)
            RuleAnalysisDetailResult drTooMuchLessonInPM = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drTooMuchLessonInPM.RuleName = "各科目下午课时占比均衡";

            int aMAvailableNumber = caseAMAvailableDayPeriod?.Count ?? 0;
            int pMAvailableNumber = casePMAvailableDayPeriod?.Count ?? 0;

            if (aMAvailableNumber > 1 && pMAvailableNumber > 1)
            {
                resultModel?.Classes?.ToList()?.ForEach(cl => {
                    var classCourse = resultModel.ResultClasses?.Where(rc => rc.ClassID == cl.ID);
                    string className = cl.Name ?? string.Empty;
                    var classCourseID = classCourse?.SelectMany(cc => cc.ResultDetails)?.Select(rd => rd.CourseID)?.Distinct()?.ToList();

                    classCourseID?.ForEach(co => {
                        string courseName = resultModel?.Courses?.FirstOrDefault(c => c.ID == co)?.Name ?? string.Empty;
                        var classCourseLesson = classCourse?.FirstOrDefault(c => c.ClassID == cl.ID)?.ResultDetails?.Where(rd => rd.CourseID == co)?.Select(rd => rd.DayPeriod)?.ToList();
                        var amCourseResult = TimeOperation.TimeSlotInterSect(classCourseLesson, caseAMAvailableDayPeriod);
                        var pmCourseResult = TimeOperation.TimeSlotInterSect(classCourseLesson, casePMAvailableDayPeriod);

                        //如果课程下午课时大于1，并且方案下午可用课时大于1（排出极端情况下的无意义计算）
                        if (pmCourseResult.Count > 1)
                        {
                            float amLessonRatio = (float)amCourseResult.Count / aMAvailableNumber;
                            float pmLessonRatio = (float)pmCourseResult.Count / pMAvailableNumber;
                            //初步定义下午上课率是上午2倍以上给出修改建议
                            if (amLessonRatio * 2 < pmLessonRatio)
                            {
                                drTooMuchLessonInPM.DetailInfo.Add($"{ className } {courseName} 的下午课时安排过多!");
                            }
                        }
                    });
                });
            }

            drList.Add(drTooMuchLessonInPM);
            #endregion

            #region 教师重点课位占比均衡
            RuleAnalysisDetailResult drTeacherLimitInImportantSlot = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drTeacherLimitInImportantSlot.RuleName = "教师重点课位占比均衡";

            /* 上午第一节，上午最后一节，下午第一节 */
            List<int> limitAmPositionValue = resultModel?.Positions.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.AM).Select(p => p.PositionOrder).Distinct().ToList();
            List<int> limitPmPositionValue = resultModel?.Positions.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.PM).Select(p => p.PositionOrder).Distinct().ToList();
            limitAmPositionValue = limitAmPositionValue.OrderBy(am => am).ToList();
            limitPmPositionValue = limitPmPositionValue.OrderBy(pm => pm).ToList();
            List<DayPeriodModel> specialTimes = new List<DayPeriodModel>();

            //上午第一节
            if (limitAmPositionValue.Count > 0)
            {
                resultModel?.Positions.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.AM && p.PositionOrder == limitAmPositionValue[0]).ToList().ForEach(p => {
                    specialTimes.Add(p.DayPeriod);
                });
            }
            //上午最后一节
            if (limitAmPositionValue.Count > 1)
            {
                resultModel?.Positions.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.AM && p.PositionOrder == limitAmPositionValue[limitAmPositionValue.Count - 1]).ToList().ForEach(p => {
                    specialTimes.Add(p.DayPeriod);
                });
            }
            //下午第一节
            if (limitPmPositionValue.Count > 0)
            {
                resultModel?.Positions.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.PM && p.PositionOrder == limitPmPositionValue[0]).ToList().ForEach(p => {
                    specialTimes.Add(p.DayPeriod);
                });
            }
            //移除不可用课位
            specialTimes = TimeOperation.TimeSlotInterSect(specialTimes, caseAvailableDayPeriod);
            
            if (specialTimes.Count > 1)
            {
                resultModel?.Teachers?.ToList()?.ForEach(te =>
                {
                    string teacherName = resultModel?.Teachers?.FirstOrDefault(t => t.ID == te.ID)?.Name ?? string.Empty;
                    var teacherLessons = allLessons?.Where(tl => tl.Teachers != null && tl.Teachers.Contains(te.ID))?.Select(tl => tl.DayPeriod);
                    List<DayPeriodModel> teacherSpecialTimes = new List<DayPeriodModel>();
                    if (teacherLessons!= null && teacherLessons.Count() > 1)
                    {
                        teacherSpecialTimes = TimeOperation.TimeSlotInterSect(teacherLessons?.ToList(), specialTimes);
                        //如果课程下午课时大于1，并且方案下午可用课时大于1（排出极端情况下的无意义计算）
                        if (teacherSpecialTimes.Count > 1)
                        {
                            float teacherSpecialLessonRatio = (float)teacherSpecialTimes.Count / teacherLessons.Count();
                            float spcialLessonRatio = (float)specialTimes.Count / caseAvailableDayPeriod.Count;
                            //初步定义教师特殊课位课时占比是特殊课位与总可用课位占比的1.5倍以上给出修改建议
                            if (spcialLessonRatio < teacherSpecialLessonRatio 
                                && spcialLessonRatio * 1.5 < teacherSpecialLessonRatio)
                            {
                                drTeacherLimitInImportantSlot.DetailInfo.Add($"教师 { teacherName } 的特殊课位课时安排过多!");
                            }
                        }
                    }
                });
            }

            drList.Add(drTeacherLimitInImportantSlot);
            #endregion

            #region 教案齐头(宽松)/教案平头
            RuleAnalysisDetailResult drTeacherTeachingPlanNoCross = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drTeacherTeachingPlanNoCross.RuleName = "教案齐头";

            RuleAnalysisDetailResult drTeacherLessonsBalancePriority = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drTeacherLessonsBalancePriority.RuleName = "教案平头";

            resultModel?.Teachers?.ToList()?.ForEach(te =>
            {
                string teacherName = resultModel?.Teachers?.FirstOrDefault(t => t.ID == te.ID)?.Name ?? string.Empty;

                //教师全部课程
                var teacherLessons = from rc in resultModel?.ResultClasses
                                     from rd in rc.ResultDetails
                                     where rd.Teachers != null && rd.Teachers.Contains(te.ID)
                                     select new { rc.ClassID, rd.CourseID, rd.DayPeriod, rd.Teachers };

                //理论上一名老师可以教多个班多个科，只有不同班的相同科目教案齐头才有意义
                var teacherLessonsGroup = teacherLessons?.GroupBy(tl => new { tl.ClassID, tl.CourseID })
                                         ?.Select(tl => new { tl.Key.ClassID, tl.Key.CourseID, LessonCount = tl.Count() });

                //教师教授多个同科班级时，按照不同课时分组
                var teacherSameLessonsGroup = teacherLessonsGroup?.Where(tl => tl.LessonCount > 1)?.GroupBy(tl => new { tl.ClassID, tl.CourseID, tl.LessonCount })
                                                ?.Select(tl => new { tl.Key.ClassID, tl.Key.CourseID, tl.Key.LessonCount });

                var teacherCourseLessonGroup = teacherSameLessonsGroup?.GroupBy(tl => new { tl.CourseID, tl.LessonCount })?
                                                .Select(sl => new { sl.Key.CourseID, sl.Key.LessonCount, Count = sl.Count() })?.ToList();

                //遍历一个教师多个班相同科目相同课时情况下检查齐头
                teacherCourseLessonGroup?.Where(tcl => tcl.Count > 1)?.ToList().ForEach(tcl => {
                    string courseName = resultModel?.Courses?.FirstOrDefault(co => co.ID == tcl.CourseID)?.Name ?? string.Empty;

                    /* Start: 教案齐头 */
                    /* 构造一个教师课位信息 */
                    Dictionary<string, int[]> everyClassCourseLesson = new Dictionary<string, int[]> { };

                    teacherSameLessonsGroup?.Where(sl => sl.LessonCount > 1 && sl.CourseID == tcl.CourseID && sl.LessonCount == tcl.LessonCount)?.ToList()?.ForEach(sl => {
                        var teacherOneClassLessonsTemp = teacherLessons.Where(tl => tl.ClassID == sl.ClassID && tl.CourseID == sl.CourseID)?.Select(tl => tl.DayPeriod)?.ToList();
                        int[] teacherSingleTimeSlot = Miscellaneous.GetPartTimeTableSlot(timeTableSlot.Item3, teacherOneClassLessonsTemp);

                        if (!everyClassCourseLesson.ContainsKey(sl.ClassID))
                        {
                            everyClassCourseLesson.Add(sl.ClassID, teacherSingleTimeSlot);
                        }
                    });

                    /* 判断教师课表是否教案齐头 */
                    bool resultFlag = Miscellaneous.CalculateTeacherTeachingPlansForSuggest(everyClassCourseLesson, globalDayPeriods);
                    
                    if (!resultFlag)
                    {
                        drTeacherTeachingPlanNoCross.DetailInfo.Add($"教师 { teacherName } {courseName} 的课表教案不齐头!");
                    }
                    /* End: 教案齐头 */

                    /* Start: 教案平头 */
                    var teacherTempData = from sl in teacherSameLessonsGroup
                                            from tl in teacherLessons
                                            where sl.LessonCount > 1 && sl.CourseID == tcl.CourseID && sl.LessonCount == tcl.LessonCount
                                                && tl.ClassID == sl.ClassID && tl.CourseID == sl.CourseID
                                            select new { Day = tl.DayPeriod.Day == 0 ? 7 : (int)tl.DayPeriod.Day, tl.DayPeriod.Period, sl.ClassID, sl.LessonCount };

                    //统计各班优先次数
                    teacherTempData = teacherTempData?.OrderBy(t => t.Day)?.ThenBy(t => t.Period);
                    int teachingPlanLesson = teacherTempData?.FirstOrDefault()?.LessonCount ?? 0;
                    string[] classIDs = teacherTempData?.Select(t => t.ClassID)?.Distinct()?.ToArray() ?? new string[] { };
                    int[] classIDPriority = new int [classIDs.Length];
                    int lessonCount = teacherTempData?.Count() ?? 0;

                    if (teachingPlanLesson > 1 && classIDs.Length > 1 && lessonCount == teachingPlanLesson * classIDs.Length)
                    {
                        for (int i = 0; i < teachingPlanLesson; i++)
                        {
                            string tempClassID = string.Empty;
                            int iIndexDP = 1000;

                            for (int j = 0; j < classIDs.Length; j++)
                            {
                                var lessonInfo = teacherTempData.Where(td => td.ClassID == classIDs[j])
                                                                .OrderBy(od => od.Day)
                                                                .ThenBy(od => od.Period)
                                                                .Skip(i).Take(1).FirstOrDefault();

                                int classDayPeriodIndex = Array.IndexOf(timeTableSlot.Item3, lessonInfo.Day * 100 + lessonInfo.Period);

                                if (classDayPeriodIndex < iIndexDP)
                                {
                                    tempClassID = classIDs[j];
                                    iIndexDP = classDayPeriodIndex;
                                }
                            }

                            int iIndex = Array.IndexOf(classIDs, tempClassID);
                            classIDPriority[iIndex] += 1;
                        }

                        float meanValue = (float)teachingPlanLesson / classIDs.Length;

                        for (int i = 0; i < classIDPriority.Length; i++)
                        {
                            if (Math.Abs(classIDPriority[i] - meanValue) >= 1.0)
                            {
                                drTeacherLessonsBalancePriority.DetailInfo.Add($"教师 { teacherName } {courseName} 的课表教案不平头!");
                                break;
                            }
                        }
                    }
                    /* End: 教案平头 */
                });
            });

            drList.Add(drTeacherTeachingPlanNoCross);
            drList.Add(drTeacherLessonsBalancePriority);
            #endregion

            return drList;
        }
    }
}
