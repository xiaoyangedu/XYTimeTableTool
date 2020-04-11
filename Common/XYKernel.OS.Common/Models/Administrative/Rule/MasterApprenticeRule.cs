using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Enums;

namespace XYKernel.OS.Common.Models.Administrative.Rule
{
    /// <summary>
    /// 师徒跟随模型
    /// </summary>
    public class MasterApprenticeRule
    {
        /// <summary>
        /// 课程ID
        /// </summary>
        public string CourseID { get; set; }

        /// <summary>
        /// 师傅ID
        /// </summary>
        public string MasterID { get; set; }

        /// <summary>
        /// 徒弟集合
        /// </summary>
        public List<string> ApprenticeIDs { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}
