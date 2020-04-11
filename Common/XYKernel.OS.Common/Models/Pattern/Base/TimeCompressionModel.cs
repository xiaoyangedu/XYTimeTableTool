using System.Collections.Generic;
using XYKernel.OS.Common.Models.Pattern.Extend;

namespace XYKernel.OS.Common.Models.Pattern.Base
{
    public class TimeCompressionModel
    {
        /// <summary>
        /// 课时压缩比率
        /// </summary>
        public int CompressionRatio { get; set; }

        /// <summary>
        /// 移除的组合
        /// </summary>
        public List<SelectionCombinationModel> RemovedCombination { get; set; }

        /// <summary>
        /// 新班额
        /// </summary>
        public List<ClassCapacityModel> ClassCapacity { get; set; }
    }
}
