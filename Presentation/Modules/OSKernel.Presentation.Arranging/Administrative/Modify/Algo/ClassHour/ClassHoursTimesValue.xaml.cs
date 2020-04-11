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
    /// 多个课时在选定课位中占用的最大数量
    /// </summary>
    public partial class ClassHoursTimesValue : UserControl
    {
        public ClassHoursTimesValue()
        {
            InitializeComponent();
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<ClassHoursTimesValueModel>();
        }

        public ClassHoursTimesValue(AdministrativeAlgoRuleEnum ruleEnum) : this()
        {
            if (ruleEnum == AdministrativeAlgoRuleEnum.ClassHoursMaxConcurrencyInSelectedTime)
            {
                dc_MaxConcurrency.Visibility = Visibility.Visible;
            }
            else if (ruleEnum == AdministrativeAlgoRuleEnum.ClassHoursOccupyMaxTimeFromSelection)
            {
                dc_Occupy.Visibility = Visibility.Visible;
            }

            (this.DataContext as ClassHoursTimesValueModel).BindData(ruleEnum);
        }
    }
}
