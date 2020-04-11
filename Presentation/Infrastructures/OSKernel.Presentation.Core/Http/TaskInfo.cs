using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Core.Http
{
    /// <summary>
    /// 任务信息（接收返回来的任务）
    /// </summary>
    public class TaskInfo
    {
        /// <summary>
        /// 总数
        /// </summary>
        public int total { get; set; }

        /// <summary>
        /// 任务记录
        /// </summary>
        public string data { get; set; }
    }
}
