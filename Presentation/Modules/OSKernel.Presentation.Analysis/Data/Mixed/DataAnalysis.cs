using System.Collections.Generic;
using System.Linq;
using XYKernel.OS.Common.Models.Mixed;
using XYKernel.OS.Common.Models.Mixed.Rule;
using OSKernel.Presentation.Analysis.Data.Mixed.Models;
using XYKernel.OS.Common.Models;
using OSKernel.Presentation.Analysis.Utilities;
using System.Text;
using System;

namespace OSKernel.Presentation.Analysis.Data.Mixed
{
    public class DataAnalysis
    {
        #region 综合
        public static int GetCaseAvailableTimeSlot(CLCase cLCase)
        {
            return cLCase?.Positions?.Where(p => p.IsSelected
                                       && p.Position != XYKernel.OS.Common.Enums.Position.AB
                                       && p.Position != XYKernel.OS.Common.Enums.Position.Noon
                                       && p.Position != XYKernel.OS.Common.Enums.Position.PB)?.Count() ?? 0;
        }

        /// <summary>
        /// 检查方案课时是否够用
        /// </summary>
        /// <param name="cLCase"></param>
        /// <returns></returns>
        public static GeneralTimeSlotAnalysisResult GetTimeSlotAnalysisResult(CLCase cLCase)
        {
            int teacherMaxHour = 0;
            int studentMaxHour = 0;

            cLCase?.Teachers?.ForEach(t =>
            {
                int teacherHours = cLCase?.ClassHours?.Where(ch => ch.TeacherIDs != null && ch.TeacherIDs.Contains(t.ID))?.Count() ?? 0;
                teacherMaxHour = System.Math.Max(teacherHours, teacherMaxHour);
            });

            cLCase?.Students?.ForEach(s =>
            {
                int studentHours = 0;
                s.Preselections?.ForEach(pr =>
                {
                    studentHours += cLCase?.Courses?.FirstOrDefault(co => co.ID == pr.CourseID)?.Levels?.FirstOrDefault(le => le.ID == pr.LevelID)?.Lessons ?? 0;
                });

                studentMaxHour = System.Math.Max(studentHours, studentMaxHour);
            });

            GeneralTimeSlotAnalysisResult ar = new GeneralTimeSlotAnalysisResult();
            ar.CaseAvailableSlot = GetCaseAvailableTimeSlot(cLCase);
            ar.CaseMinimizedSlot = System.Math.Max(teacherMaxHour, studentMaxHour);
            ar.FreeSlotNumber = ar.CaseAvailableSlot - ar.CaseMinimizedSlot;

            return ar;
        }

        /// <summary>
        /// 检查教室是否够用
        /// </summary>
        /// <param name="cLCase"></param>
        /// <returns></returns>
        public static GeneralRoomAnalysisResult GetRoomAnalysisResult(CLCase cLCase)
        {
            int minRoomNumber = 0;
            int totalClassHours = cLCase?.ClassHours?.Count ?? 0;
            int availableCaseTimeSlot = GetCaseAvailableTimeSlot(cLCase);
            if (availableCaseTimeSlot > 0)
            {
                minRoomNumber = (int)System.Math.Ceiling(totalClassHours / (double)availableCaseTimeSlot);
            }

            GeneralRoomAnalysisResult ar = new GeneralRoomAnalysisResult();
            ar.CaseAvailableRoom = cLCase.RoomLimit;
            ar.CaseMinimizedRoom = minRoomNumber;
            ar.FreeRoomNumber = cLCase.RoomLimit == 0 ? 0 : ar.CaseAvailableRoom - ar.CaseMinimizedRoom;
            return ar;
        }

        /// <summary>
        /// 统计教师课时
        /// </summary>
        /// <param name="cLCase"></param>
        /// <returns></returns>
        public static List<GeneralTeacherAnalysisResult> GetGeneralTeacherAnalysisResult(CLCase cLCase)
        {
            int no = 0;

            int caseAvailableSlot = GetCaseAvailableTimeSlot(cLCase);

            List<GeneralTeacherAnalysisResult> arList = new List<GeneralTeacherAnalysisResult>();
            cLCase?.Teachers?.ForEach(t =>
            {
                ++no;

                GeneralTeacherAnalysisResult ar = new GeneralTeacherAnalysisResult();
                ar.NO = no;
                ar.Teacher = t;

                var result = cLCase?.ClassHours?
                            .Where(ch => ch.TeacherIDs != null && ch.TeacherIDs.Contains(t.ID))?
                            .GroupBy(x => new { x.ClassID, x.CourseID, x.LevelID })?
                            .Select(g => new
                            {
                                g.Key.ClassID,
                                g.Key.CourseID,
                                g.Key.LevelID,
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
        /// 统计学生课时
        /// </summary>
        /// <param name="cLCase"></param>
        /// <returns></returns>
        public static List<GeneralStudentAnalysisResult> GetGeneralStudentAnalysisResult(CLCase cLCase)
        {
            int no = 0;

            List<GeneralStudentAnalysisResult> arList = new List<GeneralStudentAnalysisResult>();
            List<StudentLessonInfo> studentInfoList = new List<StudentLessonInfo>();

            int caseAvailableSlot = GetCaseAvailableTimeSlot(cLCase);

            cLCase?.Students?.ForEach(s =>
            {
                int lesson = 0;
                s.Preselections?.ForEach(pr =>
                {
                    lesson += cLCase?.Courses?.FirstOrDefault(co => co.ID == pr.CourseID)?.Levels?.FirstOrDefault(le => le.ID == pr.LevelID)?.Lessons ?? 0;
                });

                StudentLessonInfo si = new StudentLessonInfo();
                si.StudentID = s.ID;
                si.Lesson = lesson;
                studentInfoList.Add(si);
            });

            var studentStatistic = studentInfoList.GroupBy(s => new { s.Lesson })
                                    .Select(s => new { s.Key.Lesson, Count = s.Count() });

            studentStatistic?.ToList()?.ForEach(ss =>
            {

                ++no;
                arList.Add(new GeneralStudentAnalysisResult()
                {
                    NO = no,
                    StudentNumber = ss.Count,
                    Lesson = ss.Lesson,
                    FreeSlotNumber = caseAvailableSlot - ss.Lesson
                });
            });

            return arList;
        }

        /// <summary>
        /// 统计规则
        /// </summary>
        /// <param name="cLCase"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        public static List<RuleAnalysisResult> GetRuleAnalysisResult(CLCase cLCase, Rule rule)
        {
            List<RuleAnalysisResult> arList = new List<RuleAnalysisResult>();
            List<RuleAnalysisDetailResult> radr = GetRuleAnalysisDetailResult(cLCase, rule);

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
        /// <param name="cLCase"></param>
        /// <returns></returns>
        public static List<TeacherClassAnalysisResult> GetTeacherClassAnalysisResult(CLCase cLCase, Rule rule)
        {
            int no = 0;

            List<TeacherClassAnalysisResult> arList = new List<TeacherClassAnalysisResult>();
            cLCase?.Teachers?.ForEach(t =>
            {

                var result = cLCase?.ClassHours?
                            .Where(ch => ch.TeacherIDs != null && ch.TeacherIDs.Contains(t.ID))?
                            .GroupBy(x => new { x.ClassID, x.CourseID, x.LevelID })?
                            .Select(g => new
                            {
                                g.Key.ClassID,
                                g.Key.CourseID,
                                g.Key.LevelID,
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
                    ar.LevelID = r.LevelID;
                    ar.TeacherLesson = r.Count;
                    ar.ClassName = cLCase?.Classes?.FirstOrDefault(cl => cl.ID == r.ClassID && cl.CourseID == r.CourseID && cl.LevelID == r.LevelID)?.Name ?? string.Empty;
                    ar.CourseName = cLCase?.Courses?.FirstOrDefault(co => co.ID == r.CourseID)?.Name ?? string.Empty;
                    ar.LevelName = cLCase?.Courses?.FirstOrDefault(cl => cl.ID == r.ClassID)?.Levels?.FirstOrDefault(le => le.ID == r.LevelID)?.Name ?? string.Empty;
                    ar.LevelLesson = cLCase?.Courses?.FirstOrDefault(cl => cl.ID == r.ClassID)?.Levels?.FirstOrDefault(le => le.ID == r.LevelID)?.Lessons ?? 0;
                    ar.Continous = rule?.ArrangeContinuous?.FirstOrDefault(ca => ca.ClassID == r.ClassID)?.Count ?? 0;

                    arList.Add(ar);
                });
            });

            return arList;
        }
        #endregion

        #region 课程
        /// <summary>
        /// 课程信息统计
        /// </summary>
        /// <param name="cLCase"></param>
        /// <returns></returns>
        public static List<CourseAnalysisResult> GetCourseAnalysisResult(CLCase cLCase)
        {
            int no = 0;

            List<CourseAnalysisResult> arList = new List<CourseAnalysisResult>();
            cLCase?.Courses?.ForEach(co =>
            {
                co.Levels?.ForEach(le =>
                {
                    ++no;

                    CourseAnalysisResult ar = new CourseAnalysisResult();
                    ar.NO = no;
                    ar.CourseID = co.ID;
                    ar.CourseName = co.Name;
                    ar.LevelID = le.ID;
                    ar.LevelName = le.Name;
                    ar.ClassNumber = cLCase?.Classes?.Where(cl => cl.CourseID == co.ID && cl.LevelID == le.ID)?.Count() ?? 0;
                    ar.StudentNumber = cLCase?.Students?.Where(x => x.Preselections.Exists(pr => pr.CourseID == co.ID && pr.LevelID == le.ID))?.Count() ?? 0;

                    arList.Add(ar);
                });
            });
            return arList;
        }
        #endregion

        #region 班级
        /// <summary>
        /// 班级信息统计
        /// </summary>
        /// <param name="cLCase"></param>
        /// <returns></returns>
        public static List<ClassAnalysisResult> GetClassAnalysisResult(CLCase cLCase)
        {
            int no = 0;

            List<ClassAnalysisResult> arList = new List<ClassAnalysisResult>();

            cLCase?.Courses?.ForEach(co =>
            {
                co.Levels?.ForEach(le =>
                {

                    int classNumber = cLCase?.Classes?.Where(cl => cl.CourseID == co.ID && cl.LevelID == le.ID)?.Count() ?? 0;
                    int studentNumber = cLCase?.Students?.Where(x => x.Preselections.Exists(pr => pr.CourseID == co.ID && pr.LevelID == le.ID))?.Count() ?? 0;

                    cLCase?.Classes?.Where(cl => cl.CourseID == co.ID && cl.LevelID == le.ID)?.ToList()?.ForEach(cl =>
                    {
                        ++no;

                        ClassAnalysisResult ar = new ClassAnalysisResult();
                        ar.NO = no;
                        ar.ClassID = cl.ID;
                        ar.ClassName = cl.Name;
                        ar.CourseID = co.ID;
                        ar.CourseName = co.Name;
                        ar.LevelID = le.ID;
                        ar.LevelName = le.Name;
                        ar.Capacity = cl.Capacity;
                        if (classNumber > 0)
                        {
                            ar.AveCapacity = (int)Math.Ceiling(studentNumber / (double)classNumber);
                        }
                        ar.RedundantCapacity = ar.Capacity - ar.AveCapacity;

                        arList.Add(ar);
                    });
                });
            });

            return arList;
        }
        #endregion

        #region 学生
        /// <summary>
        /// 学生信息统计
        /// </summary>
        /// <param name="cLCase"></param>
        /// <returns></returns>
        public static List<StudentLessonAnalysisResult> GetStudentLessonAnalysisResult(CLCase cLCase)
        {
            int no = 0;

            List<StudentLessonAnalysisResult> arList = new List<StudentLessonAnalysisResult>();
            int caseAvailableSlot = GetCaseAvailableTimeSlot(cLCase);

            cLCase?.Students?.ForEach(s =>
            {
                int lesson = 0;

                s.Preselections?.ForEach(pr =>
                {
                    lesson += cLCase?.Courses?.FirstOrDefault(co => co.ID == pr.CourseID)?.Levels?.FirstOrDefault(le => le.ID == pr.LevelID)?.Lessons ?? 0;
                });

                ++no;

                StudentLessonAnalysisResult ar = new StudentLessonAnalysisResult();
                ar.NO = no;
                ar.StudentID = s.ID;
                ar.StudentName = s.Name;
                ar.Lesson = lesson;
                ar.RedundantLesson = caseAvailableSlot - lesson;

                arList.Add(ar);
            });

            return arList;
        }

        public static List<StudentSelectionCombinationAnalysisResult> GetStudentSelectionCombinationAnalysisResult(CLCase cLCase)
        {
            int no = 0;

            List<StudentSelectionCombinationAnalysisResult> arList = new List<StudentSelectionCombinationAnalysisResult>();
            List<CombinationInfo> combinationInfoList = new List<CombinationInfo>();

            int caseAvailableSlot = GetCaseAvailableTimeSlot(cLCase);

            cLCase?.Students?.ForEach(s =>
            {
                int lesson = 0;

                s.Preselections?.ForEach(pr =>
                {
                    lesson += cLCase?.Courses?.FirstOrDefault(co => co.ID == pr.CourseID)?.Levels?.FirstOrDefault(le => le.ID == pr.LevelID)?.Lessons ?? 0;
                });

                CombinationInfo ci = new CombinationInfo();
                ci.Lesson = lesson;
                ci.CombinationName = AnalysisUtility.GetStringFormatByStudentPreselection(cLCase, s.Preselections);

                combinationInfoList.Add(ci);
            });

            var studentStatistic = combinationInfoList.GroupBy(s => new { s.CombinationName, s.Lesson })
                                    .Select(s => new { s.Key.CombinationName, s.Key.Lesson, Count = s.Count() });

            studentStatistic?.ToList()?.ForEach(ss =>
            {
                ++no;

                arList.Add(new StudentSelectionCombinationAnalysisResult()
                {
                    NO = no,
                    StudentNumber = ss.Count,
                    Lesson = ss.Lesson,
                    CombinationName = ss.CombinationName,
                    RedundantLesson = caseAvailableSlot - ss.Lesson
                });
            });

            return arList;
        }
        #endregion

        #region 规则
        public static List<RuleAnalysisDetailResult> GetRuleAnalysisDetailResult(CLCase cLCase, Rule rule)
        {
            #region Common
            List<RuleAnalysisDetailResult> arList = new List<RuleAnalysisDetailResult>();

            //方案可用时间
            List<DayPeriodModel> caseAvailableDayPeriod = cLCase?.Positions?.Where(p => p.IsSelected
                                       && p.Position != XYKernel.OS.Common.Enums.Position.AB
                                       && p.Position != XYKernel.OS.Common.Enums.Position.Noon
                                       && p.Position != XYKernel.OS.Common.Enums.Position.PB)?.Select(x => x.DayPeriod)?.ToList() ?? new List<DayPeriodModel>();

            //班级课程课时对应关系
            var classCourseMapping = from x in cLCase?.Classes
                                     from y in cLCase?.Courses
                                     from z in y.Levels
                                     where x.CourseID == y.ID && x.LevelID == z.ID
                                     select new { x.ID, x.CourseID, x.LevelID, z.Lessons };

            //每个课时的可用时间(结合课程、教师)
            List<ClassCourseDetailLesson> globalLessonsAvailableTimes = new List<ClassCourseDetailLesson>() { };
            cLCase?.ClassHours?.ForEach(ch => {

                ClassCourseDetailLesson ccdl = new ClassCourseDetailLesson() { };
                ccdl.ID = ch.ID;
                ccdl.ClassID = ch.ClassID;
                ccdl.AvailableTimes = caseAvailableDayPeriod.ToList();

                if (rule?.CourseTimes?.Exists(ct => ct.Weight == 1 && ct.ClassID == ch.ClassID) ?? false)
                {
                    CourseTimeRule ctr = rule.CourseTimes.FirstOrDefault(ct => ct.ClassID == ch.ClassID);
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

                globalLessonsAvailableTimes.Add(ccdl);
            });
            #endregion

            #region 教师排课时间
            RuleAnalysisDetailResult arTeacherTime = new RuleAnalysisDetailResult();
            arTeacherTime.DetailInfo = new List<string>();
            arTeacherTime.RuleName = "教师排课时间";

            rule?.TeacherTimes?.Where(x => x.Weight == 1)?.ToList()?.ForEach(tt => {
                //1.教师的必须和禁止有冲突
                List<DayPeriodModel> teacherMustForbidConf = TimeOperation.TimeSlotInterSect(tt.ForbidTimes, tt.MustTimes);
                string teacherName = cLCase?.Teachers?.FirstOrDefault(t => t.ID == tt.TeacherID)?.Name ?? string.Empty;

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
                List<int> teacherLesson = cLCase?.ClassHours?.Where(ch => ch.TeacherIDs != null && ch.TeacherIDs.Contains(tt.TeacherID))?.Select(ch => ch.ID)?.ToList() ?? new List<int>();
                List<DayPeriodModel> teacherAvailable = TimeOperation.TimeSlotDiff(caseAvailableDayPeriod, tt.ForbidTimes);
                if (teacherAvailable.Count < teacherLesson.Count)
                {
                    arTeacherTime.DetailInfo.Add($"教师 {teacherName} 的可用课位不足!");
                }

                //3.教师与课程必须禁止冲突检查
                rule.CourseTimes?.Where(x => x.Weight == 1)?.ToList()?.ForEach(x =>
                {
                    int classCourseLesson = cLCase?.ClassHours?.Where(ch => ch.ClassID == x.ClassID)?.Count() ?? 0;
                    int classCourseTeacherLesson = cLCase?.ClassHours?.Where(ch => ch.ClassID == x.ClassID && ch.TeacherIDs != null && ch.TeacherIDs.Contains(tt.TeacherID))?.Count() ?? 0;

                    //3.1 教师禁止与课程必须冲突检查
                    if (x.MustTimes != null && !string.IsNullOrEmpty(x.ClassID))
                    {
                        var shouldCheck = cLCase?.ClassHours?.Exists(c => c.ClassID == x.ClassID && c.TeacherIDs != null && c.TeacherIDs.Contains(tt.TeacherID)) ?? false;
                        if (shouldCheck)
                        {
                            List<DayPeriodModel> dpInterSect = TimeOperation.TimeSlotInterSect(x.MustTimes, tt.ForbidTimes);
                            
                            //如果本来必须时间足够
                            if (x.MustTimes.Count >= classCourseLesson)
                            {
                                if (x.MustTimes.Count - dpInterSect.Count < classCourseTeacherLesson)
                                {
                                    arTeacherTime.DetailInfo.Add($"教师 {teacherName} 的禁止时间与 {Miscellaneous.GetCLClassCourseInfo(cLCase, x.ClassID)} 的必须时间冲突!");
                                }
                            }
                            else
                            {
                                //如果剩下的课时放不满必须禁止重合的课时，提示错误
                                if (dpInterSect.Count > classCourseLesson - classCourseTeacherLesson)
                                {
                                    arTeacherTime.DetailInfo.Add($"教师 {teacherName} 的禁止时间与 {Miscellaneous.GetCLClassCourseInfo(cLCase, x.ClassID)} 的必须时间冲突!");
                                }
                            }
                        }
                    }

                    //3.2 教师必须与课程禁止冲突检查
                    if (tt.MustTimes != null && x.ForbidTimes != null && !string.IsNullOrEmpty(x.ClassID))
                    {
                        var shouldCheck = cLCase?.ClassHours?.Exists(c => c.ClassID == x.ClassID && c.TeacherIDs != null && c.TeacherIDs.Contains(tt.TeacherID)) ?? false;
                        if (shouldCheck)
                        {
                            List<DayPeriodModel> dpInterSect = TimeOperation.TimeSlotInterSect(x.ForbidTimes, tt.MustTimes);
                            
                            if (teacherLesson.Count == classCourseTeacherLesson && dpInterSect.Count > 0)
                            {
                                arTeacherTime.DetailInfo.Add($"教师 {teacherName} 的必须时间与 {Miscellaneous.GetCLClassCourseInfo(cLCase, x.ClassID)} 的禁止时间冲突!");
                            }
                        }
                    }

                    //3.3 教师禁止与课程禁止课位叠加
                    if (x.ForbidTimes != null)
                    {
                        List<DayPeriodModel> teacherCourseAvailable = TimeOperation.TimeSlotDiff(teacherAvailable, x.ForbidTimes);
                        if (teacherCourseAvailable.Count < classCourseTeacherLesson)
                        {
                            arTeacherTime.DetailInfo.Add($"教师 {teacherName} 在 {Miscellaneous.GetCLClassCourseInfo(cLCase, x.ClassID)} 的可用课位不足!");
                        }
                    }
                });

                //4.教师的必须禁止和课时分散的冲突
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
                string className = Miscellaneous.GetCLClassCourseInfo(cLCase, ct.ClassID);
                int classCourseLesson = cLCase?.ClassHours?.Where(ch => ch.ClassID == ct.ClassID)?.Count() ?? 0;

                if (courseMustForbidConf.Count > 0)
                {
                    StringBuilder sb = new StringBuilder(100);
                    sb.Append($"{className}的课程排课时间必须禁止之间存在冲突:");

                    courseMustForbidConf.ForEach(tc => {
                        string conflictDate = TimeOperation.GetDateInfo(tc);
                        sb.Append(conflictDate + ";");
                    });

                    arCourseTime.DetailInfo.Add(sb.ToString());
                }

                //2. 课程和教师必须禁止冲突
                rule?.TeacherTimes?.ForEach(tt => {
                    int classCourseTeacherLesson = cLCase?.ClassHours?.Where(ch => ch.ClassID == ct.ClassID && ch.TeacherIDs != null && ch.TeacherIDs.Contains(tt.TeacherID))?.Count() ?? 0;
                    string teacherName = cLCase?.Teachers?.FirstOrDefault(t => t.ID == tt.TeacherID)?.Name ?? string.Empty;

                    //2.1 课程必须与教师禁止冲突
                    List<DayPeriodModel> dpCourseMustInterSect = TimeOperation.TimeSlotInterSect(ct.MustTimes, tt.ForbidTimes);
                    if (dpCourseMustInterSect.Count > 0)
                    {
                        if (ct.MustTimes.Count <= classCourseLesson)
                        {
                            if (classCourseLesson - classCourseTeacherLesson < dpCourseMustInterSect.Count)
                            {
                                arCourseTime.DetailInfo.Add($"教师{teacherName}的禁止时间与{Miscellaneous.GetCLClassCourseInfo(cLCase, ct.ClassID)}的必须时间冲突!");
                            }
                        }
                    }

                    //2.2 课程禁止和教师必须冲突
                    List<DayPeriodModel> dpCourseForbidInterSect = TimeOperation.TimeSlotInterSect(ct.ForbidTimes, tt.MustTimes);
                    if (dpCourseForbidInterSect.Count > 0)
                    {
                        if (tt.MustTimes.Count == classCourseTeacherLesson)
                        {
                            arCourseTime.DetailInfo.Add($"教师{teacherName}的必须时间与{Miscellaneous.GetCLClassCourseInfo(cLCase, ct.ClassID)}的禁止时间冲突!");
                        }
                    }
                });

                //3. 课程禁止课时过多导致课位不足的检查
                List<DayPeriodModel> courseAvailable = TimeOperation.TimeSlotDiff(caseAvailableDayPeriod, ct.ForbidTimes);
                if (courseAvailable.Count < classCourseLesson)
                {
                    arCourseTime.DetailInfo.Add($"{Miscellaneous.GetCLClassCourseInfo(cLCase, ct.ClassID)}的禁止时间过多!");
                }

                //4. 课程必须禁止和课时分散的冲突
                rule?.ClassHourAverages?.Where(cha => cha.Weight == 1 && cha.ClassID == ct.ClassID)?.ToList()?.ForEach(cha => {
                    if (rule?.ArrangeContinuous?.Exists(ca => ca.ClassID == cha.ClassID) ?? false)
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
                            arCourseTime.DetailInfo.Add($"{Miscellaneous.GetCLClassCourseInfo(cLCase, ct.ClassID)}在现有规则下无法满足课时分散要求!");
                        }

                        if (availableDaysOfWeek.Count >= classCourseLesson && availableDaysOfWeek.Count > 1 && classCourseLesson > 1)
                        {
                            int dMax = availableIntDays.Max();
                            int dMin = availableIntDays.Min();

                            if (Math.Abs(dMax - dMin) < minGaps)
                            {
                                arCourseTime.DetailInfo.Add($"{Miscellaneous.GetCLClassCourseInfo(cLCase, ct.ClassID)}在现有规则下无法满足课时分散要求!");
                            }
                        }
                    }
                });

                //5.1 课程必须禁止与连排冲突检查 -- 课程必须与连排区域不重叠
                if (rule?.ArrangeContinuous?.Exists(ca => ca.TimesWeight == 1 && ca.ClassID == ct.ClassID && (ca.Times?.Count ?? 0) > 0) ?? false)
                {
                    var continousInfo = rule.ArrangeContinuous.FirstOrDefault(ca => ca.ClassID == ct.ClassID);
                    int continousMust = Math.Min(continousInfo.Count * 2, continousInfo.Times.Count);
                    List<DayPeriodModel> intersectTimes = TimeOperation.TimeSlotInterSect(ct.MustTimes, continousInfo.Times);

                    if (intersectTimes.Count < continousMust && ct.MustTimes.Count >= classCourseLesson)
                    {
                        arCourseTime.DetailInfo.Add($"{Miscellaneous.GetCLClassCourseInfo(cLCase, ct.ClassID)} 课程排课时间与连排时间冲突!");
                    }
                }

                //5.2 课程必须禁止与连排冲突检查 -- 课程的必须区域无法满足隔天连排
                if (rule?.ArrangeContinuous?.Exists(ca => ca.IsIntervalDay && ca.IntervalDayWeight == 1 && ca.ClassID == ct.ClassID && (ct.MustTimes?.Count ?? 0) > 0) ?? false)
                {
                    var continousInfo = rule.ArrangeContinuous.FirstOrDefault(ca => ca.ClassID == ct.ClassID);
                    if (ct.MustTimes.Count >= classCourseLesson)
                    {
                        bool isEnough = Miscellaneous.DaysEnoughForEvenlyDistributed(ct.MustTimes, continousInfo.Count, 2);

                        if (!isEnough)
                        {
                            arCourseTime.DetailInfo.Add($"{Miscellaneous.GetCLClassCourseInfo(cLCase, ct.ClassID)} 课程排课必须时间无法满足隔天连排!");
                        }
                    }
                }

                //5.3 课程必须禁止与连排冲突检查 -- 课程禁止后的可排区域无法满足隔天连排
                if (rule?.ArrangeContinuous?.Exists(ca => ca.IsIntervalDay && ca.IntervalDayWeight == 1 && ca.ClassID == ct.ClassID && (ct.ForbidTimes?.Count ?? 0) > 0) ?? false)
                {
                    var continousInfo = rule.ArrangeContinuous.FirstOrDefault(ca => ca.ClassID == ct.ClassID);
                    List<DayPeriodModel> intersectTimes = TimeOperation.TimeSlotDiff(caseAvailableDayPeriod, ct.ForbidTimes);
                    bool isEnough = Miscellaneous.DaysEnoughForEvenlyDistributed(intersectTimes, continousInfo.Count, 2);

                    if (!isEnough)
                    {
                        arCourseTime.DetailInfo.Add($"{Miscellaneous.GetCLClassCourseInfo(cLCase, ct.ClassID)} 课程排课禁止时间无法满足隔天连排!");
                    }
                }
            });

            arList.Add(arCourseTime);
            #endregion

            #region 课程连排
            RuleAnalysisDetailResult arCourseArrange = new RuleAnalysisDetailResult();
            arCourseArrange.DetailInfo = new List<string>();
            arCourseArrange.RuleName = "课程连排";

            rule?.ArrangeContinuous?.ForEach(ca => {

                List<DayPeriodModel> courseAvailable = caseAvailableDayPeriod.ToList();

                //1. 连排有必须时间
                if (ca.Count >= 1 && ca.TimesWeight == 1)
                {
                    if (ca.Times != null && ca.Times.Count > 0)
                    {
                        var courseTime = rule?.CourseTimes?.FirstOrDefault(ct => ct.ClassID == ca.ClassID);
                        if (courseTime != null)
                        {
                            courseAvailable = TimeOperation.TimeSlotDiff(courseAvailable, courseTime.ForbidTimes);
                        }

                        List<DayPeriodModel> classCourseContinousAvailable = TimeOperation.TimeSlotInterSect(courseAvailable, ca.Times);

                        if (ca.Count == 1 && ca.Times.Count < 2 ||
                            (ca.Count * 2 <= ca.Times.Count && ca.Count * 2 > classCourseContinousAvailable.Count))
                        {
                            arCourseArrange.DetailInfo.Add($"{Miscellaneous.GetCLClassCourseInfo(cLCase, ca.ClassID)}在现有规则下无法满足连排课位优先要求（课位不足）!");
                        }

                        //判断可用课位是否最终有连续课位提供
                        int iContinousNum = Math.Min(ca.Count, classCourseContinousAvailable.Count / 2);
                        var result = CanHoldContinous(cLCase, classCourseContinousAvailable, iContinousNum, ca.NoCrossingBreak);

                        if (!result.Item1)
                        {
                            arCourseArrange.DetailInfo.Add($"{Miscellaneous.GetCLClassCourseInfo(cLCase, ca.ClassID)}在现有规则下无法满足连排课位!");
                        }

                        //多个连排隔天连排
                        if (result.Item1 && ca.Count > 1 && ca.IsIntervalDay && ca.IntervalDayWeight == 1)
                        {
                            bool isEnough = Miscellaneous.DaysEnoughForEvenlyDistributed(result.Item2, iContinousNum, 2);
                            if (!isEnough)
                            {
                                arCourseArrange.DetailInfo.Add($"{Miscellaneous.GetCLClassCourseInfo(cLCase, ca.ClassID)} 无法满足隔天连排!");
                            }
                        }
                    }
                    else
                    {
                        var courseTime = rule?.CourseTimes?.FirstOrDefault(ct => ct.ClassID == ca.ClassID);
                        if (courseTime != null)
                        {
                            courseAvailable = TimeOperation.TimeSlotDiff(courseAvailable, courseTime.ForbidTimes);
                        }

                        //判断可用课位是否最终有连续课位提供
                        int iContinousNum = ca.Count;
                        if (iContinousNum > courseAvailable.Count / 2)
                        {
                            iContinousNum = courseAvailable.Count / 2;
                        }

                        var result = CanHoldContinous(cLCase, courseAvailable, iContinousNum, ca.NoCrossingBreak);
                        if (!result.Item1)
                        {
                            arCourseArrange.DetailInfo.Add($"{Miscellaneous.GetCLClassCourseInfo(cLCase, ca.ClassID)}在现有规则下无法满足连排要求!");
                        }

                        //多个连排隔天连排
                        if (result.Item1 && ca.Count > 1 && ca.IsIntervalDay && ca.IntervalDayWeight == 1)
                        {
                            bool isEnough = Miscellaneous.DaysEnoughForEvenlyDistributed(result.Item2, iContinousNum, 2);
                            if (!isEnough)
                            {
                                arCourseArrange.DetailInfo.Add($"{Miscellaneous.GetCLClassCourseInfo(cLCase, ca.ClassID)} 无法满足隔天连排!");
                            }
                        }
                    }
                }
            });

            //1个班级下多个科目之间的连排冲突
            cLCase?.Classes?.ForEach(cl => {

                List<DayPeriodModel> classMustUnion = new List<DayPeriodModel>();
                int iContinousCount = 0;
                bool noCrossingBreak = true;/* 仅取一个值 */

                rule?.ArrangeContinuous?.Where(ca => ca.ClassID == cl.ID && ca.TimesWeight == 1 && ca.Times != null)?.ToList()?.ForEach(co => {
                    //目前还不能有效针对课位不足情况下的跨科目连排校验
                    if (co.Times.Count >= co.Count * 2)
                    {
                        iContinousCount += co.Count;
                        classMustUnion = TimeOperation.TimeSlotUnion(classMustUnion, co.Times);
                        noCrossingBreak = co.NoCrossingBreak;
                    }
                });

                bool result = CanHoldContinous(cLCase, classMustUnion, iContinousCount, noCrossingBreak).Item1;
                if (!result)
                {
                    string className = cLCase?.Classes?.FirstOrDefault(cla => cla.ID == cl.ID)?.Name ?? string.Empty;
                    arCourseArrange.DetailInfo.Add($"班级 {className} 在现有规则下无法满足多个科目的连排要求!");
                }
            });

            //1个老师多个班级下同科目之间的连排冲突
            cLCase?.Teachers?.ForEach(t => {
                var teacherLesson = cLCase?.ClassHours?.Where(ch => ch.TeacherIDs != null && ch.TeacherIDs.Contains(t.ID))?.Select(ch => new { ch.ClassID, ch.CourseID, t.ID });
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
                    var caRule = rule?.ArrangeContinuous?.FirstOrDefault(ca => ca.Times != null && ca.TimesWeight == 1 && ca.ClassID == tf.ClassID);
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

                bool result = CanHoldContinous(cLCase, teacherClassMustUnion, iContinousCount, noCrossingBreak).Item1;
                if (!result)
                {
                    string teacherName = cLCase?.Teachers?.FirstOrDefault(te => te.ID == t.ID)?.Name ?? string.Empty;
                    arCourseArrange.DetailInfo.Add($"教师 {teacherName} 在现有规则下无法满足在多个班级的连排要求!");
                }
            });

            arList.Add(arCourseArrange);
            #endregion

            #region 课时分散
            RuleAnalysisDetailResult raClassAverages = new RuleAnalysisDetailResult();
            raClassAverages.DetailInfo = new List<string>();
            raClassAverages.RuleName = "课时分散";

            rule?.ClassHourAverages?.Where(x => x.Weight == 1)?.ToList()?.ForEach(cha => {
                //1. 方案课位/课程禁止/教师禁止/教师必须/课程必须等 对课时分散冲突的影响
                List<DayPeriodModel> classCourseAvailable = caseAvailableDayPeriod.ToList();
                int classCourseLesson = classCourseMapping.FirstOrDefault(ccm => ccm.ID == cha.ClassID)?.Lessons ?? 0;
                bool hasContinousLesson = rule?.ArrangeContinuous.Exists(ca => ca.TimesWeight == 1 && ca.ClassID == cha.ClassID) ?? false;
                string classCourseName = Miscellaneous.GetCLClassCourseInfo(cLCase, cha.ClassID);

                if (!hasContinousLesson && classCourseAvailable.Select(d => d.Day).Distinct().Count() < classCourseLesson)
                {
                    raClassAverages.DetailInfo.Add($"{classCourseName} 结合方案排课时间无法满足课时分散!");
                }

                //2. 课程必须禁止对课时分散冲突的影响
                var ruleCourseTimes = rule?.CourseTimes.FirstOrDefault(ct => ct.Weight == 1 && ct.ClassID == cha.ClassID);
                if (ruleCourseTimes != null)
                {
                    bool dayEnough = true;

                    //不考虑连排
                    if (!hasContinousLesson)
                    {
                        if (ruleCourseTimes.ForbidTimes != null)
                        {
                            classCourseAvailable = TimeOperation.TimeSlotDiff(classCourseAvailable, ruleCourseTimes.ForbidTimes);
                            dayEnough = Miscellaneous.DaysEnoughForEvenlyDistributed(classCourseAvailable, classCourseLesson, cha.MinDay);
                        }

                        if (ruleCourseTimes.MustTimes != null)
                        {
                            List<DayPeriodModel> courseMustTime = TimeOperation.TimeSlotInterSect(classCourseAvailable, ruleCourseTimes.MustTimes);

                            if (ruleCourseTimes.MustTimes.Count >= classCourseLesson)
                            {
                                dayEnough = Miscellaneous.DaysEnoughForEvenlyDistributed(courseMustTime, classCourseLesson, cha.MinDay);
                            }
                            else
                            {
                                //课时不够时，判断这些课位是否分散
                                dayEnough = Miscellaneous.DaysEnoughForEvenlyDistributed(courseMustTime, courseMustTime.Count, cha.MinDay);
                            }
                        }
                    }

                    if (!dayEnough)
                    {
                        raClassAverages.DetailInfo.Add($"{classCourseName} 结合课程排课时间规则无法满足课时分散!");
                    }
                }

                //3. 教师必须禁止对课时分散冲突的影响
                var courseClassTeacherInfo = cLCase?.ClassHours?.Where(ch => ch.ClassID == cha.ClassID && ch.TeacherIDs != null)?.SelectMany(ch => ch.TeacherIDs)?.ToList() ?? new List<string>();
                int teachedLesson = 0;
                if (courseClassTeacherInfo.Count == classCourseLesson && courseClassTeacherInfo.Distinct().Count() == 1)
                {
                    //如果仅有1名教师且教师教所有课时
                    teachedLesson = classCourseLesson;
                }

                var ruleTeacherTimes = rule?.TeacherTimes.FirstOrDefault(tt => tt.Weight == 1 && tt.TeacherID == cha.ClassID);
                if (ruleTeacherTimes != null && teachedLesson == classCourseLesson)
                {
                    //不考虑连排
                    if (!hasContinousLesson)
                    {
                        if (ruleTeacherTimes.ForbidTimes != null)
                        {
                            classCourseAvailable = TimeOperation.TimeSlotDiff(classCourseAvailable, ruleTeacherTimes.ForbidTimes);
                            int availableDays = classCourseAvailable.Select(d => d.Day).Distinct().Count();

                            if (availableDays < classCourseLesson)
                            {
                                raClassAverages.DetailInfo.Add($"{classCourseName} 结合教师时间规则无法满足课时分散!");
                            }
                        }
                    }
                }
            });

            arList.Add(raClassAverages);
            #endregion

            #region 从班级的角度检查部分规则
            //1.如果有多个教师教同一个班级，必须时间不同造成冲突
            cLCase?.Classes?.ForEach(cl =>
            {
                //有多个教师教同一个班级，必须时间不同造成冲突
                var teachers = cLCase.ClassHours.Where(ch => ch.ClassID == cl.ID && ch.TeacherIDs != null && ch.TeacherIDs.Count > 1)?.SelectMany(ch => ch.TeacherIDs)?.Distinct()?.ToList() ?? new List<string>();

                if (teachers.Count > 1)
                {
                    List<DayPeriodModel> teachersForbidTimes = new List<DayPeriodModel>() { };
                    List<DayPeriodModel> teachersMustTimes = caseAvailableDayPeriod.ToList();

                    teachers.Sort();
                    //查出同时包含这几个教师的课时，注意这些老师理论上也可能在其他班级同时教课
                    var commonLesson = cLCase.ClassHours.Where(ch => ch.TeacherIDs != null && ch.TeacherIDs.Intersect(teachers).Count() == teachers.Count);
                    Dictionary<string, List<DayPeriodModel>> teacherMustTimeAndLessonInfo = new Dictionary<string, List<DayPeriodModel>>();
                    bool teacherMustEnough = true;
                    List<string> teacherNames = new List<string>();

                    teachers.ForEach(te =>
                    {
                        //计算所有教师的准确必须时间，因为原始必须时间中的部分可能是在方案中不可用的
                        List<DayPeriodModel> singleTeacherMustTimes = TimeOperation.TimeSlotInterSect(caseAvailableDayPeriod, rule?.TeacherTimes?.FirstOrDefault(tt => tt.TeacherID == te)?.MustTimes);
                        //求各个教师禁止时间的合集
                        teachersForbidTimes = TimeOperation.TimeSlotUnion(teachersForbidTimes, rule?.TeacherTimes?.FirstOrDefault(tt => tt.TeacherID == te)?.ForbidTimes);

                        //当前教师总课时
                        int teacherTotalLesson = cLCase.ClassHours.Where(ch => ch.TeacherIDs != null && ch.TeacherIDs.Contains(te)).Count();

                        //标记当前教师必须时间不足
                        if (teacherTotalLesson > singleTeacherMustTimes.Count)
                        {
                            teacherMustEnough = false;
                        }

                        teacherMustTimeAndLessonInfo.Add(te, singleTeacherMustTimes);
                        teacherNames.Add(cLCase.Teachers?.FirstOrDefault(t => t.ID == te)?.Name ?? string.Empty);
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

            //2.根据各个课时的可用时间统计出冲突
            List<string> timesTeachers = rule?.TeacherTimes?.Select(tt => tt.TeacherID)?.ToList() ?? new List<string>() { };
            List<string> classCourses = new List<string>() { };

            globalLessonsAvailableTimes?.ForEach(gl => {
                if (gl.AvailableTimes.Count == 0)
                {
                    if (!classCourses.Exists(cc => cc == gl.ClassID))
                    {
                        string classCourseName = Miscellaneous.GetCLClassCourseInfo(cLCase, gl.ClassID);
                        classCourses.Add( gl.ClassID );

                        if (rule?.CourseTimes?.Exists(ct => ct.ClassID == gl.ClassID) ?? false)
                        {
                            arCourseTime.DetailInfo.Add($"课程和教师的必须禁止规则导致 {classCourseName} 无足够可用课位!");
                        }

                        List<string> teachers = cLCase?.ClassHours?.Where(ch => ch.ClassID == gl.ClassID)?.SelectMany(ch => ch.TeacherIDs)?.Distinct()?.ToList() ?? new List<string>();

                        if (teachers.Intersect(timesTeachers).Count() > 0)
                        {
                            arTeacherTime.DetailInfo.Add($"课程和教师的必须禁止规则导致 {classCourseName} 无足够可用课位!");
                        }
                    }
                }
            });

            //3.同一教师在不同班级必须时间重复
            
            #endregion

            return arList;
        }
        #endregion

        /// <summary>
        /// 判断是否能容下指定的连排
        /// </summary>
        /// <param name="cLCase"></param>
        /// <param name="availableDayPeriod"></param>
        /// <param name="ContinousNumber"></param>
        /// <param name="noCrossBreak"></param>
        /// <returns>返回结果中第一个参数表示是否能容纳连排数，第二个参数表示能容纳连排的天中的一个课位</returns>
        private static Tuple<bool, List<DayPeriodModel>> CanHoldContinous(CLCase cLCase, List<DayPeriodModel> availableDayPeriod, int ContinousNumber, bool noCrossBreak)
        {
            bool result = true;
            List<DayPeriodModel> dayList = new List<DayPeriodModel>();

            #region 构造映射表
            List<int> mappingFullOld = new List<int>();
            List<int> mappingFullNew = new List<int>();

            int tempI = 0;

            cLCase?.Positions?.Where(p => p.DayPeriod.Day == DayOfWeek.Monday)?.ToList()?.OrderBy(p => p.PositionOrder)?.ToList()?.ForEach(p => {
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
       
        private class StudentLessonInfo
        {
            public string StudentID { get; set; }
            public int Lesson { get; set; }
        }

        private class CombinationInfo
        {
            public string CombinationName { get; set; }
            public int Lesson { get; set; }
        }
    }

    class ClassCourseDetailLesson
    {
        public int ID { get; set; }
        public string ClassID { get; set; }
        public List<DayPeriodModel> AvailableTimes { get; set; } = new List<DayPeriodModel> { };
    }

    class AnalysisUtility
    {
        /// <summary>
        /// Get selection road by student preselection
        /// </summary>
        /// <param name="Preselection"></param>
        /// <returns></returns>
        public static string GetStringFormatByStudentPreselection(CLCase cLCase, List<PreselectionModel> Preselection)
        {
            string result = string.Empty;

            if (Preselection != null)
            {
                List<string> tempResult = new List<string>();

                Preselection.ForEach(x =>
                {
                    string courseName = cLCase?.Courses?.FirstOrDefault(co => co.ID == x.CourseID)?.Name ?? string.Empty;
                    string levelName = cLCase?.Courses?.FirstOrDefault(co => co.ID == x.CourseID)?.Levels?.FirstOrDefault(le => le.ID == x.LevelID)?.Name ?? string.Empty;
                    tempResult.Add($"{courseName}{levelName}");
                });

                tempResult = tempResult.OrderBy(x => x).ToList();
                result = string.Join(",", tempResult);
            }

            return result;
        }
    }
}
