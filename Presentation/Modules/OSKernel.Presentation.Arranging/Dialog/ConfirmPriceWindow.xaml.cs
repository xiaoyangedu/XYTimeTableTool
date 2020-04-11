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
    /// 确认排课价格
    /// </summary>
    public partial class ConfirmPriceWindow
    {
        public ConfirmPriceWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
        }

        public ConfirmPriceWindow(double price) : this()
        {
            txt_price.Text = string.Format("{0:N}", price);
        }

        private void Btn_save_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
