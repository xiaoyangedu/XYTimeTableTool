using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Core.Http
{
    /// <summary>
    /// 走班结果
    /// </summary>
    public class ResultMixed
    {
        /// <summary>
        /// ID
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        public XYKernel.OS.Common.Models.Mixed.Result.ResultAdjustmentModel fruit { get; set; }
    }
}
