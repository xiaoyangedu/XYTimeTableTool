using OSKernel.Presentation.Core;
using OSKernel.Presentation.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Unity;
using XYKernel.OS.Common.Models.Mixed;
using XYKernel.OS.Common.Models.Pattern.Extend;

namespace OSKernel.Presentation.Arranging.Mixed.Dialog
{
    /// <summary>
    /// 模式
    /// </summary>
    public partial class SetPatternWindow
    {
        /// <summary>
        /// 模式类型
        /// </summary>
        public PatternTypeEnum PatternType { get; set; }

        public SetPatternWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<SetPatternWindowModel>();
        }
    }
}
