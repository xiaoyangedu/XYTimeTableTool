using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Core.Http
{
    /// <summary>
    /// 价格信息（价格单位：分）
    /// </summary>
    public class PriceInfo
    {
        /// <summary>
        /// 行政班价格
        /// </summary>
        public double cpprice { get; set; }

        /// <summary>
        /// 走班价格
        /// </summary>
        public double clprice { get; set; }
    }
}
