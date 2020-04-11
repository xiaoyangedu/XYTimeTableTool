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
    /// <summary>
    /// 内置课程
    /// </summary>
    public partial class SystemCourseWindow
    {
        /// <summary>
        /// 课程
        /// </summary>
        public List<UICourse> Courses { get; set; }

        public SystemCourseWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<SystemCourseWindowModel>();
        }
    }
}
