using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using OSKernel.Presentation.Arranging.Dialog;
using OSKernel.Presentation.Arranging.Mixed.Modify.Rule.ClassHour.Model;
using OSKernel.Presentation.Arranging.Mixed.Modify.Views.Model;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.EventArgs;
using OSKernel.Presentation.Core.Http;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models;
using OSKernel.Presentation.Models.Enums;
using OSKernel.Presentation.Models.Result;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Unity;
using XYKernel.OS.Common.Models.Administrative;
using XYKernel.OS.Common.Models.Mixed;

namespace OSKernel.Presentation.Arranging
{
    public class ArrangeViewModel : CommonViewModel
    {
        private UIElement _currentView;

        private bool _showFirstPanel = true;

        private bool _showSecondPanel;

        private bool _showSecondArrangeButton;

        private string _secondBarTitle;

        private ObservableCollection<Case> _cases;

        private ListCollectionView _caseCollectionView;

        private ObservableCollection<Case> _runCases;

        private Case _selectCase;

        public ICommand SecondReturnCommand
        {
            get
            {
                return new RelayCommand(SecondReturn);
            }
        }

        public ICommand ModifyOperationCommand
        {
            get
            {
                return new RelayCommand<object>(ModifyOperation);
            }
        }

        public ICommand ResultOperationCommand
        {
            get
            {
                return new RelayCommand<object>(ResultOperation);
            }
        }

        public ICommand CaseMoreCommand
        {
            get
            {
                return new RelayCommand<object>(caseMoreCommand);
            }
        }

        public ICommand CaseCommand
        {
            get
            {
                return new RelayCommand<Case>(caseCommand);
            }
        }

        public ICommand SwitchTaskCommand
        {
            get
            {
                return new RelayCommand<object>(switchTaskCommand);
            }
        }

        public ICommand ArrangeCommand
        {
            get
            {
                return new RelayCommand(arrangeCommand);
            }
        }

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

        public ObservableCollection<Case> Cases
        {
            get
            {
                return _cases;
            }

            set
            {
                _cases = value;
                RaisePropertyChanged(() => Cases);
            }
        }

        /// <summary>
        /// 显示第一个容器
        /// </summary>
        public bool ShowFirstPanel
        {
            get
            {
                return _showFirstPanel;
            }

            set
            {
                _showFirstPanel = value;
                RaisePropertyChanged(() => ShowFirstPanel);

                this.ShowSecondPanel = !value;
            }
        }

        /// <summary>
        /// 显示第二个容器
        /// </summary>
        public bool ShowSecondPanel
        {
            get
            {
                return _showSecondPanel;
            }

            set
            {
                _showSecondPanel = value;
                RaisePropertyChanged(() => ShowSecondPanel);
            }
        }

        /// <summary>
        /// 第二层标题
        /// </summary>
        public string SecondBarTitle
        {
            get
            {
                return _secondBarTitle;
            }

            set
            {
                _secondBarTitle = value;
                RaisePropertyChanged(() => SecondBarTitle);
            }
        }

        /// <summary>
        /// 选中方案
        /// </summary>
        public Case SelectCase
        {
            get
            {
                return _selectCase;
            }

            set
            {
                _selectCase = value;
                RaisePropertyChanged(() => SelectCase);
            }
        }

        public ObservableCollection<Case> RunCases
        {
            get
            {
                return _runCases;
            }

            set
            {
                _runCases = value;
                RaisePropertyChanged(() => RunCases);
            }
        }

        /// <summary>
        /// 显示
        /// </summary>
        public bool ShowSecondArrangeButton
        {
            get
            {
                return _showSecondArrangeButton;
            }

            set
            {
                _showSecondArrangeButton = value;
                RaisePropertyChanged(() => ShowSecondArrangeButton);
            }
        }

        public ArrangeViewModel()
        {
            this.Cases = new ObservableCollection<Case>();
            this.RunCases = new ObservableCollection<Case>();

            _caseCollectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(this.Cases);
            _caseCollectionView.SortDescriptions.Add(
                new System.ComponentModel.SortDescription
                {
                    Direction = System.ComponentModel.ListSortDirection.Descending,
                    PropertyName = "CreateTime"
                });
        }

        [InjectionMethod]
        public void Initilize()
        {
            this.LoadCase();
            Messenger.Default.Register<CaseEventArgs>(this, Receive);
        }

        void caseMoreCommand(object model)
        {
            Button more = model as Button;
            more.ContextMenu.DataContext = more.DataContext;

            if (more.ContextMenu.IsOpen)
            {
                more.ContextMenu.IsOpen = false;
            }
            else
            {
                more.ContextMenu.IsOpen = true;
            }
        }

        /// <summary>
        /// 接收事件消息
        /// </summary>
        /// <param name="args">参数</param>
        void Receive(CaseEventArgs args)
        {
            switch (args.EventType)
            {
                case CaseEventArgs.EventTypeEnum.Create:
                    Cases.Insert(0, args.Model);
                    break;

                case CaseEventArgs.EventTypeEnum.Clash:

                    args.Model.ShowLoading = true;
                    Task.Run(() =>
                    {
                        return OSKernel.Presentation.Core.Http.OSHttpClient.Instance.GetErroInfo(args.Model.Task.TaskID);
                    }).ContinueWith(r =>
                    {
                        args.Model.ShowLoading = false;
                        if (r.Result.Item1)
                        {
                            GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                this.ShowDialog("提示信息", r.Result.Item2, CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Error);
                            });
                        }
                        else
                        {
                            GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                this.ShowDialog("提示信息", "获取错误信息失败", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Error);
                            });
                        }
                    });

                    break;

                case CaseEventArgs.EventTypeEnum.Difficulty:

                    var difficult = OSKernel.Presentation.Core.Http.OSHttpClient.Instance.GetDifficultLog(args.Model.Task.TaskID);

                    if (!difficult.Item1)
                    {
                        this.ShowDialog("提示信息", difficult.Item2, CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Error);
                    }

                    break;

                case CaseEventArgs.EventTypeEnum.Copy:
                    this.CopyOperation(args.Model);
                    break;

                case CaseEventArgs.EventTypeEnum.Delete:
                    this.DeleteOperation(args.Model);
                    break;

                case CaseEventArgs.EventTypeEnum.LogicMessage:
                    break;

                case CaseEventArgs.EventTypeEnum.CreateResult:
                    this.createResult(args.Model);
                    break;
            }
        }

        void CopyOperation(Case model)
        {
            // 1.Copy
            var newCase = model.CopyClone();
            model.Serialize();
            newCase.Serialize();

            // 2.像缓存中添加
            this.Cases.Add(newCase);
            this.CommonDataManager.LocalCases.Add(newCase);

            // 原始目录
            var path = $"{CacheManager.Instance.GetDataPath()}\\{model.LocalID}";

            if (newCase.CaseType == CaseTypeEnum.Mixed)
            {
                #region 同时开课

                var sameOpen = MixedRuleEnum.ClassHourSameOpen.RuleDeSerialize<ObservableCollection<UISameOpenTime>>(model.LocalID);
                if (sameOpen?.Count > 0)
                {
                    MixedRuleEnum.ClassHourSameOpen.RuleSerialize(newCase.LocalID, sameOpen);
                }

                var saveCourse = model.LocalID.MixedClassFile().DeSerializeObjectFromJson<List<SaveCourseLevel>>();
                if (saveCourse?.Count > 0)
                {
                    newCase.LocalID.MixedClassFile().SerializeObjectToJson(saveCourse);
                }

                #endregion

                #region 模式

                if (model.Pattern != PatternTypeEnum.None)
                {
                    // 1.获取原始方案模式
                    var patternCase = PatternDataManager.GetCase(model.LocalID);
                    var patternRule = PatternDataManager.GetRule(model.LocalID);
                    var patternAlgoRule = PatternDataManager.GetAlgoRule(model.LocalID);
                    var patternParam = PatternDataManager.GetPatternParam(model.LocalID);

                    // 2.将模式模式保存到本地
                    patternCase.SerializePatternCase(newCase.LocalID);
                    patternRule.SerializePatternRule(newCase.LocalID);
                    patternAlgoRule.SerializePatternAlgo(newCase.LocalID);
                    newCase.LocalID.Serialize(patternParam);

                    // 3.将模式文件反序列化-并存入缓存中  TODO
                    var newPatternPattern = newCase.LocalID.DeSerialize<object>();
                    var newPatternRule = newCase.LocalID.DeSerializePatternRule();
                    var newPatternAlgo = newCase.LocalID.DeSerializePatternAlgo();
                    var newPatternCase = newCase.LocalID.DeSerializePatternCase();

                    PatternDataManager.AddAlgoRule(newCase.LocalID, newPatternAlgo);
                    PatternDataManager.AddRule(newCase.LocalID, newPatternRule);
                    PatternDataManager.AddCase(newCase.LocalID, newPatternCase);
                    PatternDataManager.AddPatternParam(newCase.LocalID, newPatternPattern);

                }

                #endregion
            }
            else
            {
                #region 同时开课

                var sameOpen = OSKernel.Presentation.Models.Enums.AdministrativeRuleEnum.ClassHourSameOpen.RuleDeSerialize<ObservableCollection<Administrative.Modify.Rule.ClassHour.Model.UISameOpenTime>>(model.LocalID);
                if (sameOpen?.Count > 0)
                {
                    OSKernel.Presentation.Models.Enums.AdministrativeRuleEnum.ClassHourSameOpen.RuleSerialize(newCase.LocalID, sameOpen);
                }

                #endregion
            }

            #region 复制方案

            if (newCase.CaseType == CaseTypeEnum.Administrative)
            {
                var cp = model.LocalID.DeSerializeCP();
                if (cp != null)
                {
                    CommonDataManager.AddAdminCase(newCase.LocalID, cp);
                    cp.Serialize(newCase.LocalID);
                }

                var rule = model.LocalID.DeSerializeRule();
                if (rule != null)
                {
                    CommonDataManager.AddAdminRule(newCase.LocalID, rule);
                    rule.Serialize(newCase.LocalID);
                }

                var algo = model.LocalID.DeSerializeAlgo();
                if (algo != null)
                {
                    CommonDataManager.AddAminAlgoRule(newCase.LocalID, algo);
                    algo.Serialize(newCase.LocalID);
                }
            }
            else if (newCase.CaseType == CaseTypeEnum.Mixed)
            {
                var cl = model.LocalID.DeSerializeCL();
                if (cl != null)
                {
                    CommonDataManager.AddMixedCase(newCase.LocalID, cl);
                    cl.Serialize(newCase.LocalID);
                }

                var rule = model.LocalID.DeSerializeMixedRule();
                if (rule != null)
                {
                    CommonDataManager.AddMixedRule(newCase.LocalID, rule);
                    rule.Serialize(newCase.LocalID);
                }

                var algo = model.LocalID.DeSerializeMixedAlgo();
                if (algo != null)
                {
                    CommonDataManager.AddMixedAlgoRule(newCase.LocalID, algo);
                    algo.Serialize(newCase.LocalID);
                }
            }

            #endregion
        }

        void DeleteOperation(Case model)
        {
            var result = this.ShowDialog("提示信息", "确认删除?", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
            if (result == CustomControl.Enums.DialogResultType.OK)
            {
                // 2.UI移除
                this.Cases.Remove(model);

                // 3.缓存移除方案
                CommonDataManager.RemoveFullCase(model.LocalID);

                // 3.删除本地文件
                model.DeleteFile();

                // 4.测试文件
                PatternDataManager.RemoveAll(model.LocalID);
            }
        }

        void SecondReturn()
        {
            this.ShowFirstPanel = true;
            this.CurrentView.FedIn();
        }

        void ModifyOperation(object model)
        {
            Case caseUI = model as Case;

            this.SecondBarTitle = $"编辑  {caseUI.Name}({caseUI.CaseType.GetLocalDescription()})";
            base.CommonDataManager.LocalID = caseUI.LocalID;

            // 是否显示排课按钮
            if (caseUI.Task != null)
            {
                if (caseUI.Task.TaskStatus == MissionStateEnum.Reloading
                     || caseUI.Task.TaskStatus == MissionStateEnum.Started
                     || caseUI.Task.TaskStatus == MissionStateEnum.Waiting)
                {
                    this.ShowSecondArrangeButton = false;
                }
                else
                {
                    this.ShowSecondArrangeButton = true;
                }
            }
            else
            {
                this.ShowSecondArrangeButton = false;
            }

            // 刷新是否显示二级排课按钮
            if (caseUI.CaseType == CaseTypeEnum.Administrative)
            {
                this.SetCurrentView(new Administrative.Modify.IndexView());
            }
            else
            {
                this.SetCurrentView(new Mixed.Modify.IndexView());
            }
        }

        void ResultOperation(object model)
        {
            Case caseUI = model as Case;

            this.SecondBarTitle = $"排课结果  {caseUI.Name}({caseUI.CaseType.GetLocalDescription()})";
            base.CommonDataManager.LocalID = caseUI.LocalID;

            if (caseUI.CaseType == CaseTypeEnum.Administrative)
            {
                this.SetCurrentView(new Administrative.Result.ResultView());
            }
            else
            {
                this.SetCurrentView(new Mixed.Result.ResultView());
            }
        }

        void SetCurrentView(UserControl view)
        {
            this.ShowFirstPanel = false;
            this.CurrentView = view;
            this.CurrentView.FedIn();
        }

        void caseCommand(Case model)
        {
            if (model.IsStart)
            {
                this.Start(model);
            }
            else
            {
                if (model.Task?.TaskID != 0)
                {
                    if (model.Task?.TaskStatus == MissionStateEnum.Completed || model.Task?.TaskStatus == MissionStateEnum.Failed)
                    {
                        model.IsStart = true;
                        this.Start(model);
                    }
                    else
                    {
                        this.Stop(model);
                    }
                }
                else
                {
                    model.IsStart = true;
                }
            }
        }

        void switchTaskCommand(object obj)
        {
            Button switchButton = obj as Button;
            Grid gridPanel = switchButton.Parent as Grid;

            if (switchButton.Tag.ToString().Equals("h"))
            {
                gridPanel.Show();
                switchButton.Tag = "s";
            }
            else
            {
                gridPanel.Hidden();
                switchButton.Tag = "h";
            }
        }

        void arrangeCommand()
        {
            // 默认启动自动排课
            var model = CommonDataManager.GetLocalCase(base.LocalID);
            this.StartArrang(model);
        }

        void createResult(Case model)
        {
            var cases = ResultDataManager.GetResults(model.LocalID);
            var no = cases.Count;

            Models.Result.UIResult result = new Models.Result.UIResult()
            {
                NO = ++no,
                CreateTime = DateTime.Now,
                LocalID = model.LocalID,
                Name = $"{model.Name}-{model.Task?.TaskID}-{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}",
                TaskID = model.Task.TaskID
            };

            cases.Add(result);

            model.Serizlize(cases);
        }

        void startCaseModel(Case model, long taskID)
        {
            if (taskID == 0)
            {
                model.ShowLoading = false;
                model.IsStart = false;
                model.Task = null;
                model.Serialize();
            }
            else
            {
                model.Task = new TaskModel();
                model.Task.IsAuto = model.IsAuto;
                model.Task.TaskID = taskID;
                model.Task.Name = model.Name;
                model.Task.StartTime = DateTime.Now;
                model.Task.TaskStatus = MissionStateEnum.Creating;
                model.Task.PropertyChanged += Task_PropertyChanged;
                model.StartRefresh();
                model.Serialize();

                if (model.IsAuto)
                {
                    model.StartAutoFresh();
                }

                GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    this.RunCases.Add(model);
                });
            }
        }

        void Start(Case model)
        {
            //if (CacheManager.Instance.LoginUser.UserName.Equals("未知用户"))
            //{
            //    model.IsStart = false;
            //    Login.LoginWindow login = new Login.LoginWindow();
            //    login.ShowDialog();
            //    return;
            //}

            model.ShowLoading = true;
            Task.Run(() =>
            {
                // 获取价格
                return Core.Http.OSHttpClient.Instance.GetPrice(CacheManager.Instance.LoginUser.AccessToken);

            }).ContinueWith(r =>
            {
                model.ShowLoading = false;

                if (!r.Result.Item1)
                {
                    // 获取最新价格
                    //if (r.Result.Item3?.IndexOf("非法或过期的令牌") == -1)
                    //{
                    //    model.IsStart = false;
                    //    this.ShowDialog("提示信息", "获取价格失败!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                    //}
                    this.ShowDialog("提示信息", "获取价格失败!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                    model.IsStart = false;
                }
                else
                {
                    // 价格
                    var priceInfo = r.Result.Item2;

                    if (model.CaseType == CaseTypeEnum.Administrative)
                    {
                        var adminAlgoRule = CommonDataManager.GetAminAlgoRule(model.LocalID);
                        var adminRule = CommonDataManager.GetAminRule(model.LocalID);
                        var cp = CommonDataManager.GetCPCase(model.LocalID);

                        // 指定是否自动
                        cp.IsAuto = model.IsAuto;

                        // 排课费用估算
                        var classCount = cp.Classes.Count;
                        var price = (priceInfo.cpprice * classCount) / 100.00;

                        // 最大金额1000元
                        if (price > 1000)
                            price = 1000;

                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            ConfirmPriceWindow confrimPrice = new ConfirmPriceWindow(price);
                            confrimPrice.ShowDialog();

                            if (!confrimPrice.DialogResult.Value)
                            {
                                model.IsStart = false;
                                return;
                            }

                            model.ShowLoading = true;
                            Task.Run(() =>
                            {
                                return Core.Http.OSHttpClient.Instance.CreateXZB(new Core.Http.CPTransfer()
                                {
                                    algo = adminAlgoRule,
                                    cp = cp,
                                    rule = adminRule
                                });
                            }).ContinueWith(rr =>
                            {
                                if (!rr.Result.Item1)
                                {
                                    model.ShowLoading = false;
                                    if (rr.Result.Item3.IndexOf("签名不正确") == -1)
                                    {
                                        model.IsStart = false;
                                        model.Task = null;
                                        model.Serialize();
                                        this.ShowDialog("提示信息", rr.Result.Item3, CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Error);
                                    }
                                    else
                                    {
                                        if (SignLogic.SignCheck())
                                        {
                                            model.ShowLoading = true;
                                            Task.Run(() =>
                                            {
                                                return Core.Http.OSHttpClient.Instance.CreateXZB(new Core.Http.CPTransfer()
                                                {
                                                    algo = adminAlgoRule,
                                                    cp = cp,
                                                    rule = adminRule
                                                });
                                            }).ContinueWith((rrr) =>
                                            {
                                                if (!rrr.Result.Item1)
                                                {
                                                    model.ShowLoading = false;
                                                    model.IsStart = false;
                                                    model.Task = null;
                                                    model.Serialize();
                                                    this.ShowDialog("提示信息", rrr.Result.Item3, CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Error);
                                                }
                                                else
                                                {
                                                    this.startCaseModel(model, rrr.Result.Item2);
                                                }
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    this.startCaseModel(model, rr.Result.Item2);
                                }
                            });
                        });


                    }
                    else
                    {
                        var mixedAlgoRule = base.GetCLAlgoRule(model.LocalID);
                        var mixedRule = base.GetClRule(model.LocalID);
                        var cl = base.GetClCase(model.LocalID);
                        cl.IsAuto = model.IsAuto;

                        // 排课费用估算
                        var studentCount = cl.Students.Count;
                        var price = (priceInfo.clprice * studentCount) / 100.00;

                        // 最大金额1000元
                        if (price > 2000)
                            price = 2000;

                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            ConfirmPriceWindow confrimPrice = new ConfirmPriceWindow(price);
                            confrimPrice.ShowDialog();

                            if (!confrimPrice.DialogResult.Value)
                            {
                                model.ShowLoading = false;
                                model.IsStart = false;
                                return;
                            }


                            model.ShowLoading = true;
                            Task.Run(() =>
                            {
                                return Core.Http.OSHttpClient.Instance.CreateJXB(new Core.Http.CLTransfer()
                                {
                                    algo = mixedAlgoRule,
                                    cl = cl,
                                    rule = mixedRule
                                });
                            }).ContinueWith(rr =>
                            {
                                if (!rr.Result.Item1)
                                {
                                    model.ShowLoading = false;
                                    if (rr.Result.Item3.IndexOf("签名不正确") == -1)
                                    {
                                        model.IsStart = false;
                                        model.Task = null;
                                        model.Serialize();
                                        this.ShowDialog("提示信息", rr.Result.Item3, CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Error);
                                    }
                                    else
                                    {
                                        if (SignLogic.SignCheck())
                                        {
                                            model.ShowLoading = true;
                                            Task.Run(() =>
                                            {
                                                return Core.Http.OSHttpClient.Instance.CreateJXB(new Core.Http.CLTransfer()
                                                {
                                                    algo = mixedAlgoRule,
                                                    cl = cl,
                                                    rule = mixedRule
                                                });

                                            }).ContinueWith(rrr =>
                                            {
                                                if (!rrr.Result.Item1)
                                                {
                                                    model.ShowLoading = false;
                                                    model.IsStart = false;
                                                    model.Task = null;
                                                    model.Serialize();
                                                    this.ShowDialog("提示信息", rrr.Result.Item3, CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Error);
                                                }
                                                else
                                                {
                                                    this.startCaseModel(model, rrr.Result.Item2);
                                                }
                                            });

                                        }
                                    }
                                }
                                else
                                {
                                    this.startCaseModel(model, rr.Result.Item2);
                                }
                            });
                        });
                    }
                }
            });
        }

        void StartArrang(Case model)
        {
            //if (CacheManager.Instance.LoginUser.UserName.Equals("未知用户"))
            //{
            //    model.IsStart = false;
            //    Login.LoginWindow login = new Login.LoginWindow();
            //    login.ShowDialog();
            //    return;
            //}
            model.IsStart = true;
            model.ShowLoading = true;
            Task.Run(() =>
            {
                // 获取价格
                return Core.Http.OSHttpClient.Instance.GetPrice(CacheManager.Instance.LoginUser.AccessToken);

            }).ContinueWith(r =>
            {
                model.ShowLoading = false;

                if (!r.Result.Item1)
                {
                    this.ShowDialog("提示信息", "获取价格失败!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                    model.IsStart = false;
                }
                else
                {
                    model.IsAuto = true;

                    // 价格
                    var priceInfo = r.Result.Item2;

                    if (model.CaseType == CaseTypeEnum.Administrative)
                    {
                        var adminAlgoRule = CommonDataManager.GetAminAlgoRule(model.LocalID);
                        var adminRule = CommonDataManager.GetAminRule(model.LocalID);
                        var cp = CommonDataManager.GetCPCase(model.LocalID);

                        // 指定是否自动
                        cp.IsAuto = model.IsAuto;

                        // 排课费用估算
                        var classCount = cp.Classes.Count;
                        var price = (priceInfo.cpprice * classCount) / 100.00;

                        // 最大金额1000元
                        if (price > 1000)
                            price = 1000;

                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            ConfirmPriceWindow confrimPrice = new ConfirmPriceWindow(price);
                            confrimPrice.ShowDialog();

                            if (!confrimPrice.DialogResult.Value)
                            {
                                model.IsStart = false;
                                return;
                            }

                            model.ShowLoading = true;
                            Task.Run(() =>
                            {
                                return Core.Http.OSHttpClient.Instance.CreateXZB(new Core.Http.CPTransfer()
                                {
                                    algo = adminAlgoRule,
                                    cp = cp,
                                    rule = adminRule
                                });
                            }).ContinueWith(rr =>
                            {
                                if (!rr.Result.Item1)
                                {
                                    model.ShowLoading = false;
                                    if (rr.Result.Item3.IndexOf("签名不正确") == -1)
                                    {
                                        model.IsStart = false;
                                        model.Task = null;
                                        model.Serialize();
                                        this.ShowDialog("提示信息", rr.Result.Item3, CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Error);
                                    }
                                    else
                                    {
                                        if (SignLogic.SignCheck())
                                        {
                                            model.ShowLoading = true;
                                            Task.Run(() =>
                                            {
                                                return Core.Http.OSHttpClient.Instance.CreateXZB(new Core.Http.CPTransfer()
                                                {
                                                    algo = adminAlgoRule,
                                                    cp = cp,
                                                    rule = adminRule
                                                });
                                            }).ContinueWith((rrr) =>
                                            {
                                                if (!rrr.Result.Item1)
                                                {
                                                    model.ShowLoading = false;
                                                    model.IsStart = false;
                                                    model.Task = null;
                                                    model.Serialize();
                                                    this.ShowDialog("提示信息", rrr.Result.Item3, CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Error);
                                                }
                                                else
                                                {
                                                    this.startCaseModel(model, rrr.Result.Item2);
                                                    SecondReturnCommand.Execute(null);
                                                }
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    this.startCaseModel(model, rr.Result.Item2);
                                    SecondReturnCommand.Execute(null);
                                }
                            });
                        });


                    }
                    else
                    {
                        var mixedAlgoRule = base.GetCLAlgoRule(model.LocalID);
                        var mixedRule = base.GetClRule(model.LocalID);
                        var cl = base.GetClCase(model.LocalID);
                        cl.IsAuto = model.IsAuto;

                        // 排课费用估算
                        var studentCount = cl.Students.Count;
                        var price = (priceInfo.clprice * studentCount) / 100.00;

                        // 最大金额1000元
                        if (price > 2000)
                            price = 2000;

                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            ConfirmPriceWindow confrimPrice = new ConfirmPriceWindow(price);
                            confrimPrice.ShowDialog();

                            if (!confrimPrice.DialogResult.Value)
                            {
                                model.ShowLoading = false;
                                model.IsStart = false;
                                return;
                            }

                            model.ShowLoading = true;
                            Task.Run(() =>
                            {
                                return Core.Http.OSHttpClient.Instance.CreateJXB(new Core.Http.CLTransfer()
                                {
                                    algo = mixedAlgoRule,
                                    cl = cl,
                                    rule = mixedRule
                                });
                            }).ContinueWith(rr =>
                            {
                                if (!rr.Result.Item1)
                                {
                                    model.ShowLoading = false;

                                    if (rr.Result.Item3.IndexOf("签名不正确") == -1)
                                    {
                                        model.IsStart = false;
                                        model.Task = null;
                                        model.Serialize();
                                        this.ShowDialog("提示信息", rr.Result.Item3, CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Error);
                                    }
                                    else
                                    {
                                        if (SignLogic.SignCheck())
                                        {
                                            model.ShowLoading = true;
                                            Task.Run(() =>
                                            {
                                                return Core.Http.OSHttpClient.Instance.CreateJXB(new Core.Http.CLTransfer()
                                                {
                                                    algo = mixedAlgoRule,
                                                    cl = cl,
                                                    rule = mixedRule
                                                });

                                            }).ContinueWith(rrr =>
                                            {
                                                if (!rrr.Result.Item1)
                                                {
                                                    model.ShowLoading = false;
                                                    model.IsStart = false;
                                                    model.Task = null;
                                                    model.Serialize();
                                                    this.ShowDialog("提示信息", rrr.Result.Item3, CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Error);
                                                }
                                                else
                                                {
                                                    this.startCaseModel(model, rrr.Result.Item2);
                                                    SecondReturnCommand.Execute(null);
                                                }
                                            });

                                        }
                                    }
                                }
                                else
                                {
                                    this.startCaseModel(model, rr.Result.Item2);
                                    SecondReturnCommand.Execute(null);
                                }
                            });
                        });
                    }
                }
            });
        }

        void Stop(Case model)
        {
            if (model.Task?.TaskID != 0)
            {
                model.ShowLoading = true;

                Task.Run(() =>
                {
                    return Core.Http.OSHttpClient.Instance.GetStateByTaskID(model.Task.TaskID);

                }).ContinueWith(r =>
                {
                    model.ShowLoading = false;

                    var value = r.Result;
                    if (!value.Item1)
                    {
                        if (value.Item3.IndexOf("签名不正确") != -1)
                        {
                            if (SignLogic.SignCheck())
                            {
                                value = Core.Http.OSHttpClient.Instance.GetStateByTaskID(model.Task.TaskID);
                            }
                        }
                    }

                    if (value.Item2 == MissionStateEnum.Waiting)
                    {
                        var cancel = Core.Http.OSHttpClient.Instance.Cancel(model.Task.TaskID);
                        if (cancel.Item1)
                        {
                            model.StopRefresh();
                            model.Task.TaskStatus = MissionStateEnum.Cancelled;
                            model.IsStart = false;
                            model.Serialize();
                        }
                        else
                        {
                            model.IsStart = true;

                            GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                this.ShowDialog("提示信息", cancel.Item2, CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                            });

                        }
                    }
                    else if (value.Item2 == MissionStateEnum.Started)
                    {
                        var stop = Core.Http.OSHttpClient.Instance.Stop(model.Task.TaskID);
                        if (stop.Item1)
                        {
                            model.IsStart = false;
                            model.Task.TaskStatus = MissionStateEnum.Stopped;
                            model.Serialize();
                            model.StopRefresh();
                        }
                        else
                        {
                            model.IsStart = true;

                            GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                this.ShowDialog("提示信息", stop.Item2, CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                            });
                        }
                    }
                    else
                    {
                        model.Task.TaskStatus = value.Item2;
                        model.Serialize();
                    }

                });
            }
        }

        void LoadCase()
        {
            Task localData = new Task(() =>
            {
                var path = CacheManager.Instance.GetDataPath();
                var has = System.IO.Directory.Exists(path);
                if (has)
                {
                    if (System.IO.Directory.Exists(path))
                    {
                        var directories = System.IO.Directory.GetDirectories(path);
                        if (directories != null)
                        {
                            foreach (var d in directories)
                            {
                                System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(d);

                                var patternDirectory = directory.Name.GetPatternDirectory();
                                if (System.IO.Directory.Exists(patternDirectory))
                                {
                                    var patternFiles = System.IO.Directory.GetFiles(patternDirectory)?.ToList();
                                    if (patternFiles != null)
                                    {
                                        patternFiles?.ForEach(pf =>
                                        {
                                            System.IO.FileInfo file = new System.IO.FileInfo(pf);
                                            if (file.Extension.Equals(".algo"))
                                            {
                                                var algo = directory.Name.DeSerializePatternAlgo();
                                                PatternDataManager.AddAlgoRule(directory.Name, algo);
                                            }
                                            else if (file.Extension.Equals(".case"))
                                            {
                                                var cl = directory.Name.DeSerializePatternCase();
                                                PatternDataManager.AddCase(directory.Name, cl);
                                            }
                                            else if (file.Extension.Equals(".rule"))
                                            {
                                                var rule = directory.Name.DeSerializePatternRule();
                                                PatternDataManager.AddRule(directory.Name, rule);

                                            }
                                            else if (file.Extension.Equals(".pattern"))
                                            {
                                                var pattern = directory.Name.DeSerialize<object>();
                                                PatternDataManager.AddPatternParam(directory.Name, pattern);
                                            }
                                        });
                                    }
                                }

                                // 显示加载进度（默认一上来应该显示方案）
                                var files = System.IO.Directory.GetFiles(d)?.ToList();
                                if (files != null)
                                {
                                    // 先获取Local方案。
                                    var localFile = files.FirstOrDefault(f => f.LastIndexOf(".local") != -1);
                                    if (localFile != null)
                                    {
                                        var caseModel = localFile.DeSerializeObjectFromJson<Case>();
                                        CommonDataManager.LocalCases.Add(caseModel);

                                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                        {
                                            this.Cases.Add(caseModel);
                                            if (caseModel.Task != null)
                                            {
                                                caseModel.Task.PropertyChanged += Task_PropertyChanged;

                                                if (caseModel.Task.TaskID != 0)
                                                {
                                                    if (caseModel.Task.TaskStatus == MissionStateEnum.Waiting
                                                    || caseModel.Task.TaskStatus == MissionStateEnum.Started
                                                    || caseModel.Task.TaskStatus == MissionStateEnum.Creating)
                                                    {
                                                        this.RunCases.Add(caseModel);
                                                        caseModel.StartRefresh();
                                                    }
                                                }
                                            }
                                        });

                                        files.Remove(localFile);

                                        // 遍历其它文件（case，rule，algo，pattern）
                                        files.ForEach(f =>
                                        {
                                            var fi = new System.IO.FileInfo(f);
                                            var extension = fi.Extension;

                                            if (extension.Equals(".case"))
                                            {
                                                if (caseModel.CaseType == CaseTypeEnum.Administrative)
                                                {
                                                    var cpCase = f.DeSerializeObjectFromJson<CPCase>();
                                                    CommonDataManager.AddAdminCase(caseModel.LocalID, cpCase);
                                                }
                                                else if (caseModel.CaseType == CaseTypeEnum.Mixed)
                                                {
                                                    var clCase = f.DeSerializeObjectFromJson<CLCase>();
                                                    CommonDataManager.AddMixedCase(caseModel.LocalID, clCase);
                                                }
                                            }
                                            else if (extension.Equals(".rule"))
                                            {
                                                if (caseModel.CaseType == CaseTypeEnum.Administrative)
                                                {
                                                    var rule = f.DeSerializeObjectFromJson<XYKernel.OS.Common.Models.Administrative.Rule.Rule>();
                                                    CommonDataManager.AddAdminRule(caseModel.LocalID, rule);
                                                }
                                                else if (caseModel.CaseType == CaseTypeEnum.Mixed)
                                                {
                                                    var rule = f.DeSerializeObjectFromJson<XYKernel.OS.Common.Models.Mixed.Rule.Rule>();
                                                    CommonDataManager.AddMixedRule(caseModel.LocalID, rule);
                                                }
                                            }
                                            else if (extension.Equals(".algo"))
                                            {
                                                if (caseModel.CaseType == CaseTypeEnum.Administrative)
                                                {
                                                    var rule = f.DeSerializeObjectFromJson<XYKernel.OS.Common.Models.Administrative.AlgoRule.AlgoRule>();
                                                    CommonDataManager.AddAminAlgoRule(caseModel.LocalID, rule);
                                                }
                                                else if (caseModel.CaseType == CaseTypeEnum.Mixed)
                                                {
                                                    var rule = f.DeSerializeObjectFromJson<XYKernel.OS.Common.Models.Mixed.AlgoRule.AlgoRule>();
                                                    CommonDataManager.AddMixedAlgoRule(caseModel.LocalID, rule);
                                                }
                                            }
                                            else if (extension.Equals(".result"))
                                            {
                                                var resuls = f.DeSerializeObjectFromJson<List<UIResult>>();
                                                ResultDataManager.AddRangeResult(caseModel.LocalID, resuls);
                                            }
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            });

            localData.Start();
        }

        void Task_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            TaskModel model = sender as TaskModel;

            if (e.PropertyName.Equals(nameof(model.TaskStatus)))
            {
                if (model.TaskStatus == MissionStateEnum.Aborted
                    || model.TaskStatus == MissionStateEnum.Cancelled
                    || model.TaskStatus == MissionStateEnum.Completed
                    || model.TaskStatus == MissionStateEnum.Expired
                    || model.TaskStatus == MissionStateEnum.Failed
                    || model.TaskStatus == MissionStateEnum.Stopped
                    )
                {
                    var hasRunCase = this.RunCases.FirstOrDefault(rc => rc.Task.TaskID.Equals(model.TaskID));
                    if (hasRunCase != null)
                    {
                        this.RunCases.Remove(hasRunCase);
                        model.PropertyChanged -= Task_PropertyChanged;
                    }
                }
            }
        }

        public bool CanReCaseName(string caseName)
        {
            if (string.IsNullOrEmpty(caseName))
            {
                this.ShowDialog("提示信息", "方案名称不能为空!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return false;
            }

            var has = this.Cases.Any(c => c.Name.Equals(caseName));
            if (has)
            {
                this.ShowDialog("提示信息", "方案名称不能重复.", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return false;
            }
            else
                return true;
        }
    }
}
