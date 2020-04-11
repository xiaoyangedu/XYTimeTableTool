using OSKernel.Presentation.Models.Administrative;
using OSKernel.Presentation.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models;
using XYKernel.OS.Common.Models.Administrative;

namespace OSKernel.Presentation.Arranging.Administrative
{
    public static class CPExtend
    {
        /// <summary>
        /// 获取课时
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static List<UIClassHour> GetClassHours(this CPCase cp, int[] ids)
        {
            List<UIClassHour> results = new List<UIClassHour>();

            if (cp == null || ids == null)
                return results;

            var teachers = cp.Teachers;
            var classes = cp.Classes;
            var courses = cp.Courses;

            var classHours = (from i in ids from ch in cp.ClassHours where i == ch.ID select ch);

            foreach (var ch in classHours)
            {
                UIClassHour uiClassHour = new UIClassHour();
                uiClassHour.ClassID = ch.ClassID;
                uiClassHour.CourseID = ch.CourseID;
                uiClassHour.ID = ch.ID;

                if (ch.TeacherIDs == null)
                {
                    uiClassHour.Teachers = new List<TeacherModel>();
                }
                else
                {
                    uiClassHour.Teachers = (from id in ch.TeacherIDs from t in teachers where t.ID == id select t)?.ToList();
                }

                uiClassHour.Class = classes.FirstOrDefault(c => c.ID.Equals(ch.ClassID))?.Name;
                uiClassHour.Course = courses.FirstOrDefault(c => c.ID.Equals(ch.CourseID))?.Name;

                results.Add(uiClassHour);
            }

            return results;
        }

        /// <summary>
        /// 获取节次
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public static List<DayPeriodModel> GetDayPeriods(this CPCase cp)
        {
            List<DayPeriodModel> dayPeriods = new List<DayPeriodModel>();

            var positions = cp.Positions.Where(p =>
              p.Position != XYKernel.OS.Common.Enums.Position.AB &&
              p.Position != XYKernel.OS.Common.Enums.Position.PB &&
              p.Position != XYKernel.OS.Common.Enums.Position.Noon);

            if (positions != null)
            {
                var groups = positions.GroupBy(p => p.DayPeriod.PeriodName);
                foreach (var g in groups)
                {
                    dayPeriods.Add(g.First().DayPeriod);
                }
            }
            return dayPeriods;
        }

        /// <summary>
        /// 获取课程
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="teacherIDs"></param>
        /// <returns></returns>
        public static List<UICourse> GetCourses(this CPCase cp, string teacherID)
        {
            List<UICourse> courses = new List<UICourse>();

            var classHours = (from ch in cp.ClassHours where ch.TeacherIDs.Contains(teacherID) select ch);
            var groups = classHours?.GroupBy(g => g.CourseID);

            if (groups != null)
            {
                foreach (var g in groups)
                {
                    var course = cp.Courses.FirstOrDefault(ch => ch.ID.Equals(g.Key));
                    if (course != null)
                    {
                        UICourse ui = new UICourse()
                        {
                            ID = course.ID,
                            Name = course.Name
                        };
                        courses.Add(ui);
                    }
                }
            }

            return courses;
        }

        /// <summary>
        /// 根据班级ID获取课程
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="classID">班级ID</param>
        /// <returns></returns>
        public static List<UICourse> GetCoursesByClassID(this CPCase cp, string classID)
        {
            List<UICourse> courses = new List<UICourse>();

            var classHours = (from ch in cp.ClassHours where ch.ClassID.Equals(classID) select ch);
            var groups = classHours?.GroupBy(g => g.CourseID);

            if (groups != null)
            {
                foreach (var g in groups)
                {
                    var course = cp.Courses.FirstOrDefault(ch => ch.ID.Equals(g.Key));
                    if (course != null)
                    {
                        UICourse ui = new UICourse()
                        {
                            ID = course.ID,
                            Name = course.Name
                        };
                        courses.Add(ui);
                    }
                }
            }

            return courses;
        }

        /// <summary>
        /// 根据教师ID获取班级
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="teacherID">教师ID</param>
        /// <returns></returns>
        public static List<UIClass> GetClassesByTeacherID(this CPCase cp, string teacherID)
        {
            List<UIClass> classes = new List<UIClass>();

            var classHours = (from ch in cp.ClassHours where ch.TeacherIDs.Contains(teacherID) select ch);

            var groups = classHours?.GroupBy(g => $"{g.CourseID}|{g.ClassID}");
            if (groups != null)
            {
                foreach (var g in groups)
                {
                    var keys = g.Key.Split('|');

                    var classModel = cp.Classes.FirstOrDefault(ch => ch.ID.Equals(keys[1]));
                    if (classModel != null)
                    {
                        var course = cp.Courses.FirstOrDefault(c => c.ID.Equals(keys[0]))?.Name;

                        UIClass ui = new UIClass()
                        {
                            ID = classModel.ID,
                            Name = classModel.Name,
                            Course = course,
                            CourseID = keys[0]
                        };
                        classes.Add(ui);
                    }
                }
            }
            return classes;
        }

        /// <summary>
        /// 获取班级，根据课程ID
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="courseID"></param>
        /// <returns></returns>
        public static List<UIClass> GetClasses(this CPCase cp, string courseID)
        {
            List<UIClass> classes = new List<UIClass>();

            var results = cp.Classes.Where(c => c.Settings.Any(s => s.CourseID.Equals(courseID)));
            if (results != null)
            {
                var course = cp.Courses.FirstOrDefault(c => c.ID.Equals(courseID));

                results?.ToList()?.ForEach(r =>
                {
                    classes.Add(new UIClass()
                    {
                        ID = r.ID,
                        CourseID = courseID,
                        Course = course.Name,
                        Name = r.Name
                    });
                });
            }
            return classes;
        }

        /// <summary>
        /// 获取课时根据课程ID
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="courseID"></param>
        /// <param name="classID"></param>
        /// <returns></returns>
        public static List<UIClassHour> GetClassHours(this CPCase cp, string courseID, string classID)
        {
            List<UIClassHour> results = new List<UIClassHour>();

            var teachers = cp.Teachers;
            var classes = cp.Classes;
            var courses = cp.Courses;

            var classHours = cp.ClassHours.Where(c => c.CourseID.Equals(courseID) && c.ClassID.Equals(classID));
            foreach (var ch in classHours)
            {
                UIClassHour uiClassHour = new UIClassHour();
                uiClassHour.ClassID = ch.ClassID;
                uiClassHour.CourseID = ch.CourseID;
                uiClassHour.ID = ch.ID;

                if (ch.TeacherIDs == null)
                {
                    uiClassHour.Teachers = new List<TeacherModel>();
                }
                else
                {
                    uiClassHour.Teachers = (from id in ch.TeacherIDs from t in teachers where t.ID == id select t)?.ToList();
                }

                uiClassHour.Class = classes.FirstOrDefault(c => c.ID.Equals(ch.ClassID))?.Name;
                uiClassHour.Course = courses.FirstOrDefault(c => c.ID.Equals(ch.CourseID))?.Name;

                results.Add(uiClassHour);
            }

            return results;
        }

        /// <summary>
        /// 根据课程选择教师
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="courseID">科目ID</param>
        /// <returns>教师</returns>
        public static List<UITeacher> GetTeachers(this CPCase cp, string courseID)
        {
            List<UITeacher> teachers = new List<UITeacher>();

            var classhours = (from c in cp.ClassHours
                              from tid in c.TeacherIDs
                              from t in cp.Teachers
                              where tid.Equals(t.ID)
                              select new
                              {
                                  c.CourseID,
                                  t
                              });

            var results = classhours.Where(c => c.CourseID.Equals(courseID));
            if (results != null)
            {
                var groups = results.GroupBy(r => r.t);
                foreach (var g in groups)
                {
                    UITeacher uiTeacher = new UITeacher()
                    {
                        ID = g.Key.ID,
                        Name = g.Key.Name
                    };
                    teachers.Add(uiTeacher);
                }
            }
            return teachers;
        }

        /// <summary>
        /// 根据科目，班级 获取教师信息
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="courseID"></param>
        /// <param name="classID"></param>
        /// <returns></returns>
        public static List<UITeacher> GetTeachers(this CPCase cp, string courseID, string classID)
        {
            List<UITeacher> teachers = new List<UITeacher>();

            var classhours = (from c in cp.ClassHours
                              from tid in c.TeacherIDs
                              from t in cp.Teachers
                              where tid.Equals(t.ID)
                              select new
                              {
                                  c.ClassID,
                                  c.CourseID,
                                  t
                              });

            var results = classhours.Where(c => c.CourseID.Equals(courseID) && c.ClassID.Equals(classID));
            if (results != null)
            {
                var groups = results.GroupBy(r => r.t);
                foreach (var g in groups)
                {
                    UITeacher uiTeacher = new UITeacher()
                    {
                        ID = g.Key.ID,
                        Name = g.Key.Name
                    };
                    teachers.Add(uiTeacher);
                }
            }
            return teachers;
        }

        /// <summary>
        /// 根据教师ID集合
        /// </summary>
        /// <param name="cp">方案模型</param>
        /// <param name="teacherIDs">教师ID集合</param>
        /// <returns></returns>
        public static List<TeacherModel> GetTeachersByIds(this CPCase cp, List<string> teacherIDs)
        {
            return (from tid in teacherIDs from t in cp.Teachers where t.ID.Equals(tid) select t)?.ToList();
        }
    }
}
