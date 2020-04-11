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
using GalaSoft.MvvmLight.Messaging;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Models.Administrative;
using Unity;
namespace OSKernel.Presentation.Arranging.Administrative.Dialog
{
    /// <summary>
    /// 课时设置界面
    /// </summary>
    public partial class ClassHourSettingWindow
    {
        public ClassHourSettingWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<ClassHourSettingWindowModel>();
        }

        public ClassHourSettingWindow(List<UIClassHour> classHours) : this()
        {
            dg_classHour.ItemsSource = classHours;
        }

        private void Btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Btn_save_Click(object sender, RoutedEventArgs e)
        {

            this.DialogResult = true;
        }
    }
}
