using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Core.EventArgs
{
    /// <summary>
    /// 方案事件参数
    /// </summary>
    public class CaseEventArgs
    {
        /// <summary>
        /// 方案
        /// </summary>
        public Case Model { get; set; }

        /// <summary>
        /// 事件类型
        /// </summary>
        public EventTypeEnum EventType { get; set; }

        /// <summary>
        /// 事件类型枚举
        /// </summary>
        public enum EventTypeEnum
        {
            /// <summary>
            /// 创建方案
            /// </summary>
            Create,

            /// <summary>
            /// 复制方案
            /// </summary>
            Copy,

            /// <summary>
            /// 删除方案
            /// </summary>
            Delete,

            /// <summary>
            /// 困难信息
            /// </summary>
            Difficulty,

            /// <summary>
            /// 错误信息
            /// </summary>
            Clash,

            /// <summary>
            /// 逻辑消息
            /// </summary>
            LogicMessage,

            /// <summary>
            /// 创建XML
            /// </summary>
            CreateResult
        }
    }
}
