using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.DataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Arranging.Mixed
{
    /// <summary>
    /// 走班数据帮助类
    /// </summary>
    public class MixedDataHelper
    {
        /// <summary>
        /// 时间发生改变(清除所有时间规则）
        /// </summary>
        public static void TimeChanged(string localID, ICommonDataManager CommonDataManager)
        {
            var rule = CommonDataManager.GetMixedRule(localID);
            var algo = CommonDataManager.GetMixedAlgoRule(localID);

            if (rule != null)
            {
                rule.TeacherTimes?.Clear();
                rule.CourseLimits?.Clear();
                rule.CourseTimes?.Clear();
                rule.ArrangeContinuous?.Clear();
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
            var rule = CommonDataManager.GetMixedRule(localID);
            var algo = CommonDataManager.GetMixedAlgoRule(localID);
            var cl = CommonDataManager.GetCLCase(localID);

            if (cl != null)
            {
                cl.Classes?.ForEach(c =>
                {
                    c.TeacherIDs?.RemoveAll(t => t.Contains(teacher.ID));
                });

                cl.ClassHours?.ForEach(ch =>
                {
                    ch.TeacherIDs?.RemoveAll(t => t.Contains(teacher.ID));
                });

                cl.Serialize(localID);
            }

            if (rule != null)
            {
                rule.TeacherTimes?.RemoveAll(t => t.TeacherID.Contains(teacher.ID));
                rule.MaxGapsPerDay?.RemoveAll(t => t.TeacherID.Contains(teacher.ID));
                rule.MaxDaysPerWeek?.RemoveAll(t => t.TeacherID.Contains(teacher.ID));
                rule.MaxHoursDaily?.RemoveAll(t => t.TeacherID.Contains(teacher.ID));
                rule.Serialize(localID);
            }

            if (algo != null)
            {
                algo.TeacherMaxGapsPerDays?.RemoveAll(t => t.TeacherID.Equals(teacher.ID));
                algo.TeacherMaxHoursDailys?.RemoveAll(t => t.TeacherID.Equals(teacher.ID));
                algo.TeacherMaxDaysPerWeeks?.RemoveAll(t => t.TeacherID.Equals(teacher.ID));
                algo.TeacherNotAvailableTimes?.RemoveAll(t => t.TeacherID.Equals(teacher.ID));
                algo.Serialize(localID);
            }
        }

        public static void CourseChanged(List<Models.Mixed.UICourse> courses, string localID, ICommonDataManager CommonDataManager)
        {
            var rule = CommonDataManager.GetMixedRule(localID);
            var algo = CommonDataManager.GetMixedAlgoRule(localID);
            var cl = CommonDataManager.GetCLCase(localID);

            if (rule != null)
            {
                courses.ForEach(c =>
                {
                    var classes = cl.Classes.Where(cc => cc.CourseID.Equals(c.ID))?.ToList();
                    classes?.ForEach(tc =>
                    {
                        rule.CourseTimes?.RemoveAll(r => r.ClassID.Equals(tc.ID));
                        rule.ArrangeContinuous?.RemoveAll(r => r.ClassID.Equals(tc.ID));
                        rule.ClassHourAverages?.RemoveAll(r => r.ClassID.Equals(tc.ID));
                        rule.ClassHourSameOpens?.Clear();
                    });

                    rule.AmPmClassHours?.RemoveAll(r => r.CourseID.Equals(c.ID));
                    rule.CourseLimits?.RemoveAll(r => r.CourseID.Equals(c.ID));
                });
            }

            if (algo != null)
            {
                var classHours = cl.ClassHours.Where(ch => courses.Any(c => ch.CourseID.Equals(c.ID)))?.ToList();

                classHours?.ForEach(ch =>
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
                        var ids = a.ID?.ToList();
                        ids?.RemoveAll(ri => ri.Equals(ch.ID));
                        a.ID = ids?.ToArray();
                    });
                    algo.ClassHoursSameStartingTimes?.RemoveAll(a => a.ID?.Count() == 0);

                    algo.ClassHoursSameStartingHours?.ForEach(a =>
                    {
                        var ids = a.ID?.ToList();
                        ids?.RemoveAll(ri => ri.Equals(ch.ID));
                        a.ID = ids?.ToArray();
                    });
                    algo.ClassHoursSameStartingHours?.RemoveAll(a => a.ID?.Count() == 0);

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

                    algo.TwoClassHoursContinuous?.RemoveAll(r => r.FirstID.Equals(ch.ID) || r.SecondID.Equals(ch.ID));
                    algo.TwoClassHoursOrdered?.RemoveAll(r => r.FirstID.Equals(ch.ID) || r.SecondID.Equals(ch.ID));
                    algo.TwoClassHoursGrouped?.RemoveAll(r => r.FirstID.Equals(ch.ID) || r.SecondID.Equals(ch.ID));
                    algo.ThreeClassHoursGrouped?.RemoveAll(r => r.FirstID.Equals(ch.ID) || r.SecondID.Equals(ch.ID) || r.ThirdID.Equals(ch.ID));

                    algo.ClassHourRequiredStartingTimes?.RemoveAll(r => r.ID.Equals(ch.ID));
                    algo.ClassHourRequiredTimes?.RemoveAll(r => r.ID.Equals(ch.ID));
                    algo.ClassHourRequiredStartingTime?.RemoveAll(r => r.ID.Equals(ch.ID));
                });

                algo.Serialize(localID);
            }

            if (cl != null)
            {
                courses.ForEach(c =>
                {
                    cl.Courses.RemoveAll(dc => dc.ID.Equals(c.ID));
                    cl.Classes.RemoveAll(ch => ch.CourseID.Equals(c.ID));


                    cl.ClassHours.RemoveAll(ch => ch.CourseID.Equals(c.ID));

                    cl.Students?.ForEach(st =>
                    {
                        st.Preselections?.RemoveAll(p => p.CourseID.Equals(c.ID));
                    });
                });

                cl.Serialize(localID);
            }
        }

        public static void LevelChanged(List<Models.Mixed.UILevel> levels, string localID, ICommonDataManager CommonDataManager)
        {
            var rule = CommonDataManager.GetMixedRule(localID);
            var algo = CommonDataManager.GetMixedAlgoRule(localID);
            var cl = CommonDataManager.GetCLCase(localID);

            if (rule != null)
            {
                levels.ForEach(c =>
                {
                    var classes = cl.Classes.Where(cc => cc.LevelID.Equals(c.ID) && cc.CourseID.Equals(c.CourseID))?.ToList();
                    classes?.ForEach(tc =>
                    {
                        rule.CourseTimes?.RemoveAll(r => r.ClassID.Equals(tc.ID));
                        rule.ArrangeContinuous?.RemoveAll(r => r.ClassID.Equals(tc.ID));
                        rule.ClassHourAverages?.RemoveAll(r => r.ClassID.Equals(tc.ID));
                    });

                    rule.ClassHourSameOpens?.Clear();
                    rule.AmPmClassHours?.RemoveAll(r => r.CourseID.Equals(c.ID));
                    rule.CourseLimits?.RemoveAll(r => r.CourseID.Equals(c.ID));
                });
            }

            if (algo != null)
            {
                var classHours = cl.ClassHours.Where(ch => levels.Any(c => ch.LevelID.Equals(c.ID)))?.ToList();

                classHours?.ForEach(ch =>
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
                        var ids = a.ID?.ToList();
                        ids?.RemoveAll(ri => ri.Equals(ch.ID));
                        a.ID = ids?.ToArray();
                    });
                    algo.ClassHoursSameStartingTimes?.RemoveAll(a => a.ID?.Count() == 0);

                    algo.ClassHoursSameStartingHours?.ForEach(a =>
                    {
                        var ids = a.ID?.ToList();
                        ids?.RemoveAll(ri => ri.Equals(ch.ID));
                        a.ID = ids?.ToArray();
                    });
                    algo.ClassHoursSameStartingHours?.RemoveAll(a => a.ID?.Count() == 0);

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

                    algo.TwoClassHoursContinuous?.RemoveAll(r => r.FirstID.Equals(ch.ID) || r.SecondID.Equals(ch.ID));
                    algo.TwoClassHoursOrdered?.RemoveAll(r => r.FirstID.Equals(ch.ID) || r.SecondID.Equals(ch.ID));
                    algo.TwoClassHoursGrouped?.RemoveAll(r => r.FirstID.Equals(ch.ID) || r.SecondID.Equals(ch.ID));
                    algo.ThreeClassHoursGrouped?.RemoveAll(r => r.FirstID.Equals(ch.ID) || r.SecondID.Equals(ch.ID) || r.ThirdID.Equals(ch.ID));

                    algo.ClassHourRequiredStartingTimes?.RemoveAll(r => r.ID.Equals(ch.ID));
                    algo.ClassHourRequiredTimes?.RemoveAll(r => r.ID.Equals(ch.ID));
                    algo.ClassHourRequiredStartingTime?.RemoveAll(r => r.ID.Equals(ch.ID));
                });

                algo.Serialize(localID);
            }

            if (cl != null)
            {
                levels.ForEach(c =>
                {
                    var courseInfo = cl.Courses.FirstOrDefault(cc => cc.ID.Equals(c.CourseID));
                    if (courseInfo != null)
                    {
                        courseInfo.Levels.RemoveAll(rl => rl.ID.Equals(c.ID));
                    }

                    cl.Classes.RemoveAll(ch => ch.LevelID.Equals(c.ID) && ch.CourseID.Equals(c.CourseID));

                    cl.ClassHours.RemoveAll(ch => ch.LevelID.Equals(c.ID) && ch.CourseID.Equals(c.CourseID));

                    cl.Students?.ForEach(st =>
                    {
                        st.Preselections?.RemoveAll(p => p.LevelID.Equals(c.ID) && p.CourseID.Equals(c.CourseID));
                    });
                });
                cl.Serialize(localID);
            }
        }

        public static void ClassHourChanged(string localID, ICommonDataManager CommonDataManager)
        {
            var rule = CommonDataManager.GetMixedRule(localID);
            var algo = CommonDataManager.GetMixedAlgoRule(localID);
            var cl = CommonDataManager.GetCLCase(localID);

            if (rule != null)
            {
                OSKernel.Presentation.Models.Enums.MixedRuleEnum.ClassHourSameOpen.DeleteRule(localID);
                rule.ClassHourSameOpens?.Clear();
                rule.ClassHourAverages?.Clear();
                rule.CourseTimes?.Clear();
                rule.ArrangeContinuous?.Clear();
                rule.AmPmClassHours?.Clear();

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
