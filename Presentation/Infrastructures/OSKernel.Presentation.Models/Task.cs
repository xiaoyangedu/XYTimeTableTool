using GalaSoft.MvvmLight;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Models
{
    /// <summary>
    /// 任务(包括任务进度）
    /// </summary>
    public class TaskModel : ObservableObject
    {
        private long _taskID;

        private double _currentProcess;

        private double _maxProcess;

        private double _totalProcess = 1;

        private double _percentCurrent;

        private double _percentMax;

        private DateTime _startTime;

        private DateTime _maxValueTime;

        private DateTime _runTime;

        private DateTime _stopTime;

        private string _maxValueTimeString;

        private string _runTimeString;

        private string _autoRunTimeString;

        private Enums.MissionStateEnum _taskStatus = Enums.MissionStateEnum.Creating;

        /// <summary>
        /// 任务名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErroMessage { get; set; }

        /// <summary>
        /// 运行时间
        /// </summary>
        public string RunTimeString
        {
            get
            {
                if (RunTime == DateTime.MinValue)
                {
                    return "--";
                }
                else
                {
                    if (TaskStatus == Enums.MissionStateEnum.Completed || TaskStatus == Enums.MissionStateEnum.Failed)
                    {
                        var timeSpan = StopTime - _runTime;
                        return $"{timeSpan.Hours}时{timeSpan.Minutes}分{timeSpan.Seconds}秒";
                    }
                    else
                    {
                        var timeSpan = DateTime.Now - _runTime;
                        return $"{timeSpan.Hours}时{timeSpan.Minutes}分{timeSpan.Seconds}秒";
                    }
                }
            }
            set
            {
                _runTimeString = value;
            }
        }

        /// <summary>
        /// 最大时间字符串
        /// </summary>
        public string MaxValueTimeString
        {
            get
            {
                if (LastMaxProcess == 0)
                    return "--";
                else
                    return MaxValueTime.ToString("yyyy年MM月dd日 HH:mm:ss");

            }
            set
            {
                _maxValueTimeString = value;
                RaisePropertyChanged(() => MaxValueTimeString);
            }
        }

        /// <summary>
        /// 状态字符串
        /// </summary>
        public string StatusString
        {
            get
            {
                return TaskStatus.GetLocalDescription();
            }
        }

        /// <summary>
        /// 是否有排课结果
        /// </summary>
        public bool HasResult { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        public Enums.MissionStateEnum TaskStatus
        {
            get
            {
                return _taskStatus;
            }

            set
            {
                _taskStatus = value;

                RaisePropertyChanged(() => TaskStatus);
                RaisePropertyChanged(() => StatusString);

                RaisePropertyChanged(() => ShowDifficulty);
                RaisePropertyChanged(() => ShowClash);
            }
        }

        /// <summary>
        /// 任务ID
        /// </summary>
        public long TaskID
        {
            get
            {
                return _taskID;
            }

            set
            {
                _taskID = value;
                RaisePropertyChanged(() => TaskID);
            }
        }

        /// <summary>
        /// 最后最大进度
        /// </summary>
        public double LastMaxProcess { get; set; } = 0;

        /// <summary>
        /// 是否为自动排课。
        /// </summary>
        public bool IsAuto { get; set; }

        /// <summary>
        /// 最大进度
        /// </summary>
        public double MaxProcess
        {
            get
            {
                return _maxProcess;
            }

            set
            {
                if (value > LastMaxProcess)
                {
                    this.LastMaxProcess = value;
                    this.MaxValueTime = DateTime.Now;
                }

                _maxProcess = value;
                RaisePropertyChanged(() => MaxProcess);
                PercentMax = TotalProcess == 0 ? 0 : Math.Round(MaxProcess / TotalProcess, 5);
            }
        }

        /// <summary>
        /// 总进度
        /// </summary>
        public double TotalProcess
        {
            get
            {
                return _totalProcess;
            }

            set
            {
                _totalProcess = value;
                RaisePropertyChanged(() => TotalProcess);
            }
        }

        /// <summary>
        /// 当前进度
        /// </summary>
        public double CurrentProcess
        {
            get
            {
                return _currentProcess;
            }

            set
            {
                _currentProcess = value;
                RaisePropertyChanged(() => CurrentProcess);

                PercentCurrent = TotalProcess == 0 ? 0 : Math.Round(CurrentProcess / TotalProcess, 5);
            }
        }

        /// <summary>
        /// 当前百分比
        /// </summary>
        public double PercentCurrent
        {
            get
            {
                return _percentCurrent;
            }

            set
            {
                _percentCurrent = value;
                RaisePropertyChanged(() => PercentCurrent);
            }
        }

        /// <summary>
        /// 百分比最大值
        /// </summary>
        public double PercentMax
        {
            get
            {
                return _percentMax;
            }

            set
            {
                _percentMax = value;
                RaisePropertyChanged(() => PercentMax);
            }
        }

        /// <summary>
        /// 最大时间
        /// </summary>
        public DateTime MaxValueTime
        {
            get
            {
                return _maxValueTime;
            }

            set
            {
                _maxValueTime = value;
                RaisePropertyChanged(() => MaxValueTime);
            }
        }

        /// <summary>
        /// 运行时间
        /// </summary>
        public DateTime RunTime
        {
            get
            {
                return _runTime;
            }

            set
            {
                _runTime = value;
                RaisePropertyChanged(() => RunTime);
            }
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime StopTime
        {
            get
            {
                return _stopTime;
            }
            set
            {
                _stopTime = value;
                RaisePropertyChanged(() => StopTime);
            }
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }

            set
            {
                _startTime = value;
                RaisePropertyChanged(() => StartTime);
            }
        }

        /// <summary>
        /// 显示困难信息菜单
        /// </summary>
        public bool ShowDifficulty
        {
            get
            {
                if (TaskStatus == Enums.MissionStateEnum.Started)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 显示错误信息菜单
        /// </summary>
        public bool ShowClash
        {
            get
            {
                if (TaskStatus == Enums.MissionStateEnum.Failed && !IsAuto)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 查看结果
        /// </summary>
        public bool ShowView
        {
            get
            {
                if (TaskStatus == Enums.MissionStateEnum.Completed)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 查看结果
        /// </summary>
        public bool ShowCancel
        {
            get
            {
                if (TaskStatus == Enums.MissionStateEnum.Reloading || TaskStatus == Enums.MissionStateEnum.Waiting || TaskStatus == Enums.MissionStateEnum.Started)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 显示删除
        /// </summary>
        public bool ShowDelete
        {
            get
            {
                if ((int)TaskStatus > 3)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void RaiseChanged()
        {
            this.RaisePropertyChanged(() => RunTimeString);
            this.RaisePropertyChanged(() => MaxValueTimeString);
        }
    }
}
