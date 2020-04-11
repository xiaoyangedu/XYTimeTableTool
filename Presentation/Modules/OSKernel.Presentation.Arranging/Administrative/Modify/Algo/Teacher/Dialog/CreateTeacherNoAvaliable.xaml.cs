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
    /// 创建教师不可用时间
    /// </summary>
    public partial class CreateTeacherNoAvaliable
    {
        public UITeacherRule Modify { get; set; }
        public UITeacherRule Add { get; set; }

        public CreateTeacherNoAvaliableModel VM
        {
            get
            {
                return this.DataContext as CreateTeacherNoAvaliableModel;
            }
        }

        public CreateTeacherNoAvaliable()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<CreateTeacherNoAvaliableModel>();
        }

        public CreateTeacherNoAvaliable(OperatorEnum operatorEnum, AdministrativeAlgoRuleEnum ruleType) : this()
        {
            VM?.SetValue(operatorEnum, ruleType);
        }

        public CreateTeacherNoAvaliable(OperatorEnum operatorEnum, AdministrativeAlgoRuleEnum ruleType, UITeacherRule rule) : this()
        {
            VM?.SetValue(operatorEnum, ruleType, rule);
        }
    }
}
