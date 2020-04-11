using OSKernel.Presentation.CustomControl.Enums;
using System.Windows;

namespace OSKernel.Presentation.CustomControl
{
    public class DialogWindowSettings
    {  
        /// <summary>
        /// 内容标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 具体信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 按钮显示样式
        /// </summary>
        public DialogSettingType SettingType { get; set; }

        /// <summary>
        /// 对话框类型：警告、错误
        /// </summary>
        public DialogType WindowType { get; set; }

        /// <summary>
        /// 对话框宽度，默认null
        /// </summary>
        public double? Width { get; set; }

        /// <summary>
        /// 对话框高度，默认null
        /// </summary>
        public double? Height { get; set; }

        /// <summary>
        /// 是否显示复选框
        /// </summary>
        public bool ShowChecked = false;

        /// <summary>
        /// 复选框显示内容
        /// </summary>
        public string CheckBoxString = "";

        /// <summary>
        /// Ok按钮内容
        /// </summary>
        public string OkbtnContent { get; set; }

        /// <summary>
        /// 取消按钮内容
        /// </summary>
        public string CancelContent { get; set; }

        /// <summary>
        /// 忽略按钮内容
        /// </summary>
        public string IgnoreContent { get; set; }

        /// <summary>
        /// 确定按钮样式
        /// </summary>
        public Style Okstyle { get; set; }

        /// <summary>
        /// 取消按钮样式
        /// </summary>
        public Style CancelStyle
        {
            get; set;
        }

        /// <summary>
        /// 忽略按钮样式
        /// </summary>
        public Style IgnoreStyle { get; set; }
    }
}
