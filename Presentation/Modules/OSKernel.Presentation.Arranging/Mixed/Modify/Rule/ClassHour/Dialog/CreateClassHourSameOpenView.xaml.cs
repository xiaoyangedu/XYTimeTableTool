using OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Teacher.Model;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Models.Mixed;
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
namespace OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Teacher.Dialog
{
    /// <summary>
    /// 创建同时开课
    /// </summary>
    public partial class CreateClassHourSameOpenView
    {
        /// <summary>
        /// 是否保存
        /// </summary>
        //public bool IsSave { get; set; }

        /// <summary>
        /// 选择的班级
        /// </summary>
        public List<UIClass> Classes { get; set; }

        public CreateClassHourSameOpenView()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<CreateClassHourSameOpenViewModel>();
            //this.Closing += CreateClassHourSameOpenView_Closing;
        }

        private void CreateClassHourSameOpenView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //this.DialogResult = IsSave;
        }
    }
}
