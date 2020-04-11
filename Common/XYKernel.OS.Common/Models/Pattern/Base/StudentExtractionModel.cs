using System.Collections.Generic;
using XYKernel.OS.Common.Models.Pattern.Extend;
using XYKernel.OS.Common.Models.Mixed;

namespace XYKernel.OS.Common.Models.Pattern.Base
{
    public class StudentExtractionModel
    {
        /// <summary>
        /// 抽样比率
        /// </summary>
        public int ExtractionRatio { get; set; }

        /// <summary>
        /// 在平均班额基础上增加的班额数量
        /// </summary>
        public int IncreasedCapacity { get; set; }

        /// <summary>
        /// 移除的组合
        /// </summary>
        public List<SelectionCombinationModel> RemovedCombination { get; set; }

        /// <summary>
        /// 新课位
        /// </summary>
        public List<CoursePositionModel> Positions { get; set; }
    }
}
