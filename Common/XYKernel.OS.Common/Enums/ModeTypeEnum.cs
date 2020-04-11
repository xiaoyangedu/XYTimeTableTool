using System;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace XYKernel.OS.Common.Enums
{
    /// <summary>
    /// 模式类型枚举
    /// </summary>
    public enum ModeTypeEnum
    {
        /// <summary>
        /// 常规排课
        /// </summary>
        [Description("常规排课")]
        Normal = 0,
        /// <summary>
        /// 数据核查
        /// </summary>
        [Description("数据核查")]
        CheckData = 1,
        /// <summary>
        /// 部分数据（调整精度,班额相应增加）
        /// </summary>
        [Description("精度调整")]
        Partial_PrecisionClassSize = 2,
        /// <summary>
        /// 压缩学生
        /// </summary>
        [Description("压缩学生")]
        Zip_Student = 3,
        /// <summary>
        /// 课位压缩
        /// </summary>
        [Description("课位压缩")]
        Zip_ClassPosition = 4,
        /// <summary>
        /// 深度优化
        /// </summary>
        [Description("精度优化")]
        Deep = 5,
        /// <summary>
        /// 成绩均衡
        /// </summary>
        [Description("成绩均衡")]
        AchievementBalance = 6,
        /// <summary>
        /// 压缩优化
        /// </summary>
        [Description("压缩优化")]
        ZipOptimization = 7
    }
}
