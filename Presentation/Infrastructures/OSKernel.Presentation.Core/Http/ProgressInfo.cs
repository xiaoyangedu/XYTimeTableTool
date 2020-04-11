using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Core.Http
{
    /// <summary>
    /// 进度
    /// </summary>
    public class ProgressInfo
    {
        public int total { get; set; }

        public int top { get; set; }

        public int cur { get; set; }
    }
}
