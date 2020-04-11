using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models.Administrative;

namespace OSKernel.Presentation.Models.Administrative
{
    public class UIClassCourse : GalaSoft.MvvmLight.ObservableObject
    {
        private int _lessons = 5;

        /// <summary>
        /// 班级ID
        /// </summary>
        public string ClassID { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 课程ID
        /// </summary>
        public string CourseID { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        public string Course { get; set; }

        /// <summary>
        /// 教师列表
        /// </summary>
        public List<TeacherModel> Teachers { get; set; }

        /// <summary>
        /// 教师拼接名称
        /// </summary>
        private string _teacherString;
        public string TeacherString
        {
            get
            {
                return Teachers?.Select(s=>s.Name)?.Parse();
            }

            set
            {
                _teacherString = value;
            }
        }

        /// <summary>
        /// 课时列表
        /// </summary>
        public List<UIClassHour> ClassHours { get; set; }

        /// <summary>
        /// 课时
        /// </summary>
        public int Lessons
        {
            get
            {
                return _lessons;
            }

            set
            {
                _lessons = value;
                RaisePropertyChanged(() => Lessons);
            }
        }

        public UIClassCourse()
        {
            this.Teachers = new List<TeacherModel>();

            this.ClassHours = new List<UIClassHour>();
        }

        public void RaiseChanged()
        {
            this.RaisePropertyChanged(() => TeacherString);
        }
    }
}
