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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unity;

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Rule.ClassHour
{
    /// <summary>
    /// 同时开课
    /// </summary>
    public partial class ClassHourSameOpenView : UserControl
    {
        public ClassHourSameOpenView()
        {
            InitializeComponent();
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<ClassHourSameOpenViewModel>();
        }
    }
}
