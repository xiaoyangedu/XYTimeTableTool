using OSKernel.Presentation.Models;
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

namespace OSKernel.Presentation.Arranging.Dialog
{
    /// <summary>
    /// 选择课时窗口
    /// </summary>
    public partial class SetCapacityWindow
    {
        public int Capacity = 5;
        public SetCapacityWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
        }

        private void Btn_save_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(txt_capacity.Text, out Capacity);
            this.DialogResult = true;
        }

        private void Btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
