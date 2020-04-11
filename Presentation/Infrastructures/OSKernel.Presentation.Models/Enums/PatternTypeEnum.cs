using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Models.Enums
{
    /// <summary>
    /// 模式类型枚举
    /// </summary>
    public enum PatternTypeEnum
    { /// <summary>
      /// 无模式
      /// </summary>
        [Description("无模式")]
        None = 0,

        /// <summary>
        /// 常规模式
        /// </summary>
        [Description("常规模式")]
        Normal = 1,
        /// <summary>
        /// 数据核查
        /// </summary>
        [Description("数据核查")]
        Validation = 2,
        /// <summary>
        /// 抽样排课
        /// </summary>
        [Description("抽样排课")]
        Extraction = 3,
        /// <summary>
        /// 抽样排课优化
        /// </summary>
        [Description("抽样排课优化")]
        OptimizedExtraction = 4,
        /// <summary>
        /// 课位压缩
        /// </summary>
        [Description("课位压缩")]
        Compression = 5,
        /// <summary>
        /// 课位压缩优化
        /// </summary>
        [Description("课位压缩优化")]
        OptimizedCompression = 6,
    }
}
