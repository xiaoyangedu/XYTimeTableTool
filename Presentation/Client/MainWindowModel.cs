using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using OSKernel.Presentation.Arranging.Dialog;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.Models;
using OSKernel.Presentation.Utilities;
using Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using OSKernel.Presentation.Arranging;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Login;
using OSKernel.Presentation.Core.EventArgs;
using OSKernel.Presentation.Models.Base;
using XYKernel.OS.Common.Models.Administrative;
using GalaSoft.MvvmLight.Threading;

namespace Client
{
    public class MainWindowModel : CommonViewModel, IInitilize, IDisposable
    {
        private UIElement _currentView;
        private System.Windows.Forms.NotifyIcon _notifyIcon;
        private bool _isArrangChecked;
        private string _userName;
        private Dictionary<string, UIElement> _uiDictionaries;
        private bool _showEmpty = true;

        private string _version;

        /// <summary>
        /// 用户名称
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

        public ICommand UserMouseLeftDownCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(userMouseLeftDown);
            }
        }

        public ICommand UserMenuCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<string>(userMenuCommand);
            }
        }

        public ICommand OpenCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(openCommand);
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<string>(closeCommand);
            }
        }

        public ICommand WindowCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<string>(windowCommand);
            }
        }

        public ICommand MenuCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<string>(menuCommand);
            }
        }

        public ICommand CreateCaseCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(createCaseCommand);
            }
        }

        public ICommand LoginCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(loginCommand);
            }
        }

        /// <summary>
        /// 当前模块视图
        /// </summary>
        public UIElement CurrentView
        {
            get
            {
                return _currentView;
            }

            set
            {
                _currentView = value;
                RaisePropertyChanged(() => CurrentView);
            }
        }

        public bool IsArrangChecked
        {
            get
            {
                return _isArrangChecked;
            }

            set
            {
                _isArrangChecked = value;
                RaisePropertyChanged(() => IsArrangChecked);
            }
        }

        /// <summary>
        /// 是否显示空
        /// </summary>
        public bool ShowEmpty
        {
            get
            {
                return _showEmpty;
            }

            set
            {
                _showEmpty = value;
                RaisePropertyChanged(() => ShowEmpty);
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

        [InjectionMethod]
        public void Initilize()
        {
            _uiDictionaries = new Dictionary<string, UIElement>();

            Messenger.Default.Register<string>(this, Receive);

            this.UserName = CacheManager.Instance.LoginUser.UserName;

            #region 系统托盘

            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            _notifyIcon.BalloonTipText = $"晓羊教育(排课工具)";
            _notifyIcon.ShowBalloonTip(2000);
            _notifyIcon.Text = _notifyIcon.BalloonTipText;
            _notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            _notifyIcon.Visible = true;

            _notifyIcon.MouseClick += _notifyIcon_MouseClick;

            _notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler((o, e) =>
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    openCommand(Application.Current.MainWindow);
                }
            });

            #endregion

            this.Version = $"{CacheManager.Instance.Version.Version}{CacheManager.Instance.Version.VersionType.GetLocalDescription()}";

        }

        public void Receive(string message)
        {
            this.UserName = CacheManager.Instance.LoginUser.UserName;
        }

        void userMouseLeftDown(object e)
        {
            var button = e as System.Windows.Controls.Button;
            var contextMenu = button.ContextMenu;
            if (contextMenu.DataContext is null)
            {
                contextMenu.DataContext = this;
            }
            contextMenu.IsOpen = true;
        }

        void userMenuCommand(string parms)
        {
            if (parms.Equals("logout"))
            {
                System.Diagnostics.Process.Start(System.IO.Path.Combine(Environment.CurrentDirectory, "Client.exe"));

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Environment.Exit(0);
                });
            }
            else
            {
                this.loginCommand();
            }
        }

        void openCommand(object obj)
        {
            MainWindow win = obj as MainWindow;

            if (win.WindowState == WindowState.Maximized || win.WindowState == WindowState.Normal)
            {
                win.WindowState = WindowState.Minimized;
            }
            else if (win.WindowState == WindowState.Minimized)
            {
                win.WindowState = WindowState.Normal;
            }
        }

        void closeCommand(object obj)
        {
            Application.Current.Shutdown();
        }

        void windowCommand(string parms)
        {
            if (parms.Equals("loaded"))
            {
                // 默认打开排课界面
                menuCommand("caseManager");
                IsArrangChecked = true;
            }
            else if (parms.Equals("closed"))
            {
                _notifyIcon.Visible = false;

                this.Dispose();

                Environment.Exit(0);

                //OperaterUtils.ClearToken();
                //Task.Run(() => XYConfiguration.Current.Shutdown()).ContinueWith(r =>
                //{
                //    Environment.Exit(0);
                //});
            }
            else if (parms.Equals("unloaded"))
            {
                this.Dispose();
            }
        }

        void menuCommand(string parms)
        {
            if (this.ShowEmpty == true)
            {
                this.ShowEmpty = false;
            }

            var contain = _uiDictionaries.ContainsKey(parms);

            // 1.缓存模块
            if (parms.Equals("caseManager"))
            {
                if (!contain)
                {
                    _uiDictionaries.Add(parms, new OSKernel.Presentation.Arranging.ArrangeView());
                }
            }
            else if (parms.Equals("recycleManager"))
            {
                if (!contain)
                {
                    //_uiDictionaries.Add(parms, new XYKernel.Presentation.Cloud.CloudView());
                    //CacheManager.Instance.CurrentView = CurrentViewEnum.Cloud;
                }
            }
            else if (parms.Equals("settingManager"))
            {
                if (!contain)
                {
                    //_uiDictionaries.Add(parms, new XYKernel.Presentation.Manager.ManagerView());
                    //CacheManager.Instance.CurrentView = CurrentViewEnum.Role_User;
                }
            }

            // 2.设置当前界面
            this.CurrentView = _uiDictionaries[parms];
            this.CurrentView.FedIn();
        }

        void createCaseCommand()
        {
            CreateCaseWindow createWindow = new CreateCaseWindow();
            createWindow.Closed += (s, arg) =>
            {
                if (createWindow.DialogResult == true)
                {
                    if (string.IsNullOrEmpty(createWindow.CaseName))
                    {
                        this.ShowDialog("提示信息", "方案名称为空!", OSKernel.Presentation.CustomControl.Enums.DialogSettingType.OnlyOkButton, OSKernel.Presentation.CustomControl.Enums.DialogType.Warning);
                        return;
                    }

                    // TODO 界面可以进行编辑
                    Case caseModel = new Case();
                    caseModel.LocalID = Guid.NewGuid().ToString();
                    caseModel.CaseType = createWindow.CaseType;
                    caseModel.IsAuto = createWindow.IsAuto;
                    caseModel.Name = createWindow.CaseName;
                    caseModel.CreateTime = createWindow.CreateDate;

                    if (base.CommonDataManager.LocalCases.Any(c =>
                    {
                        if (c.Name == null)
                            return false;
                        else
                            return c.Name.Equals(caseModel.Name);
                    }))
                    {
                        this.ShowDialog("提示信息", "存在相同名称的方案!", OSKernel.Presentation.CustomControl.Enums.DialogSettingType.OnlyOkButton, OSKernel.Presentation.CustomControl.Enums.DialogType.Warning);
                        return;
                    }
                    else
                    {
                        if (caseModel.CaseType == OSKernel.Presentation.Models.Enums.CaseTypeEnum.Administrative)
                        {
                            var cp = CommonDataManager.GetCPCase(caseModel.LocalID);
                            cp.Name = caseModel.Name;
                            this.CreateAdministratorTime(cp);
                            cp.Serialize(caseModel.LocalID);
                        }
                        else
                        {
                            var cl = CommonDataManager.GetCLCase(caseModel.LocalID);
                            cl.Name = caseModel.Name;
                            this.CreateMixedTime(cl);
                            cl.Serialize(caseModel.LocalID);
                        }

                        caseModel.Serialize();

                        base.CommonDataManager.LocalCases.Add(caseModel);

                        Messenger.Default.Send<CaseEventArgs>(new CaseEventArgs()
                        {
                            EventType = CaseEventArgs.EventTypeEnum.Create,
                            Model = caseModel
                        });

                        if (!this.IsArrangChecked)
                        {
                            this.IsArrangChecked = true;
                            this.menuCommand("caseManager");
                        }
                    }
                }
            };
            createWindow.ShowDialog();
        }

        void loginCommand()
        {
            if (CacheManager.Instance.LoginUser.UserName.Equals("未知用户"))
            {
                LoginWindow loginWin = new LoginWindow();
                loginWin.Closed += (s, arg) =>
                {
                    if (loginWin.DialogResult.Value)
                    {
                        this.UserName = CacheManager.Instance.LoginUser.UserName;
                    }
                };
                loginWin.ShowDialog();
            }
            else
            {
                OSKernel.Presentation.Login.Summary.IndexView userInfo = new OSKernel.Presentation.Login.Summary.IndexView("userInfo");
                userInfo.ShowDialog();
            }
        }

        void _notifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                System.Windows.Controls.ContextMenu NotifyIconMenu = (System.Windows.Controls.ContextMenu)Application.Current.FindResource("NotifyIconMenu");
                NotifyIconMenu.IsOpen = true;
            }
        }

        public void Dispose()
        {
            //HandlerManager.Instance.OnConnectionClosedHandler -= Instance_OnConnectionClosedHandler;
            //HandlerManager.Instance.OnDownloadingHandler -= Instance_OnDownloadingHandler;
            //Messenger.Default.Unregister<Case>(this, Receive);
        }

        /// <summary>
        /// 创建行政班数据
        /// </summary>
        public void CreateAdministratorTime(CPCase cp)
        {
            List<UIPosition> positions = new List<UIPosition>();

            #region 上午

            UIPosition am1 = new UIPosition()
            {
                PeriodString = "第一节",
                PositionType = XYKernel.OS.Common.Enums.Position.AM
            };

            UIPosition am2 = new UIPosition()
            {
                PeriodString = "第二节",
                PositionType = XYKernel.OS.Common.Enums.Position.AM
            };

            UIPosition bk = new UIPosition()
            {
                PeriodString = "上午大课间",
                PositionType = XYKernel.OS.Common.Enums.Position.AB
            };

            UIPosition am3 = new UIPosition()
            {
                PeriodString = "第三节",
                PositionType = XYKernel.OS.Common.Enums.Position.AM
            };

            UIPosition am4 = new UIPosition()
            {
                PeriodString = "第四节",
                PositionType = XYKernel.OS.Common.Enums.Position.AM
            };

            positions.Add(am1);
            positions.Add(am2);
            positions.Add(bk);
            positions.Add(am3);
            positions.Add(am4);

            #endregion

            #region 下午

            UIPosition noon = new UIPosition()
            {
                PeriodString = "午休",
                PositionType = XYKernel.OS.Common.Enums.Position.Noon,
                CanOperation = false,
                IsFridayChecked = false,
                IsMondayChecked = false,
                IsSaturdayChecked = false,
                IsSundayChecked = false,
                IsThursdayChecked = false,
                IsTuesdayChecked = false,
                IsWednesdayChecked = false
            };

            UIPosition pm1 = new UIPosition()
            {
                PeriodString = "第五节",
                PositionType = XYKernel.OS.Common.Enums.Position.PM
            };

            UIPosition pm2 = new UIPosition()
            {
                PeriodString = "第六节",
                PositionType = XYKernel.OS.Common.Enums.Position.PM
            };

            UIPosition pm3 = new UIPosition()
            {
                PeriodString = "第七节",
                PositionType = XYKernel.OS.Common.Enums.Position.PM
            };

            UIPosition pm4 = new UIPosition()
            {
                PeriodString = "第八节",
                PositionType = XYKernel.OS.Common.Enums.Position.PM
            };

            positions.Add(noon);
            positions.Add(pm1);
            positions.Add(pm2);
            positions.Add(pm3);
            positions.Add(pm4);

            #endregion

            // 遍历课位集合
            for (int i = 0; i < positions.Count; i++)
            {
                var p = positions[i];
                #region 周一至周日

                var monday = new CoursePositionModel()
                {
                    DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                    {
                        Day = DayOfWeek.Monday,
                        Period = i,
                        PeriodName = p.PeriodString
                    },
                    IsSelected = p.IsMondayChecked,
                    Position = p.PositionType,
                    PositionOrder = i
                };
                cp.Positions.Add(monday);

                var tuesday = new CoursePositionModel()
                {
                    DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                    {
                        Day = DayOfWeek.Tuesday,
                        Period = i,
                        PeriodName = p.PeriodString
                    },
                    IsSelected = p.IsTuesdayChecked,
                    Position = p.PositionType,
                    PositionOrder = i
                };
                cp.Positions.Add(tuesday);

                var wednesday = new CoursePositionModel()
                {
                    DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                    {
                        Day = DayOfWeek.Wednesday,
                        Period = i,
                        PeriodName = p.PeriodString
                    },
                    IsSelected = p.IsWednesdayChecked,
                    Position = p.PositionType,
                    PositionOrder = i
                };
                cp.Positions.Add(wednesday);

                var thursday = new CoursePositionModel()
                {
                    DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                    {
                        Day = DayOfWeek.Thursday,
                        Period = i,
                        PeriodName = p.PeriodString
                    },
                    IsSelected = p.IsThursdayChecked,
                    Position = p.PositionType,
                    PositionOrder = i
                };
                cp.Positions.Add(thursday);

                var Friday = new CoursePositionModel()
                {
                    DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                    {
                        Day = DayOfWeek.Friday,
                        Period = i,
                        PeriodName = p.PeriodString,
                    },
                    IsSelected = p.IsFridayChecked,
                    Position = p.PositionType,
                    PositionOrder = i
                };
                cp.Positions.Add(Friday);

                var saturday = new CoursePositionModel()
                {
                    DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                    {
                        Day = DayOfWeek.Saturday,
                        Period = i,
                        PeriodName = p.PeriodString
                    },
                    IsSelected = p.IsSaturdayChecked,
                    Position = p.PositionType,
                    PositionOrder = i
                };
                cp.Positions.Add(saturday);

                var sunday = new CoursePositionModel()
                {
                    DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                    {
                        Day = DayOfWeek.Sunday,
                        Period = i,
                        PeriodName = p.PeriodString
                    },
                    IsSelected = p.IsSundayChecked,
                    Position = p.PositionType,
                    PositionOrder = i
                };
                cp.Positions.Add(sunday);

                #endregion
            }
        }

        /// <summary>
        /// 创建走班数据
        /// </summary>
        /// <param name="cl"></param>
        public void CreateMixedTime(XYKernel.OS.Common.Models.Mixed.CLCase cl)
        {
            List<UIPosition> positions = new List<UIPosition>();

            #region 上午

            UIPosition am1 = new UIPosition()
            {
                PeriodString = "第一节",
                PositionType = XYKernel.OS.Common.Enums.Position.AM
            };

            UIPosition am2 = new UIPosition()
            {
                PeriodString = "第二节",
                PositionType = XYKernel.OS.Common.Enums.Position.AM
            };

            UIPosition bk = new UIPosition()
            {
                PeriodString = "上午大课间",
                PositionType = XYKernel.OS.Common.Enums.Position.AB
            };

            UIPosition am3 = new UIPosition()
            {
                PeriodString = "第三节",
                PositionType = XYKernel.OS.Common.Enums.Position.AM
            };

            UIPosition am4 = new UIPosition()
            {
                PeriodString = "第四节",
                PositionType = XYKernel.OS.Common.Enums.Position.AM
            };

            positions.Add(am1);
            positions.Add(am2);
            positions.Add(bk);
            positions.Add(am3);
            positions.Add(am4);

            #endregion

            #region 下午

            UIPosition noon = new UIPosition()
            {
                PeriodString = "午休",
                PositionType = XYKernel.OS.Common.Enums.Position.Noon,
                CanOperation = false,
                IsFridayChecked = false,
                IsMondayChecked = false,
                IsSaturdayChecked = false,
                IsSundayChecked = false,
                IsThursdayChecked = false,
                IsTuesdayChecked = false,
                IsWednesdayChecked = false
            };

            UIPosition pm1 = new UIPosition()
            {
                PeriodString = "第五节",
                PositionType = XYKernel.OS.Common.Enums.Position.PM
            };

            UIPosition pm2 = new UIPosition()
            {
                PeriodString = "第六节",
                PositionType = XYKernel.OS.Common.Enums.Position.PM
            };

            UIPosition pm3 = new UIPosition()
            {
                PeriodString = "第七节",
                PositionType = XYKernel.OS.Common.Enums.Position.PM
            };

            UIPosition pm4 = new UIPosition()
            {
                PeriodString = "第八节",
                PositionType = XYKernel.OS.Common.Enums.Position.PM
            };

            positions.Add(noon);
            positions.Add(pm1);
            positions.Add(pm2);
            positions.Add(pm3);
            positions.Add(pm4);

            #endregion

            // 遍历课位集合
            for (int i = 0; i < positions.Count; i++)
            {
                var p = positions[i];
                #region 周一至周日

                var monday = new XYKernel.OS.Common.Models.Mixed.CoursePositionModel()
                {
                    DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                    {
                        Day = DayOfWeek.Monday,
                        Period = i,
                        PeriodName = p.PeriodString
                    },
                    IsSelected = p.IsMondayChecked,
                    Position = p.PositionType,
                    PositionOrder = i
                };
                cl.Positions.Add(monday);

                var tuesday = new XYKernel.OS.Common.Models.Mixed.CoursePositionModel()
                {
                    DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                    {
                        Day = DayOfWeek.Tuesday,
                        Period = i,
                        PeriodName = p.PeriodString
                    },
                    IsSelected = p.IsTuesdayChecked,
                    Position = p.PositionType,
                    PositionOrder = i
                };
                cl.Positions.Add(tuesday);

                var wednesday = new XYKernel.OS.Common.Models.Mixed.CoursePositionModel()
                {
                    DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                    {
                        Day = DayOfWeek.Wednesday,
                        Period = i,
                        PeriodName = p.PeriodString
                    },
                    IsSelected = p.IsWednesdayChecked,
                    Position = p.PositionType,
                    PositionOrder = i
                };
                cl.Positions.Add(wednesday);

                var thursday = new XYKernel.OS.Common.Models.Mixed.CoursePositionModel()
                {
                    DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                    {
                        Day = DayOfWeek.Thursday,
                        Period = i,
                        PeriodName = p.PeriodString
                    },
                    IsSelected = p.IsThursdayChecked,
                    Position = p.PositionType,
                    PositionOrder = i
                };
                cl.Positions.Add(thursday);

                var Friday = new XYKernel.OS.Common.Models.Mixed.CoursePositionModel()
                {
                    DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                    {
                        Day = DayOfWeek.Friday,
                        Period = i,
                        PeriodName = p.PeriodString,
                    },
                    IsSelected = p.IsFridayChecked,
                    Position = p.PositionType,
                    PositionOrder = i
                };
                cl.Positions.Add(Friday);

                var saturday = new XYKernel.OS.Common.Models.Mixed.CoursePositionModel()
                {
                    DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                    {
                        Day = DayOfWeek.Saturday,
                        Period = i,
                        PeriodName = p.PeriodString
                    },
                    IsSelected = p.IsSaturdayChecked,
                    Position = p.PositionType,
                    PositionOrder = i
                };
                cl.Positions.Add(saturday);

                var sunday = new XYKernel.OS.Common.Models.Mixed.CoursePositionModel()
                {
                    DayPeriod = new XYKernel.OS.Common.Models.DayPeriodModel()
                    {
                        Day = DayOfWeek.Sunday,
                        Period = i,
                        PeriodName = p.PeriodString
                    },
                    IsSelected = p.IsSundayChecked,
                    Position = p.PositionType,
                    PositionOrder = i
                };
                cl.Positions.Add(sunday);

                #endregion
            }
        }
    }
}
