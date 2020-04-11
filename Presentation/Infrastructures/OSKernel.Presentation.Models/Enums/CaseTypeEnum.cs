using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Models.Enums
{
    /// <summary>
    /// 方案类型枚举
    /// </summary>
    public enum CaseTypeEnum
    {
        /// <summary>
        /// 行政班
        /// </summary>
        [Description("行政班")]
        Administrative = 0,

        /// <summary>
        /// 走班
        /// </summary>
        [Description("走班")]
        Mixed = 1,

        /// <summary>
        /// 分班
        /// </summary>
        [Description("分班")]
        Divide = 2,
    }
}
