using Unity;
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
using OSKernel.Presentation.Core;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Views
{
    /// <summary>
    /// Interaction logic for TeacherView.xaml
    /// </summary>
    public partial class TeacherView : UserControl
    {
        public TeacherView()
        {
            InitializeComponent();
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<TeacherViewModel>();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as TeacherViewModel).SaveCommand.Execute(sender);
        }
    }
}
