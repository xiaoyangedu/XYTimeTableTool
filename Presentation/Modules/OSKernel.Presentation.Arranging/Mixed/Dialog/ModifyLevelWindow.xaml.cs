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

namespace OSKernel.Presentation.Arranging.Mixed.Dialog
{
    /// <summary>
    /// 修改层名称
    /// </summary>
    public partial class ModifyLevelWindow
    {
        /// <summary>
        /// 层名称
        /// </summary>
        public string Level { get; set; }

        public ModifyLevelWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.txt_level.Focus();
        }

        public ModifyLevelWindow(string level) : this()
        {
            this.txt_level.Text = level;
        }

        private void Btn_save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_level.Text))
            {
                txt_message.Visibility = Visibility.Visible;
                txt_message.Text = "层为空";
                return;
            }

            this.Level = this.txt_level.Text;
            this.DialogResult = true;
        }

        private void Btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
