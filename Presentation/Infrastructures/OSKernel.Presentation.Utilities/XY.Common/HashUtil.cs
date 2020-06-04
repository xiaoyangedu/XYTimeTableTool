using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Utilities.XY.Common
{
    public static class HashUtil
    {
        /// <summary>
        /// MD5
        /// </summary>
        /// <param name="source">原始字符串</param>
        /// <param name="length16">指定长度为16位</param>
        /// <returns>MD5结果</returns>
        public static string MD5(string source, bool length16 = false)
        {
            return MD5(source, Encoding.UTF8, length16);
        }

        /// <summary>
        /// MD5
        /// </summary>
        /// <param name="source">原始字符串</param>
        /// <param name="encoding">编码方式</param>
        /// <param name="length16">指定长度为16位</param>
        /// <returns>MD5结果</returns>
        public static string MD5(string source, Encoding encoding, bool length16 = false)
        {
            StringBuilder sb = new StringBuilder(32);

            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                byte[] t = md5.ComputeHash(encoding.GetBytes(source));
                for (int i = 0; i < t.Length; i++)
                {
                    sb.Append(t[i].ToString("x").PadLeft(2, '0'));
                }
                if (length16)
                {
                    return sb.ToString().Substring(8, 24);
                }
                return sb.ToString();
            }
        }
    }
}
