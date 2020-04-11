using System.ComponentModel;

namespace XYKernel.OS.Common.Enums
{
    public enum ClassHourResultType
    {
        /// <summary>
        /// 双周课时
        /// </summary>
        [Description("双周")]
        Dual = -2,
        /// <summary>
        /// 单周课时
        /// </summary>
        [Description("单周")]
        Odd = -1,
        /// <summary>
        /// 常规课时
        /// </summary>
        [Description("正常")]
        Normal = 0
    }
}
