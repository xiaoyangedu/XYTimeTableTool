using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OSKernel.Presentation.Arranging.Mixed.Pattern
{
    /// <summary>
    /// 模式对应的操作
    /// </summary>
    public class UIPatternOperator
    {
        /// <summary>
        /// 操作名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.Operator.GetLocalDescription();
            }
        }

        /// <summary>
        /// 操作类型
        /// </summary>
        public OperatorEnum Operator { get; set; }

        /// <summary>
        /// 控件
        /// </summary>
        public UserControl View { get; set; }

        /// <summary>
        /// 操作枚举（与UIPattern 关联属于当前类的一部分）
        /// </summary>
        public enum OperatorEnum
        {
            /// <summary>
            /// 去除组合
            /// </summary>
            [Description("去除组合")]
            RemoveCombination,

            /// <summary>
            /// 班额调整
            /// </summary>
            [Description("班额调整")]
            ClassCapacity,

            /// <summary>
            /// 课位调整
            /// </summary>
            [Description("课位调整")]
            Position,
        }
    }
}
