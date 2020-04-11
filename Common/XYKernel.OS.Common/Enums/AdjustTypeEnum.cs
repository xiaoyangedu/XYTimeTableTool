using System;
using System.Collections.Generic;
using System.Text;

namespace XYKernel.OS.Common.Enums
{
    /// <summary>
    /// 调整类型枚举
    /// </summary>
    public enum AdjustTypeEnum
    {
        /// <summary>
        /// 空（从一个位置拖拽到另一个位置）
        /// </summary>
        Empty = 0,

        /// <summary>
        /// 替换（两个课位进行替换）
        /// </summary>
        Replace = 1,

        /// <summary>
        /// 课程框
        /// </summary>
        CourseFrame = 2,
    }
}
