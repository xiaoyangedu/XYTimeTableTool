using OSKernel.Presentation.Arranging.Administrative.Modify.Algo.ClassHour.Model;
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
namespace OSKernel.Presentation.Arranging.Administrative.Modify.Algo.ClassHour.Dialog
{
    /// <summary>
    /// 多个课时不同时开课
    /// </summary>
    public partial class CreateRequiredStartingTime
    {
        public CreateRequiredStartingTimeModel VM
        {
            get
            {
                return this.DataContext as CreateRequiredStartingTimeModel;
            }
        }

        public UIClassHourRule Modify { get; set; }
        public UIClassHourRule Add { get; set; }

        public CreateRequiredStartingTime()
        {
            InitializeComponent();
            this.Owner = System.Windows.Application.Current.MainWindow;
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<CreateRequiredStartingTimeModel>();
        }

        public CreateRequiredStartingTime(OperatorEnum operatorEnum, AdministrativeAlgoRuleEnum rule) : this()
        {
            VM?.SetValue(operatorEnum, rule);
        }

        public CreateRequiredStartingTime(OperatorEnum operatorEnum, AdministrativeAlgoRuleEnum ruleType, UIClassHourRule rule) : this()
        {
            VM?.SetValue(operatorEnum, ruleType, rule);
        }
    }
}
