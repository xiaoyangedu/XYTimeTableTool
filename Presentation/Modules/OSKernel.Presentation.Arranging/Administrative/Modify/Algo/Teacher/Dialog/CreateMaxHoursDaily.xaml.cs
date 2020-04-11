using OSKernel.Presentation.Arranging.Administrative.Modify.Algo.Teacher.Model;
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
using XYKernel.OS.Common.Models.Administrative.AlgoRule;

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Algo.Teacher.Dialog
{
    /// <summary>
    /// 创建教师每天最大课时
    /// </summary>
    public partial class CreateMaxHoursDaily
    {
        public UITeacherRule Modify { get; set; }
        public UITeacherRule Add { get; set; }

        public CreateMaxHoursDailyModel VM
        {
            get
            {
                return this.DataContext as CreateMaxHoursDailyModel;
            }
        }

        public CreateMaxHoursDaily()
        {
            InitializeComponent();
            Owner = System.Windows.Application.Current.MainWindow;
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<CreateMaxHoursDailyModel>();
        }

        public CreateMaxHoursDaily(OperatorEnum operatorEnum, AdministrativeAlgoRuleEnum rule) : this()
        {
            VM?.SetValue(operatorEnum, rule);
        }

        public CreateMaxHoursDaily(OperatorEnum operatorEnum, AdministrativeAlgoRuleEnum ruleType, UITeacherRule rule) : this()
        {
            VM?.SetValue(operatorEnum, ruleType, rule);
        }
    }
}
