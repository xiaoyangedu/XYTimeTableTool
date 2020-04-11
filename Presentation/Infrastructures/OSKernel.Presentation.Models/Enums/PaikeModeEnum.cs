using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Models.Enums
{
    /// <summary>
    /// 排课模式枚举
    /// </summary>
    public enum PaikeModeEnum : short
    {
        /// <summary>
        /// 行政班
        /// </summary>
        [Description("行政班")]
        CP = 0,
        /// <summary>
        /// 走班
        /// </summary>
        [Description("走班")]
        CL = 1,
        /// <summary>
        /// 分班
        /// </summary>
        [Description("分班")]
        DC = 2,
        /// <summary>
        /// 试卷讲评
        /// </summary>
        [Description("试卷讲评")]
        PR = 3
    }
}
