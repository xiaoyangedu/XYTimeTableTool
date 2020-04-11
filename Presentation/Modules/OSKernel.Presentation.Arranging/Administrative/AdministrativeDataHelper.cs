using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.DataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models.Administrative.AlgoRule;

namespace OSKernel.Presentation.Arranging.Administrative
{
    /// <summary>
    /// 行政班数据帮助类
    /// </summary>
    public static class AdministrativeDataHelper
    {
        /// <summary>
        /// 时间发生改变(清除所有时间规则）
        /// </summary>
        public static void TimeChanged(string localID, ICommonDataManager CommonDataManager)
        {
            var rule = CommonDataManager.GetAminRule(localID);
            var algo = CommonDataManager.GetAminAlgoRule(localID);

            if (rule != null)
            {
                rule.TeacherTimes?.Clear();
                rule.CourseLimits?.Clear();
                rule.CourseTimes?.Clear();
                rule.CourseArranges?.Clear();
                rule.MinorCourseAvoidTime?.Clear();
                rule.MajorCoursePreferredTime?.Clear();
                rule.LimitInSpecialTime?.Clear();
                rule.TimeTableLockedTimes = null;
                rule.Serialize(localID);
            }

            if (algo != null)
            {
                algo.TeacherNotAvailableTimes?.Clear();
                algo.ClassHourRequiredStartingTimes?.Clear();
                algo.ClassHourRequiredTimes?.Clear();
                algo.ClassHoursRequiredStartingTimes?.Clear();
                algo.ClassHoursRequiredTimes?.Clear();
                algo.ClassHourRequiredStartingTime?.Clear();
                algo.ClassHoursMaxConcurrencyInSelectedTimes?.Clear();
                algo.ClassHoursOccupyMaxTimeFromSelections?.Clear();
                algo.Serialize(localID);
            }
        }

        public static void TeacherChanged(Models.Base.UITeacher teacher, string localID, ICommonDataManager CommonDataManager)
        {
            var rule = CommonDataManager.GetAminRule(localID);
            var algo = CommonDataManager.GetAminAlgoRule(localID);
            var cp = CommonDataManager.GetCPCase(localID);

            if (cp != null)
            {
                cp.Classes?.ForEach(c =>
                {
                    c.Settings?.ForEach(s =>
                    {
                        s.TeacherIDs?.RemoveAll(t => t.Contains(teacher.ID));
                    });
                });

                cp.ClassHours?.ForEach(ch =>
                {
                    ch.TeacherIDs?.RemoveAll(t => t.Contains(teacher.ID));
                });

                cp.Serialize(localID);
            }

            if (rule != null)
            {
                rule.TeacherTimes?.RemoveAll(t => t.TeacherID.Equals(teacher.ID));
                rule.MasterApprenttices?.RemoveAll(t => t.MasterID.Equals(teacher.ID));
                rule.MasterApprenttices?.ForEach(m =>
                {
                    m.ApprenticeIDs?.RemoveAll(a => a.Equals(teacher.ID));
                });
                rule.PlanFlushes?.RemoveAll(t => t.TeacherID.Equals(teacher.ID));
                rule.AmPmNoContinues?.RemoveAll(t => t.TeacherID.Equals(teacher.ID));
                rule.MaxGapsPerDay?.RemoveAll(t => t.TeacherID.Equals(teacher.ID));
                rule.MaxDaysPerWeek?.RemoveAll(t => t.TeacherID.Equals(teacher.ID));
                rule.MaxHoursDaily?.RemoveAll(t => t.TeacherID.Equals(teacher.ID));
                rule.ContinuousPlanFlushes?.RemoveAll(t => t.TeacherID.Equals(teacher.ID));
                rule.ClassHourPriorityBalance?.RemoveAll(t => t.TeacherID.Equals(teacher.ID));
                rule.LimitInSpecialTime?.ForEach(t =>
                {
                    t.TeacherIDs?.RemoveAll(ti => ti.Equals(teacher.ID));
                });
                rule.HalfDayWork?.RemoveAll(t => t.TeacherID.Equals(teacher.ID));

                var teacherCourses = cp.GetCourses(teacher.ID);
                if (rule.TimeTableLockedTimes != null)
                {
                    rule.TimeTableLockedTimes.LockedTimeTable?.RemoveAll(lt => lt.LockedCourseTimeTable.Any(lct => teacherCourses.Any(tc => tc.ID.Equals(lct.CourseID))));
                }

                rule.Serialize(localID);
            }

            if (algo != null)
            {
                algo.TeacherMaxGapsPerDays?.RemoveAll(t => t.TeacherID.Equals(teacher.ID));
                algo.TeacherMaxHoursDailys?.RemoveAll(t => t.TeacherID.Equals(teacher.ID));
                algo.TeacherMaxDaysPerWeeks?.RemoveAll(t => t.TeacherID.Equals(teacher.ID));
                algo.TeacherNotAvailableTimes?.RemoveAll(t => t.TeacherID.Equals(teacher.ID));
            }
        }

        public static void CourseChanged(Models.Base.UICourse course, string localID, ICommonDataManager CommonDataManager)
        {
            var rule = CommonDataManager.GetAminRule(localID);
            var algo = CommonDataManager.GetAminAlgoRule(localID);
            var cp = CommonDataManager.GetCPCase(localID);

            var classHourIDs = cp.ClassHours.Where(c => c.CourseID.Equals(course.ID))?.ToList();

            if (cp != null)
            {
                cp.ClassHours?.RemoveAll(ch => ch.CourseID.Equals(course.ID));
                cp.Serialize(localID);
            }

            if (rule != null)
            {
                rule.Mutexes?.RemoveAll(r => r.CourseIDs.Contains(course.ID));
                rule.ClassUnions?.RemoveAll(r => r.CourseID.Equals(course.ID));
                rule.CourseLimits?.RemoveAll(r => r.CourseID.Equals(course.ID));
                rule.MasterApprenttices?.RemoveAll(r => r.CourseID.Equals(course.ID));
                rule.PlanFlushes?.RemoveAll(r => r.CourseID.Equals(course.ID));
                rule.ClassHourAverages?.RemoveAll(r => r.CourseID.Equals(course.ID));
                rule.ClassHourSameOpens?.Clear();
                //rule.ClassHourSameOpens?.ForEach(s =>
                //{
                //    s.Details?.ForEach(sd =>
                //    {
                //        sd.Classes.RemoveAll(c => c.CourseID.Equals(course.ID));
                //    });
                //    s.Details?.RemoveAll(sd => sd.Classes?.Count == 0);
                //});
                //rule.ClassHourSameOpens?.RemoveAll(s => s.Details?.Count == 0);

                rule.ContinuousPlanFlushes?.RemoveAll(r => r.CourseID.Equals(course.ID));
                rule.CourseTimes?.RemoveAll(r => r.CourseID.Equals(course.ID));
                rule.CourseArranges?.RemoveAll(r => r.CourseID.Equals(course.ID));
                rule.AmPmClassHours?.RemoveAll(r => r.CourseID.Equals(course.ID));
                rule.ClassHourPriorityBalance?.RemoveAll(r => r.CourseID.Equals(course.ID));
                rule.MinorCourseAvoidTime?.ForEach(r =>
                {
                    r.CourseIDs?.Remove(course.ID);
                });

                rule.MajorCoursePreferredTime?.ForEach(r =>
                {
                    r.CourseIDs?.Remove(course.ID);
                });

                if (rule.TimeTableLockedTimes != null)
                {
                    rule.TimeTableLockedTimes.LockedTimeTable?.RemoveAll(lt => lt.LockedCourseTimeTable.Any(lct => lct.CourseID.Equals(course.ID)));
                }

                rule.Serialize(localID);
            }

            if (algo != null)
            {
                classHourIDs?.ForEach(ch =>
                {
                    algo.ClassHoursSameStartingDays?.ForEach(a =>
                    {
                        var ids = a.ID?.ToList();
                        ids?.RemoveAll(ri => ri.Equals(ch.ID));
                        a.ID = ids?.ToArray();
                    });
                    algo.ClassHoursSameStartingDays?.RemoveAll(a => a.ID?.Count() == 0);

                    algo.ClassHoursSameStartingTimes?.ForEach(a =>
                    {
                        var ids = a.Id?.ToList();
                        ids?.RemoveAll(ri => ri.Equals(ch.ID));
                        a.Id = ids?.ToArray();
                    });
                    algo.ClassHoursSameStartingTimes?.RemoveAll(a => a.Id?.Count() == 0);

                    algo.ClassHoursSameStartingHours?.ForEach(a =>
                    {
                        var ids = a.ID?.ToList();
                        ids?.RemoveAll(ri => ri.Equals(ch.ID));
                        a.ID = ids?.ToArray();
                    });
                    algo.ClassHoursSameStartingHours?.RemoveAll(a => a.ID?.Count() == 0);


                    algo.TwoClassHoursContinuous?.RemoveAll(r => r.FirstID.Equals(ch.ID) || r.SecondID.Equals(ch.ID));
                    algo.TwoClassHoursOrdered?.RemoveAll(r => r.FirstID.Equals(ch.ID) || r.SecondID.Equals(ch.ID));
                    algo.TwoClassHoursGrouped?.RemoveAll(r => r.FirstID.Equals(ch.ID) || r.SecondID.Equals(ch.ID));
                    algo.ThreeClassHoursGrouped?.RemoveAll(r => r.FirstID.Equals(ch.ID) || r.SecondID.Equals(ch.ID) || r.ThirdID.Equals(ch.ID));

                    algo.MinDaysBetweenClassHours?.ForEach(a =>
                    {
                        var ids = a.ID?.ToList();
                        ids?.RemoveAll(ri => ri.Equals(ch.ID));
                        a.ID = ids?.ToArray();
                    });
                    algo.MinDaysBetweenClassHours?.RemoveAll(a => a.ID?.Count() == 0);

                    algo.MaxDaysBetweenClassHours?.ForEach(a =>
                    {
                        var ids = a.ID?.ToList();
                        ids?.RemoveAll(ri => ri.Equals(ch.ID));
                        a.ID = ids?.ToArray();
                    });
                    algo.MaxDaysBetweenClassHours?.RemoveAll(a => a.ID?.Count() == 0);

                    algo.ClassHoursNotOverlaps?.ForEach(a =>
                    {
                        var ids = a.ID?.ToList();
                        ids?.RemoveAll(ri => ri.Equals(ch.ID));
                        a.ID = ids?.ToArray();
                    });
                    algo.ClassHoursNotOverlaps?.RemoveAll(a => a.ID?.Count() == 0);

                    algo.ClassHoursMaxConcurrencyInSelectedTimes?.ForEach(a =>
                    {
                        var ids = a.ID?.ToList();
                        ids?.RemoveAll(ri => ri.Equals(ch.ID));
                        a.ID = ids?.ToArray();
                    });
                    algo.ClassHoursMaxConcurrencyInSelectedTimes?.RemoveAll(a => a.ID?.Count() == 0);

                    algo.ClassHoursOccupyMaxTimeFromSelections?.ForEach(a =>
                    {
                        var ids = a.ID?.ToList();
                        ids?.RemoveAll(ri => ri.Equals(ch.ID));
                        a.ID = ids?.ToArray();
                    });
                    algo.ClassHoursOccupyMaxTimeFromSelections?.RemoveAll(a => a.ID?.Count() == 0);

                    algo.ClassHourRequiredStartingTimes?.RemoveAll(r => r.ID.Equals(ch.ID));
                    algo.ClassHourRequiredTimes?.RemoveAll(r => r.ID.Equals(ch.ID));
                    algo.ClassHourRequiredStartingTime?.RemoveAll(r => r.ID.Equals(ch.ID));
                });

                algo.Serialize(localID);
            }
        }

        public static void ClassHourChanged(string localID, ICommonDataManager CommonDataManager)
        {
            var rule = CommonDataManager.GetAminRule(localID);
            var algo = CommonDataManager.GetAminAlgoRule(localID);
            var cp = CommonDataManager.GetCPCase(localID);

            if (rule != null)
            {
                rule.ClassHourSameOpens?.Clear();
                rule.CourseTimes?.Clear();
                rule.CourseArranges?.Clear();
                rule.ClassHourAverages?.Clear();
                rule.AmPmClassHours?.Clear();
                rule.TimeTableLockedTimes = null;

                rule.Serialize(localID);
            }

            if (algo != null)
            {
                algo.ClassHoursSameStartingDays?.Clear();
                algo.ClassHoursSameStartingTimes?.Clear();
                algo.ClassHoursSameStartingHours?.Clear();
                algo.TwoClassHoursContinuous?.Clear();
                algo.TwoClassHoursOrdered?.Clear();
                algo.TwoClassHoursGrouped?.Clear();
                algo.ThreeClassHoursGrouped?.Clear();
                algo.MinDaysBetweenClassHours?.Clear();
                algo.ClassHourRequiredStartingTimes?.Clear();
                algo.ClassHourRequiredTimes?.Clear();
                algo.MaxDaysBetweenClassHours?.Clear();
                algo.ClassHoursNotOverlaps?.Clear();
                algo.ClassHourRequiredStartingTime?.Clear();
                algo.ClassHoursMaxConcurrencyInSelectedTimes?.Clear();
                algo.ClassHoursOccupyMaxTimeFromSelections?.Clear();
                algo.Serialize(localID);
            }
        }

    }
}
