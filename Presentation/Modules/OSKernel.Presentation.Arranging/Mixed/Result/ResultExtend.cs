using OSKernel.Presentation.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models.Mixed.Result;

namespace OSKernel.Presentation.Arranging.Mixed.Result
{
    public static class ResultExtend
    {
        public static Models.Mixed.UIClass GetClassByID(this ResultModel r, string classID)
        {
            var f = r.Classes.FirstOrDefault(c => c.ID.Equals(classID));
            if (f != null)
            {
                var model = new Models.Mixed.UIClass()
                {
                    ID = f.ID,
                    CourseID = f.CourseID,
                    LevelID = f.LevelID,
                    Name = f.Name
                };
                var course = r.Courses.FirstOrDefault(c => c.ID.Equals(f.CourseID));
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

        public static List<UITeacher> GetTeachers(this ResultModel r)
        {
            List<UITeacher> teachers = new List<UITeacher>();

            var details = (from c in r.ResultClasses
                           from rc in c.ResultDetails
                           select new
                           {
                               rc.ClassHourId,
                               rc.Teachers,
                           });

            r.Teachers?.ToList()?.ForEach(t =>
            {
                UITeacher teacher = new UITeacher();
                teacher.ID = t.ID;
                teacher.Name = t.Name;
                teacher.ClassHourIDs = details.Where(d => d.Teachers.Contains(t.ID))?.Select(s => s.ClassHourId)?.ToList();
                teachers.Add(teacher);
            });

            return teachers;


        }

        public static List<UITeacher> GetTeachersByTeacherIDs(this ResultModel r, List<string> teacherIDs)
        {
            List<UITeacher> teachers = new List<UITeacher>();
            teacherIDs?.ForEach(t =>
            {
                var teacher = r.Teachers.FirstOrDefault(tt => tt.ID.Equals(t));
                if (teacher != null)
                {
                    UITeacher newTeacher = new UITeacher()
                    {
                        ID = teacher.ID,
                        Name = teacher.Name
                    };
                    teachers.Add(newTeacher);
                }
            });

            return teachers;
        }

        public static Models.Mixed.UIClass GetClassByClassHourID(this ResultModel r, int classHourID)
        {
            var classInfo = r.ResultClasses.FirstOrDefault(rc => rc.ResultDetails.Any(rd => rd.ClassHourId == classHourID));
            var uiclass = r.GetClassByID(classInfo?.ClassID);
            if (uiclass != null)
            {
                uiclass.TeacherIDs= r.GetTeacherIDs(classHourID);
            }
            return uiclass;
        }

        public static List<Models.Base.UIStudent> getStudentByStudentIDs(this ResultModel r, List<string> studentIDs)
        {
            List<Models.Base.UIStudent> students = new List<UIStudent>();
            studentIDs?.ForEach(s =>
            {
                var student = r.Students.FirstOrDefault(rs => rs.ID.Equals(s));
                if (student != null)
                {
                    students.Add(new UIStudent()
                    {
                        ID = student.ID,
                        Name = student.Name
                    });
                }
            });

            return students;
        }

        public static List<string> GetTeacherIDs(this ResultModel r, int classHourID)
        {
            var classHourInfo = (from c in r.ResultClasses from rd in c.ResultDetails select rd)?.FirstOrDefault(rd => rd.ClassHourId.Equals(classHourID));

            return classHourInfo?.Teachers?.ToList();
        }
    }
}
