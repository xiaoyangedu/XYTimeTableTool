using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Enums
{
    /// <summary>
    /// TODO删除（师徒跟随不区分早与晚于）
    /// </summary>
    public enum MasterApprenticeFollow
    {
        /// <summary>
        /// 师傅早于徒弟
        /// </summary>
        Before = 1,
        /// <summary>
        /// 师傅晚于徒弟
        /// </summary>
        After = 2
    }
}
