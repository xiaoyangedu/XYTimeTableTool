using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Mixed
{
    /// <summary>
    /// 层
    /// </summary>
    public class LevelModel
    {
        /// <summary>
        /// 层ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 层
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 课时数
        /// </summary>
        public int Lessons { get; set; }
    }
}
