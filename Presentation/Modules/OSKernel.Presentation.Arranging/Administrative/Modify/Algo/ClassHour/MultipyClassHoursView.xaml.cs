using OSKernel.Presentation.Arranging.Administrative.Modify.Algo.ClassHour.Dialog;
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
    /// 多个课时规则
    /// </summary>
    public partial class MultipyClassHoursView : UserControl
    {
        public MultipyClassHoursView()
        {
            InitializeComponent();
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<MultipyClassHoursViewModel>();
        }

        public MultipyClassHoursView(AdministrativeAlgoRuleEnum ruleEnum) : this()
        {
            (this.DataContext as MultipyClassHoursViewModel).BindData(ruleEnum);
        }
    }
}
