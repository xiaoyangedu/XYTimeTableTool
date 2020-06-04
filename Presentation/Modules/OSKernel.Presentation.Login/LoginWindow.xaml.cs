﻿using OSKernel.Presentation.Core;
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
using Unity;
namespace OSKernel.Presentation.Login
{
    /// <summary>
    /// 登录界面
    /// </summary>
    public partial class LoginWindow : Window
    {
        private string userpasswordFile = "password.info";

        public LoginWindowModel VM
        {
            get
            {
                return this.DataContext as LoginWindowModel;
            }
        }

        public LoginWindow()
        {
            InitializeComponent();
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<LoginWindowModel>();
            this.MouseLeftButtonDown += Login_MouseLeftButtonDown;
            this.Closed += LoginWindow_Closed;
            this.Loaded += LoginWindow_Loaded;
        }

        private void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (System.IO.File.Exists(userpasswordFile.CombineCurrentDirectory()))
            {
                // 用户密码
                this.pb_password.Password = userpasswordFile.CombineCurrentDirectory().DeSerializeObjectFromJson<string>();
                VM.IsRememberPassWord = true;
            }
        }

        private void LoginWindow_Closed(object sender, EventArgs e)
        {
            if (!this.DialogResult.Value)
            {
                if (Application.Current.MainWindow == null)
                    Environment.Exit(0);
            }
        }

        private void Login_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void btn_extend_Click(object sender, RoutedEventArgs e)
        {
            grid_base.Visibility = Visibility.Collapsed;
            grid_set.Visibility = Visibility.Visible;
            btn_back.Visibility = Visibility.Visible;
            btn_extend.Visibility = Visibility.Collapsed;
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            grid_set.Visibility = Visibility.Collapsed;
            btn_back.Visibility = Visibility.Collapsed;
            grid_base.Visibility = Visibility.Visible;
            btn_extend.Visibility = Visibility.Visible;
        }
    }
}
