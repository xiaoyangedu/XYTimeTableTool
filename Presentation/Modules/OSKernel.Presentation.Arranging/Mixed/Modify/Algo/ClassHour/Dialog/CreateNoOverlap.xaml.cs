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
    /// 多个课时不同时开课
    /// </summary>
    public partial class CreateNoOverlap
    {
        public CreateNoOverlapModel VM
        {
            get
            {
                return this.DataContext as CreateNoOverlapModel;
            }
        }

        public UIClassHourRule Modify { get; set; }
        public UIClassHourRule Add { get; set; }

        public CreateNoOverlap()
        {
            InitializeComponent();
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<CreateNoOverlapModel>();
            Owner = System.Windows.Application.Current.MainWindow;
        }

        public CreateNoOverlap(OperatorEnum operatorEnum, AdministrativeAlgoRuleEnum rule) : this()
        {
            VM?.SetValue(operatorEnum, rule);
        }

        public CreateNoOverlap(OperatorEnum operatorEnum, AdministrativeAlgoRuleEnum ruleType, UIClassHourRule rule) : this()
        {
            VM?.SetValue(operatorEnum, ruleType, rule);
        }
    }
}
