using GalaSoft.MvvmLight.Threading;
using OSKernel.Presentation.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using XYKernel.Presentation.Core;
using Unity;
using OSKernel.Presentation.Core.DataManager;
using Unity.Lifetime;
using System.IO;
using OSKernel.Presentation.Utilities;
using OSKernel.Presentation.Models;
using System.Text;
using System.Diagnostics;
using OSKernel.Presentation.Core.Http;
using OSKernel.Presentation.CustomControl;

namespace Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 设置线程帮助类
            DispatcherHelper.Initialize();

            // 设置当前关闭模式
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // 注册异常事件
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // 注册容器
            Unity.UnityContainer container = new Unity.UnityContainer();

            container.RegisterType<ICommonDataManager, CommonDataManager>(new ContainerControlledLifetimeManager());
            container.RegisterType<IResultDataManager, ResultDataManager>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPatternDataManager, PatternDataManager>(new ContainerControlledLifetimeManager());

            CacheManager.Instance.UnityContainer = container;

            // 获取版本信息
            if (System.IO.File.Exists(CommonPath.ClientUpdate))
            {
                // 1.获取本地版本信息
                CacheManager.Instance.Version = CommonPath.ClientUpdate.LoadFromXml<ClientUpdate>();
            }
#if !DEBUG
            this.AutoUpdate();
#endif

            // 检查本地用户是否登录

            OSKernel.Presentation.Login.LoginWindow login = new OSKernel.Presentation.Login.LoginWindow();
            login.Closed += (s, arg) =>
            {
                if (login.DialogResult.Value)
                {
                    MainWindow main = new MainWindow();
                    main.Show();
                }
            };
            login.ShowDialog();

            //var has = System.IO.Directory.Exists(CommonPath.Data.CombineCurrentDirectory());
            //if (has)
            //{
            //    OSKernel.Presentation.Login.LoginWindow login = new OSKernel.Presentation.Login.LoginWindow();
            //    login.Closed += (s, arg) =>
            //    {
            //        if (login.DialogResult.Value)
            //        {
            //            MainWindow main = new MainWindow();
            //            main.Show();
            //        }
            //    };
            //    login.ShowDialog();
            //}
            //else
            //{
            //    CacheManager.Instance.LoginUser = new User
            //    {
            //        UserName = "未知用户",
            //    };

            //    MainWindow main = new MainWindow();
            //    main.Show();
            //}
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            DialogWindowHelper.ShowDialog("DispatcherUnhandledException", e.ExceptionObject.ToString(), OSKernel.Presentation.CustomControl.Enums.DialogSettingType.OnlyOkButton, OSKernel.Presentation.CustomControl.Enums.DialogType.Error);

            LogManager.Logger.Error(e.ExceptionObject);

            if (e.IsTerminating)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        Dispose();
                    }
                    finally
                    {
                        await Task.Delay(2000);
                        Environment.Exit(0);
                    }
                });
            }
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            DialogWindowHelper.ShowDialog("DispatcherUnhandledException", e.Exception.Message, OSKernel.Presentation.CustomControl.Enums.DialogSettingType.OnlyOkButton, OSKernel.Presentation.CustomControl.Enums.DialogType.Error);

            LogManager.Logger.Error(e.Exception);

            // 出现异常，但程序未关闭，主动关闭程序，避免卡死。
            if (Application.Current.MainWindow == null || !Application.Current.MainWindow.IsLoaded)
            {
                try
                {
                    this.Dispose();

                }
                finally
                {
                    LogManager.Logger.Info("退出系统");
                    Environment.Exit(0);
                }
            }
        }

        private static readonly string[] KillEXE = { "Update" };
        public void AutoUpdate()
        {
            if (!CacheManager.Instance.Version.IsOpen) return;

            foreach (var item in KillEXE)
            {
                foreach (var p in Process.GetProcessesByName(item))
                {
                    p.Kill();
                    p.WaitForExit(1000);
                }
            }

            this.GetLastVersionInfo();
        }

        #region update

        /// <summary>
        /// 获得最后的版本信息
        /// </summary>
        void GetLastVersionInfo()
        {
            var path = Path.Combine(CacheManager.Instance.Version.URL, CacheManager.Instance.Version.Path);

            WebClientPro client = new WebClientPro(2000);

            try
            {
                var bytes = client.DownloadData(path);

                string serviceInfo = Encoding.UTF8.GetString(client.DownloadData(path));
                var serviceUpdate = serviceInfo.XmlDeserailize<ServiceUpdate>();

                // 获取原始路径
                if (File.Exists("target.info"))
                {
                    var targetPath = (string)"target.info".FileDeSerialize();

                    if (Directory.Exists(targetPath))
                        Directory.Delete(targetPath, true);
                }

                var localVersion = new Version(CacheManager.Instance.Version.Version);
                var serviceVersion = new Version(serviceUpdate.Version);

                if (localVersion < serviceVersion)
                {
                    CheckUpdate checkUpdate = new CheckUpdate();
                    checkUpdate.ShowDialog();

                    System.Environment.Exit(-1);
                }
                else
                {
                    LogManager.Logger.Info($"localVersion:{localVersion} serviceVersion:{serviceVersion}");
                }
            }
            catch (Exception ex)
            {
                LogManager.Logger.Warn($"GetLastVersionInfo {ex.Message}");
                return;
            }
        }

        #endregion

        public void Dispose()
        {
            CacheManager.Instance.UnityContainer.Dispose();
        }
    }
}
