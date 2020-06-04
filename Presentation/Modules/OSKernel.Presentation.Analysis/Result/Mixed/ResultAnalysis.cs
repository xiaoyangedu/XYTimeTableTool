using System;
using System.Linq;
using System.Collections.Generic;
using XYKernel.OS.Common.Models;
using XYKernel.OS.Common.Models.Mixed;
using XYKernel.OS.Common.Models.Mixed.Rule;
using XYKernel.OS.Common.Models.Mixed.AlgoRule;
using XYKernel.OS.Common.Models.Mixed.Result;
using OSKernel.Presentation.Analysis.Result.Models;
using OSKernel.Presentation.Models.Enums;
using OSKernel.Presentation.Analysis.Utilities;

namespace OSKernel.Presentation.Analysis.Result.Mixed
{
    public class ResultAnalysis
    {
        /// <summary>
        /// 排课结果规则匹配度检测
        /// </summary>
        /// <param name="cLCase"></param>
        /// <param name="rule"></param>
        /// <param name="algoRule"></param>
        /// <param name="resultModel"></param>
        /// <returns></returns>
        public static List<RuleAnalysisDetailResult> GetResultRuleFitDegreeAnalysis(CLCase cLCase, Rule rule, AlgoRule algoRule, ResultModel resultModel)
        {
            #region 公共变量
            List<RuleAnalysisDetailResult> drList = new List<RuleAnalysisDetailResult>() { };
            List<DayPeriodModel> amDayPeriod = cLCase?.Positions?.Where(p => p.IsSelected && (p.Position == XYKernel.OS.Common.Enums.Position.AM || p.Position == XYKernel.OS.Common.Enums.Position.MS))?
                                                .Select(p => p.DayPeriod)?.ToList() ?? new List<DayPeriodModel>() { };

            List<DayPeriodModel> pmDayPeriod = cLCase?.Positions?.Where(p => p.IsSelected && (p.Position == XYKernel.OS.Common.Enums.Position.PM || p.Position == XYKernel.OS.Common.Enums.Position.ES))?
                                                .Select(p => p.DayPeriod)?.ToList() ?? new List<DayPeriodModel>() { };

            //班级课程课时对应关系
            var classCourseMapping = from x in cLCase?.Classes
                                     from y in cLCase?.Courses
                                     from z in y.Levels
                                     where x.CourseID == y.ID && x.LevelID == z.ID
                                     select new { x.ID, x.CourseID, x.LevelID, z.Lessons };

            //排课结果
            var allResultLessons = from rc in resultModel?.ResultClasses
                                   from rd in rc.ResultDetails
                                   select new { rc.ClassID, rd.DayPeriod, rd.Teachers };

            //全局课表位置信息
            var allCaseDayPeriods = resultModel?.Positions?.Where(p => p.Position != XYKernel.OS.Common.Enums.Position.AB
                                                && p.Position != XYKernel.OS.Common.Enums.Position.PB
                                                && p.Position != XYKernel.OS.Common.Enums.Position.Noon
                                                )?.Select(p => p.DayPeriod)?.ToList();

            //获取上下午大课间和午休关联的课位信息
            List<DayPeriodModel> aMBreakPreviousNextTimes = new List<DayPeriodModel>();
            List<DayPeriodModel> pMBreakPreviousNextTimes = new List<DayPeriodModel>();
            List<DayPeriodModel> noonBreakPreviousNextTimes = new List<DayPeriodModel>();
            Tuple<List<DayPeriodModel>, List<DayPeriodModel>, List<DayPeriodModel>> aMPMNoonBreakTimes = Miscellaneous.GetCLAMPMNoonBreak(cLCase?.Positions);
            aMBreakPreviousNextTimes = aMPMNoonBreakTimes.Item1;
            noonBreakPreviousNextTimes = aMPMNoonBreakTimes.Item2;
            pMBreakPreviousNextTimes = aMPMNoonBreakTimes.Item3;
            #endregion

            #region 上下午课时
            RuleAnalysisDetailResult drAmPmClassHours = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drAmPmClassHours.RuleName = Miscellaneous.GetLocalDescription(MixedRuleEnum.AmPmClassHour);

            rule?.AmPmClassHours?.Where(ap => ap.Weight == 1)?.ToList()?.ForEach(ap => {

                string classCourseName = Miscellaneous.GetCLClassCourseInfo(cLCase, ap.ClassID);
                var courseClassInfo = resultModel?.ResultClasses?.Where(rc => rc.ClassID == ap.ClassID)?
                .FirstOrDefault()?.ResultDetails?
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
            
            #region 课时分散
            RuleAnalysisDetailResult drClassHourAverages = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drClassHourAverages.RuleName = Miscellaneous.GetLocalDescription(MixedRuleEnum.ClassHourAverage);

            rule?.ClassHourAverages?.Where(cha => cha.Weight == 1)?.ToList()?.ForEach(cha => {
                int courseContinouCount = rule?.ArrangeContinuous?.Where(ca => ca.ClassID == cha.ClassID)?.FirstOrDefault()?.Count ?? 0;
                int lesson = classCourseMapping?.FirstOrDefault(cc => cc.ID == cha.ClassID)?.Lessons ?? 0;
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

                List<int> classHours = cLCase?.ClassHours?.Where(ch => ch.ClassID == cha.ClassID)?.Select(ch => ch.ID)?.ToList() ?? new List<int>();

                var classhours = from rc in resultModel?.ResultClasses
                                 from rd in rc.ResultDetails
                                 where classHours.Contains(rd.ClassHourId)
                                 select rd.DayPeriod;

                bool dayEnough = Miscellaneous.DaysEnoughForEvenlyDistributed(classhours?.ToList(), needDays, cha.MinDay);

                if (!dayEnough)
                {
                    string classCourseName = Miscellaneous.GetCLClassCourseInfo(cLCase, cha.ClassID);
                    drClassHourAverages.DetailInfo.Add($"{ classCourseName } 的课表不满足最小间隔为 {cha.MinDay} 天的课时分散!");
                }
            });

            drList.Add(drClassHourAverages);
            #endregion

            #region 同时开课
            RuleAnalysisDetailResult drClassHourSameOpens = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drClassHourSameOpens.RuleName = Miscellaneous.GetLocalDescription(MixedRuleEnum.ClassHourSameOpen);

            rule?.ClassHourSameOpens?.Where(so => so.Weight == 1)?.ToList()?.ForEach(so => {

                List<string> ccmList = new List<string>();

                //获取全部教学班级
                so.Details.ForEach(de => {
                    de.Classes?.ForEach(cl => {
                        if (!ccmList.Exists(cc => cc == cl))
                        {
                            ccmList.Add(cl);
                        }
                    });
                });

                if (ccmList.Any())
                {
                    //获取全部班级课时ID
                    List<int> sameOpenRelatedLessons = new List<int>();

                    ccmList.ForEach(cc => {
                        List<int> singleClassLesson = cLCase?.ClassHours?.Where(ch => ch.ClassID == cc)?.Select(ch => ch.ID)?.ToList() ?? new List<int>();
                        sameOpenRelatedLessons.AddRange(singleClassLesson);
                    });

                    sameOpenRelatedLessons = sameOpenRelatedLessons.Distinct().ToList();

                    //获取全部上课时间
                    var lessonResult = from rc in resultModel?.ResultClasses
                                       from rd in rc.ResultDetails
                                       where sameOpenRelatedLessons.Contains(rd.ClassHourId)
                                       select new { rd.DayPeriod, rc.ClassID };

                    var lessonResultGroup = lessonResult.GroupBy(lr => new { lr.DayPeriod.Day, lr.DayPeriod.Period })
                                            .Select(g => new { g.Key.Day, g.Key.Period, Count = g.Count() });

                    var lessonResultGroupDescending = lessonResultGroup.OrderByDescending(g => g.Count);

                    //班级课程上课时间分组
                    ClassHourSameOpenRule chsoResultRule = new ClassHourSameOpenRule() { Details = new List<SameOpenDetailsModel> { } };

                    lessonResultGroupDescending?.ToList()?.ForEach(g => {

                        SameOpenDetailsModel sodmResult = new SameOpenDetailsModel() { Classes = new List<string>() { } };

                        List<string> courseClassResult = new List<string>() { };
                        var lessonResultPerSlot = lessonResult.Where(lr => lr.DayPeriod.Day == g.Day && lr.DayPeriod.Period == g.Period);

                        lessonResultPerSlot?.ToList()?.ForEach(lrp => {
                            if (!courseClassResult.Exists(cc => cc == lrp.ClassID))
                            {
                                courseClassResult.Add(lrp.ClassID);
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
                        int lesson = classCourseMapping.FirstOrDefault(ccm => ccm.ID == cc)?.Lessons ?? 0;
                        classCourseLesson += lesson;
                    });
                    //获取班级科目名称
                    List<string> classCourseNameList = new List<string>();
                    ccmList.ForEach(cc => {
                        classCourseNameList.Add(Miscellaneous.GetCLClassCourseInfo(cLCase, cc));
                    });
                    //仅检查所有课时都设定同时开课的情况
                    if (classCourseLesson == sourceRuleLesson)
                    {
                        int resultRuleLesson = chsoResultRule.Details.Take(tempLesson).SelectMany(de => de.Classes).Count();
                        if (resultRuleLesson < sourceRuleLesson)
                        {
                            checkResult = false;
                        }

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

            #region 课程连排
            RuleAnalysisDetailResult drCourseArranges = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drCourseArranges.RuleName = Miscellaneous.GetLocalDescription(MixedRuleEnum.CourseArrangeContinuous);

            rule?.ArrangeContinuous?.ForEach(ca => {
                //1. 计算连排数
                var classCourseLesson = allResultLessons?.Where(rl => rl.ClassID == ca.ClassID);
                var lessons = classCourseLesson?.Select(cl => cl.DayPeriod)?.ToList();
                Tuple<int[], int[], int[]> timeTableSlot = Miscellaneous.GetGlobalTimeTableSlot(allCaseDayPeriods);
                int[] classCourseTimeTableSlot = Miscellaneous.GetPartTimeTableSlot(timeTableSlot.Item3, lessons);
                Tuple<int, List<int>> classCourseContinousInfo = Miscellaneous.ComputeContinousNumberByTimeTable(classCourseTimeTableSlot, timeTableSlot.Item1, timeTableSlot.Item2);
                int continousNumber = classCourseContinousInfo.Item1;
                string classCourseName = Miscellaneous.GetCLClassCourseInfo(cLCase, ca.ClassID);

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
            drCourseLimits.RuleName = Miscellaneous.GetLocalDescription(MixedRuleEnum.CourseLimit);

            rule?.CourseLimits?.Where(cl => cl.Weight == 1)?.ToList()?.ForEach(cl => {

                string cousreName = cLCase?.Courses?.FirstOrDefault(co => co.ID == cl.CourseID)?.Name ?? string.Empty;
                List<int> classHours = cLCase?.ClassHours?.Where(ch => ch.CourseID == cl.CourseID)?.Select(ch => ch.ID)?.ToList() ?? new List<int>();

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
                            string periodName = cLCase.Positions.Where(p => p.DayPeriod.Day == lr.Day && p.DayPeriod.Period == lr.Period)?.FirstOrDefault()?.DayPeriod?.PeriodName ?? string.Empty;
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
            drCourseTimes.RuleName = Miscellaneous.GetLocalDescription(MixedRuleEnum.CourseTime);
            rule?.CourseTimes?.Where(ct => ct.Weight == 1)?.ToList()?.ForEach(ct => {

                List<int> classHours = cLCase?.ClassHours?.Where(ch => ch.ClassID == ct.ClassID)?.Select(ch => ch.ID)?.ToList() ?? new List<int>();
                string classCourseName = Miscellaneous.GetCLClassCourseInfo(cLCase, ct.ClassID);
                int lesson = classCourseMapping?.FirstOrDefault(cc => cc.ID == ct.ClassID)?.Lessons ?? 0;

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

            #region 教师每周最大工作天数
            RuleAnalysisDetailResult drMaxDaysPerWeek = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drMaxDaysPerWeek.RuleName = Miscellaneous.GetLocalDescription(MixedRuleEnum.TeacherMaxDaysPerWeek);

            rule?.MaxDaysPerWeek?.Where(md => md.Weight == 1)?.ToList()?.ForEach(md => {
                if (!string.IsNullOrEmpty(md.TeacherID))
                {
                    List<int> teacherHours = cLCase?.ClassHours?.Where(ch => ch.TeacherIDs != null && ch.TeacherIDs.Contains(md.TeacherID))?.Select(ch => ch.ID)?.ToList() ?? new List<int>();
                    string teacherName = cLCase?.Teachers?.FirstOrDefault(t => t.ID == md.TeacherID)?.Name ?? string.Empty;

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
            drMaxGapsPerDay.RuleName = Miscellaneous.GetLocalDescription(MixedRuleEnum.TeacherMaxGapsPerDay);
            var periodMappingInfo = Miscellaneous.GetCLCasePeriodMapping(cLCase?.Positions);

            rule?.MaxGapsPerDay?.Where(mg => mg.Weight == 1)?.ToList()?.ForEach(mg => {
                if (!string.IsNullOrEmpty(mg.TeacherID))
                {
                    List<int> teacherHours = cLCase?.ClassHours?.Where(ch => ch.TeacherIDs != null && ch.TeacherIDs.Contains(mg.TeacherID))?.Select(ch => ch.ID)?.ToList() ?? new List<int>();
                    string teacherName = cLCase?.Teachers?.FirstOrDefault(t => t.ID == mg.TeacherID)?.Name ?? string.Empty;

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
            drMaxHoursDaily.RuleName = Miscellaneous.GetLocalDescription(MixedRuleEnum.TeacherMaxHoursDaily);

            rule?.MaxHoursDaily?.Where(mh => mh.Weight == 1)?.ToList()?.ForEach(mh => {
                if (!string.IsNullOrEmpty(mh.TeacherID))
                {
                    List<int> teacherHours = cLCase?.ClassHours?.Where(ch => ch.TeacherIDs != null && ch.TeacherIDs.Contains(mh.TeacherID))?.Select(ch => ch.ID)?.ToList() ?? new List<int>();
                    string teacherName = cLCase?.Teachers?.FirstOrDefault(t => t.ID == mh.TeacherID)?.Name ?? string.Empty;

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

            #region 教师排课时间
            RuleAnalysisDetailResult drTeacherTimes = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drTeacherTimes.RuleName = Miscellaneous.GetLocalDescription(MixedRuleEnum.TeacherTime);

            rule?.TeacherTimes?.Where(tt => tt.Weight == 1)?.ToList()?.ForEach(tt => {

                List<int> teacherHours = cLCase?.ClassHours?.Where(ch => ch.TeacherIDs != null && ch.TeacherIDs.Contains(tt.TeacherID))?.Select(ch => ch.ID)?.ToList() ?? new List<int>();
                string teacherName = cLCase?.Teachers?.FirstOrDefault(t => t.ID == tt.TeacherID)?.Name ?? string.Empty;

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
            var periodMappingInfo = Miscellaneous.GetCLCasePeriodMapping(resultModel?.Positions?.ToList());

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

            //班级课程课时对应关系
            var classCourseMapping = from x in resultModel?.Classes
                                     from y in resultModel?.Courses
                                     from z in y.Levels
                                     where x.CourseID == y.ID && x.LevelID == z.ID
                                     select new { ClassID = x.ID, x.CourseID, x.LevelID, ClassName = x.Name, CourseName = y.Name, LevelName = z.Name };

            //构造全局课表位置信息
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
                var singleClassLessons = resultModel.ResultClasses?.Where(rc => rc.ClassID == cl.ID)?.FirstOrDefault(c => c.ClassID == cl.ID)?.ResultDetails?.Select(rd => rd.DayPeriod)?.ToList();
                int[] singleTimeSlot = Miscellaneous.GetPartTimeTableSlot(timeTableSlot.Item3, singleClassLessons);
                bool resultFlag = Miscellaneous.CalculateLessonsEvenlyDistributedForSuggest(singleTimeSlot, globalDayPeriods);

                if (!resultFlag)
                {
                    drClassHourAverages.DetailInfo.Add($"{GetClassCourseInfoByResultModel(resultModel, cl.ID)} 的课表不满足课时分散!");
                }
            });

            drList.Add(drClassHourAverages);
            #endregion

            #region 音乐、体育、美术不排在上午前两节
            RuleAnalysisDetailResult drMusicPEArtFirstTwoLessons = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drMusicPEArtFirstTwoLessons.RuleName = "音乐、体育、美术不排在上午前两节";

            resultModel?.ResultClasses?.ToList()?.ForEach(cl => {
                 
                string className = GetClassCourseInfoByResultModel(resultModel, cl.ClassID);
                string courseName = classCourseMapping?.FirstOrDefault(cm => cm.ClassID == cl.ClassID)?.CourseName ?? string.Empty;
    
                if (courseName == "音乐" || courseName == "体育" || courseName == "美术")
                {
                    var classCourseDay = cl?.ResultDetails?.Select(rd => rd.DayPeriod)?.ToList();
                    var amPostions = resultModel?.Positions?.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.AM)?.Select(p => p.PositionOrder)?.Distinct();

                    if (amPostions != null && amPostions.Count() > 1)
                    {
                        var firstTwoPositon = amPostions.OrderBy(p => p).Take(2);
                        var amFirstTwo = resultModel.Positions.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.AM && firstTwoPositon.Contains(p.PositionOrder)).Select(p => p.DayPeriod);

                        var timeInterSect = TimeOperation.TimeSlotInterSect(amFirstTwo?.ToList(), classCourseDay);
                        if (timeInterSect != null && timeInterSect.Count() > 0)
                        {
                            drMusicPEArtFirstTwoLessons.DetailInfo.Add($"{ className } 的课表占用了上午前2节课!");
                        }
                    }
                } 
            });

            drList.Add(drMusicPEArtFirstTwoLessons);
            #endregion

            #region 2、3课时科目尽量分散
            RuleAnalysisDetailResult drDisperseAMAP = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drDisperseAMAP.RuleName = "2、3课时科目尽量分散";

            resultModel?.ResultClasses?.ToList()?.ForEach(cl => {
                string className = GetClassCourseInfoByResultModel(resultModel, cl.ClassID);
                /* 查找2/3课时课程 */
                int classLessonNumber = cl.ResultDetails?.Count ?? 0;
                if (classLessonNumber == 2 || classLessonNumber == 3)
                {
                    bool resultFlag = true;
                    var classCourseDay = cl?.ResultDetails?.Select(rd => (int)rd.DayPeriod.Day).Distinct().ToList();
                    classCourseDay = classCourseDay?.Select(cd => cd == 0 ? 7 : cd)?.ToList() ?? new List<int> { };/* 避免周六日被判断为非连续 */

                    if (classLessonNumber > classCourseDay.Count)
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
                        drDisperseAMAP.DetailInfo.Add($"{ className } 课时应尽量分散!");
                    }
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
                resultModel?.ResultClasses?.ToList()?.ForEach(cl => {
                    string className = GetClassCourseInfoByResultModel(resultModel, cl.ClassID);
                    var classCourseLesson = cl?.ResultDetails?.Select(rd => rd.DayPeriod)?.ToList();
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
                            drTooMuchLessonInPM.DetailInfo.Add($"{ className } 的下午课时安排过多!");
                        }
                    }
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
                    if (teacherLessons != null && teacherLessons.Count() > 1)
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

            #region 教案齐头(宽松)
            RuleAnalysisDetailResult drTeacherTeachingPlanNoCross = new RuleAnalysisDetailResult() { DetailInfo = new List<string> { } };
            drTeacherTeachingPlanNoCross.RuleName = "教案齐头";

            resultModel?.Teachers?.ToList()?.ForEach(te =>
            {
                string teacherName = resultModel?.Teachers?.FirstOrDefault(t => t.ID == te.ID)?.Name ?? string.Empty;

                //教师全部课程
                var teacherLessons = from rc in resultModel?.ResultClasses
                                     from rd in rc.ResultDetails
                                     where rd.Teachers != null && rd.Teachers.Contains(te.ID)
                                     select new { rc.ClassID, rd.DayPeriod, rd.Teachers };

                var teacherCourseLevelLessons = from tl in teacherLessons
                                                from ccm in classCourseMapping
                                                where tl.ClassID == ccm.ClassID 
                                                select new { tl.ClassID, tl.DayPeriod, tl.Teachers, ccm.CourseID, ccm.LevelID };

                //理论上一名老师可以教多个班多个科，只有同科同层不同班的教案齐头才有意义
                var teacherLessonsGroup = teacherCourseLevelLessons?.GroupBy(tl => new { tl.ClassID, tl.CourseID, tl.LevelID })
                                         ?.Select(tl => new { tl.Key.ClassID, tl.Key.CourseID, tl.Key.LevelID, LessonCount = tl.Count() });

                //教师教授多个同科班级时，按照不同课时分组
                var teacherSameLessonsGroup = teacherLessonsGroup?.GroupBy(tl => new { tl.ClassID, tl.CourseID, tl.LevelID, tl.LessonCount })
                                                ?.Select(tl => new {tl.Key.ClassID, tl.Key.CourseID, tl.Key.LevelID, tl.Key.LessonCount });

                var teacherCourseLevelLessonGroup = teacherSameLessonsGroup?.GroupBy(tl => new { tl.CourseID, tl.LevelID, tl.LessonCount })?
                                                .Select(sl => new { sl.Key.CourseID, sl.Key.LevelID, sl.Key.LessonCount, Count = sl.Count() })?.ToList();

                //遍历一个教师多个班相同科目相同课时情况下检查齐头
                teacherCourseLevelLessonGroup?.Where(tcl => tcl.Count > 1)?.ToList().ForEach(tcl => {
                    string courseName = resultModel?.Courses?.FirstOrDefault(co => co.ID == tcl.CourseID)?.Name ?? string.Empty;
                    string courseLevelName = resultModel?.Courses?.FirstOrDefault(co => co.ID == tcl.CourseID)?.Levels?.FirstOrDefault(le => le.ID == tcl.LevelID)?.Name ?? string.Empty;

                    /* Start: 教案齐头 */
                    /* 构造一个教师课位信息 */
                    int[] teacherSomeTimeTableSlot = new int[globalWeekDays.Length * globalDayPeriods.Length];
                    Dictionary<string, int[]> everyClassCourseLesson = new Dictionary<string, int[]> { };

                    teacherSameLessonsGroup?.Where(sl => sl.LessonCount > 1 && sl.CourseID == tcl.CourseID && sl.LevelID == tcl.LevelID && sl.LessonCount == tcl.LessonCount)?.ToList()?.ForEach(sl => {
                        var teacherOneClassLessonsTemp = teacherLessons.Where(tl => tl.ClassID == sl.ClassID)?.Select(tl => tl.DayPeriod)?.ToList();
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
                        drTeacherTeachingPlanNoCross.DetailInfo.Add($"教师 { teacherName } {courseName}{courseLevelName} 的课表教案不齐头!");
                    }
                    /* End: 教案齐头 */
                });
            });

            drList.Add(drTeacherTeachingPlanNoCross);
            #endregion

            return drList;
        }

        #region 通过结果模型获得班级名称
        private static string GetClassCourseInfoByResultModel(ResultModel resultModel, string classID)
        {
            string classCourseInfo = string.Empty;

            if (resultModel != null && !string.IsNullOrEmpty(classID))
            {
                var classObj = resultModel.Classes?.FirstOrDefault(cl => cl.ID == classID);

                string className = classObj?.Name ?? string.Empty;
                string courseName = resultModel.Courses?.FirstOrDefault(co => co.ID == classObj.CourseID)?.Name ?? string.Empty;
                string levelName = resultModel.Courses?.FirstOrDefault(co => co.ID == classObj.CourseID)?.Levels?.FirstOrDefault(le => le.ID == classObj.LevelID)?.Name ?? string.Empty;

                classCourseInfo = $"{courseName}{levelName}{className}";
            }

            return classCourseInfo;
        }
        #endregion
    }
}
