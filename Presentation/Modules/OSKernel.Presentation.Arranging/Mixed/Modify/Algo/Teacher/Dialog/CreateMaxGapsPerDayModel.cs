using GalaSoft.MvvmLight.Messaging;
using OSKernel.Presentation.Arranging.Mixed.Modify.Algo.Teacher.Model;
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
using XYKernel.OS.Common.Models.Mixed.AlgoRule;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Algo.Teacher.Dialog
{
    public class CreateMaxGapsPerDayModel : BaseWindowModel, IInitilize
    {
        private int _maxGaps = 0;

        private string _maxTips;

        private MixedAlgoRuleEnum _timeRule;

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

        public int MaxGaps
        {
            get
            {
                return _maxGaps;
            }

            set
            {
                if (value < 0)
                    value = 0;
                else if (value > MaxHour)
                    value = MaxHour;

                _maxGaps = value;
                RaisePropertyChanged(() => MaxGaps);
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

        public CreateMaxGapsPerDayModel()
        {
            Teachers = new List<UITeacher>();
        }

        public void SetValue(OperatorEnum opratorEnum, MixedAlgoRuleEnum timeRule)
        {
            this.getBase(opratorEnum, timeRule);
        }

        public void SetValue(OperatorEnum opratorEnum, MixedAlgoRuleEnum timeRule, UITeacherRule rule)
        {
            this.getBase(opratorEnum, timeRule);

            this.bind(rule);
        }

        void save(object obj)
        {
            CreateMaxGapsPerDay window = obj as CreateMaxGapsPerDay;

            if (this.OpratorEnum == OperatorEnum.Add)
            {
                if (this.SelectTeacher == null) return;

                window.Add = new Model.UITeacherRule()
                {
                    IsActive = this.IsActive,
                    UID = this.UID,
                    TeacherID = this.SelectTeacher.ID,
                    Name = this.SelectTeacher.Name,
                    Value = this.MaxGaps,
                    Weight = this.Weight
                };
            }
            else
            {
                window.Modify = new UITeacherRule()
                {
                    IsActive = this.IsActive,
                    Value = this.MaxGaps,
                    Weight = this.Weight
                };
            }

            window.DialogResult = true;
        }

        void cancel(object obj)
        {
            CreateMaxGapsPerDay window = obj as CreateMaxGapsPerDay;
            window.Close();
        }

        void getBase(OperatorEnum opratorEnum, MixedAlgoRuleEnum timeRule)
        {
            base.OpratorEnum = opratorEnum;
            this._timeRule = timeRule;
            this.TitleString = $"{ opratorEnum.GetLocalDescription()}-{timeRule.GetLocalDescription()}";
            this.RaisePropertyChanged(() => ShowRead);
            this.RaisePropertyChanged(() => ShowEdit);

            //this.MaxTips = $"(1-{CommonDataManager.Hours})";

            if (timeRule == MixedAlgoRuleEnum.TeachersMaxGapsPerDay)
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
                var cl = base.GetClCase(base.LocalID);

                this.Teachers = cl.Teachers.Select(t =>
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
            this.MaxGaps = receive.Value;
        }

        [InjectionMethod]
        public void Initilize()
        {
            this.MaxTips = $"(0-{MaxHour})";
        }
    }
}
