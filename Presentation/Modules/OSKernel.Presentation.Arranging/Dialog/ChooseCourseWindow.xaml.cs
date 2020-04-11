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
namespace OSKernel.Presentation.Arranging.Dialog
{
    /// <summary>
    /// Interaction logic for ChooseCourseWindow.xaml
    /// </summary>
    public partial class ChooseCourseWindow
    {
        /// <summary>
        /// 选中课程
        /// </summary>
        public List<Models.Base.UICourse> Courses { get; set; }

        public ChooseCourseWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<ChooseCourseWindowModel>();
        }

        public ChooseCourseWindow(List<Models.Base.UICourse> current, List<Models.Base.UICourse> all) : this()
        {
            (this.DataContext as ChooseCourseWindowModel).BindData(current, all);
        }
    }
}
