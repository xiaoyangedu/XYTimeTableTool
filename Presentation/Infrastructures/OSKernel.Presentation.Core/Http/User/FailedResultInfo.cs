using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Core.Http.User
{
    /// <summary>
    /// 登录错误结果
    /// </summary>
    public class FailedResultInfo
    {
        /// <summary>
        /// 失败码
        /// </summary>
        public int status_code { get; set; }

        /// <summary>
        /// 失败原因
        /// </summary>
        public string status_msg { get; set; }

        /// <summary>
        /// 网关失败吗
        /// </summary>
        public int code { get; set; }
    }
}
