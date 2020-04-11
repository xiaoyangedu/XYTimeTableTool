using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Mixed
{
    /// <summary>
    /// 课程
    /// </summary>
    public class CourseModel
    {
        /// <summary>
        /// 课程ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 层
        /// </summary>
        public List<LevelModel> Levels { get; set; }
    }
}
