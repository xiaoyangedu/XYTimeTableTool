namespace XYKernel.OS.Common.Models.Mixed.Rule
{
    /// <summary>
    ///  教师每天最大课时间隔
    /// </summary>
    public class TeacherMaxGapsPerDayRule
    {
        /// <summary>
        /// 教师ID
        /// </summary>
        public string TeacherID { get; set; }

        /// <summary>
        /// 间隔
        /// </summary>
        public int MaxIntervel { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}
