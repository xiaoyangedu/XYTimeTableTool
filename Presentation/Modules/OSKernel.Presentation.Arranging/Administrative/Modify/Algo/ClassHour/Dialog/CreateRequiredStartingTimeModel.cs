using OSKernel.Presentation.Arranging.Administrative.Modify.Algo.ClassHour.Model;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.CustomControl.Enums;
using OSKernel.Presentation.Models.Administrative;
using OSKernel.Presentation.Models.Enums;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XYKernel.OS.Common.Models;

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Algo.ClassHour.Dialog
{
    public class CreateRequiredStartingTimeModel : BaseWindowModel, IInitilize
    {
        private AdministrativeAlgoRuleEnum _timeRule;

        private List<UIClassHour> _classHours;

        private Dictionary<string, DayOfWeek> _days;

        private List<DayPeriodModel> _periods;

        private UIClassHour _selectClassHour;

        private DayPeriodModel _selectPeriod;

        private DayOfWeek _selectDay;

        private string _day;

        private string _classHourString;

        public List<UIClassHour> Sources { get; set; }

        public List<UIClassHour> ClassHours
        {
            get
            {
                return _classHours;
            }

            set
            {
                _classHours = value;
                RaisePropertyChanged(() => ClassHours);
            }
        }

        public Dictionary<string, DayOfWeek> Days
        {
            get
            {
                return _days;
            }

            set
            {
                _days = value;
                RaisePropertyChanged(() => Days);
            }
        }

        public UIClassHour SelectClassHour
        {
            get
            {
                return _selectClassHour;
            }

            set
            {
                _selectClassHour = value;
                RaisePropertyChanged(() => SelectClassHour);
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(save);
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(cancel);
            }
        }

        public List<DayPeriodModel> Periods
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

        public DayPeriodModel SelectPeriod
        {
            get
            {
                return _selectPeriod;
            }

            set
            {
                _selectPeriod = value;
                RaisePropertyChanged(() => SelectPeriod);
            }
        }

        public DayOfWeek SelectDay
        {
            get
            {
                return _selectDay;
            }

            set
            {
                _selectDay = value;
                RaisePropertyChanged(() => SelectDay);
            }
        }

        public string Day
        {
            get
            {
                return _day;
            }

            set
            {
                _day = value;
                RaisePropertyChanged(() => Day);
            }
        }

        public string ClassHourString
        {
            get
            {
                return _classHourString;
            }

            set
            {
                _classHourString = value;
                RaisePropertyChanged(() => ClassHourString);
            }
        }

        public CreateRequiredStartingTimeModel()
        {
            ClassHours = new List<UIClassHour>();
            Days = new Dictionary<string, DayOfWeek>();
            Periods = new List<DayPeriodModel>();
        }

        public void SetValue(OperatorEnum opratorEnum, AdministrativeAlgoRuleEnum timeRule)
        {
            this.getBase(opratorEnum, timeRule);
        }

        public void SetValue(OperatorEnum opratorEnum, AdministrativeAlgoRuleEnum timeRule, UIClassHourRule rule)
        {
            this.getBase(opratorEnum, timeRule);

            this.bind(rule);
        }

        void save(object obj)
        {
            CreateRequiredStartingTime window = obj as CreateRequiredStartingTime;

            if (!this.Validate())
                return;

            if (this.OpratorEnum == OperatorEnum.Add)
            {
                if (this.SelectClassHour == null)
                    return;

                window.Add = new UIClassHourRule
                {
                    UID = Guid.NewGuid().ToString(),
                    IsActive = this.IsActive,
                    Period = this.SelectPeriod,
                    Day = this.SelectDay,
                    FirstID = this.SelectClassHour.ID,
                    Weight = this.Weight
                };
                window.Add.Period.Day = this.SelectDay;
            }
            else
            {
                window.Modify = new UIClassHourRule
                {
                    UID = Guid.NewGuid().ToString(),
                    IsActive = this.IsActive,
                    Period = this.SelectPeriod,
                    Day = this.SelectDay,
                    FirstID = this.SelectClassHour.ID,
                    Weight = this.Weight
                };
                window.Modify.Period.Day = this.SelectDay;
            }

            window.DialogResult = true;
        }

        void cancel(object obj)
        {
            CreateRequiredStartingTime window = obj as CreateRequiredStartingTime;
            window.Close();
        }

        void getBase(OperatorEnum opratorEnum, AdministrativeAlgoRuleEnum timeRule)
        {
            this._timeRule = timeRule;
            base.OpratorEnum = opratorEnum;
            this.TitleString = $"{ opratorEnum.GetLocalDescription()}-{timeRule.GetLocalDescription()}";
            this.RaisePropertyChanged(() => ShowRead);
            this.RaisePropertyChanged(() => ShowEdit);

            this.Days = new Dictionary<string, DayOfWeek>()
            {
                {"星期一",DayOfWeek.Monday},
                {"星期二",DayOfWeek.Tuesday},
                {"星期三",DayOfWeek.Wednesday},
                {"星期四",DayOfWeek.Thursday},
                {"星期五",DayOfWeek.Friday},
                {"星期六",DayOfWeek.Saturday},
                {"星期日",DayOfWeek.Sunday},
            };

            this.Day = "星期一";

            var cp = CommonDataManager.GetCPCase(base.LocalID);
            this.Periods = cp.GetDayPeriods();

            this.SelectPeriod = this.Periods.FirstOrDefault();

            Sources = cp.GetClassHours(cp.ClassHours?.Select(c => c.ID)?.ToArray());

            this.Search();
        }

        void bind(UIClassHourRule receive)
        {
            base.UID = receive.UID;

            this.Weight = receive.Weight;
            this.IsActive = receive.IsActive;
            this.Day = this.Days.First(k => k.Value == receive.Day).Key;
            this.SelectPeriod = receive.Period;
            this.ClassHourString = receive.FirstID.ToString();
        }

        public override void Search()
        {
            var source = this.Sources?.ToList();

            if (!string.IsNullOrEmpty(base.SelectTeacher?.ID))
            {
                source = source.Where(s => s.Teachers.Any(t => t.ID.Equals(base.SelectTeacher?.ID)))?.ToList();
            }

            if (!string.IsNullOrEmpty(base.SelectCourse?.ID))
            {
                source = source.Where(s => s.CourseID.Equals(base.SelectCourse?.ID))?.ToList();
            }

            if (!string.IsNullOrEmpty(base.SelectClass?.ID))
            {
                source = source.Where(s => s.ClassID.Equals(base.SelectClass?.ID))?.ToList();
            }

            ClassHours = source;
            this.SelectClassHour = ClassHours?.FirstOrDefault();
        }

        public void Initilize() { }

        public bool Validate()
        {
            if (this.SelectClassHour == null)
            {
                this.ShowDialog("提示信息", "选择课时为空!", DialogSettingType.NoButton);
                return false;
            }
            else
                return true;
        }
    }
}
