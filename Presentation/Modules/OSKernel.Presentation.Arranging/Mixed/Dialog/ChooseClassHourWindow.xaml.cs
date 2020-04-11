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
namespace OSKernel.Presentation.Arranging.Mixed.Dialog
{
    public partial class ChooseClassHourWindow
    {
        /// <summary>
        /// 是否保存操作
        /// </summary>
        public bool IsSave { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        public List<UICourseLevelTree> CourseLevels { get; set; }

        public ChooseClassHourWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<ChooseClassHourWindowModel>();
            this.Closing += ChooseClassHourWindow_Closing;
        }

        private void ChooseClassHourWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.DialogResult = IsSave;
        }
    }
}
