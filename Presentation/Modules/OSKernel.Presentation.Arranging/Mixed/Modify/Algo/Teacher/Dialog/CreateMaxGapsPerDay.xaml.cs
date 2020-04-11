using OSKernel.Presentation.Arranging.Mixed.Modify.Algo.Teacher.Model;
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
namespace OSKernel.Presentation.Arranging.Mixed.Modify.Algo.Teacher.Dialog
{
    /// <summary>
    /// CreateMaxGapsPerDay.xaml 的交互逻辑
    /// </summary>
    public partial class CreateMaxGapsPerDay
    {
        public UITeacherRule Modify { get; set; }
        public UITeacherRule Add { get; set; }

        public CreateMaxGapsPerDayModel VM
        {
            get
            {
                return this.DataContext as CreateMaxGapsPerDayModel;
            }
        }

        public CreateMaxGapsPerDay()
        {
            InitializeComponent();
            Owner = System.Windows.Application.Current.MainWindow;
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<CreateMaxGapsPerDayModel>();
        }

        public CreateMaxGapsPerDay(OperatorEnum operatorEnum, MixedAlgoRuleEnum rule) : this()
        {
            VM?.SetValue(operatorEnum, rule);
        }

        public CreateMaxGapsPerDay(OperatorEnum operatorEnum, MixedAlgoRuleEnum ruleType, UITeacherRule rule) : this()
        {
            VM?.SetValue(operatorEnum, ruleType, rule);
        }

    }
}
