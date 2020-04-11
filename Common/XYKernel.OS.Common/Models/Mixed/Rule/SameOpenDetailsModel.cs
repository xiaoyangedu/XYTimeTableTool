using System.Collections.Generic;

namespace XYKernel.OS.Common.Models.Mixed.Rule
{
    /// <summary>
    /// 同时开课规则
    /// </summary>
    public class SameOpenDetailsModel
    {
        /// <summary>
        /// 课时索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        public List<string> Classes { get; set; }
    }
}
