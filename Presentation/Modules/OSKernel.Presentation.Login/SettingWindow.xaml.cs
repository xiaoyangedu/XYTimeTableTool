using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.CustomControl.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace OSKernel.Presentation.Login
{
    /// <summary>
    /// 设置窗口
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();

            this.Owner = Application.Current.MainWindow;
            this.MouseLeftButtonDown += SettingWindow_MouseLeftButtonDown;
            this.Loaded += SettingWindow_Loaded;
        }

        private void SettingWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void SettingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            txt_url.Text = cfa.AppSettings.Settings["xy:address"].Value;
            txt_port.Text = cfa.AppSettings.Settings["xy:port"].Value;

            txt_loginUrl.Text = cfa.AppSettings.Settings["xy:login.address"].Value;
            txt_loginport.Text = cfa.AppSettings.Settings["xy:login.port"].Value;
        }

        private void Btn_close_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Btn_save_Click(object sender, RoutedEventArgs e)
        {
            var url = txt_url.Text;
            var port = txt_port.Text;
            var loginUrl = txt_loginUrl.Text;
            var loginPort = txt_loginport.Text;

            if (string.IsNullOrEmpty(url))
            {
                this.ShowDialog("提示", "排课地址不能为空!", DialogSettingType.OnlyOkButton, DialogType.Warning);
                return;
            }

            if (string.IsNullOrEmpty(loginUrl))
            {
                this.ShowDialog("提示", "登录地址不能为空!", DialogSettingType.OnlyOkButton, DialogType.Warning);
                return;
            }

            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfa.AppSettings.Settings["xy:address"].Value = url;
            cfa.AppSettings.Settings["xy:port"].Value = port;

            cfa.AppSettings.Settings["xy:login.address"].Value = loginUrl;
            cfa.AppSettings.Settings["xy:login.port"].Value = loginPort;
            cfa.Save();

            // 保存当前配置信息
            this.DialogResult = true;
        }
    }
}
