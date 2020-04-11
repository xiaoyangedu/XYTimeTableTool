using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Core.Http
{
    /// <summary>
    /// 任务结果
    /// </summary>
    public class TaskResult
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// 任务模式
        /// </summary>
        public byte mode { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        public byte state { get; set; }

        /// <summary>
        /// 任务开始时间
        /// </summary>
        public DateTime start { get; set; }

        /// <summary>
        /// 任务结束时间
        /// </summary>
        public DateTime end { get; set; }
    }
}
