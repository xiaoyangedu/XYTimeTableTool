using System.Collections.Generic;

namespace XYKernel.OS.Common.Models.Mixed.Rule
{
    /// <summary>
    /// 同时开课
    /// </summary>
    public class ClassHourSameOpenRule
    {
        /// <summary>
        /// 同时开课详细
        /// </summary>
        public List<SameOpenDetailsModel> Details { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}
