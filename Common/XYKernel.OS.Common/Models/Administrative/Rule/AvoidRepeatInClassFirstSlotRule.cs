namespace XYKernel.OS.Common.Models.Administrative.Rule
{
    /// <summary>
    /// 各班级第一个课时尽量不重复
    /// </summary>
    public class AvoidRepeatInClassFirstSlotRule
    {
        /// <summary>
        /// 默认不启用
        /// </summary>
        public bool Active = false;

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}