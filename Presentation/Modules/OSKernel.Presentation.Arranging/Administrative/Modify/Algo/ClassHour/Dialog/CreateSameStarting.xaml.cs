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
    /// CreateSameStarting.xaml 的交互逻辑
    /// </summary>
    public partial class CreateSameStarting
    {
        public UIClassHourRule Modify { get; set; }
        public UIClassHourRule Add { get; set; }

        public CreateSameStartingModel VM
        {
            get
            {
                return this.DataContext as CreateSameStartingModel;
            }
        }

        public CreateSameStarting()
        {
            InitializeComponent();
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<CreateSameStartingModel>();
            Owner = System.Windows.Application.Current.MainWindow;
        }

        public CreateSameStarting(OperatorEnum operatorEnum, AdministrativeAlgoRuleEnum rule) : this()
        {
            VM?.SetValue(operatorEnum, rule);
        }

        public CreateSameStarting(OperatorEnum operatorEnum, AdministrativeAlgoRuleEnum ruleType, UIClassHourRule rule) : this()
        {
            VM?.SetValue(operatorEnum, ruleType, rule);
        }
    }
}
