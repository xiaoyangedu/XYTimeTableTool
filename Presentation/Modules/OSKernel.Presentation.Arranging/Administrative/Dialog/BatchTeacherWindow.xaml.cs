using OSKernel.Presentation.Core;
using System;
using Unity;
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
using OSKernel.Presentation.Models.Base;

namespace OSKernel.Presentation.Arranging.Administrative.Dialog
{
    /// <summary>
    /// 批量设置教师
    /// </summary>
    public partial class BatchTeacherWindow
    {
        /// <summary>
        /// 是否点击保存
        /// </summary>
        public bool IsSave { get; set; }

        /// <summary>
        /// 课程层
        /// </summary>
        public List<UIClass> Classes { get; set; }

        public BatchTeacherWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<BatchTeacherWindowModel>();
            this.Closing += BatchTeacherWindow_Closing;
        }

        private void BatchTeacherWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.DialogResult = IsSave;
        }

        public BatchTeacherWindow(List<UIClass> classes) : this()
        {
            (this.DataContext as BatchTeacherWindowModel).BindData(classes);
        }
    }
}
