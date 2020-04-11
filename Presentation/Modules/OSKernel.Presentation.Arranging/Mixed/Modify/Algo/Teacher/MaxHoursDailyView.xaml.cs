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
namespace OSKernel.Presentation.Arranging.Mixed.Modify.Algo.Teacher
{
    /// <summary>
    /// 教师每天最大课时数
    /// </summary>
    public partial class MaxHoursDailyView : UserControl
    {
        public MaxHoursDailyView()
        {
            InitializeComponent();
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<MaxHoursDailyViewModel>();
        }

        public MaxHoursDailyView(MixedAlgoRuleEnum ruleEnum) : this()
        {
            (this.DataContext as MaxHoursDailyViewModel).BindData(ruleEnum);
        }
    }
}
