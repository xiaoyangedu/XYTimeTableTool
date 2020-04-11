using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Views.Model
{
    /// <summary>
    /// 存储开班信息层的数据
    /// </summary>
    public class SaveCourseLevel
    {
        /// <summary>
        /// 科目ID
        /// </summary>
        public string CourseID { get; set; }

        /// <summary>
        /// 层ID
        /// </summary>
        public string LevelID { get; set; }

        /// <summary>
        /// 课时
        /// </summary>
        public int Lessons { get; set; }
    }
}
