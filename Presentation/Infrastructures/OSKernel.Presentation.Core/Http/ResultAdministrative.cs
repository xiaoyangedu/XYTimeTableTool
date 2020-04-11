using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Core.Http
{
    public class ResultAdministrative
    {
        /// <summary>
        /// ID
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// 调整结果
        /// </summary>
        public XYKernel.OS.Common.Models.Administrative.Result.ResultAdjustmentModel fruit { get; set; }
    }
}
