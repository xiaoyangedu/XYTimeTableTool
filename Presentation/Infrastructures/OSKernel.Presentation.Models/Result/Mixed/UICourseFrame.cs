using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using XYKernel.OS.Common.Models;

namespace OSKernel.Presentation.Models.Result.Mixed
{
    /// <summary>
    /// 课程框
    /// </summary>
    public class UICourseFrame
    {
        /// <summary>
        /// 课时ID
        /// </summary>
        public int ClassHourId { get; set; }

        /// <summary>
        /// 班级ID
        /// </summary>
        public int ClassID { get; set; }

        /// <summary>
        /// 课程ID
        /// </summary>
        public string CourseID { get; set; }

        /// <summary>
        /// 课程名称
        /// </summary>
        public string Course { get; set; }

        /// <summary>
        /// 课位信息
        /// </summary>
        public DayPeriodModel DayPeriod { get; set; }

        /// <summary>
        /// 教师集合
        /// </summary>
        public ICollection<string> Teachers { get; set; }

        /// <summary>
        /// 教师名称,分隔
        /// </summary>
        public string TeacherString
        {
            get
            {
                return Teachers.Parse();
            }
        }

    }
}
