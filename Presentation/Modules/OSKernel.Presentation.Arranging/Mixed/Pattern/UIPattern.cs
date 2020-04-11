using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OSKernel.Presentation.Arranging.Mixed.Pattern
{
    /// <summary>
    /// 模式
    /// </summary>
    public class UIPattern
    {
        /// <summary>
        /// 模式
        /// </summary>
        public Models.Enums.PatternTypeEnum Pattern { get; set; }

        /// <summary>
        /// 界面
        /// </summary>
        public UserControl View { get; set; }

        /// <summary>
        /// 模式名称
        /// </summary>
        public string Name
        {
            get
            {
                return Pattern.GetLocalDescription();
            }
        }

    }
}
