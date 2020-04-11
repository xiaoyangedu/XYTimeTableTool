using OSKernel.Presentation.Core;
using OSKernel.Presentation.Models.Base;
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
namespace OSKernel.Presentation.Arranging.Administrative.Dialog
{
    /// <summary>
    /// Interaction logic for CreateMutexGroup.xaml
    /// </summary>
    public partial class CreateClassUnion
    {
        /// <summary>
        /// 是否保存
        /// </summary>
        //public bool IsSave { get; set; }

        /// <summary>
        /// 选中课程
        /// </summary>
        public UICourse SelectCourse { get; set; }

        /// <summary>
        /// 选中班级
        /// </summary>
        public List<UIClass> SelectClasses { get; set; }

        public CreateClassUnion()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<CreateClassUnionModel>();

            //this.Closing += CreateClassUnion_Closing;
        }

        private void CreateClassUnion_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //this.DialogResult = IsSave;
        }
    }
}
