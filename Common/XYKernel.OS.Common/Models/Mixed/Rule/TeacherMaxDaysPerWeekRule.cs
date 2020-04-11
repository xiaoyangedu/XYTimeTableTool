namespace XYKernel.OS.Common.Models.Mixed.Rule
{
    /// <summary>
    /// 教师每周最大工作天数
    /// </summary>
    public class TeacherMaxDaysPerWeekRule
    {
        /// <summary>
        /// 教师
        /// </summary>
        public string TeacherID { get; set; }

        /// <summary>
        /// 最大
        /// </summary>
        public int MaxDay { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}
