using System;
using System.IO;
using System.Net;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using OSKernel.Presentation.Utilities;
using OSKernel.Presentation.Models;
using OSKernel.Presentation.Core;

namespace Client
{
    public class CheckUpdateModel : GalaSoft.MvvmLight.ObservableObject
    {
        private string _content;

        private string _message;

        private string _filePath = string.Empty;

        private string _tempPath = string.Empty;

        private bool _pursueEnable = true;

        private double _executionPercentage;

        private ServiceUpdate _serviceUpdate;

        public int Progress { get; set; }

        /// <summary>
        /// 状态信息
        /// </summary>
        public string StatusMessage { get; set; }

        public ICommand PursueCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(Pursue);
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(Close);
            }
        }

        public CheckUpdateModel()
        {
            ZipHelper.ExtractProgress += ZipHelper_ExtractProgress;
            FileHelper.CopyProgress += FileHelper_CopyProgress;

            if (!System.IO.File.Exists(CommonPath.ClientUpdate.CombineCurrentDirectory()))
            {
                //LogManager.Logger.Warn($"没有找到 {CommonPath.ClientUpdate.CombineCurrentDirectory()}");
                return;
            }

            // 获取本地版本
            CacheManager.Instance.Version = CommonPath.ClientUpdate.LoadFromXml<ClientUpdate>();

            // 获取服务端版本
            var path = CacheManager.Instance.Version.URL + CacheManager.Instance.Version.Path;

            try
            {
                WebClientPro client = new WebClientPro();
                string serviceInfo = Encoding.UTF8.GetString(client.DownloadData(path));
                _serviceUpdate = serviceInfo.XmlDeserailize<ServiceUpdate>();

                var localVersion = $"{CacheManager.Instance.Version.Version}_{CacheManager.Instance.Version.VersionType}";
                var serviceVersoin = $"{_serviceUpdate.Version}_{_serviceUpdate.VersionType}";

                this.Message = $"发现最新版本:{serviceVersoin},升级?";

                StringBuilder strb = new StringBuilder();
                strb.AppendLine($"{serviceVersoin} 升级：");
                strb.Append(_serviceUpdate.Conent);

                this.Content = strb.ToString();
            }
            catch (Exception ex)
            {
                //LogManager.Logger.Warn($"下载版本信息失败:{ex.Message}");
            }
        }

        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
                RaisePropertyChanged(() => Content);
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                RaisePropertyChanged(() => Message);
            }
        }

        public bool PursueEnable
        {
            get
            {
                return _pursueEnable;
            }
            set
            {
                _pursueEnable = value;
                RaisePropertyChanged(() => PursueEnable);
            }
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

        void Pursue()
        {
            // 1.禁用按钮
            this.PursueEnable = false;

            // 2.设置临时路径
            _tempPath = FileHelper.GetTempDirectory();
            if (!Directory.Exists(_tempPath))
            {
                Directory.CreateDirectory(_tempPath);
                //LogManager.Logger.Info($"升级：创建临时路径{_tempPath}");
            }
            _filePath = Path.Combine(_tempPath, _serviceUpdate.Path);

            // 4.向安装路径写入当前路径
            try
            {
                Path.Combine(_tempPath, "source.info").FileSerialize(System.Environment.CurrentDirectory);
                //LogManager.Logger.Info($"升级：修改路径{Path.Combine(_tempPath, "source.info")} To {System.Environment.CurrentDirectory}");
            }
            catch (Exception ex)
            {
                //LogManager.Logger.Warn(ex.Message);

                StatusMessage = "升级失败!详细请查看日志!";
                RaisePropertyChanged(() => StatusMessage);

                this.PursueEnable = true;

                return;
            }

            WebClient Client = new WebClient();
            Client.DownloadProgressChanged += Client_DownloadProgressChanged;
            Client.DownloadFileCompleted += Client_DownloadFileCompleted;

            Uri url = new Uri(CacheManager.Instance.Version.URL + _serviceUpdate.Path);

            try
            {
                Client.DownloadFileAsync(url, _filePath);
            }
            catch (Exception ex)
            {
                //LogManager.Logger.Warn(ex.Message);

                StatusMessage = "升级失败!详细请查看日志!";
                RaisePropertyChanged(() => StatusMessage);

                this.PursueEnable = true;
            }
        }

        void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                this.StatusMessage = "更新被取消";
                this.PursueEnable = true;
                base.RaisePropertyChanged(() => StatusMessage);
                return;
            }

            if (e.Error is WebException)
            {
                this.StatusMessage = "请检测您的网络或防火墙设置！";
                this.PursueEnable = true;
                base.RaisePropertyChanged(() => StatusMessage);
                return;
            }

            // 更新
            this.StatusMessage = "正在更新";

            Task.Run(() =>
            {
                string erroMessage = string.Empty;
                var isTrue = ZipHelper.Decompress(_tempPath, _filePath, out erroMessage);
                if (!isTrue)
                {
                    //LogManager.Logger.Warn(erroMessage);
                    MessageBox.Show(erroMessage);
                    return;
                }

                var servicePath = Path.Combine(_tempPath, "ServiceUpdate.xml");
                servicePath.SaveToXml<ServiceUpdate>(_serviceUpdate);

                var clientPath = Path.Combine(_tempPath, "ClientUpdate.xml");
                clientPath.SaveToXml<ClientUpdate>(CacheManager.Instance.Version);

                this.PursueEnable = true;

                System.Threading.Thread.Sleep(500);

                Process process = new Process();
                process.StartInfo.FileName = _tempPath + "\\Update.exe";
                process.StartInfo.WorkingDirectory = _tempPath;
                process.Start();

                GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    System.Environment.Exit(-2);
                });
            });

        }

        void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.ExecutionPercentage = e.ProgressPercentage / 3D;
            this.Setprocess(10 + (int)(e.ProgressPercentage / 10) * 9);
        }

        void FileHelper_CopyProgress(double arg1, string arg2)
        {
            this.ExecutionPercentage = 100 / 3 + arg1 * 100 / 3;
        }

        void ZipHelper_ExtractProgress(double arg1, string arg2)
        {
            this.ExecutionPercentage = 200 / 3 + arg1 * 100 / 3;
        }

        void Setprocess(int process)
        {
            if (process <= 100)
            {
                this.Progress = process;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    base.RaisePropertyChanged(nameof(Progress));
                });
            }
        }

        void Close(object win)
        {
            Window loginWindow = win as Window;
            loginWindow.Close();
        }
    }
}
