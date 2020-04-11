using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Arranging.Mixed.Pattern.Model
{
    /// <summary>
    /// 数据核查
    /// </summary>
    public class UIValidation
    {
        /// <summary>
        /// 只排教师
        /// </summary>
        public bool OnlyTeacher { get; set; }

        /// <summary>
        /// 包含已分配到班的所有学生
        /// </summary>
        public bool AssignedStudents { get; set; }
    }
}
