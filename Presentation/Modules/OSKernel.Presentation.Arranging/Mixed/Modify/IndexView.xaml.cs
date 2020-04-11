using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.DataManager;
using OSKernel.Presentation.Core.Http;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unity;
using XYKernel.Application.OS.Models.Xml.MixedClass;
using XYKernel.Application.OS.Data.MixedClass;
using XYKernel.OS.Common.Models.Pattern.Base;
using System.IO;
using System.Xml.Serialization;
using XYKernel.OS.Common.Models.Mixed;
using XYKernel.OS.Common.Models.Mixed.AlgoRule;

namespace OSKernel.Presentation.Arranging.Mixed.Modify
{
    /// <summary>
    /// 编辑页面首页
    /// </summary>
    public partial class IndexView : UserControl
    {
        public IndexView()
        {
            InitializeComponent();
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<IndexViewModel>();
        }
    }
}
