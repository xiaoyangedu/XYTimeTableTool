using OSKernel.Presentation.Models.Base;
using OSKernel.Presentation.Models.Mixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models;
using XYKernel.OS.Common.Models.Mixed;

namespace OSKernel.Presentation.Arranging.Mixed
{
    public static class CLExtend
    {
        /// <summary>
        /// 获取课时
        /// </summary>
        /// <param name="cl"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static List<UIClassHour> GetClassHours(this CLCase cl, int[] ids)
        {
            List<UIClassHour> results = new List<UIClassHour>();

            if (cl == null || ids == null)
                return results;

            var teachers = cl.Teachers;
            var classes = cl.Classes;
            var courses = cl.Courses;

            var classHours = (from i in ids from ch in cl.ClassHours where i == ch.ID select ch);

            foreach (var ch in classHours)
            {
                UIClassHour uiClassHour = new UIClassHour();
                uiClassHour.ClassID = ch.ClassID;
                uiClassHour.CourseID = ch.CourseID;
                uiClassHour.ID = ch.ID;
                uiClassHour.LevelID = ch.LevelID;
                uiClassHour.Tags = ch.TagIDs;

                if (ch.TeacherIDs == null)
                {
                    uiClassHour.Teachers = new List<TeacherModel>();
                }
                else
                {
                    uiClassHour.Teachers = (from id in ch.TeacherIDs from t in teachers where t.ID == id select t)?.ToList();
                }

                uiClassHour.Class = classes.FirstOrDefault(c => c.ID.Equals(ch.ClassID))?.Name;

                var course = courses.FirstOrDefault(c => c.ID.Equals(ch.CourseID));
                if (course != null)
                {
                    var defaultLevel = course.Levels.FirstOrDefault(l => l.ID.Equals(ch.LevelID));

                    uiClassHour.Course = course.Name;
                    uiClassHour.Level = defaultLevel?.Name;
                }

                results.Add(uiClassHour);
            }

            return results;
        }

        public static List<UIClassHour> GetClassHoursByCouresAndLevel(this CLCase cl, string courseID, string levelID)
        {
            List<UIClassHour> results = new List<UIClassHour>();

            var classHours= cl.ClassHours.Where(c => c.CourseID.Equals(courseID) && c.LevelID.Equals(levelID))?.ToList();

            var teachers = cl.Teachers;
            var classes = cl.Classes;
            var courses = cl.Courses;

            foreach (var ch in classHours)
            {
                UIClassHour uiClassHour = new UIClassHour();
                uiClassHour.ClassID = ch.ClassID;
                uiClassHour.CourseID = ch.CourseID;
                uiClassHour.ID = ch.ID;
                uiClassHour.LevelID = ch.LevelID;
                uiClassHour.Tags = ch.TagIDs;

                if (ch.TeacherIDs == null)
                {
                    uiClassHour.Teachers = new List<TeacherModel>();
                }
                else
                {
                    uiClassHour.Teachers = (from id in ch.TeacherIDs from t in teachers where t.ID == id select t)?.ToList();
                }

                uiClassHour.Class = classes.FirstOrDefault(c => c.ID.Equals(ch.ClassID))?.Name;

                var course = courses.FirstOrDefault(c => c.ID.Equals(ch.CourseID));
                if (course != null)
                {
                    var defaultLevel = course.Levels.FirstOrDefault(l => l.ID.Equals(ch.LevelID));

                    uiClassHour.Course = course.Name;
                    uiClassHour.Level = defaultLevel?.Name;
                }

                results.Add(uiClassHour);
            }

            return results;
        }

        /// <summary>
        /// 获取节次
        /// </summary>
        /// <param name="cl"></param>
        /// <returns></returns>
        public static List<DayPeriodModel> GetDayPeriods(this CLCase cl)
        {
            List<DayPeriodModel> dayPeriods = new List<DayPeriodModel>();

            var positions = cl.Positions.Where(p =>
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
        /// <param name="cl"></param>
        /// <param name="teacherIDs"></param>
        /// <returns></returns>
        public static List<Models.Mixed.UICourse> GetCourses(this CLCase cl, string teacherID)
        {
            List<Models.Mixed.UICourse> courses = new List<Models.Mixed.UICourse>();

            var classHours = (from ch in cl.ClassHours where ch.TeacherIDs.Contains(teacherID) select ch);
            var groups = classHours?.GroupBy(g => g.CourseID);

            if (groups != null)
            {
                foreach (var g in groups)
                {
                    var course = cl.Courses.FirstOrDefault(ch => ch.ID.Equals(g.Key));
                    if (course != null)
                    {
                        Models.Mixed.UICourse ui = new Models.Mixed.UICourse()
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
        /// <param name="cl"></param>
        /// <param name="classID">班级ID</param>
        /// <returns></returns>
        public static List<Models.Mixed.UICourse> GetCoursesByClassID(this CLCase cl, string classID)
        {
            List<Models.Mixed.UICourse> courses = new List<Models.Mixed.UICourse>();

            var classHours = (from ch in cl.ClassHours where ch.ClassID.Equals(classID) select ch);
            var groups = classHours?.GroupBy(g => g.CourseID);

            if (groups != null)
            {
                foreach (var g in groups)
                {
                    var course = cl.Courses.FirstOrDefault(ch => ch.ID.Equals(g.Key));
                    if (course != null)
                    {
                        Models.Mixed.UICourse ui = new Models.Mixed.UICourse()
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
        /// <param name="cl"></param>
        /// <param name="teacherID">教师ID</param>
        /// <returns></returns>
        public static List<Models.Mixed.UIClass> GetClassesByTeacherID(this CLCase cl, string teacherID)
        {
            List<Models.Mixed.UIClass> classes = new List<Models.Mixed.UIClass>();

            var classHours = (from ch in cl.ClassHours where ch.TeacherIDs.Contains(teacherID) select ch);

            var groups = classHours?.GroupBy(g => g.ClassID);
            if (groups != null)
            {
                foreach (var g in groups)
                {
                    var classModel = cl.Classes.FirstOrDefault(ch => ch.ID.Equals(g.Key));
                    if (classModel != null)
                    {
                        Models.Mixed.UIClass ui = new Models.Mixed.UIClass()
                        {
                            ID = classModel.ID,
                            Name = classModel.Name,
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
        /// <param name="cl"></param>
        /// <param name="courseID"></param>
        /// <returns></returns>
        public static List<Models.Mixed.UIClass> GetClasses(this CLCase cl, string courseID)
        {
            List<Models.Mixed.UIClass> classes = new List<Models.Mixed.UIClass>();

            var filter = cl.Classes.Where(c => c.CourseID.Equals(courseID));
            if (filter != null)
            {
                foreach (var f in filter)
                {
                    var model = new Models.Mixed.UIClass()
                    {
                        ID = f.ID,
                        CourseID = f.CourseID,
                        Capacity = f.Capacity,
                        LevelID = f.LevelID,
                        StudentIDs = f.StudentIDs,
                        TeacherIDs = f.TeacherIDs,
                    };

                    var course = cl.Courses.FirstOrDefault(c => c.ID.Equals(f.CourseID));
                    if (course != null)
                    {
                        var level = course.Levels.FirstOrDefault(l => l.ID.Equals(model.LevelID));
                        model.Name = f.Name;
                        model.Level = level?.Name;
                        model.Course = course.Name;
                    }

                    classes.Add(model);
                }
            }
            return classes;
        }

        public static List<Models.Mixed.UIClass> GetClasses(this CLCase cl)
        {
            List<Models.Mixed.UIClass> classes = new List<Models.Mixed.UIClass>();

            var filter = cl.Classes;
            if (filter != null)
            {
                foreach (var f in filter)
                {
                    var model = new Models.Mixed.UIClass()
                    {
                        ID = f.ID,
                        CourseID = f.CourseID,
                        Capacity = f.Capacity,
                        LevelID = f.LevelID,
                        StudentIDs = f.StudentIDs,
                        TeacherIDs = f.TeacherIDs,
                    };

                    var course = cl.Courses.FirstOrDefault(c => c.ID.Equals(f.CourseID));
                    if (course != null)
                    {
                        var level = course.Levels.FirstOrDefault(l => l.ID.Equals(model.LevelID));
                        if (string.IsNullOrEmpty(level.ID))
                        {
                            model.Name = f.Name;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(level.Name))
                            {
                                model.Name = f.Name;
                            }
                            else
                            {
                                model.Name = $"{level.Name}-{f.Name}";
                            }
                        }
                        model.Course = course.Name;
                    }

                    classes.Add(model);
                }
            }
            return classes;
        }

        public static Models.Mixed.UIClass GetClassByID(this CLCase cl, string classID)
        {
            var f = cl.Classes.FirstOrDefault(c => c.ID == classID);
            if (f != null)
            {
                var model = new Models.Mixed.UIClass()
                {
                    ID = f.ID,
                    CourseID = f.CourseID,
                    Capacity = f.Capacity,
                    LevelID = f.LevelID,
                    StudentIDs = f.StudentIDs,
                    TeacherIDs = f.TeacherIDs,
                };

                var course = cl.Courses.FirstOrDefault(c => c.ID.Equals(f.CourseID));
                if (course != null)
                {
                    var level = course.Levels.FirstOrDefault(l => l.ID.Equals(model.LevelID));
                    model.Name = f.Name;
                    model.Level = level?.Name;
                    model.Course = course.Name;
                }
                return model;
            }
            else
                return null;
        }

        /// <summary>
        /// 获取课时根据课程ID
        /// </summary>
        /// <param name="cl"></param>
        /// <param name="courseID"></param>
        /// <param name="classID"></param>
        /// <returns></returns>
        public static List<UIClassHour> GetClassHours(this CLCase cl, string courseID, string classID, string levelID)
        {
            List<UIClassHour> results = new List<UIClassHour>();

            var teachers = cl.Teachers;
            var classes = cl.Classes;
            var courses = cl.Courses;

            var classHours = cl.ClassHours.Where(c => c.CourseID.Equals(courseID) && c.ClassID.Equals(classID) && c.LevelID.Equals(levelID));
            foreach (var ch in classHours)
            {
                UIClassHour uiClassHour = new UIClassHour();
                uiClassHour.ClassID = ch.ClassID;
                uiClassHour.CourseID = ch.CourseID;
                uiClassHour.LevelID = ch.LevelID;
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

                var course = courses.FirstOrDefault(c => c.ID.Equals(ch.CourseID));
                uiClassHour.Course = course?.Name;
                if (course != null)
                {
                    uiClassHour.Level = course.Levels?.FirstOrDefault(l => l.ID.Equals(ch.LevelID))?.Name;
                }

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
        public static List<UITeacher> GetTeachers(this CLCase cp, string courseID)
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
        /// 根据教师ID集合
        /// </summary>
        /// <param name="cl">方案模型</param>
        /// <param name="teacherIDs">教师ID集合</param>
        /// <returns></returns>
        public static List<TeacherModel> GetTeachersByIds(this CLCase cl, List<string> teacherIDs)
        {
            return (from tid in teacherIDs from t in cl.Teachers where t.ID.Equals(tid) select t)?.ToList();
        }
    }
}
