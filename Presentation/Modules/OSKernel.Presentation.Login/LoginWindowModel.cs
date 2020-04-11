using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.Http;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Enums;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Unity;

namespace OSKernel.Presentation.Login
{
    /// <summary>
    /// 登录界面ViewModel
    /// </summary>
    public class LoginWindowModel : CommonViewModel, IInitilize
    {
        private string _userID;

        private string _userName;

        private bool _isRemember = true;

        private string usernameFile = "user.info";

        private string lastAdress = "last.adress";

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get
            {
                return _userName;
            }

            set
            {
                _userName = value;
                RaisePropertyChanged(() => UserName);
            }
        }

        #region 绑定签名信息

        private SecretTypeEnum _selectSecretType;

        private Dictionary<string, SecretTypeEnum> _secretTypes;

        private string _privateKey;

        private string _publicKey;

        private string _version;

        #endregion

        /// <summary>
        /// 是否记住密码
        /// </summary>
        public bool IsRemember
        {
            get
            {
                return _isRemember;
            }

            set
            {
                _isRemember = value;
                RaisePropertyChanged(() => IsRemember);
            }
        }

        /// <summary>
        /// 公钥
        /// </summary>
        public string PublicKey
        {
            get
            {
                return _publicKey;
            }

            set
            {
                _publicKey = value;
                RaisePropertyChanged(() => PublicKey);
            }
        }

        /// <summary>
        /// 私钥
        /// </summary>
        public string PrivateKey
        {
            get
            {
                return _privateKey;
            }

            set
            {
                _privateKey = value;
                RaisePropertyChanged(() => PrivateKey);
            }
        }

        /// <summary>
        /// 加密类型
        /// </summary>
        public Dictionary<string, SecretTypeEnum> SecretTypes
        {
            get
            {
                return _secretTypes;
            }

            set
            {
                _secretTypes = value;
                RaisePropertyChanged(() => SecretTypes);
            }
        }

        /// <summary>
        /// 选择加密类型
        /// </summary>
        public SecretTypeEnum SelectSecretType
        {
            get
            {
                return _selectSecretType;
            }

            set
            {
                _selectSecretType = value;
                RaisePropertyChanged(() => SelectSecretType);
            }
        }

        /// <summary>
        /// 生成Key密钥
        /// </summary>
        public ICommand CreateKeyCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(createKeyCommand);
            }
        }

        /// <summary>
        /// 绑定Key密钥
        /// </summary>
        public ICommand BindingKeyCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(bindingKeyCommand);
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(closeCommand);
            }
        }

        public ICommand SettingCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(settingCommand);
            }
        }

        public ICommand LoginCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(loginCommand);
            }
        }

        public ICommand RegisterCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(Register);
            }
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID
        {
            get
            {
                return _userID;
            }

            set
            {
                _userID = value;
                RaisePropertyChanged(() => UserID);
            }
        }

        /// <summary>
        /// 版本信息
        /// </summary>
        public string Version
        {
            get
            {
                return _version;
            }

            set
            {
                _version = value;
                RaisePropertyChanged(() => Version);
            }
        }

        public LoginWindowModel()
        {
            if (System.IO.File.Exists(usernameFile.CombineCurrentDirectory()))
            {
                this.UserName = usernameFile.CombineCurrentDirectory().DeSerializeObjectFromJson<string>();
            }
        }

        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="obj">登录窗口</param>
        void loginCommand(object obj)
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            string loginUrl = cfa.AppSettings.Settings["xy:login.address"].Value;
            string loginPort = cfa.AppSettings.Settings["xy:login.port"].Value;
            string loginVersion = cfa.AppSettings.Settings["xy:login.version"].Value;
            string loginPath = $"{loginUrl}:{loginPort}";

            OSHttpClient.Instance.SetLoginFactory(loginPath, loginVersion);

            LoginWindow win = obj as LoginWindow;
            string password = win.pb_password.Password;

            if (string.IsNullOrEmpty(password))
            {
                this.ShowDialog("提示信息", "密码为空!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

            if (string.IsNullOrEmpty(this.UserName))
            {
                this.ShowDialog("提示信息", "用户名不能为空!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

            if (this.IsRemember)
            {
                // 记录用户名
                usernameFile.CombineCurrentDirectory().SerializeObjectToJson(this.UserName);
            }

            base.ShowLoading = true;

            Task.Run(() =>
            {
                return OSHttpClient.Instance.Login(this.UserName, password);
            }).ContinueWith(r =>
            {
                this.ShowLoading = false;

                if (r.Result.Item1)
                {
                    CacheManager.Instance.LoginUser = new Models.User()
                    {
                        ID = r.Result.Item2.user_id,
                        UserName = r.Result.Item2.user_name,
                        AccessToken = r.Result.Item2.access_token,
                        MixKey = r.Result.Item2.mix_key,
                        PassWord = password
                    };

                    string key = $"{CacheManager.Instance.LoginUser.UserName}.key";

                    // 检查IP地址
                    if (System.IO.File.Exists(lastAdress.CombineCurrentDirectory()))
                    {
                        string check_url = cfa.AppSettings.Settings["xy:address"].Value;
                        string check_port = cfa.AppSettings.Settings["xy:port"].Value;
                        string check_path = $"{check_url}:{check_port}";

                        var lassAdress = lastAdress.CombineCurrentDirectory().DeSerializeObjectFromJson<string>();

                        if (!lassAdress.Equals(check_path))
                        {
                            System.IO.File.Delete(key.CombineCurrentDirectory());
                        }
                    }

                    // 检查本地是否有（密钥和公钥）
                    if (System.IO.File.Exists(key.CombineCurrentDirectory()))
                    {
                        // 0-公钥,1-私钥
                        List<string> keys = key.CombineCurrentDirectory().DeSerializeObjectFromJson<List<string>>();
                        CacheManager.Instance.LoginUser.PublickKey = keys[0];
                        CacheManager.Instance.LoginUser.PrivateKey = keys[1];
                    }
                    else
                    {
                        // 1.生成密钥
                        List<string> keys = new List<string>();

                        keys = XY.Common.Utilities.RSAUtil.GenerateSuiteKeys()?.ToList();

                        CacheManager.Instance.LoginUser.PublickKey = keys[0];
                        CacheManager.Instance.LoginUser.PrivateKey = keys[1];

                        key.CombineCurrentDirectory().SerializeObjectToJson(keys);

                        // 2.绑定密钥
                        var secret = OSHttpClient.Instance.SetSecret(keys[0], 0, CacheManager.Instance.LoginUser.AccessToken);
                        if (!secret.Item1)
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                var dialog = this.ShowDialog("提示信息", secret.Item2, CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                            });
                            return;
                        }
                    }

                    #region 设置排课连接

                    string url = cfa.AppSettings.Settings["xy:address"].Value;
                    string port = cfa.AppSettings.Settings["xy:port"].Value;
                    string version = cfa.AppSettings.Settings["xy:version"].Value;
                    string path = $"{url}:{port}";

                    OSHttpClient.Instance.SetFactory(path, version, CacheManager.Instance.LoginUser.ID);

                    lastAdress.CombineCurrentDirectory().SerializeObjectToJson(path);

                    #endregion

                    Messenger.Default.Send<string>("refresh");

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        win.DialogResult = true;
                    });
                }
                else
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        this.ShowDialog("提示信息", r.Result.Item3, CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                    });
                }
            });

        }

        void closeCommand(object obj)
        {
            if (Application.Current.MainWindow == null)
            {
                Application.Current.Shutdown();
            }

            LoginWindow win = obj as LoginWindow;
            win.Close();
        }

        void Register()
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var adress = cfa.AppSettings.Settings["xy:login.address"].Value;
            var port = cfa.AppSettings.Settings["xy:login.port"].Value;

            var url = $@"http://{adress}:{port}/home/signin";
            System.Diagnostics.Process.Start(url);
        }

        void settingCommand()
        {
            SettingWindow setting = new SettingWindow();
            //setting.Closed += (s, arg) =>
            //{
            //    if (setting.DialogResult.Value)
            //    {
            //        // 重新启动程序

            //        System.Diagnostics.Process.Start(System.IO.Path.Combine(Environment.CurrentDirectory, "Client.exe"));
            //        DispatcherHelper.CheckBeginInvokeOnUI(() =>
            //        {
            //            Environment.Exit(0);
            //        });
            //    }
            //};
            setting.ShowDialog();
        }

        void createKeyCommand()
        {
            // 创建密钥
            // TODO
        }

        void bindingKeyCommand()
        {
            if (string.IsNullOrEmpty(this.UserID))
            {
                this.ShowDialog("提示信息", "请先登录", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

            if (string.IsNullOrEmpty(this.PrivateKey))
            {
                this.ShowDialog("提示信息", "私有Key", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

            if (string.IsNullOrEmpty(this.PublicKey))
            {
                this.ShowDialog("提示信息", "没有公有Key", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

            OSHttpClient.Instance.SetSecret(PublicKey, (byte)this.SelectSecretType, CacheManager.Instance.LoginUser.AccessToken);
        }

        [InjectionMethod]
        public void Initilize()
        {
            this.SecretTypes = new Dictionary<string, SecretTypeEnum>()
            {
                {"RSA", SecretTypeEnum.RSA },
                {"RSA2", SecretTypeEnum.RSA2 },
                {"MD5", SecretTypeEnum.MD5 }
            };

            this.SelectSecretType = SecretTypeEnum.RSA2;
            this.Version = CacheManager.Instance.Version.Version;
        }
    }
}
