using OSKernel.Presentation.Utilities;
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
    /// 创建层
    /// </summary>
    public partial class CreateLevelWindow
    {
        /// <summary>
        /// 层
        /// </summary>
        public List<string> Levels { get; set; }

        public CreateLevelWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.txt_level.Focus();
        }

        private void Btn_save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_level.Text))
            {
                txt_message.Visibility = Visibility.Visible;
                txt_message.Text = "层为空";
                return;
            }

            this.Levels = txt_level.Text.Trim().Split(new string[] { "\r\n" }, StringSplitOptions.None)?.ToList();
            this.Levels.RemoveAll(c => string.IsNullOrEmpty(c.Trim()));

            var hasRepeat = this.Levels.IsRepeatHashSet();
            if (hasRepeat.Count > 0)
            {
                txt_message.Visibility = Visibility.Visible;
                txt_message.Text = $"重复层{hasRepeat.Parse()}";
                return;
            }

            this.DialogResult = true;
        }

        private void Btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
