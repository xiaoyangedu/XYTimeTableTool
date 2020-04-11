using log4net;
using log4net.Config;
using OSKernel.Presentation.Models;
using OSKernel.Presentation.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Update
{
    public class MainWindowModel : GalaSoft.MvvmLight.ObservableObject
    {
        private double _executionPercentage;

        private ServiceUpdate _serviceInfo;

        private ClientUpdate _clientInfo;

        private string _sourcePath = string.Empty;

        private static ILog _logger;

        public int Progress { get; set; }

        public MainWindowModel()
        {
            // 注册日志
            var logCfg = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
            XmlConfigurator.ConfigureAndWatch(logCfg);
            _logger = LogManager.GetLogger("ClientLog");

            ZipHelper.ExtractProgress += ZipHelper_ExtractProgress;

            try
            {
                Task.Run(() =>
                {
                    KillMainProcess("Client");

                    bool run = true;
                    while (run)
                    {
                        Process[] runApplications = Process.GetProcessesByName("Client");
                        if (runApplications.Length == 0)
                        {
                            run = false;
                        }
                        Thread.Sleep(500);
                    }

                    // 获取原始路径
                    if (File.Exists("source.info"))
                    {
                        try
                        {
                            _sourcePath = "source.info".FileDeSerialize() as string;
                            _logger.Info($"反序列化原始路径:{_sourcePath}");
                        }
                        catch (Exception ex)
                        {
                            _logger.Warn(ex.Message);
                            MessageBox.Show($"序列化失败:{ex.Message}");
                            return;
                        }

                        // 清除原始文件
                        var hasPath = Directory.Exists(_sourcePath);
                        if (hasPath)
                        {
                            _logger.Info($"清除原始文件");

                            var allFiles = Directory.GetFiles(_sourcePath);
                            var files = allFiles?.Where(f => f.LastIndexOf("user.info") == -1);
                            if (files != null)
                            {
                                foreach (var f in files)
                                {
                                    File.Delete(f);
                                    _logger.Info($"正在删除文件:{f}");
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("没有找到source.info");
                        _logger.Info("没有找到source.info");
                        return;
                    }

                    _logger.Info($"获取服务端客户端配置文件");

                    // 获取服务端客户端配置文件
                    if (File.Exists(CommonPath.ServiceUpdate.CombineCurrentDirectory()))
                        _serviceInfo = CommonPath.ServiceUpdate.CombineCurrentDirectory().LoadFromXml<ServiceUpdate>();
                    // 获取本地客户端配置文件
                    if (File.Exists(CommonPath.ClientUpdate.CombineCurrentDirectory()))
                        _clientInfo = (CommonPath.ClientUpdate.CombineCurrentDirectory().LoadFromXml<ClientUpdate>());

                    _logger.Info($"解压文件到原始路径");

                    var erroMessage = string.Empty;
                    // 解压文件到原始路径
                    var success = ZipHelper.Decompress(_sourcePath, Path.Combine(Environment.CurrentDirectory, _serviceInfo.Path), out erroMessage);
                    if (!success)
                    {
                        _logger.Warn(erroMessage);
                        MessageBox.Show($"序列化失败:{erroMessage}");
                        return;
                    }

                    _logger.Info($"1.正在解压文件{_sourcePath} To {Path.Combine(Environment.CurrentDirectory, _serviceInfo.Path)}");

                    // 删除原始路径的ZIP文件
                    System.IO.File.Delete(Path.Combine(_sourcePath, _serviceInfo.Path));
                    _logger.Info($"删除下载的Zip文件;{Path.Combine(_sourcePath, _serviceInfo.Path)}");

                    _clientInfo.UpdateTime = DateTime.Now;
                    _clientInfo.Version = _serviceInfo.Version;
                    _clientInfo.VersionType = _serviceInfo.VersionType;
                    _clientInfo.Conent = _serviceInfo.Conent;

                    try
                    {
                        // 更新客户端文件
                        Path.Combine(_sourcePath, "ClientUpdate.xml").SaveToXml<ClientUpdate>(_clientInfo);

                        // 设置更新路径
                        Path.Combine(_sourcePath, "target.info").FileSerialize(Environment.CurrentDirectory);
                    }
                    catch (Exception ex)
                    {
                        _logger.Info($"{ex.Message}");
                    }

                    _logger.Info($"启动任务;{Path.Combine(_sourcePath, "Client.exe")}");

                    // 启动原始程序 Client
                    try
                    {
                        Process process = new Process();
                        process.StartInfo.FileName = Path.Combine(_sourcePath, "Client.exe");
                        process.StartInfo.WorkingDirectory = _sourcePath;
                        process.Start();
                    }
                    catch (Exception ex)
                    {
                        _logger.Info($"启动任务失败:{ex.Message}");
                    }

                    _logger.Info($"已经启动;{Path.Combine(_sourcePath, "Client.exe")}");

                    // 关闭当前程序
                    GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        System.Environment.Exit(-5);
                        _logger.Info($"关闭当前升级程序,升级成功.");
                    });
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void Close(object win)
        {
            Window loginWindow = win as Window;
            loginWindow.Close();
        }

        private void ZipHelper_ExtractProgress(double arg1, string arg2)
        {
            GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                ExecutionPercentage = arg1 * 100;
            });
        }

        public double ExecutionPercentage
        {
            get
            {
                return _executionPercentage;
            }
            set
            {
                _executionPercentage = value;
                RaisePropertyChanged(() => ExecutionPercentage);
            }
        }

        public static void KillMainProcess(string processName)
        {
            Process[] process = Process.GetProcessesByName(processName);
            if (process != null)
            {
                for (int i = 0; i < process.Length; i++)
                {
                    process[i].Kill();
                    process[i].WaitForExit(2000);

                    _logger.Info($"自动更新-杀掉进程: {processName}");
                }
            }
        }
    }
}
