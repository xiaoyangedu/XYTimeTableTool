using GalaSoft.MvvmLight.Messaging;
using OSKernel.Presentation.Arranging.Administrative.Modify.Algo.Teacher.Model;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Base;
using OSKernel.Presentation.Models.Enums;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;
using XYKernel.OS.Common.Models.Administrative.AlgoRule;

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Algo.Teacher.Dialog
{
    public class CreateMaxHoursDailyModel : BaseWindowModel,IInitilize
    {
        private int _maxHours = 1;

        private string _maxTips;

        private AdministrativeAlgoRuleEnum _timeRule;

        private List<UITeacher> _teachers;

        private UITeacher _selectTeacher;

        public List<UITeacher> Teachers
        {
            get
            {
                return _teachers;
            }

            set
            {
                _teachers = value;
                RaisePropertyChanged(() => Teachers);
            }
        }

        public int MaxHours
        {
            get
            {
                return _maxHours;
            }

            set
            {
                if (value < 1)
                    value = 1;
                else if (value > MaxHour)
                    value = MaxHour;

                _maxHours = value;
                RaisePropertyChanged(() => MaxHours);
            }
        }

        public string MaxTips
        {
            get
            {
                return _maxTips;
            }

            set
            {
                _maxTips = value;
                RaisePropertyChanged(() => MaxTips);
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

        /// <summary>
        /// 选中教师
        /// </summary>
        public UITeacher SelectTeacher
        {
            get
            {
                return _selectTeacher;
            }

            set
            {
                _selectTeacher = value;
                RaisePropertyChanged(() => SelectTeacher);
            }
        }

        public CreateMaxHoursDailyModel()
        {
            Teachers = new List<UITeacher>();
        }

        public void SetValue(OperatorEnum opratorEnum, AdministrativeAlgoRuleEnum timeRule)
        {
            this.getBase(opratorEnum, timeRule);
        }

        public void SetValue(OperatorEnum opratorEnum, AdministrativeAlgoRuleEnum timeRule, UITeacherRule rule)
        {
            this.getBase(opratorEnum, timeRule);

            this.bind(rule);
        }

        void save(object obj)
        {
            CreateMaxHoursDaily window = obj as CreateMaxHoursDaily;

            if (this.OpratorEnum == OperatorEnum.Add)
            {
                if (this.SelectTeacher == null) return;

                window.Add = new Model.UITeacherRule()
                {
                    IsActive = this.IsActive,
                    UID = this.UID,
                    TeacherID = this.SelectTeacher.ID,
                    Name = this.SelectTeacher.Name,
                    Value = this.MaxHours,
                    Weight = this.Weight
                };
            }
            else
            {
                window.Modify = new UITeacherRule()
                {
                    IsActive = this.IsActive,
                    Value = this.MaxHours,
                    Weight = this.Weight
                };
            }

            window.DialogResult = true;
        }

        void cancel(object obj)
        {
            CreateMaxHoursDaily window = obj as CreateMaxHoursDaily;
            window.Close();
        }

        void getBase(OperatorEnum opratorEnum, AdministrativeAlgoRuleEnum timeRule)
        {
            base.OpratorEnum = opratorEnum;
            this._timeRule = timeRule;
            this.TitleString = $"{ opratorEnum.GetLocalDescription()}-{timeRule.GetLocalDescription()}";
            this.RaisePropertyChanged(() => ShowRead);
            this.RaisePropertyChanged(() => ShowEdit);

            if (timeRule == AdministrativeAlgoRuleEnum.TeachersMaxHoursDaily)
            {
                this.Teachers = new List<UITeacher>() { new UITeacher
                {
                     ID=string.Empty,
                      Name="所有教师"
                } };

                this.SelectTeacher = this.Teachers.First();
            }
            else
            {
                var cp = CommonDataManager.GetCPCase(base.LocalID);

                this.Teachers = cp.Teachers.Select(t =>
                  {
                      return new UITeacher()
                      {
                          ID = t.ID,
                          Name = t.Name
                      };
                  })?.ToList();
            }

            this.SelectTeacher = this.Teachers.FirstOrDefault();

        }

        void bind(UITeacherRule receive)
        {
            this.UID = receive.UID;
            this.Weight = receive.Weight;
            this.IsActive = receive.IsActive;
            this.SelectTeacher = this.Teachers.FirstOrDefault(t => t.ID.Equals(receive.TeacherID));
            this.MaxHours = receive.Value;
        }

        [InjectionMethod]
        public void Initilize()
        {
            this.MaxTips = $"(1-{MaxHour})";
        }
    }
}
