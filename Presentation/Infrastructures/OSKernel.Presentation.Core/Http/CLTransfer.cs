using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models.Mixed;

namespace OSKernel.Presentation.Core.Http
{
    /// <summary>
    /// 走班
    /// </summary>
    public class CLTransfer
    {
        public CLCase cl { get; set; }

        public XYKernel.OS.Common.Models.Mixed.Rule.Rule rule { get; set; }

        public XYKernel.OS.Common.Models.Mixed.AlgoRule.AlgoRule algo { get; set; }
    }
}
