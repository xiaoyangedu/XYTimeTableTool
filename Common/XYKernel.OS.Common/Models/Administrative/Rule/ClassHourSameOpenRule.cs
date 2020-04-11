using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Administrative.Rule
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
