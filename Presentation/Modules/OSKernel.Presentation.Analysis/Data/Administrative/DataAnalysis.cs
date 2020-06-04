using System.Collections.Generic;
using System.Linq;
using XYKernel.OS.Common.Models.Administrative;
using XYKernel.OS.Common.Models.Administrative.Rule;
using OSKernel.Presentation.Analysis.Data.Administrative.Models;
using OSKernel.Presentation.Analysis.Utilities;
using XYKernel.OS.Common.Models;
using System.Text;
using System;

namespace OSKernel.Presentation.Analysis.Data.Administrative
{
    public class DataAnalysis
    {
        public static int GetCaseAvailableTimeSlot(CPCase cPCase)
        {
            return cPCase?.Positions?.Where(p => p.IsSelected
                                       && p.Position != XYKernel.OS.Common.Enums.Position.AB
                                       && p.Position != XYKernel.OS.Common.Enums.Position.Noon
                                       && p.Position != XYKernel.OS.Common.Enums.Position.PB)?.Count() ?? 0;
        }

        #region 综合
        /// <summary>
        /// 课位统计
        /// </summary>
        /// <param name="cPCase"></param>
        /// <returns></returns>
        public static List<ClassTimeSlotAnalysisResult> GetClassTimeSlotAnalysisResult(CPCase cPCase, Rule rule)
        {
            int caseAvailableSlot = GetCaseAvailableTimeSlot(cPCase);

            List<ClassTimeSlotAnalysisResult> arList = new List<ClassTimeSlotAnalysisResult>();

            int no = 0;

            cPCase?.Classes?.ForEach(cl =>
            {
                ++no;

                ClassTimeSlotAnalysisResult ar = new ClassTimeSlotAnalysisResult();
                ar.NO = no;
                ar.ClassID = cl.ID;
                ar.ClassName = cl.Name;

                cl.Settings?.ForEach(co =>
                {
                    ar.CourseNumber = ar.CourseNumber + 1;
                    ar.ClassLesson = ar.ClassLesson + co.Lessons;
                    //考虑单双周规则造成的课时统计错误
                    rule?.OddDualWeeks?.Where(od => od.ClassID == cl.ID)?.ToList()?.ForEach(od =>
                    {
                        if (od.DualWeekCourseID == co.CourseID || od.OddWeekCourseID == co.CourseID)
                        {
                            //如果设有单双周，则只计算一半课时
                            ar.ClassLesson = ar.ClassLesson - co.Lessons / (float)2.0;
                        }
                    });
                });

                ar.FreeSlotNumber = caseAvailableSlot - ar.ClassLesson;

                arList.Add(ar);
            });

            return arList;
        }

        /// <summary>
        /// 教师工作量统计
        /// </summary>
        /// <param name="cPCase"></param>
        /// <returns></returns>
        public static List<GeneralTeacherAnalysisResult> GetGeneralTeacherAnalysisResult(CPCase cPCase)
        {
            int caseAvailableSlot = GetCaseAvailableTimeSlot(cPCase);

            int no = 0;

            List<GeneralTeacherAnalysisResult> arList = new List<GeneralTeacherAnalysisResult>();
            cPCase?.Teachers?.ForEach(t =>
            {
                ++no;

                GeneralTeacherAnalysisResult ar = new GeneralTeacherAnalysisResult();
                ar.NO = no;
                ar.Teacher = t;

                var result = cPCase?.ClassHours?
                            .Where(ch => ch.TeacherIDs != null && ch.TeacherIDs.Contains(t.ID))?
                            .GroupBy(x => x.ClassID)?
                            .Select(g => new
                            {
                                ClassID = g.Key,
                                Count = g.Count()
                            });

                ar.Lesson = result?.Sum(x => x.Count) ?? 0;
                ar.ClassNumber = result?.Count() ?? 0;
                if (caseAvailableSlot > 0)
                {
                    ar.LessonRatio = ar.Lesson / (float)caseAvailableSlot;
                }

                arList.Add(ar);
            });
            return arList;
        }

        /// <summary>
        /// 规则统计
        /// </summary>
        /// <param name="cPCase"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        public static List<RuleAnalysisResult> GetRuleAnalysisResult(CPCase cPCase, Rule rule)
        {
            List<RuleAnalysisResult> arList = new List<RuleAnalysisResult>();
            List<RuleAnalysisDetailResult> radr = GetRuleAnalysisDetailResult(cPCase, rule);

            radr.ForEach(ra => {
                arList.Add(new RuleAnalysisResult() { RuleName = ra.RuleName, HasConflict = ra.DetailInfo.Count > 0 ? true : false });
            });

            return arList;
        }
        #endregion

        #region 教师
        /// <summary>
        /// 教师班级工作量统计
        /// </summary>
        /// <param name="cPCase"></param>
        /// <returns></returns>
        public static List<TeacherClassAnalysisResult> GetTeacherClassAnalysisResult(CPCase cPCase, Rule rule)
        {
            int no = 0;

            List<TeacherClassAnalysisResult> arList = new List<TeacherClassAnalysisResult>();
            cPCase?.Teachers?.ForEach(t =>
            {

                var result = cPCase?.ClassHours?
                            .Where(ch => ch.TeacherIDs != null && ch.TeacherIDs.Contains(t.ID))?
                            .GroupBy(x => new { x.ClassID, x.CourseID })?
                            .Select(g => new
                            {
                                g.Key.ClassID,
                                g.Key.CourseID,
                                Count = g.Count()
                            })?.ToList();

                result?.ForEach(r =>
                {
                    ++no;

                    TeacherClassAnalysisResult ar = new TeacherClassAnalysisResult();
                    ar.NO = no;
                    ar.Teacher = t;
                    ar.ClassID = r.ClassID;
                    ar.CourseID = r.CourseID;
                    ar.TeacherLesson = r.Count;
                    ar.ClassName = cPCase?.Classes?.FirstOrDefault(cl => cl.ID == r.ClassID)?.Name ?? string.Empty;
                    ar.CourseName = cPCase?.Courses?.FirstOrDefault(co => co.ID == r.CourseID)?.Name ?? string.Empty;
                    ar.ClassCourseLesson = cPCase?.Classes?.FirstOrDefault(cl => cl.ID == r.ClassID)?.Settings?.FirstOrDefault(s => s.CourseID == r.CourseID)?.Lessons ?? 0;
                    ar.Continous = rule?.CourseArranges?.FirstOrDefault(ca => ca.ClassID == r.ClassID && ca.CourseID == r.CourseID)?.Count ?? 0;

                    arList.Add(ar);
                });
            });

            return arList;
        }
        #endregion

        #region 班级课程
        /// <summary>
        /// 班级课程
        /// </summary>
        /// <param name="cPCase"></param>
        /// <returns></returns>
        public static List<ClassCourseAnalysisResult> GetClassCourseAnalysisResult(CPCase cPCase)
        {
            int no = 0;
            List<ClassCourseAnalysisResult> arList = new List<ClassCourseAnalysisResult>();
            cPCase?.Classes?.ForEach(cl =>
            {
                cl.Settings?.ForEach(s =>
                {

                    ++no;

                    ClassCourseAnalysisResult ar = new ClassCourseAnalysisResult();
                    ar.NO = no;
                    ar.ClassID = cl.ID;
                    ar.CourseID = s.CourseID;
                    ar.ClassName = cl.Name;
                    ar.CourseName = cPCase?.Courses?.FirstOrDefault(co => co.ID == s.CourseID)?.Name ?? string.Empty;
                    ar.Lesson = s.Lessons;

                    arList.Add(ar);
                });
            });

            return arList;
        }
        #endregion

        #region 规则
        public static List<RuleAnalysisDetailResult> GetRuleAnalysisDetailResult(CPCase cPCase, Rule rule)
        {
            #region Common
            List<RuleAnalysisDetailResult> arList = new List<RuleAnalysisDetailResult>();

            //方案可用时间
            List<DayPeriodModel> caseAvailableDayPeriod = cPCase?.Positions?.Where(p => p.IsSelected
                                       && p.Position != XYKernel.OS.Common.Enums.Position.AB
                                       && p.Position != XYKernel.OS.Common.Enums.Position.Noon
                                       && p.Position != XYKernel.OS.Common.Enums.Position.PB)?.Select(x => x.DayPeriod)?.ToList() ?? new List<DayPeriodModel>();

            //班级课程课时对应关系
            var classCourseMapping = from x in cPCase?.Classes
                                     from y in x.Settings
                                     select new { x.ID, y.CourseID, y.Lessons };

            //每个课位理论上最大开课数量
            int maxDayPeriodHoldLesson = cPCase?.Classes?.Count ?? 0;
            //总课时
            int totalLesson = cPCase?.ClassHours?.Count ?? 0;

            //每个教师的必须时间
            Dictionary<string, List<DayPeriodModel>> globalTeachersMustTimes = new Dictionary<string, List<DayPeriodModel>>();
            rule?.TeacherTimes?.ForEach(tt => {
                List<DayPeriodModel> teacherMustTimes = TimeOperation.TimeSlotInterSect(caseAvailableDayPeriod, tt.MustTimes);
                globalTeachersMustTimes.Add(tt.TeacherID, teacherMustTimes);
            });

            //每个课时的可用时间(结合课程、教师)
            List<ClassCourseAvailableTimes> globalClassCourseAvailableTimesWithCourseRule = new List<ClassCourseAvailableTimes>() { };
            List<ClassCourseAvailableTimes> globalClassCourseAvailableTimesWithTeacherRule = new List<ClassCourseAvailableTimes>() { };
            List<ClassCourseDetailLesson> globalLessonsAvailableTimesWithTeacherCourseRule = new List<ClassCourseDetailLesson>() { };
            cPCase?.ClassHours?.ForEach(ch => {

                ClassCourseDetailLesson ccdl = new ClassCourseDetailLesson() { };
                ccdl.ID = ch.ID;
                ccdl.ClassID = ch.ClassID;
                ccdl.CourseID = ch.CourseID;
                ccdl.AvailableTimes = caseAvailableDayPeriod.ToList();

                if (rule?.CourseTimes?.Exists(ct => ct.Weight == 1 && ct.ClassID == ch.ClassID && ct.CourseID == ch.CourseID) ?? false)
                {
                    CourseTimeRule ctr = rule.CourseTimes.FirstOrDefault(ct => ct.ClassID == ch.ClassID && ct.CourseID == ch.CourseID);
                    if ((ctr.ForbidTimes?.Count ?? 0) > 0)
                    {
                        ccdl.AvailableTimes = TimeOperation.TimeSlotDiff(ccdl.AvailableTimes, ctr.ForbidTimes);
                    }

                    if ((ctr.MustTimes?.Count ?? 0) > 0)
                    {
                        ccdl.AvailableTimes = TimeOperation.TimeSlotInterSect(ccdl.AvailableTimes, ctr.MustTimes);
                    }

                }

                ch.TeacherIDs?.ForEach(tid => {
                    if (rule.TeacherTimes?.Exists(tt => tt.Weight == 1 && tt.TeacherID == tid) ?? false)
                    {
                        TeacherTimeRule ttr = rule.TeacherTimes.FirstOrDefault(tt => tt.Weight == 1 && tt.TeacherID == tid);
                        if ((ttr.ForbidTimes?.Count ?? 0) > 0)
                        {
                            ccdl.AvailableTimes = TimeOperation.TimeSlotDiff(ccdl.AvailableTimes, ttr.ForbidTimes);
                        }

                        if ((ttr.MustTimes?.Count ?? 0) > 0)
                        {
                            ccdl.AvailableTimes = TimeOperation.TimeSlotInterSect(ccdl.AvailableTimes, ttr.MustTimes);
                        }
                    }
                });

                globalLessonsAvailableTimesWithTeacherCourseRule.Add(ccdl);
            });

            //计算班级下课程的可用时间-基于课程必须禁止规则
            cPCase?.Classes?.ForEach(cl => {
                cl.Settings?.ForEach(co => {
                    if (rule?.CourseTimes?.Exists(ct => ct.Weight == 1 && ct.ClassID == cl.ID && ct.CourseID == co.CourseID) ?? false)
                    {
                        ClassCourseAvailableTimes ccdlCourse = new ClassCourseAvailableTimes() { };
                        ccdlCourse.ClassID = cl.ID;
                        ccdlCourse.CourseID = co.CourseID;
                        ccdlCourse.Lesson = co.Lessons;
                        ccdlCourse.AvailableTimes = caseAvailableDayPeriod.ToList();

                        CourseTimeRule ctr = rule.CourseTimes.FirstOrDefault(ct => ct.ClassID == cl.ID && ct.CourseID == co.CourseID);
                        if ((ctr.ForbidTimes?.Count ?? 0) > 0)
                        {
                            ccdlCourse.AvailableTimes = TimeOperation.TimeSlotDiff(ccdlCourse.AvailableTimes, ctr.ForbidTimes);
                        }

                        if ((ctr.MustTimes?.Count ?? 0) > 0)
                        {
                            ccdlCourse.AvailableTimes = TimeOperation.TimeSlotInterSect(ccdlCourse.AvailableTimes, ctr.MustTimes);
                        }

                        ccdlCourse.MustLesson = Math.Min(co.Lessons, ccdlCourse.AvailableTimes.Count());

                        globalClassCourseAvailableTimesWithCourseRule.Add(ccdlCourse);
                    }
                });
            });

            //计算班级下课程的可用时间-基于教师必须禁止规则
            cPCase?.Classes?.ForEach(cl => {
                cl.Settings?.ForEach(co => {
                    List<string> classCourseTeachers = cPCase?.ClassHours?.Where(ch => ch.ClassID == cl.ID && ch.CourseID == co.CourseID && ch.TeacherIDs != null)?
                                                                            .SelectMany(ch => ch.TeacherIDs)?.ToList() ?? new List<string>() { };
                    int teacherCount = classCourseTeachers.Distinct().Count();
                    //仅检查各课时教师相同且是同一教师的情况
                    if (teacherCount == 1 && classCourseTeachers.Count == co.Lessons)
                    {
                        string tid = classCourseTeachers.FirstOrDefault();

                        if (rule.TeacherTimes?.Exists(tt => tt.Weight == 1 && tt.TeacherID == tid) ?? false)
                        {
                            ClassCourseAvailableTimes ccdlCourse = new ClassCourseAvailableTimes() { };
                            ccdlCourse.ClassID = cl.ID;
                            ccdlCourse.CourseID = co.CourseID;
                            ccdlCourse.Lesson = co.Lessons;
                            ccdlCourse.AvailableTimes = caseAvailableDayPeriod.ToList();

                            TeacherTimeRule ttr = rule.TeacherTimes.FirstOrDefault(tt => tt.Weight == 1 && tt.TeacherID == tid);
                            if ((ttr.ForbidTimes?.Count ?? 0) > 0)
                            {
                                ccdlCourse.AvailableTimes = TimeOperation.TimeSlotDiff(ccdlCourse.AvailableTimes, ttr.ForbidTimes);
                            }

                            if ((ttr.MustTimes?.Count ?? 0) > 0)
                            {
                                ccdlCourse.AvailableTimes = TimeOperation.TimeSlotInterSect(ccdlCourse.AvailableTimes, ttr.MustTimes);
                            }

                            int teacherOtherLesson = cPCase.ClassHours.Where(ch => ch.TeacherIDs != null && ch.TeacherIDs.Contains(tid) && 
                                                                            (ch.ClassID != cl.ID || (ch.ClassID == cl.ID && ch.CourseID != co.CourseID))).Count();

                            int minAvailableTimes = ccdlCourse.AvailableTimes.Count() - teacherOtherLesson;
                            minAvailableTimes = Math.Max(minAvailableTimes, 0);
                            ccdlCourse.MustLesson = Math.Min(co.Lessons, minAvailableTimes);

                            globalClassCourseAvailableTimesWithTeacherRule.Add(ccdlCourse);
                        }
                    }
                });
            });
            #endregion

            #region 教师排课时间
            RuleAnalysisDetailResult arTeacherTime = new RuleAnalysisDetailResult();
            arTeacherTime.DetailInfo = new List<string>();
            arTeacherTime.RuleName = "教师排课时间";

            rule?.TeacherTimes?.Where(x => x.Weight == 1)?.ToList()?.ForEach(tt => {
                //1.教师的必须和禁止有冲突
                List<DayPeriodModel> teacherMustForbidConf = TimeOperation.TimeSlotInterSect(tt.ForbidTimes, tt.MustTimes);
                string teacherName = cPCase?.Teachers?.FirstOrDefault(t => t.ID == tt.TeacherID)?.Name ?? string.Empty;

                if (teacherMustForbidConf.Count > 0)
                {
                    StringBuilder sb = new StringBuilder(100);
                    sb.Append($"{teacherName}的教师排课时间必须禁止之间存在冲突:");

                    teacherMustForbidConf.ForEach(tc => {
                        string conflictDate = TimeOperation.GetDateInfo(tc);
                        sb.Append(conflictDate + ";");
                    });

                    arTeacherTime.DetailInfo.Add(sb.ToString());
                }

                //2.教师禁止时间过多
                List<int> teacherLesson = cPCase?.ClassHours?.Where(ch => ch.TeacherIDs != null && ch.TeacherIDs.Contains(tt.TeacherID))?.Select(ch => ch.ID)?.ToList() ?? new List<int>();

                List<DayPeriodModel> teacherAvailable = TimeOperation.TimeSlotDiff(caseAvailableDayPeriod, tt.ForbidTimes);
                if (teacherAvailable.Count < teacherLesson.Count)
                {
                    arTeacherTime.DetailInfo.Add($"教师{teacherName}的可用课位不足(结合方案时间，教师个人禁止时间和教师课时数)!");
                }

                //3.教师与课程必须禁止冲突检查
                rule.CourseTimes?.Where(x => x.Weight == 1)?.ToList()?.ForEach(x =>
                {
                    int classCourseLesson = cPCase?.ClassHours?.Where(ch => ch.ClassID == x.ClassID && ch.CourseID == x.CourseID)?.Count() ?? 0;
                    int classCourseTeacherLesson = cPCase?.ClassHours?.Where(ch => ch.ClassID == x.ClassID && ch.CourseID == x.CourseID && ch.TeacherIDs != null && ch.TeacherIDs.Contains(tt.TeacherID))?.Count() ?? 0;

                    //3.1 教师禁止与课程必须冲突检查
                    if (x.MustTimes != null && !string.IsNullOrEmpty(x.ClassID) && !string.IsNullOrEmpty(x.CourseID))
                    {
                        var shouldCheck = cPCase?.ClassHours?.Exists(c => c.ClassID == x.ClassID && c.CourseID == x.CourseID && c.TeacherIDs != null && c.TeacherIDs.Contains(tt.TeacherID)) ?? false;
                        if (shouldCheck)
                        {
                            List<DayPeriodModel> dpInterSect = TimeOperation.TimeSlotInterSect(x.MustTimes, tt.ForbidTimes);
                            //如果本来必须时间足够
                            if (x.MustTimes.Count >= classCourseLesson)
                            {
                                if (x.MustTimes.Count - dpInterSect.Count < classCourseTeacherLesson)
                                {
                                    arTeacherTime.DetailInfo.Add($"教师{teacherName}的禁止时间与{Miscellaneous.GetCPClassCourseInfo(cPCase, x.ClassID, x.CourseID)}的必须时间冲突!");
                                }
                            }
                            else
                            {
                                //如果剩下的课时放不满必须禁止重合的课时，提示错误
                                if (dpInterSect.Count > classCourseLesson - classCourseTeacherLesson)
                                {
                                    arTeacherTime.DetailInfo.Add($"教师{teacherName}的禁止时间与{Miscellaneous.GetCPClassCourseInfo(cPCase, x.ClassID, x.CourseID)}的必须时间冲突!");
                                }
                            }
                        }
                    }

                    //3.2 教师必须与课程禁止冲突检查
                    if (tt.MustTimes != null && x.ForbidTimes != null && !string.IsNullOrEmpty(x.ClassID) && !string.IsNullOrEmpty(x.CourseID))
                    {
                        var shouldCheck = cPCase?.ClassHours?.Exists(c => c.ClassID == x.ClassID && c.CourseID == x.CourseID && c.TeacherIDs != null && c.TeacherIDs.Contains(tt.TeacherID)) ?? false;
                        if (shouldCheck)
                        {
                            List<DayPeriodModel> dpInterSect = TimeOperation.TimeSlotInterSect(x.ForbidTimes, tt.MustTimes);

                            if (teacherLesson.Count == classCourseTeacherLesson && dpInterSect.Count > 0)
                            {
                                arTeacherTime.DetailInfo.Add($"教师 {teacherName} 的必须时间与{Miscellaneous.GetCPClassCourseInfo(cPCase, x.ClassID, x.CourseID)}的禁止时间冲突!");
                            }
                        }
                    }

                    //3.3 教师禁止与课程禁止课位叠加
                    if (x.ForbidTimes != null)
                    {
                        List<DayPeriodModel> teacherCourseAvailable = TimeOperation.TimeSlotDiff(teacherAvailable, x.ForbidTimes);
                        if (teacherCourseAvailable.Count < classCourseTeacherLesson)
                        {
                            arTeacherTime.DetailInfo.Add($"教师 {teacherName} 在 {Miscellaneous.GetCPClassCourseInfo(cPCase, x.ClassID, x.CourseID)} 的可用课位不足!");
                        }
                    }
                });
            });

            arList.Add(arTeacherTime);
            #endregion

            #region 课程排课时间
            RuleAnalysisDetailResult arCourseTime = new RuleAnalysisDetailResult();
            arCourseTime.DetailInfo = new List<string>();
            arCourseTime.RuleName = "课程排课时间";

            rule?.CourseTimes?.Where(x => x.Weight == 1)?.ToList()?.ForEach(ct => {
                //1. 课程禁止和必须冲突
                List<DayPeriodModel> courseMustForbidConf = TimeOperation.TimeSlotInterSect(ct.ForbidTimes, ct.MustTimes);
                string courseName = cPCase?.Courses?.FirstOrDefault(c => c.ID == ct.CourseID)?.Name ?? string.Empty;
                string className = cPCase?.Classes?.FirstOrDefault(c => c.ID == ct.ClassID)?.Name ?? string.Empty;
                int classCourseLesson = cPCase?.ClassHours?.Where(ch => ch.ClassID == ct.ClassID && ch.CourseID == ct.CourseID)?.Count() ?? 0;

                if (courseMustForbidConf.Count > 0)
                {
                    StringBuilder sb = new StringBuilder(100);
                    sb.Append($"班级{className}的科目{courseName}的课程排课时间必须禁止之间存在冲突:");

                    courseMustForbidConf.ForEach(tc => {
                        string conflictDate = TimeOperation.GetDateInfo(tc);
                        sb.Append(conflictDate + ";");
                    });

                    arCourseTime.DetailInfo.Add(sb.ToString());
                }

                //2. 课程和教师必须禁止冲突
                rule?.TeacherTimes?.ForEach(tt => {
                    int classCourseTeacherLesson = cPCase?.ClassHours?.Where(ch => ch.ClassID == ct.ClassID && ch.CourseID == ct.CourseID && ch.TeacherIDs != null && ch.TeacherIDs.Contains(tt.TeacherID))?.Count() ?? 0;
                    string teacherName = cPCase?.Teachers?.FirstOrDefault(t => t.ID == tt.TeacherID)?.Name ?? string.Empty;

                    //2.1 课程必须与教师禁止冲突
                    List<DayPeriodModel> dpCourseMustInterSect = TimeOperation.TimeSlotInterSect(ct.MustTimes, tt.ForbidTimes);
                    if (dpCourseMustInterSect.Count > 0)
                    {
                        if (ct.MustTimes.Count <= classCourseLesson)
                        {
                            if (classCourseLesson - classCourseTeacherLesson < dpCourseMustInterSect.Count)
                            {
                                arCourseTime.DetailInfo.Add($"教师{teacherName}的禁止时间与{Miscellaneous.GetCPClassCourseInfo(cPCase, ct.ClassID, ct.CourseID)}的必须时间冲突!");
                            }
                        }
                    }

                    //2.2 课程禁止和教师必须冲突
                    List<DayPeriodModel> dpCourseForbidInterSect = TimeOperation.TimeSlotInterSect(ct.ForbidTimes, tt.MustTimes);
                    if (dpCourseForbidInterSect.Count > 0)
                    {
                        if (tt.MustTimes.Count == classCourseTeacherLesson)
                        {
                            arCourseTime.DetailInfo.Add($"教师{teacherName}的必须时间与{Miscellaneous.GetCPClassCourseInfo(cPCase, ct.ClassID, ct.CourseID)}的禁止时间冲突!");
                        }
                    }
                });

                //3. 课程禁止课时过多导致课位不足的检查
                List<DayPeriodModel> courseAvailable = TimeOperation.TimeSlotDiff(caseAvailableDayPeriod, ct.ForbidTimes);
                if (courseAvailable.Count < classCourseLesson)
                {
                    arCourseTime.DetailInfo.Add($"{Miscellaneous.GetCPClassCourseInfo(cPCase, ct.ClassID, ct.CourseID)}的禁止时间过多!");
                }

                //4. 课程必须禁止和课时分散的冲突
                rule?.ClassHourAverages?.Where(cha => cha.Weight == 1 && cha.ClassID == ct.ClassID && cha.CourseID == ct.CourseID)?.ToList()?.ForEach(cha => {
                    if (rule?.CourseArranges?.Exists(ca => ca.ClassID == cha.ClassID && ca.CourseID == cha.CourseID) ?? false)
                    {
                        //有连排
                    }
                    else
                    {
                        //没有连排
                        int minDistance = classCourseLesson + (classCourseLesson - 1) * (cha.MinDay - 1);
                        int minGaps = minDistance - 1;

                        List<DayOfWeek> availableDaysOfWeek = TimeOperation.GetFormattedAvailableTims(courseAvailable);
                        List<int> availableIntDays = TimeOperation.ConvertDayOfWeek(availableDaysOfWeek);

                        if (availableDaysOfWeek.Count < classCourseLesson)
                        {
                            arCourseTime.DetailInfo.Add($"{Miscellaneous.GetCPClassCourseInfo(cPCase, ct.ClassID, ct.CourseID)}在现有规则下无法满足课时分散要求!");
                        }

                        if (availableDaysOfWeek.Count >= classCourseLesson && availableDaysOfWeek.Count > 1 && classCourseLesson > 1)
                        {
                            int dMax = availableIntDays.Max();
                            int dMin = availableIntDays.Min();

                            if (Math.Abs(dMax - dMin) < minGaps)
                            {
                                arCourseTime.DetailInfo.Add($"{Miscellaneous.GetCPClassCourseInfo(cPCase, ct.ClassID, ct.CourseID)}在现有规则下无法满足课时分散要求!");
                            }
                        }
                    }
                });

                //5.1 课程必须禁止与连排冲突检查 -- 课程必须与连排区域不重叠
                if (rule?.CourseArranges?.Exists(ca => ca.TimesWeight == 1 && ca.ClassID == ct.ClassID && ca.CourseID == ct.CourseID && (ca.Times?.Count ?? 0) >0) ?? false)
                {
                    var continousInfo = rule.CourseArranges.FirstOrDefault(ca => ca.ClassID == ct.ClassID && ca.CourseID == ct.CourseID);
                    int continousMust = Math.Min(continousInfo.Count * 2, continousInfo.Times.Count);
                    List<DayPeriodModel> intersectTimes = TimeOperation.TimeSlotInterSect(ct.MustTimes, continousInfo.Times);

                    if (intersectTimes.Count < continousMust && ct.MustTimes.Count >= classCourseLesson)
                    {
                        arCourseTime.DetailInfo.Add($"{Miscellaneous.GetCPClassCourseInfo(cPCase, ct.ClassID, ct.CourseID)} 课程排课时间与连排时间冲突!");
                    }
                }

                //5.2 课程必须禁止与连排冲突检查 -- 课程的必须区域无法满足隔天连排
                if (rule?.CourseArranges?.Exists(ca => ca.IsIntervalDay && ca.IntervalDayWeight == 1 && ca.ClassID == ct.ClassID && ca.CourseID == ct.CourseID && (ct.MustTimes?.Count ?? 0) > 0) ?? false)
                {
                    var continousInfo = rule.CourseArranges.FirstOrDefault(ca => ca.ClassID == ct.ClassID && ca.CourseID == ct.CourseID);
                    if (ct.MustTimes.Count >= classCourseLesson)
                    {
                        bool isEnough = Miscellaneous.DaysEnoughForEvenlyDistributed(ct.MustTimes, continousInfo.Count, 2);

                        if (!isEnough)
                        {
                            arCourseTime.DetailInfo.Add($"{Miscellaneous.GetCPClassCourseInfo(cPCase, ct.ClassID, ct.CourseID)} 课程排课必须时间无法满足隔天连排!");
                        }
                    }
                }

                //5.3 课程必须禁止与连排冲突检查 -- 课程禁止后的可排区域无法满足隔天连排
                if (rule?.CourseArranges?.Exists(ca => ca.IsIntervalDay && ca.IntervalDayWeight == 1 && ca.ClassID == ct.ClassID && ca.CourseID == ct.CourseID && (ct.ForbidTimes?.Count ?? 0) > 0) ?? false)
                {
                    var continousInfo = rule.CourseArranges.FirstOrDefault(ca => ca.ClassID == ct.ClassID && ca.CourseID == ct.CourseID);
                    List<DayPeriodModel> intersectTimes = TimeOperation.TimeSlotDiff(caseAvailableDayPeriod, ct.ForbidTimes);
                    bool isEnough = Miscellaneous.DaysEnoughForEvenlyDistributed(intersectTimes, continousInfo.Count, 2);

                    if (!isEnough)
                    {
                        arCourseTime.DetailInfo.Add($"{Miscellaneous.GetCPClassCourseInfo(cPCase, ct.ClassID, ct.CourseID)} 课程排课禁止时间无法满足隔天连排!");
                    }
                }
            });

            arList.Add(arCourseTime);
            #endregion

            #region 课程连排
            RuleAnalysisDetailResult arCourseArrange = new RuleAnalysisDetailResult();
            arCourseArrange.DetailInfo = new List<string>();
            arCourseArrange.RuleName = "课程连排";

            rule?.CourseArranges?.ForEach(ca => {

                List<DayPeriodModel> courseAvailable = caseAvailableDayPeriod.ToList();

                //1. 连排有必须时间
                if (ca.Count >= 1 && ca.TimesWeight == 1)
                {
                    if (ca.Times != null && ca.Times.Count > 0)
                    {
                        var courseTime = rule?.CourseTimes?.FirstOrDefault(ct => ct.ClassID == ca.ClassID && ct.CourseID == ca.CourseID);
                        if (courseTime != null)
                        {
                            courseAvailable = TimeOperation.TimeSlotDiff(courseAvailable, courseTime.ForbidTimes);
                        }

                        List<DayPeriodModel> classCourseContinousAvailable = TimeOperation.TimeSlotInterSect(courseAvailable, ca.Times);

                        if (ca.Count == 1 && ca.Times.Count < 2 ||
                            (ca.Count * 2 <= ca.Times.Count && ca.Count * 2 > classCourseContinousAvailable.Count))
                        {
                            arCourseArrange.DetailInfo.Add($"{Miscellaneous.GetCPClassCourseInfo(cPCase, ca.ClassID, ca.CourseID)}在现有规则下无法满足连排课位优先要求（课位不足）!");
                        }

                        //判断可用课位是否最终有连续课位提供
                        int iContinousNum = Math.Min(ca.Count, classCourseContinousAvailable.Count / 2);
                        var result = CanHoldContinous(cPCase, classCourseContinousAvailable, iContinousNum, ca.NoCrossingBreak);

                        if (!result.Item1)
                        {
                            arCourseArrange.DetailInfo.Add($"{Miscellaneous.GetCPClassCourseInfo(cPCase, ca.ClassID, ca.CourseID)}在现有规则下无法满足连排课位!");
                        }

                        //多个连排隔天连排
                        if (result.Item1 && ca.Count > 1 && ca.IsIntervalDay && ca.IntervalDayWeight == 1)
                        {
                            bool isEnough = Miscellaneous.DaysEnoughForEvenlyDistributed(result.Item2, iContinousNum, 2);
                            if (!isEnough)
                            {
                                arCourseArrange.DetailInfo.Add($"{Miscellaneous.GetCPClassCourseInfo(cPCase, ca.ClassID, ca.CourseID)} 无法满足隔天连排!");
                            }
                        }
                    }
                    else
                    {
                        var courseTime = rule?.CourseTimes?.FirstOrDefault(ct => ct.ClassID == ca.ClassID && ct.CourseID == ca.CourseID);
                        if (courseTime != null)
                        {
                            courseAvailable = TimeOperation.TimeSlotDiff(courseAvailable, courseTime.ForbidTimes);
                        }

                        //判断可用课位是否最终有连续课位提供
                        int iContinousNum = Math.Min(ca.Count, courseAvailable.Count / 2);
                        var result = CanHoldContinous(cPCase, courseAvailable, iContinousNum, ca.NoCrossingBreak);

                        if (!result.Item1)
                        {
                            arCourseArrange.DetailInfo.Add($"{Miscellaneous.GetCPClassCourseInfo(cPCase, ca.ClassID, ca.CourseID)}在现有规则下无法满足连排要求!");
                        }

                        //多个连排隔天连排
                        if (result.Item1 && ca.Count > 1 && ca.IsIntervalDay && ca.IntervalDayWeight == 1)
                        {
                            bool isEnough = Miscellaneous.DaysEnoughForEvenlyDistributed(result.Item2, iContinousNum, 2);
                            if (!isEnough)
                            {
                                arCourseArrange.DetailInfo.Add($"{Miscellaneous.GetCPClassCourseInfo(cPCase, ca.ClassID, ca.CourseID)} 无法满足隔天连排!");
                            }
                        }
                    }
                }
            });

            //1个班级下多个科目之间的连排冲突
            cPCase?.Classes?.ForEach(cl => {

                List<DayPeriodModel> classMustUnion = new List<DayPeriodModel>();
                int iContinousCount = 0;
                bool noCrossingBreak = true;/* 仅取一个值 */

                rule?.CourseArranges?.Where(ca => ca.ClassID == cl.ID && ca.TimesWeight == 1 && ca.Times != null)?.ToList()?.ForEach(co => {
                    //目前还不能有效针对课位不足情况下的跨科目连排校验
                    if (co.Times.Count >= co.Count * 2)
                    {
                        iContinousCount += co.Count;
                        classMustUnion = TimeOperation.TimeSlotUnion(classMustUnion, co.Times);
                        noCrossingBreak = co.NoCrossingBreak;
                    }
                });

                bool result = CanHoldContinous(cPCase, classMustUnion, iContinousCount, noCrossingBreak).Item1;
                if (!result)
                {
                    string className = cPCase?.Classes?.FirstOrDefault(cla => cla.ID == cl.ID)?.Name ?? string.Empty;
                    arCourseArrange.DetailInfo.Add($"班级 {className} 在现有规则下无法满足多个科目的连排要求!");
                }
            });

            //1个老师多个班级下同科目之间的连排冲突
            cPCase?.Teachers?.ForEach(t => {
                var teacherLesson = cPCase?.ClassHours?.Where(ch => ch.TeacherIDs != null && ch.TeacherIDs.Contains(t.ID))?.Select(ch => new { ch.ClassID, ch.CourseID, t.ID });
                //统计班级课程教师任课课时
                var teacherClassCourse = teacherLesson?.GroupBy(tl => new { tl.ClassID, tl.CourseID })?
                .Select(tl => new
                {
                    tl.Key.ClassID,
                    tl.Key.CourseID,
                    Lessons = tl.Count()
                });

                //找出教师全程上课班级
                var teacherFullTimeCourse = from x in classCourseMapping
                                            from y in teacherClassCourse
                                            where x.ID == y.ClassID && x.CourseID == y.CourseID && x.Lessons == y.Lessons
                                            select new { y.ClassID, y.CourseID };

                List<DayPeriodModel> teacherClassMustUnion = new List<DayPeriodModel>();
                int iContinousCount = 0;
                bool noCrossingBreak = true;/* 仅取一个值 */

                teacherFullTimeCourse?.ToList()?.ForEach(tf => {
                    var caRule = rule?.CourseArranges?.FirstOrDefault(ca => ca.Times != null && ca.TimesWeight == 1 && ca.ClassID == tf.ClassID && ca.CourseID == tf.CourseID);
                    if (caRule != null)
                    {
                        //目前还不能有效针对课位不足情况下的跨科目连排校验
                        if (caRule.Times.Count >= caRule.Count * 2)
                        {
                            iContinousCount += caRule.Count;
                            teacherClassMustUnion = TimeOperation.TimeSlotUnion(teacherClassMustUnion, caRule.Times);
                            noCrossingBreak = caRule.NoCrossingBreak;
                        }
                    }

                });

                bool result = CanHoldContinous(cPCase, teacherClassMustUnion, iContinousCount, noCrossingBreak).Item1;
                if (!result)
                {
                    string teacherName = cPCase?.Teachers?.FirstOrDefault(te => te.ID == t.ID)?.Name ?? string.Empty;
                    arCourseArrange.DetailInfo.Add($"教师 {teacherName} 在现有规则下无法满足在多个班级的连排要求!");
                }
            });

            arList.Add(arCourseArrange);
            #endregion

            #region 合班上课
            RuleAnalysisDetailResult raClassUnion = new RuleAnalysisDetailResult();
            raClassUnion.DetailInfo = new List<string>();
            raClassUnion.RuleName = "合班上课";

            rule?.ClassUnions?.ForEach(cu => {
                //课程必须禁止冲突
                
                List<DayPeriodModel> classForbidUnion = new List<DayPeriodModel>();
                int courseLesson = classCourseMapping.FirstOrDefault(ccm => ccm.CourseID == cu.CourseID && ccm.ID == cu.ClassIDs.FirstOrDefault())?.Lessons ?? 0;
                string courseName = cPCase?.Courses?.FirstOrDefault(co => co.ID == cu.CourseID)?.Name ?? string.Empty;
                List<string> classNames = new List<string>();
                //获取班级名称集合
                cu.ClassIDs?.ForEach(ci => {
                    string className = cPCase?.Classes?.FirstOrDefault(cl => cl.ID == ci)?.Name ?? string.Empty;
                    classNames.Add(className);
                });

                //获取禁止时间合集
                cu.ClassIDs?.ForEach(ci => {
                    var courseTimeRule = rule?.CourseTimes?.FirstOrDefault(ct => ct.CourseID == cu.CourseID && ct.ClassID == ci && ct.Weight == 1);

                    if (courseTimeRule?.ForbidTimes != null)
                    {
                        classForbidUnion = TimeOperation.TimeSlotUnion(classForbidUnion, courseTimeRule.ForbidTimes);
                    }
                });
                
                //检查禁止造成的排课冲突
                var availableExceptForbid = TimeOperation.TimeSlotDiff(caseAvailableDayPeriod, classForbidUnion);

                if (availableExceptForbid.Count <= courseLesson)
                {
                    raClassUnion.DetailInfo.Add($"科目 {courseName} 下的合班规则(班级:{string.Join(",", classNames)})因课程禁止设置无法满足要求!");
                }

                //检查必须造成的排课冲突 //仅查2种情况
                int fullMustTimeClass = 0;
                int noFullMustTimeClass = 0;

                cu.ClassIDs?.ForEach(ci => {
                    int lesson = rule?.CourseTimes?.FirstOrDefault(ct => ct.CourseID == cu.CourseID && ct.ClassID == ci && ct.Weight == 1)?.MustTimes?.Count ?? 0;
                    if (lesson >= courseLesson || lesson == 0)
                    {
                        fullMustTimeClass++;
                    }
                    else
                    {
                        noFullMustTimeClass++;
                    }
                });

                //1.所有必须课时都存在且必须课位充足，或者必须课位不存在
                if (fullMustTimeClass == cu.ClassIDs.Count)
                {
                    List<DayPeriodModel> classMustInterSect = new List<DayPeriodModel>();
                    //获取必须时间交集
                    for (int i = 0; i < cu.ClassIDs.Count; i++)
                    {
                        for (int j = i + 1; j < cu.ClassIDs.Count; j++)
                        {
                            var iCourseTimeRule = rule?.CourseTimes?.FirstOrDefault(ct => ct.CourseID == cu.CourseID && ct.ClassID == cu.ClassIDs[i] && ct.Weight == 1);
                            var jCourseTimeRule = rule?.CourseTimes?.FirstOrDefault(ct => ct.CourseID == cu.CourseID && ct.ClassID == cu.ClassIDs[j] && ct.Weight == 1);

                            if (iCourseTimeRule?.MustTimes != null && jCourseTimeRule?.MustTimes != null)
                            {
                                classMustInterSect = TimeOperation.TimeSlotInterSect(iCourseTimeRule.MustTimes, jCourseTimeRule.MustTimes);
                            }
                        }
                    }

                    cu.ClassIDs?.ForEach(ci => {
                        var courseTimeRule = rule?.CourseTimes?.FirstOrDefault(ct => ct.CourseID == cu.CourseID && ct.ClassID == ci && ct.Weight == 1);

                        if (courseTimeRule?.MustTimes != null)
                        {
                            List<DayPeriodModel> classCourseTimes = new List<DayPeriodModel>();
                            classCourseTimes = TimeOperation.TimeSlotInterSect(classMustInterSect, courseTimeRule.MustTimes);

                            //每个班级的必须时间分别和各个班的交集时间对比
                            if (classCourseTimes.Count < courseLesson)
                            {
                                raClassUnion.DetailInfo.Add($"科目 {courseName} 下的合班规则(班级:{string.Join(",", classNames)})因课程必须时间设置冲突无法满足要求!");
                            }
                        }
                    });
                }

                //2.所有必须课位存在且都不充足
                if (noFullMustTimeClass == cu.ClassIDs.Count)
                {
                    List<DayPeriodModel> classMustUnion = new List<DayPeriodModel>();
                    //获取必须时间合集
                    cu.ClassIDs?.ForEach(ci => {
                        var courseTimeRule = rule?.CourseTimes?.FirstOrDefault(ct => ct.CourseID == cu.CourseID && ct.ClassID == ci && ct.Weight == 1);

                        if (courseTimeRule?.MustTimes != null)
                        {
                            classMustUnion = TimeOperation.TimeSlotUnion(classMustUnion, courseTimeRule.MustTimes);
                        }
                    });

                    //各班级的必须时间合集和班级课时比较
                    if (classMustUnion.Count > courseLesson)
                    {
                        raClassUnion.DetailInfo.Add($"科目 {courseName} 下的合班规则(班级:{string.Join(",", classNames)})因课程必须时间设置冲突无法满足要求!");
                    }
                }

                //检查合班的班级单双周设置，是否存在一班单周和其他班单周科目不同
                List<string> oddCourseIDList = new List<string>();
                List<string> dualCouseIDList = new List<string>();

                cu.ClassIDs?.ForEach(cid => {
                    rule?.OddDualWeeks?.Where(od => od.ClassID == cid && od.OddWeekCourseID == cu.CourseID)?.ToList()?.ForEach(od => {
                        oddCourseIDList.Add(od.OddWeekCourseID);
                        dualCouseIDList.Add(od.DualWeekCourseID);
                    });

                    rule?.OddDualWeeks?.Where(od => od.ClassID == cid && od.DualWeekCourseID == cu.CourseID)?.ToList()?.ForEach(od => {
                        oddCourseIDList.Add(od.OddWeekCourseID);
                        dualCouseIDList.Add(od.DualWeekCourseID);
                    });
                });

                if (oddCourseIDList.Distinct().Count() > 1 || dualCouseIDList.Distinct().Count() > 1)
                {
                    raClassUnion.DetailInfo.Add($"科目 {courseName} 下的合班规则(班级:{string.Join(",", classNames)}) 因课程单双周设置冲突无法满足要求!");
                }
            });

            arList.Add(raClassUnion);
            #endregion

            #region 课时分散
            RuleAnalysisDetailResult raClassAverages = new RuleAnalysisDetailResult();
            raClassAverages.DetailInfo = new List<string>();
            raClassAverages.RuleName = "课时分散";

            rule?.ClassHourAverages?.Where(x => x.Weight == 1)?.ToList()?.ForEach(cha => {
                //1. 方案课位/课程禁止/教师禁止/教师必须/课程必须等 对课时分散冲突的影响
                string classCourseName = Miscellaneous.GetCPClassCourseInfo(cPCase, cha.ClassID, cha.CourseID);
                List<DayPeriodModel> classCourseAvailable = caseAvailableDayPeriod.ToList();
                int classCourseLesson = cPCase?.Classes?.FirstOrDefault(cl => cl.ID == cha.ClassID)?.Settings?.FirstOrDefault(se => se.CourseID == cha.CourseID)?.Lessons ?? 0;
                //连排数
                int continousNumber = rule?.CourseArranges?.FirstOrDefault(ca => ca.TimesWeight == 1 && ca.ClassID == cha.ClassID && ca.CourseID == cha.CourseID)?.Count ?? 0;
                continousNumber = Math.Min(continousNumber, classCourseLesson/2);
                //虚拟课时
                int virtualLesson = classCourseLesson - continousNumber;
                //是否有连排
                bool hasContinousLesson = continousNumber > 0 ? true : false;

                //2. 课程必须禁止对课时分散冲突的影响
                var ruleCourseTimes = rule?.CourseTimes.FirstOrDefault(ct => ct.Weight == 1 && ct.ClassID == cha.ClassID && ct.CourseID == cha.CourseID);
                if (ruleCourseTimes != null)
                {
                    bool dayEnough = true;
                    List<bool> dayEnoughList = new List<bool>() { };

                    if (ruleCourseTimes.ForbidTimes != null)
                    {
                        classCourseAvailable = TimeOperation.TimeSlotDiff(classCourseAvailable, ruleCourseTimes.ForbidTimes);
                        dayEnough = Miscellaneous.DaysEnoughForEvenlyDistributed(classCourseAvailable, virtualLesson, cha.MinDay);
                        dayEnoughList.Add(dayEnough);
                    }

                    if (ruleCourseTimes.MustTimes != null)
                    {
                        List<DayPeriodModel> courseMustTime = TimeOperation.TimeSlotInterSect(classCourseAvailable, ruleCourseTimes.MustTimes);

                        if (ruleCourseTimes.MustTimes.Count >= classCourseLesson)
                        {
                            dayEnough = Miscellaneous.DaysEnoughForEvenlyDistributed(courseMustTime, virtualLesson, cha.MinDay);
                            dayEnoughList.Add(dayEnough);
                        }
                        else
                        {
                            //课时不够时，判断这些课位是否分散
                            int adjustedLesson = courseMustTime.Count;
                            if (continousNumber > 0)
                            {
                                int holdContinous = Math.Min(continousNumber, courseMustTime.Count / 2);
                                adjustedLesson = holdContinous + courseMustTime.Count - holdContinous * 2;
                            }

                            dayEnough = Miscellaneous.DaysEnoughForEvenlyDistributed(courseMustTime, adjustedLesson, cha.MinDay);
                            dayEnoughList.Add(dayEnough);
                        }
                    }

                    if (dayEnoughList.Exists(de => de == false))
                    {
                        raClassAverages.DetailInfo.Add($"{classCourseName} 结合课程排课时间规则无法满足课时分散!");
                    }
                }
                else
                {
                    bool dayEnough = Miscellaneous.DaysEnoughForEvenlyDistributed(classCourseAvailable, virtualLesson, cha.MinDay);

                    if (!dayEnough)
                    {
                        raClassAverages.DetailInfo.Add($"{classCourseName} 无法满足课时分散规则!");
                    }
                }

                //3. 教师必须禁止对课时分散冲突的影响
                var courseClassTeacherInfo = cPCase?.ClassHours?.Where(ch => ch.ClassID == cha.ClassID && ch.CourseID == cha.CourseID && ch.TeacherIDs != null)?.SelectMany(ch => ch.TeacherIDs)?.ToList() ?? new List<string>();
                int teachedLesson = 0;
                if (courseClassTeacherInfo.Count == classCourseLesson && courseClassTeacherInfo.Distinct().Count() == 1)
                {
                    //如果仅有1名教师且教师教所有课时
                    teachedLesson = classCourseLesson;
                }

                var ruleTeacherTimes = rule?.TeacherTimes.FirstOrDefault(tt => tt.Weight == 1 && courseClassTeacherInfo.Count > 0 && tt.TeacherID == courseClassTeacherInfo.FirstOrDefault());
                if (ruleTeacherTimes != null && teachedLesson == classCourseLesson)
                {
                    if (ruleTeacherTimes.ForbidTimes != null)
                    {
                        classCourseAvailable = TimeOperation.TimeSlotDiff(classCourseAvailable, ruleTeacherTimes.ForbidTimes);
                        bool dayEnough = Miscellaneous.DaysEnoughForEvenlyDistributed(classCourseAvailable, virtualLesson, cha.MinDay);

                        if (!dayEnough)
                        {
                            raClassAverages.DetailInfo.Add($"{classCourseName} 结合教师时间规则无法满足课时分散!");
                        }
                    }
                }
            });

            arList.Add(raClassAverages);
            #endregion

            #region 从班级的角度检查部分规则
            //1.如果有多个教师教同一个班级的同一科目，必须时间不同造成冲突
            cPCase?.Classes?.ForEach(cl =>
            {
                cl.Settings?.ForEach(cs =>
                {
                    //有多个教师教同一个班级，必须时间不同造成冲突
                    var teachers = cPCase.ClassHours.Where(ch => ch.ClassID == cl.ID && ch.CourseID == cs.CourseID && ch.TeacherIDs != null && ch.TeacherIDs.Count > 1)?.SelectMany(ch => ch.TeacherIDs)?.Distinct()?.ToList() ?? new List<string>();

                    if (teachers.Count > 1)
                    {
                        List<DayPeriodModel> teachersForbidTimes = new List<DayPeriodModel>() { };

                        teachers.Sort();
                        //查出同时包含这几个教师的课时，注意这些老师理论上也可能在其他班级同时教课
                        var commonLesson = cPCase.ClassHours.Where(ch => ch.TeacherIDs != null && ch.TeacherIDs.Intersect(teachers).Count() == teachers.Count);
                        Dictionary<string, List<DayPeriodModel>> teacherMustTimeAndLessonInfo = new Dictionary<string, List<DayPeriodModel>>();
                        bool teacherMustEnough = true;
                        List<string> teacherNames = new List<string>();

                        teachers.ForEach(te =>
                        {
                            //计算所有教师的准确必须时间，因为原始必须时间中的部分可能是在方案中不可用的
                            List<DayPeriodModel> singleTeacherMustTimes = TimeOperation.TimeSlotInterSect(caseAvailableDayPeriod, rule?.TeacherTimes?.FirstOrDefault(tt => tt.TeacherID == te)?.MustTimes);
                            //求各个教师禁止时间的合集
                            teachersForbidTimes = TimeOperation.TimeSlotUnion(teachersForbidTimes, rule?.TeacherTimes?.FirstOrDefault(tt => tt.TeacherID == te)?.ForbidTimes);

                            //教师总课时
                            int teacherTotalLesson = cPCase.ClassHours.Where(ch => ch.TeacherIDs != null && ch.TeacherIDs.Contains(te)).Count();
                            if (teacherTotalLesson > singleTeacherMustTimes.Count)
                            {
                                teacherMustEnough = false;
                            }

                            teacherMustTimeAndLessonInfo.Add(te, singleTeacherMustTimes);
                            teacherNames.Add(cPCase.Teachers?.FirstOrDefault(t => t.ID == te)?.Name ?? string.Empty);
                        });

                        /* 各教师的公共课时的必须时间都充足，检查必须时间交集是否足够 */
                        if ((commonLesson?.Count() ?? 0) > 0)
                        {
                            if (teacherMustEnough)
                            {
                                List<DayPeriodModel> teacherMustCommon = caseAvailableDayPeriod.ToList();

                                foreach (var item in teacherMustTimeAndLessonInfo)
                                {
                                    teacherMustCommon = TimeOperation.TimeSlotInterSect(teacherMustCommon, item.Value);
                                }
                                //从必须公共时间合集移除所有教师禁止时间
                                teacherMustCommon = TimeOperation.TimeSlotDiff(teacherMustCommon, teachersForbidTimes);

                                if (teacherMustCommon.Count < commonLesson.Count())
                                {
                                    arTeacherTime.DetailInfo.Add($"{string.Join(",", teacherNames)} 的教师必须禁止时间规则冲突，他们有 {commonLesson.Count()} 个共同上课课时，但是只有 {teacherMustCommon.Count} 个公共课位!");
                                }
                            }
                        }
                    }
                });
            });

            //2.从班级维度分析方案禁止时间冲突
            cPCase?.Classes?.ForEach(cl => {
                //计算出的各个课位能让容纳的课时（最大为1，所以主要是计算哪些课位不能容纳课时）
                List<int> classDayPeriodCanHoldMaxLesson = new List<int> { };
                //班级总课时
                int totalClassLesson = cPCase?.ClassHours?.Where(ch => ch.ClassID == cl.ID)?.Count() ?? 0;

                caseAvailableDayPeriod.ForEach(pd => {
                    //课位禁止的教师
                    List<string> forbidTeachers = rule?.TeacherTimes?.Where(tt => tt.ForbidTimes != null && tt.ForbidTimes.Exists(ft => ft.Day == pd.Day && ft.Period == pd.Period))?
                                                                    .Select(t => t.TeacherID)?.ToList() ?? new List<string>();

                    //课位禁止的班级课程
                    var forbidClassCourse = from cs in rule?.CourseTimes
                                            where cs.ForbidTimes != null && cs.ForbidTimes.Exists(ft => ft.Day == pd.Day && ft.Period == pd.Period) && cs.ClassID == cl.ID
                                            select new { cs.CourseID };
                    
                    var notForbidLesson = from ch in cPCase?.ClassHours
                                          select ch;

                    if (forbidTeachers.Count > 0)
                    {
                        notForbidLesson = (from ch in notForbidLesson
                                           where (ch.TeacherIDs == null || ch.TeacherIDs.Intersect(forbidTeachers).Count() == 0)
                                           select ch);
                    }

                    if ((forbidClassCourse?.Count() ?? 0) > 0)
                    {
                        notForbidLesson = (from ch in notForbidLesson
                                           where !forbidClassCourse.Any(fcc => fcc.CourseID == ch.CourseID)
                                           select ch);
                    }

                    classDayPeriodCanHoldMaxLesson.Add(Math.Min(1, notForbidLesson.Count()));
                });

                if (classDayPeriodCanHoldMaxLesson.Sum() < totalClassLesson)
                {
                    string courseName = cPCase?.Classes?.FirstOrDefault(cla => cla.ID == cl.ID)?.Name ?? string.Empty;
                    arCourseTime.DetailInfo.Add($"课程和教师的禁止规则导致班级 {courseName} 的课位只能容纳 {classDayPeriodCanHoldMaxLesson.Sum()} 个课时，而班级总计有 {totalClassLesson} 个课时!");
                }
            });
            
            //3.根据各个课时的可用时间统计出冲突
            List<string> timesTeachers = rule?.TeacherTimes?.Select(tt => tt.TeacherID)?.ToList() ?? new List<string>() { };
            List<ClassCourse> classCourses = new List<ClassCourse>() { };

            globalLessonsAvailableTimesWithTeacherCourseRule?.ForEach(gl => {
                if (gl.AvailableTimes.Count == 0)
                {
                    if (!classCourses.Exists( cc => cc.ClassID == gl.ClassID && cc.CourseID == gl.CourseID))
                    {
                        string classCourseName = Miscellaneous.GetCPClassCourseInfo(cPCase, gl.ClassID, gl.CourseID);
                        classCourses.Add(new ClassCourse() { CourseID = gl.CourseID, ClassID = gl.ClassID });

                        if (rule?.CourseTimes?.Exists(ct => ct.ClassID == gl.ClassID && ct.CourseID == gl.CourseID) ?? false)
                        {
                            arCourseTime.DetailInfo.Add($"课程和教师的必须禁止规则导致 {classCourseName} 无足够可用课位!");
                        }

                        List<string> teachers = cPCase?.ClassHours?.Where(ch => ch.ClassID == gl.ClassID && ch.CourseID == gl.CourseID)?.SelectMany(ch => ch.TeacherIDs)?.Distinct()?.ToList() ?? new List<string>();

                        if (teachers.Intersect(timesTeachers).Count() > 0)
                        {
                            arTeacherTime.DetailInfo.Add($"课程和教师的必须禁止规则导致 {classCourseName} 无足够可用课位!");
                        }
                    }
                }
            });

            //4.同班下不同科目的两教师必须时间重复
            cPCase?.Classes?.ForEach(cl => {
                List<CourseGroup> courseGroup = new List<CourseGroup>() { };

                globalClassCourseAvailableTimesWithTeacherRule.Where(cr => cr.ClassID == cl.ID).ToList().ForEach(co => {
                    globalClassCourseAvailableTimesWithTeacherRule.Where(cr => cr.ClassID == cl.ID && cr.CourseID != co.CourseID).ToList()?.ForEach(col => {
                        if (!courseGroup.Exists(cg => cg.FirstID == co.CourseID && cg.SecondID == col.CourseID) &&
                            !courseGroup.Exists(cg => cg.SecondID == co.CourseID && cg.FirstID == col.CourseID))
                        {
                            int twoCourseMustLessons = co.MustLesson + col.MustLesson;
                            List<DayPeriodModel> unionTimes = TimeOperation.TimeSlotUnion(co.AvailableTimes, col.AvailableTimes);
                            if (unionTimes.Count < twoCourseMustLessons)
                            {
                                string classCourseNameFirst = Miscellaneous.GetCPClassCourseInfo(cPCase, cl.ID, co.CourseID);
                                string classCourseNameSecond = Miscellaneous.GetCPClassCourseInfo(cPCase, cl.ID, col.CourseID);
                                arCourseTime.DetailInfo.Add($"教师的必须禁止规则导致 {classCourseNameFirst} 和 {classCourseNameSecond} 时间冲突，无足够可用课位!");
                            }

                            courseGroup.Add(new CourseGroup() { FirstID = co.CourseID, SecondID = col.CourseID });
                        }
                    });
                });
            });

            //5.同班两课程必须时间重复
            cPCase?.Classes?.ForEach(cl => {
                List<CourseGroup> courseGroup = new List<CourseGroup>() { };

                globalClassCourseAvailableTimesWithCourseRule.Where(cr => cr.ClassID == cl.ID).ToList().ForEach(co => {
                    globalClassCourseAvailableTimesWithCourseRule.Where(cr => cr.ClassID == cl.ID && cr.CourseID != co.CourseID).ToList()?.ForEach(col => {
                        if (!courseGroup.Exists(cg => cg.FirstID == co.CourseID && cg.SecondID == col.CourseID) && 
                            !courseGroup.Exists(cg => cg.SecondID == co.CourseID && cg.FirstID == col.CourseID))
                        {
                            int twoCourseMustLessons = co.MustLesson + col.MustLesson;
                            List<DayPeriodModel> unionTimes = TimeOperation.TimeSlotUnion(co.AvailableTimes, col.AvailableTimes);
                            if (unionTimes.Count < twoCourseMustLessons)
                            {
                                string classCourseNameFirst = Miscellaneous.GetCPClassCourseInfo(cPCase, cl.ID, co.CourseID);
                                string classCourseNameSecond = Miscellaneous.GetCPClassCourseInfo(cPCase, cl.ID, col.CourseID);
                                arCourseTime.DetailInfo.Add($"课程的必须禁止规则导致 {classCourseNameFirst} 和 {classCourseNameSecond} 时间冲突，无足够可用课位!");
                            }

                            courseGroup.Add(new CourseGroup() { FirstID = co.CourseID, SecondID = col.CourseID });
                        }
                    });
                });
            });

            #endregion

            #region 从课位维度分析方案禁止时间冲突
            //计算出的各个课位能让容纳的最大课时
            List<int> dayPeriodCanHoldMaxLesson = new List<int> { };
            bool existsForbidTeacher = false;
            bool existsForbidClassCourse = false;

            caseAvailableDayPeriod.ForEach(pd => {
                //课位禁止的教师
                List<string> forbidTeachers = rule?.TeacherTimes?.Where(tt => tt.ForbidTimes != null && tt.ForbidTimes.Exists(ft => ft.Day == pd.Day && ft.Period == pd.Period))?
                                                                .Select(t => t.TeacherID)?.ToList() ?? new List<string>();

                //课位禁止的班级课程
                var forbidClassCourse = from cs in rule?.CourseTimes
                                        where cs.ForbidTimes != null && cs.ForbidTimes.Exists(ft => ft.Day == pd.Day && ft.Period == pd.Period)
                                        select new { cs.ClassID, cs.CourseID };

                var notForbidLesson = from ch in cPCase?.ClassHours
                                      select ch;

                if (forbidTeachers.Count > 0)
                {
                    existsForbidTeacher = true;

                    notForbidLesson = (from ch in notForbidLesson
                                       where (ch.TeacherIDs == null || ch.TeacherIDs.Intersect(forbidTeachers).Count() == 0)
                                       select ch);
                }

                if (forbidClassCourse.Any())
                {
                    existsForbidClassCourse = true;

                    notForbidLesson = (from ch in notForbidLesson
                                        where !forbidClassCourse.Any(fcc => fcc.ClassID == ch.ClassID && fcc.CourseID == ch.CourseID)
                                        select ch);
                }

                dayPeriodCanHoldMaxLesson.Add(Math.Min(maxDayPeriodHoldLesson, notForbidLesson.Count()));
            });

            if (dayPeriodCanHoldMaxLesson.Sum(x => x) < totalLesson)
            {
                if (existsForbidTeacher)
                {
                    arTeacherTime.DetailInfo.Add($"教师禁止时间规则导致方案只能容纳 {dayPeriodCanHoldMaxLesson.Sum(x => x)} 个课时，而方案总计有 {totalLesson} 个课时!");
                }

                if (existsForbidClassCourse)
                {
                    arCourseTime.DetailInfo.Add($"课程禁止时间规则导致方案只能容纳 {dayPeriodCanHoldMaxLesson.Sum(x => x)} 个课时，而方案总计有 {totalLesson} 个课时!");
                }
            }
            #endregion

            return arList;
        }
        #endregion

        /// <summary>
        /// 判断是否能容下指定的连排
        /// </summary>
        /// <param name="cPCase"></param>
        /// <param name="availableDayPeriod"></param>
        /// <param name="ContinousNumber"></param>
        /// <param name="noCrossBreak"></param>
        /// <returns>返回结果中第一个参数表示是否能容纳连排数，第二个参数表示能容纳连排的天中的一个课位</returns>
        private static Tuple<bool, List<DayPeriodModel>> CanHoldContinous(CPCase cPCase, List<DayPeriodModel> availableDayPeriod, int ContinousNumber, bool noCrossBreak)
        {
            bool result = true;
            List<DayPeriodModel> dayList = new List<DayPeriodModel>();

            #region 构造映射表
            List<int> mappingFullOld = new List<int>();
            List<int> mappingFullNew = new List<int>();

            int tempI = 0;

            cPCase?.Positions?.Where(p => p.DayPeriod.Day == DayOfWeek.Monday)?.ToList()?.OrderBy(p => p.PositionOrder)?.ToList()?.ForEach(p => {
                if (noCrossBreak)
                {
                    if (p.Position != XYKernel.OS.Common.Enums.Position.AB && p.Position != XYKernel.OS.Common.Enums.Position.PB && p.Position != XYKernel.OS.Common.Enums.Position.Noon)
                    {
                        mappingFullOld.Add(p.DayPeriod.Period);
                        mappingFullNew.Add(tempI);
                    }

                    tempI++;
                }
                else
                {
                    if (p.Position != XYKernel.OS.Common.Enums.Position.AB && p.Position != XYKernel.OS.Common.Enums.Position.PB && p.Position != XYKernel.OS.Common.Enums.Position.Noon)
                    {
                        mappingFullOld.Add(p.DayPeriod.Period);
                        mappingFullNew.Add(tempI);

                        tempI++;
                    }
                }
            });
            #endregion

            List<DayOfWeek> dof = availableDayPeriod.Select(d => d.Day).Distinct().ToList();
            int holdContinousNumber = 0;

            dof.ForEach(d => {
                List<DayPeriodModel> oneDay = availableDayPeriod.Where(p => p.Day == d).OrderBy(p => p.Period).ToList();
                List<int> mappingSelectedDayOld = new List<int>();
                List<int> mappingSelectedDayNew = new List<int>();

                oneDay.ForEach(od => {
                    mappingSelectedDayOld.Add(od.Period);
                    int iIndex = mappingFullOld.IndexOf(od.Period);
                    int newValue = mappingFullOld[iIndex];
                    mappingSelectedDayNew.Add(newValue);
                });

                int continousNumberOneDay = Miscellaneous.ContinuousNumber(mappingSelectedDayNew);

                if (continousNumberOneDay > 0)
                {
                    dayList.Add(oneDay.FirstOrDefault());
                    holdContinousNumber = holdContinousNumber + continousNumberOneDay;
                }
            });

            if (holdContinousNumber < ContinousNumber)
            {
                result = false;
            }

            return Tuple.Create(result, dayList);
        }
    }

    class ClassCourseDetailLesson
    { 
        public int ID { get; set; }
        public string ClassID { get; set; }
        public string CourseID { get; set; }
        public List<DayPeriodModel> AvailableTimes { get; set; } = new List<DayPeriodModel> { };
    }

    class ClassCourseAvailableTimes
    {
        public string ClassID { get; set; }
        public string CourseID { get; set; }
        public int Lesson { get; set; }
        public int MustLesson { get; set; }
        public List<DayPeriodModel> AvailableTimes { get; set; } = new List<DayPeriodModel> { };
    }
    class ClassCourse { 
        public string ClassID { get; set; } 
        public string CourseID { get; set; } 
    }

    class CourseGroup { 
        public string FirstID { get; set; }
        public string SecondID { get; set; }
    }
}