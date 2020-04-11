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

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Algo.ClassHour
{
    /// <summary>
    /// 多个课时视图
    /// </summary>
    public partial class BetweenClassHoursView : UserControl
    {
        public BetweenClassHoursView()
        {
            InitializeComponent();
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<BetweenClassHoursViewModel>();
        }

        public BetweenClassHoursView(AdministrativeAlgoRuleEnum ruleEnum) : this()
        {
            (this.DataContext as BetweenClassHoursViewModel).BindData(ruleEnum);
        }
    }
}
