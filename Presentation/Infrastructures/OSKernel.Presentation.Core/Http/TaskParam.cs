using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Core.Http
{
    /// <summary>
    /// 任务参数
    /// </summary>
    public class TaskParam
    {
        /// <summary>
        /// 索引
        /// </summary>
        public short index { get; set; }

        /// <summary>
        /// 容量
        /// </summary>
        public byte size { get; set; }

        /// <summary>
        /// 任务ID
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        public string state { get; set; }

        /// <summary>
        /// 任务模式
        /// </summary>
        public string mode { get; set; }
    }
}
