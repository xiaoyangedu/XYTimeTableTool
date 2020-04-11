using System.ComponentModel;

namespace OSKernel.Presentation.Models.Enums
{
    /// <summary>
    /// 操作枚举类
    /// </summary>
    public enum OperatorEnum
    {
        /// <summary>
        /// 添加
        /// </summary>
        [Description("新建")]
        Add = 0,
        /// <summary>
        /// 修改
        /// </summary>
        [Description("修改")]
        Modify = 1,
        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        Delete = 2
    }
}
