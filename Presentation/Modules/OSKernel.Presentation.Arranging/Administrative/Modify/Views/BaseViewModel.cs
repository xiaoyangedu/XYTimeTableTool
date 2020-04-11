using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;
using XYKernel.OS.Common.Enums;

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Views
{
    /// <summary>
    /// 基础信息模型实现
    /// </summary>
    public class BaseViewModel : CommonViewModel, IInitilize
    {
        private string _version;
        private string _school;
        private Dictionary<string, LearningPeriod> _periods;
        private LearningPeriod _selectLearningPeriod;

        private bool _isTeacherHalfDay;
        private bool _isTeacherClassBalance;
        private bool _isTeacherPositionBalance;
        private bool _isPEAllowLast;
        private bool _isTeacherContinousSameDay;
        private bool _isTeacherDayGapsLimit;
        private bool _isTwoClassHourLimit;
        private bool _isThreeClassHourLimit;
        private bool _isMajorCourseSameDay;


        public ICommand SaveCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(Save);
            }
        }

        public BaseViewModel()
        {
            this.Periods = new Dictionary<string, LearningPeriod>()
            {
                { "幼儿园（建设中，默认为小学，不影响排课）",LearningPeriod.Kindergarten},
                {"小学",LearningPeriod.PrimarySchool},
                {"初中",LearningPeriod.JuniorMiddleSchool},
                {"高中",LearningPeriod.SeniorMiddleSchool},
            };
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(this.School))
            {
                this.ShowDialog("提示信息", "学校名称不能为空!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

            var cp = CommonDataManager.GetCPCase(base.LocalID);
            cp.LearningPeriod = this.SelectLearningPeriod == LearningPeriod.Kindergarten ? LearningPeriod.PrimarySchool : this.SelectLearningPeriod;
            cp.SchoolName = this.School;
            cp.Version = this.Version;
            cp.IsMajorCourseSameDay = this.IsMajorCourseSameDay;
            cp.IsPEAllowLast = this.IsPEAllowLast;
            cp.IsTeacherClassBalance = this.IsTeacherClassBalance;
            cp.IsTeacherContinousSameDay = this.IsTeacherContinousSameDay;
            cp.IsTeacherDayGapsLimit = this.IsTeacherDayGapsLimit;
            cp.IsTeacherHalfDay = this.IsTeacherHalfDay;
            cp.IsTeacherPositionBalance = this.IsTeacherPositionBalance;
            cp.IsThreeClassHourLimit = this.IsThreeClassHourLimit;
            cp.IsTwoClassHourLimit = this.IsTwoClassHourLimit;
            cp.Serialize(base.LocalID);

            this.ShowDialog("提示信息", "保存成功!", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
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

        /// <summary>
        /// 学校
        /// </summary>
        public string School
        {
            get
            {
                return _school;
            }

            set
            {
                _school = value;
                RaisePropertyChanged(() => School);
            }
        }

        /// <summary>
        /// 学段
        /// </summary>
        public Dictionary<string, LearningPeriod> Periods
        {
            get
            {
                return _periods;
            }

            set
            {
                _periods = value;
                RaisePropertyChanged(() => Periods);
            }
        }

        /// <summary>
        /// 当前选择的学段
        /// </summary>
        public LearningPeriod SelectLearningPeriod
        {
            get
            {
                return _selectLearningPeriod;
            }

            set
            {
                _selectLearningPeriod = value;
                RaisePropertyChanged(() => SelectLearningPeriod);
            }
        }

        public bool IsTeacherHalfDay
        {
            get
            {
                return _isTeacherHalfDay;
            }

            set
            {
                _isTeacherHalfDay = value;
                RaisePropertyChanged(() => IsTeacherHalfDay);
            }
        }

        public bool IsTeacherClassBalance
        {
            get
            {
                return _isTeacherClassBalance;
            }

            set
            {
                _isTeacherClassBalance = value;
                RaisePropertyChanged(() => IsTeacherClassBalance);
            }
        }

        public bool IsTeacherPositionBalance
        {
            get
            {
                return _isTeacherPositionBalance;
            }

            set
            {
                _isTeacherPositionBalance = value;
                RaisePropertyChanged(() => IsTeacherPositionBalance);
            }
        }

        public bool IsPEAllowLast
        {
            get
            {
                return _isPEAllowLast;
            }

            set
            {
                _isPEAllowLast = value;
                RaisePropertyChanged(() => IsPEAllowLast);
            }
        }

        public bool IsTeacherContinousSameDay
        {
            get
            {
                return _isTeacherContinousSameDay;
            }

            set
            {
                _isTeacherContinousSameDay = value;
                RaisePropertyChanged(() => IsTeacherContinousSameDay);
            }
        }

        public bool IsTeacherDayGapsLimit
        {
            get
            {
                return _isTeacherDayGapsLimit;
            }

            set
            {
                _isTeacherDayGapsLimit = value;
                RaisePropertyChanged(() => IsTeacherDayGapsLimit);
            }
        }

        public bool IsTwoClassHourLimit
        {
            get
            {
                return _isTwoClassHourLimit;
            }

            set
            {
                _isTwoClassHourLimit = value;
                RaisePropertyChanged(() => IsTwoClassHourLimit);
            }
        }

        public bool IsThreeClassHourLimit
        {
            get
            {
                return _isThreeClassHourLimit;
            }

            set
            {
                _isThreeClassHourLimit = value;
                RaisePropertyChanged(() => IsThreeClassHourLimit);
            }
        }

        public bool IsMajorCourseSameDay
        {
            get
            {
                return _isMajorCourseSameDay;
            }

            set
            {
                _isMajorCourseSameDay = value;
                RaisePropertyChanged(() => IsMajorCourseSameDay);
            }
        }

        [InjectionMethod]
        public void Initilize()
        {
            // 获取走班
            var cp = CommonDataManager.GetCPCase(base.LocalID);

            this.SelectLearningPeriod = cp.LearningPeriod;

            this.School = cp.SchoolName;
            this.Version = cp.Version;

            this.IsMajorCourseSameDay = cp.IsMajorCourseSameDay;
            this.IsPEAllowLast = cp.IsPEAllowLast;
            this.IsTeacherClassBalance = cp.IsTeacherClassBalance;
            this.IsTeacherContinousSameDay = cp.IsTeacherContinousSameDay;
            this.IsTeacherDayGapsLimit = cp.IsTeacherDayGapsLimit;
            this.IsTeacherHalfDay = cp.IsTeacherHalfDay;
            this.IsTeacherPositionBalance = cp.IsTeacherPositionBalance;
            this.IsThreeClassHourLimit = cp.IsThreeClassHourLimit;
            this.IsTwoClassHourLimit = cp.IsTwoClassHourLimit;
        }
    }
}
