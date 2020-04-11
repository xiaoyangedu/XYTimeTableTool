using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Core.Http.User
{
    /// <summary>
    /// 设置公（秘）钥
    /// </summary>
    public class KeyBodyInfo
    {
        /// <summary>
        /// 签名类型（枚举）
        /// （0：RSA2（建议）1：RSA； 2：MD5）
        /// </summary>
        public byte secret_type { get; set; }

        /// <summary>
        /// 签名公钥
        /// </summary>
        public string public_key { get; set; }
    }
}
