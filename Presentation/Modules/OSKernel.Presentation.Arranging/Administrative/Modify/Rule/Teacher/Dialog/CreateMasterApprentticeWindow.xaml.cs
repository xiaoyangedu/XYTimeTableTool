using OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Teacher.Model;
using OSKernel.Presentation.Core;
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
namespace OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Teacher.Dialog
{
    /// <summary>
    /// 创建师徒跟随
    /// </summary>
    public partial class CreateMasterApprentticeWindow
    {
        /// <summary>
        /// 是否保存
        /// </summary>
        //public bool IsSave { get; set; }

        public UIMasterApprenttice Rule { get; set; }

        public Models.Enums.WeightTypeEnum WeightType { get; set; }

        public CreateMasterApprentticeWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<CreateMasterApprentticeWindowModel>();
            //this.Closing += CreateMasterApprentticeWindow_Closing;
        }

        private void CreateMasterApprentticeWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //this.DialogResult = IsSave;
        }
    }
}
