using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Models.Enums
{
    /// <summary>
    /// 权重类型枚举
    /// </summary>
    public enum WeightTypeEnum
    {
        /// <summary>
        /// 高
        /// </summary>
        [Description("高")]
        Hight = 1,
        /// <summary>
        /// 中
        /// </summary>
        [Description("中")]
        Medium = 2,
        /// <summary>
        /// 低
        /// </summary>
        [Description("低")]
        Low = 3
    }
}
