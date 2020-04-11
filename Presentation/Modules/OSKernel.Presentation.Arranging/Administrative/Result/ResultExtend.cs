using OSKernel.Presentation.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models.Administrative.Result;

namespace OSKernel.Presentation.Arranging.Administrative.Result
{
    public static class ResultExtend
    {
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

            teachers.OrderBy(t => t.Name);
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

        /// <summary>
        /// 获取教师所教科目
        /// </summary>
        /// <param name="r"></param>
        /// <param name="teacherID">教师ID</param>
        /// <returns></returns>
        public static List<string> GetTeacherCourse(this ResultModel r, string teacherID)
        {
            var courseIDs = (from c in r.ResultClasses from cd in c.ResultDetails where cd.Teachers.Any(t => t.Equals(teacherID)) select cd.CourseID)?.Distinct();
            if (courseIDs != null)
            {
                return (from c in courseIDs from rc in r.Courses where c.Equals(rc.ID) select rc.Name)?.ToList();
            }
            else
                return new List<string>();
        }
    }
}
