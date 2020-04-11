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
namespace OSKernel.Presentation.Arranging.Mixed.Modify.Rule.ClassHour
{
    /// <summary>
    /// 课时分散
    /// </summary>
    public partial class ClassHourAverageView : UserControl
    {
        public ClassHourAverageView()
        {
            InitializeComponent();
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<ClassHourAverageViewModel>();
        }
    }
}
