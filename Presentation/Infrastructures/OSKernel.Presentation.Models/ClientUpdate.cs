using OSKernel.Presentation.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Models
{
    /// <summary>
    /// 客户端更新文件
    /// </summary>
    [Serializable]
    public class ClientUpdate
    {
        /// <summary>
        /// 版本信息
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 升级内容
        /// </summary>
        public string Conent { get; set; }

        /// <summary>
        /// 版本类型
        /// </summary>
        public VersionTypeEnum VersionType { get; set; }

        /// <summary>
        /// 是否开启自动更新
        /// </summary>
        public bool IsOpen { get; set; }

        /// <summary>
        /// 升级文件所在服务端的网址
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// 服务路径-目标版本路径
        /// </summary>
        public string Path { get; set; }
    }
}
