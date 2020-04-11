using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models.Administrative;

namespace OSKernel.Presentation.Core.Http
{
    /// <summary>
    /// 行政班
    /// </summary>
    public class CPTransfer
    {
        public CPCase cp { get; set; }

        public XYKernel.OS.Common.Models.Administrative.Rule.Rule rule { get; set; }

        public XYKernel.OS.Common.Models.Administrative.AlgoRule.AlgoRule algo { get; set; }
    }
}
