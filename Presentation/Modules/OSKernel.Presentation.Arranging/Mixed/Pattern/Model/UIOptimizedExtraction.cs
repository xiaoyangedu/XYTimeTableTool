using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models.Mixed.Result;
using XYKernel.OS.Common.Models.Pattern.Extend;

namespace OSKernel.Presentation.Arranging.Mixed.Pattern.Model
{
    /// <summary>
    /// 抽样排课优化
    /// </summary>
    public class UIOptimizedExtraction
    {
        /// <summary>
        /// 排课结果
        /// </summary>
        public ResultModel Result { get; set; }

        /// <summary>
        /// 班额
        /// </summary>
        public int ClassCapacity { get; set; }

        /// <summary>
        /// 移除的组合
        /// </summary>
        public List<SelectionCombinationModel> RemovedCombination { get; set; }
    }
}
