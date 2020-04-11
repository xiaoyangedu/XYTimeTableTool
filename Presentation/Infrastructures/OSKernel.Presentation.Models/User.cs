using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Models
{
    /// <summary>
    /// 登录用户信息
    /// </summary>
    public class User
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// 未知用户（需要登录）
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }

        /// <summary>
        /// 用户token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// 服务端建议使用（待确认）
        /// </summary>
        public string MixKey { get; set; }

        #region RSA

        /// <summary>
        /// 私钥
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// 公钥
        /// </summary>
        public string PublickKey { get; set; }

        #endregion

    }
}
