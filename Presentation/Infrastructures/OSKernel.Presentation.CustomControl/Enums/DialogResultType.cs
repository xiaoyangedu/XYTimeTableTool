using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.CustomControl.Enums
{
    /// <summary>
    /// 对话框窗体返回类型
    /// </summary>
    public enum DialogResultType
    {
        /// <summary>
        /// 确定
        /// </summary>
        OK,
        /// <summary>
        /// 取消
        /// </summary>
        Cancel,
        /// <summary>
        /// 忽略
        /// </summary>
        Ignore,
        /// <summary>
        /// 直接点击的关闭按钮关闭的窗口
        /// </summary>
        None
    }
}
