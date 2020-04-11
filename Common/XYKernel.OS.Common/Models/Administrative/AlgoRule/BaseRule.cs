using System;
using System.Collections.Generic;
using System.Text;

namespace XYKernel.OS.Common.Models.Administrative.AlgoRule
{
    /// <summary>
    /// 规则基础
    /// </summary>
    public class BaseRule
    {
        /// <summary>
        /// 规则唯一ID
        /// </summary>
        public string UID { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Active { get; set; }
    }
}
