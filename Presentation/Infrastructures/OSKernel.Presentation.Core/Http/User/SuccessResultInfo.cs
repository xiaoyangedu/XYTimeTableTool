using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Core.Http.User
{
    /// <summary>
    /// 登录结果
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long user_id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string user_name { get; set; }

        /// <summary>
        /// 混淆密钥（服务端建议）
        /// </summary>
        public string mix_key { get; set; }

        /// <summary>
        /// 认证token
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// Token颁发时间戳
        /// </summary>
        public int issue { get; set; }

        //public string refresh_token { get; set; }

        /// <summary>
        /// Token有效标识
        /// </summary>
        //public bool valid { get; set; }
    }
}
