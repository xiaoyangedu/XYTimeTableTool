using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Core.Http.User
{
    /// <summary>
    /// 登录反馈码
    /// </summary>
    public class ResponseInfo
    {
        /// <summary>
        /// 网关返回码
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// 网关返回码描述
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 业务返回码
        /// </summary>
        public string sub_code { get; set; }

        /// <summary>
        /// 业务返回码描述
        /// </summary>
        public string sub_msg { get; set; }

        /// <summary>
        /// 返回内容
        /// </summary>
        public string result { get; set; }
    }
}
