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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unity;
namespace OSKernel.Presentation.Arranging.Mixed.Modify.Algo.ClassHour
{
    /// <summary>
    /// 多个课时有相同的开始日期（日期）
    /// </summary>
    public partial class HoursSameStartingView : UserControl
    {
        public HoursSameStartingView()
        {
            InitializeComponent();
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<HoursSameStartingViewModel>();
        }

        public HoursSameStartingView(MixedAlgoRuleEnum ruleEnum) : this()
        {
            (this.DataContext as HoursSameStartingViewModel).BindData(ruleEnum);
        }
    }
}
