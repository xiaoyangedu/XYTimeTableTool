using OSKernel.Presentation.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using OSKernel.Presentation.Models;
using OSKernel.Presentation.Core.EventArgs;
using System.Timers;
using System.Windows;
using XYKernel.Presentation.Core;

namespace OSKernel.Presentation.Core
{
    /// <summary>
    /// 方案（本地）模型
    /// </summary>
    public class Case : GalaSoft.MvvmLight.ObservableObject, ICloneable
    {
        private bool _showLoading = false;

        private bool isStart;

        private bool _isAuto;

        private TaskModel _task;

        private string _name;

        /// <summary>
        /// 显示加载
        /// </summary>
        public bool ShowLoading
        {
            get
            {
                return _showLoading;
            }
            set
            {
                _showLoading = value;
                RaisePropertyChanged(() => ShowLoading);
            }
        }

        /// <summary>
        /// 本地ID
        /// </summary>
        public string LocalID { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 被Copy 次数
        /// </summary>
        public int CopyNO { get; set; }

        /// <summary>
        /// 本地文件路径
        /// </summary>
        public string LocalPath { get; set; }

        /// <summary>
        /// 方案类型
        /// </summary>
        public CaseTypeEnum CaseType { get; set; }

        /// <summary>
        /// 模式（只有走班有）
        /// </summary>
        public PatternTypeEnum Pattern { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 当前锁定任务ID
        /// </summary>
        public long LockedTaskID { get; set; }

        /// <summary>
        /// 课程颜色值
        /// </summary>
        public Dictionary<string, string> CourseColors { get; set; }

        /// <summary>
        /// 教师科目
        /// </summary>
        public Dictionary<string, List<string>> TeacherCourses { get; set; }

        /// <summary>
        /// 复制
        /// </summary>
        public ICommand CopyCommand
        {
            get
            {
                return new RelayCommand(copyCommand);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public ICommand DeleteCommand
        {
            get
            {
                return new RelayCommand(deleteCommand);
            }
        }

        /// <summary>
        /// 困难信息
        /// </summary>
        public ICommand DiffcultyCommand
        {
            get
            {
                return new RelayCommand(diffcultyCommand);
            }
        }

        public ICommand ClashingCommand
        {
            get
            {
                return new RelayCommand(clashingCommand);
            }
        }

        public ICommand CopyShortcutCommand
        {
            get
            {
                return new RelayCommand(copyShortcutCommand);
            }
        }

        public ICommand StopArrangCommand
        {
            get
            {
                return new RelayCommand(stopArrangCommand);
            }
        }

        public ICommand RunArrangCommand
        {
            get
            {
                return new RelayCommand(runArrangCommand);
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(this.Refresh);
            }
        }

        public bool IsStart
        {
            get
            {
                return isStart;
            }

            set
            {
                isStart = value;
                RaisePropertyChanged(() => IsStart);
            }
        }

        public TaskModel Task
        {
            get
            {
                return _task;
            }

            set
            {
                _task = value;
                RaisePropertyChanged(() => Task);
            }
        }

        /// <summary>
        /// 是否自动
        /// </summary>
        public bool IsAuto
        {
            get
            {
                return _isAuto;
            }

            set
            {
                _isAuto = value;
                RaisePropertyChanged(() => IsAuto);
            }
        }

        /// <summary>
        /// 方案名称
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public Case()
        {
            this.CourseColors = new Dictionary<string, string>();
            this.TeacherCourses = new Dictionary<string, List<string>>();
        }

        void copyCommand()
        {
            CaseEventArgs eventArgs = new CaseEventArgs()
            {
                EventType = CaseEventArgs.EventTypeEnum.Copy,
                Model = this
            };
            Messenger.Default.Send<CaseEventArgs>(eventArgs);
        }

        void deleteCommand()
        {
            CaseEventArgs eventArgs = new CaseEventArgs()
            {
                EventType = CaseEventArgs.EventTypeEnum.Delete,
                Model = this
            };
            Messenger.Default.Send<CaseEventArgs>(eventArgs);
        }

        void diffcultyCommand()
        {
            CaseEventArgs eventArgs = new CaseEventArgs()
            {
                EventType = CaseEventArgs.EventTypeEnum.Difficulty,
                Model = this
            };
            Messenger.Default.Send<CaseEventArgs>(eventArgs);
        }

        void clashingCommand()
        {
            CaseEventArgs eventArgs = new CaseEventArgs()
            {
                EventType = CaseEventArgs.EventTypeEnum.Clash,
                Model = this
            };
            Messenger.Default.Send<CaseEventArgs>(eventArgs);
        }

        void copyShortcutCommand()
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("LocalID");
            str.AppendLine(this.LocalID.ToString());
            str.AppendLine("方案名称");
            str.AppendLine(this.Name.ToString());
            str.AppendLine("TaskID");
            if (this.Task != null)
                str.AppendLine(this.Task.TaskID.ToString());

            str.AppendLine("本地方案路径");
            str.AppendLine($"{System.Environment.CurrentDirectory}\\{this.LocalPath}");

            Clipboard.SetDataObject(str.ToString(), true);
        }

        void stopArrangCommand()
        {

        }

        void runArrangCommand()
        {

        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public Case CopyClone()
        {
            CopyNO = CopyNO + 1;
            var copy = (Case)this.Clone();
            copy.CopyNO = 0;
            copy.LocalID = Guid.NewGuid().ToString();
            copy.Name += $"- {CopyNO}";
            copy.CreateTime = DateTime.Now;
            copy.Task = new TaskModel();
            copy.refreshTimer = null;
            copy.IsStart = false;
            return copy;
        }

        #region Timer

        Timer refreshTimer;

        Timer progressTimer;

        Timer autoFreshTimer;

        /// <summary>
        /// 开始刷新
        /// </summary>
        public void StartRefresh()
        {
            if (refreshTimer == null)
            {
                refreshTimer = new Timer();
                refreshTimer.Interval = 5000;
                refreshTimer.Enabled = true;
                refreshTimer.Elapsed += RefreshTimer_Elapsed;
                refreshTimer.Start();
            }
            else
            {
                refreshTimer.Start();
            }
        }

        /// <summary>
        /// 启动进度
        /// </summary>
        public void StartProgress()
        {
            if (progressTimer == null)
            {
                progressTimer = new Timer();
                progressTimer.Interval = 5000;
                progressTimer.Enabled = true;
                progressTimer.Elapsed += ProgressTimer_Elapsed;
                progressTimer.Start();
            }
            else
            {
                progressTimer.Start();
            }
        }

        /// <summary>
        /// 开始自动刷新
        /// </summary>
        public void StartAutoFresh()
        {
            if (autoFreshTimer == null)
            {
                autoFreshTimer = new Timer();
                autoFreshTimer.Interval = 1000;
                autoFreshTimer.Enabled = true;
                autoFreshTimer.Elapsed += AutoFreshTimer_Elapsed;
                autoFreshTimer.Start();
            }
            else
            {
                autoFreshTimer.Start();
            }
        }

        private void AutoFreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Task != null)
            {
                Task.RaiseChanged();
            }
        }

        private void ProgressTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            progressTimer.Stop();

            var value = Http.OSHttpClient.Instance.Progress(this.Task.TaskID);
            if (value.Item1)
            {
                this.Task.TotalProcess = value.Item2.total;
                this.Task.MaxProcess = value.Item2.top;
                this.Task.CurrentProcess = value.Item2.cur;

                if (this.Task.CurrentProcess == 100)
                {
                    progressTimer.Stop();
                }
            }
            else
            {
                if (this.Task.TaskStatus == MissionStateEnum.Waiting
                    || this.Task.TaskStatus == MissionStateEnum.Creating
                    || this.Task.TaskStatus == MissionStateEnum.Started)
                {
                    // 进度调整
                    progressTimer.Start();
                }
            }
        }

        /// <summary>
        /// 停止进度
        /// </summary>
        public void StopProgress()
        {
            if (progressTimer != null)
            {
                progressTimer.Stop();
            }
        }

        /// <summary>
        /// 停止刷新
        /// </summary>
        public void StopRefresh()
        {
            if (refreshTimer != null)
            {
                refreshTimer.Stop();
            }

            if (autoFreshTimer != null)
            {
                autoFreshTimer.Stop();
            }
        }

        private void RefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            refreshTimer.Elapsed -= RefreshTimer_Elapsed;

            refreshTimer.Stop();

            try
            {

                var value = Http.OSHttpClient.Instance.GetStateByTaskID(this.Task.TaskID);
                if (value.Item1)
                {
                    if (value.Item2 != MissionStateEnum.Creating)
                    {
                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            this.ShowLoading = false;
                        });
                    }

                    GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        this.Task.StopTime = DateTime.Now;
                        this.Task.TaskStatus = value.Item2;
                        this.Task.RaiseChanged();
                        this.Serialize();
                    });

                    if (value.Item2 == MissionStateEnum.Started)
                    {
                        if (Task.RunTime == DateTime.MinValue)
                        {
                            Task.RunTime = DateTime.Now;
                        }

                        if (!this.IsAuto)
                        {
                            var progress = Http.OSHttpClient.Instance.Progress(this.Task.TaskID);
                            if (progress.Item1)
                            {
                                GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                {
                                    this.Task.TotalProcess = progress.Item2.total;
                                    this.Task.MaxProcess = progress.Item2.top;
                                    this.Task.CurrentProcess = progress.Item2.cur;
                                });

                            }
                        }
                    }

                    if (value.Item2 == MissionStateEnum.Completed
                        || value.Item2 == MissionStateEnum.Expired
                        || value.Item2 == MissionStateEnum.Failed
                        || value.Item2 == MissionStateEnum.Stopped
                        || value.Item2 == MissionStateEnum.Cancelled)
                    {
                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            this.Task.StopTime = DateTime.Now;
                            this.Task.TotalProcess = 1;
                            this.Task.CurrentProcess = 1;
                            this.Task.MaxProcess = 1;
                            this.Task.RaiseChanged();
                            this.ShowLoading = false;
                            this.Serialize();
                        });

                        if (value.Item2 == MissionStateEnum.Completed)
                        {
                            CaseEventArgs eventArgs = new CaseEventArgs()
                            {
                                EventType = CaseEventArgs.EventTypeEnum.CreateResult,
                                Model = this
                            };
                            Messenger.Default.Send<CaseEventArgs>(eventArgs);
                        }
                    }
                    else
                    {
                        refreshTimer.Start();
                    }
                }
                else
                {
                    GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        this.ShowLoading = false;
                    });

                    CaseEventArgs eventArgs = new CaseEventArgs()
                    {
                        EventType = CaseEventArgs.EventTypeEnum.LogicMessage,
                        Model = this
                    };
                    Messenger.Default.Send<CaseEventArgs>(eventArgs);
                }
            }
            catch (Exception ex)
            {
                LogManager.Logger.Error(ex.Message.ToString());

                GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    this.ShowLoading = false;
                });
#if DEBUG 
                Console.WriteLine(ex.Message);
#endif
            }

            refreshTimer.Elapsed += RefreshTimer_Elapsed;
        }

        /// <summary>
        /// 检查状态
        /// </summary>
        public void CheckStatus()
        {
            if (this.Task?.TaskID != 0)
            {
                if (this.Task.TaskStatus == MissionStateEnum.Creating ||
                    this.Task.TaskStatus == MissionStateEnum.Reloading ||
                    this.Task.TaskStatus == MissionStateEnum.Started ||
                    this.Task.TaskStatus == MissionStateEnum.Waiting)
                {
                    var value = OSKernel.Presentation.Core.Http.OSHttpClient.Instance.GetStateByTaskID(this.Task.TaskID);

                    if (value.Item1)
                    {
                        this.Task.TaskStatus = value.Item2;
                        this.Serialize();

                        if (this.Task.TaskStatus == MissionStateEnum.Completed)
                        {
                            CaseEventArgs eventArgs = new CaseEventArgs()
                            {
                                EventType = CaseEventArgs.EventTypeEnum.CreateResult,
                                Model = this
                            };
                            Messenger.Default.Send<CaseEventArgs>(eventArgs);
                        }
                        else
                        {
                            // 启动刷新函数
                            this.StartRefresh();
                        }
                    }
                }
            }
        }

        public void Refresh()
        {
            if (this.Task == null)
                return;

            if (this.Task.TaskStatus != MissionStateEnum.Creating
                   || this.Task.TaskStatus != MissionStateEnum.Started
                   || this.Task.TaskStatus != MissionStateEnum.Waiting)
            {
                return;
            }

            // 获取进度
            if (!this.IsAuto)
            {
                var progress = Http.OSHttpClient.Instance.Progress(this.Task.TaskID);
                if (progress.Item1)
                {
                    GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        this.Task.TotalProcess = progress.Item2.total;
                        this.Task.MaxProcess = progress.Item2.top;
                        this.Task.CurrentProcess = progress.Item2.cur;
                        this.Serialize();
                    });
                }
            }

            var value = Http.OSHttpClient.Instance.GetStateByTaskID(this.Task.TaskID);
            if (value.Item1)
            {
                GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    this.Task.StopTime = DateTime.Now;
                    this.Task.TaskStatus = value.Item2;
                    this.Serialize();
                    this.Task.RaiseChanged();
                });

                if (value.Item2 == MissionStateEnum.Started)
                {
                    if (Task.RunTime == DateTime.MinValue)
                    {
                        Task.RunTime = DateTime.Now;
                    }

                    if (!this.IsAuto)
                    {
                        var progress = Http.OSHttpClient.Instance.Progress(this.Task.TaskID);
                        if (progress.Item1)
                        {
                            GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                this.Task.TotalProcess = progress.Item2.total;
                                this.Task.MaxProcess = progress.Item2.top;
                                this.Task.CurrentProcess = progress.Item2.cur;
                                this.Serialize();
                            });
                        }
                    }
                }

                // 获取状态
                if (value.Item2 == MissionStateEnum.Completed
                    || value.Item2 == MissionStateEnum.Expired
                    || value.Item2 == MissionStateEnum.Failed
                    || value.Item2 == MissionStateEnum.Stopped
                    || value.Item2 == MissionStateEnum.Cancelled)
                {
                    GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        this.Task.StopTime = DateTime.Now;
                        this.Task.TotalProcess = 1;
                        this.Task.CurrentProcess = 1;
                        this.Task.MaxProcess = 1;
                        this.Serialize();
                        this.Task.RaiseChanged();
                    });

                    if (value.Item2 == MissionStateEnum.Completed)
                    {
                        CaseEventArgs eventArgs = new CaseEventArgs()
                        {
                            EventType = CaseEventArgs.EventTypeEnum.CreateResult,
                            Model = this
                        };
                        Messenger.Default.Send<CaseEventArgs>(eventArgs);
                    }
                }
            }
            else
            {
                CaseEventArgs eventArgs = new CaseEventArgs()
                {
                    EventType = CaseEventArgs.EventTypeEnum.LogicMessage,
                    Model = this
                };
                Messenger.Default.Send<CaseEventArgs>(eventArgs);
            }
        }

        #endregion
    }
}
