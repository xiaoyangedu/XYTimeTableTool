using System.Collections.Generic;
using XYKernel.OS.Common.Models.Pattern.Extend;
using XYKernel.OS.Common.Models.Mixed;

namespace XYKernel.OS.Common.Models.Pattern.Base
{
    public class NormalModel
    {
        /// <summary>
        /// 移除的组合
        /// </summary>
        public List<SelectionCombinationModel> RemovedCombination { get; set; }

        /// <summary>
        /// 新班额
        /// </summary>
        public List<ClassCapacityModel> ClassCapacity { get; set; }

        /// <summary>
        /// 新课位
        /// </summary>
        public List<CoursePositionModel> Positions { get; set; }
    }
}
