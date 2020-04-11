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
namespace OSKernel.Presentation.Arranging.Administrative.Modify.Algo.Teacher.Dialog
{
    /// <summary>
    /// 创建教师每周最大工作天数
    /// </summary>
    public partial class CreateMaxDaysPerWeek
    {
        public UITeacherRule Modify { get; set; }
        public UITeacherRule Add { get; set; }

        public CreateMaxDaysPerWeekModel VM
        {
            get
            {
                return this.DataContext as CreateMaxDaysPerWeekModel;
            }
        }

        public CreateMaxDaysPerWeek()
        {
            InitializeComponent();
            Owner = System.Windows.Application.Current.MainWindow;

            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<CreateMaxDaysPerWeekModel>();
        }

        public CreateMaxDaysPerWeek(OperatorEnum operatorEnum, AdministrativeAlgoRuleEnum rule) : this()
        {
            VM?.SetValue(operatorEnum, rule);
        }

        public CreateMaxDaysPerWeek(OperatorEnum operatorEnum, AdministrativeAlgoRuleEnum ruleType, UITeacherRule rule) : this()
        {
            VM?.SetValue(operatorEnum, ruleType, rule);
        }
    }
}
