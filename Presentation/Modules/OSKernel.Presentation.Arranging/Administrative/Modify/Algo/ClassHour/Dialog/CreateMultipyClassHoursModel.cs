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

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Algo.ClassHour.Dialog
{
    public class CreateMultipyClassHoursModel : BaseWindowModel, IInitilize
    {
        private AdministrativeAlgoRuleEnum _timeRule;

        private List<UIClassHour> _firstHours;

        private List<UIClassHour> _secondHours;

        private List<UIClassHour> _thirdHours;

        private UIClassHour _selectFirstHour;

        private UIClassHour _selectSecondHour;

        private UIClassHour _selectThirdHour;

        private bool _showThirdHour;

        public List<UIClassHour> FirstHours
        {
            get
            {
                return _firstHours;
            }

            set
            {
                _firstHours = value;
                RaisePropertyChanged(() => FirstHours);
            }
        }

        public List<UIClassHour> SecondHours
        {
            get
            {
                return _secondHours;
            }

            set
            {
                _secondHours = value;
                RaisePropertyChanged(() => SecondHours);
            }
        }

        public List<UIClassHour> ThirdHours
        {
            get
            {
                return _thirdHours;
            }

            set
            {
                _thirdHours = value;
                RaisePropertyChanged(() => ThirdHours);
            }
        }

        public List<UIClassHour> Sources { get; set; }

        public UIClassHour SelectFirstHour
        {
            get
            {
                return _selectFirstHour;
            }

            set
            {
                _selectFirstHour = value;
                RaisePropertyChanged(() => SelectFirstHour);
            }
        }

        public UIClassHour SelectSecondHour
        {
            get
            {
                return _selectSecondHour;
            }

            set
            {
                _selectSecondHour = value;
                RaisePropertyChanged(() => SelectSecondHour);
            }
        }

        public UIClassHour SelectThirdHour
        {
            get
            {
                return _selectThirdHour;
            }

            set
            {
                _selectThirdHour = value;
                RaisePropertyChanged(() => SelectThirdHour);
            }
        }

        public bool ShowThirdHour
        {
            get
            {
                return _showThirdHour;
            }

            set
            {
                _showThirdHour = value;
                RaisePropertyChanged(() => ShowThirdHour);
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

        public CreateMultipyClassHoursModel()
        {
            FirstHours = new List<UIClassHour>();
            SecondHours = new List<UIClassHour>();
            ThirdHours = new List<UIClassHour>();
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
            CreateMultipyClassHours window = obj as CreateMultipyClassHours;

            if (!this.Validate())
                return;

            if (this.OpratorEnum == OperatorEnum.Add)
            {
                window.Add = new UIClassHourRule
                {
                    UID = Guid.NewGuid().ToString(),
                    IsActive = this.IsActive,
                    Weight = this.Weight,
                    FirstID = this.SelectFirstHour == null ? 0 : this.SelectFirstHour.ID,
                    SecondID = this.SelectSecondHour == null ? 0 : this.SelectSecondHour.ID,
                    ThirdID = this.SelectThirdHour == null ? 0 : this.SelectThirdHour.ID,
                };

                List<UIClassHour> classhours = new List<UIClassHour>();
                if (this.SelectFirstHour != null)
                    classhours.Add(this.SelectFirstHour);

                if (this.SelectSecondHour != null)
                    classhours.Add(this.SelectSecondHour);

                if (this.SelectThirdHour != null)
                    classhours.Add(this.SelectThirdHour);

                window.Add.ClassHours = classhours;
            }
            else
            {
                window.Modify = new UIClassHourRule
                {
                    UID = this.UID,
                    Weight = this.Weight,
                    IsActive = this.IsActive,
                    FirstID = this.SelectFirstHour == null ? 0 : this.SelectFirstHour.ID,
                    SecondID = this.SelectSecondHour == null ? 0 : this.SelectSecondHour.ID,
                    ThirdID = this.SelectThirdHour == null ? 0 : this.SelectThirdHour.ID,
                };

                List<UIClassHour> classhours = new List<UIClassHour>();
                if (this.SelectFirstHour != null)
                    classhours.Add(this.SelectFirstHour);

                if (this.SelectSecondHour != null)
                    classhours.Add(this.SelectSecondHour);

                if (this.SelectThirdHour != null)
                    classhours.Add(this.SelectThirdHour);

                window.Modify.ClassHours = classhours;
            }

            window.DialogResult = true;
        }

        void cancel(object obj)
        {
            CreateMultipyClassHours window = obj as CreateMultipyClassHours;
            window.Close();
        }

        void getBase(OperatorEnum opratorEnum, AdministrativeAlgoRuleEnum timeRule)
        {
            base.OpratorEnum = opratorEnum;
            this._timeRule = timeRule;
            this.TitleString = $"{ opratorEnum.GetLocalDescription()}-{timeRule.GetLocalDescription()}";
            this.RaisePropertyChanged(() => ShowRead);
            this.RaisePropertyChanged(() => ShowEdit);

            var cp = CommonDataManager.GetCPCase(base.LocalID);

            if (timeRule == AdministrativeAlgoRuleEnum.ThreeClassHoursGrouped)
            {
                ShowThirdHour = true;
            }

            this.Sources = cp.GetClassHours(cp.ClassHours?.Select(ch => ch.ID)?.ToArray());

            this.Search();
        }

        void bind(UIClassHourRule receive)
        {
            this.Weight = receive.Weight;
            this.IsActive = receive.IsActive;
            base.UID = receive.UID;

            this.SelectFirstHour = this.FirstHours?.FirstOrDefault(f => f.ID.Equals(receive.FirstID));
            this.SelectSecondHour = this.FirstHours?.FirstOrDefault(f => f.ID.Equals(receive.SecondID));

            if (_timeRule == AdministrativeAlgoRuleEnum.ThreeClassHoursGrouped)
            {
                this.SelectThirdHour = this.FirstHours?.FirstOrDefault(f => f.ID.Equals(receive.ThirdID));
            }
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

            FirstHours = source;
            SecondHours = source;
            ThirdHours = source;

            SelectFirstHour = FirstHours?.FirstOrDefault();
            SelectSecondHour = SecondHours?.FirstOrDefault();

            if (_timeRule == AdministrativeAlgoRuleEnum.ThreeClassHoursGrouped)
            {
                this.SelectThirdHour = SelectThirdHour = ThirdHours?.FirstOrDefault();
            }

        }

        public void Initilize()
        {

        }

        public bool Validate()
        {
            if (_timeRule == AdministrativeAlgoRuleEnum.ThreeClassHoursGrouped)
            {
                if ((SelectFirstHour?.ID != SelectSecondHour?.ID)
                    && (SelectSecondHour?.ID != SelectThirdHour?.ID)
                    && (SelectFirstHour?.ID != SelectThirdHour?.ID))
                {
                    return true;
                }
                else
                {
                    this.ShowDialog("提示信息", "选择课时不能相同!", DialogSettingType.NoButton);
                    return false;
                }
            }
            else if (_timeRule == AdministrativeAlgoRuleEnum.TwoClassHoursContinuous
                || _timeRule == AdministrativeAlgoRuleEnum.TwoClassHoursGrouped
                || _timeRule == AdministrativeAlgoRuleEnum.TwoClassHoursOrdered)
            {
                if (SelectFirstHour?.ID != SelectSecondHour?.ID)
                {
                    return true;
                }
                else
                {
                    this.ShowDialog("提示信息", "选择课时不能相同!", DialogSettingType.NoButton);
                    return false;
                }
            }

            return true;
        }
    }
}
