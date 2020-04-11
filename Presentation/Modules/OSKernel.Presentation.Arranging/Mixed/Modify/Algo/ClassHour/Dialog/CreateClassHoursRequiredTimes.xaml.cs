using OSKernel.Presentation.Arranging.Mixed.Modify.Algo.ClassHour.Model;
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
namespace OSKernel.Presentation.Arranging.Mixed.Modify.Algo.ClassHour.Dialog
{
    /// <summary>
    /// CreateSameStarting.xaml 的交互逻辑
    /// </summary>
    public partial class CreateClassHoursRequiredTimes
    {
        public UIClassHourRule Modify { get; set; }
        public UIClassHourRule Add { get; set; }

        public CreateClassHoursRequiredTimesModel VM
        {
            get
            {
                return this.DataContext as CreateClassHoursRequiredTimesModel;
            }
        }

        public CreateClassHoursRequiredTimes()
        {
            InitializeComponent();
            this.Owner = System.Windows.Application.Current.MainWindow;
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<CreateClassHoursRequiredTimesModel>();
        }

        public CreateClassHoursRequiredTimes(OperatorEnum operatorEnum, MixedAlgoRuleEnum rule) : this()
        {
            VM?.SetValue(operatorEnum, rule);
        }

        public CreateClassHoursRequiredTimes(OperatorEnum operatorEnum, MixedAlgoRuleEnum ruleType, UIClassHourRule rule) : this()
        {
            VM?.SetValue(operatorEnum, ruleType, rule);
        }
    }
}
