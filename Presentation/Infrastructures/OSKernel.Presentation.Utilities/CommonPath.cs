using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace OSKernel.Presentation.Utilities
{
    /// <summary>
    /// 全局路径
    /// </summary>
    public static class CommonPath
    {
        /// <summary>
        /// 默认用户数据路径
        /// </summary>
        public readonly static string DefaultData = @"Data\Default";

        /// <summary>
        /// 数据根目录
        /// </summary>
        public readonly static string Data = @"Data\";

        /// <summary>
        /// 客户端更新
        /// </summary>
        public readonly static string ClientUpdate = "ClientUpdate.xml";

        /// <summary>
        /// 服务端更新
        /// </summary>
        public readonly static string ServiceUpdate = "ServiceUpdate.xml";

        /// <summary>
        /// 返回文件存储地址
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string CombineCurrentDirectory(this string path)
        {
            return System.IO.Path.Combine(Environment.CurrentDirectory, path);
        }
    }
}
