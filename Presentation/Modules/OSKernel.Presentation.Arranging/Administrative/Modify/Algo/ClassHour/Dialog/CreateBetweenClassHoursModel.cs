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
    public class CreateBetweenClassHoursModel : BaseWindowModel, IInitilize
    {
        //private string _teacher = string.Empty;
        //private string _subject = string.Empty;
        //private string _student = string.Empty;

        public int MaxDays { get; set; } = 6;

        public int MinDays { get; set; } = 1;

        public bool ShowMax
        {
            get
            {
                if (_timeRule == AdministrativeAlgoRuleEnum.MaxDaysBetweenClassHours)
                {
                    return true;
                }
                else return false;
            }
        }

        public bool ShowMin
        {
            get
            {
                if (_timeRule == AdministrativeAlgoRuleEnum.MinDaysBetweenClassHours)
                    return true;
                else return false;
            }
        }

        //public string Teacher
        //{
        //    get
        //    {
        //        return _teacher;
        //    }

        //    set
        //    {
        //        _teacher = value;
        //        RaisePropertyChanged(() => Teacher);

        //        this.Search();
        //    }
        //}

        //public string Subject
        //{
        //    get
        //    {
        //        return _subject;
        //    }

        //    set
        //    {
        //        _subject = value;
        //        RaisePropertyChanged(() => Subject);

        //        this.Search();
        //    }
        //}

        //public string Student
        //{
        //    get
        //    {
        //        return _student;
        //    }

        //    set
        //    {
        //        _student = value;
        //        RaisePropertyChanged(() => Student);
        //        this.Search();
        //    }
        //}

        private AdministrativeAlgoRuleEnum _timeRule;

        private ObservableCollection<UIClassHour> _classHours;

        private ObservableCollection<UIClassHour> _targetHours;

        public List<UIClassHour> Sources { get; set; }

        public ObservableCollection<UIClassHour> ClassHours
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

        public ObservableCollection<UIClassHour> TargetHours
        {
            get
            {
                return _targetHours;
            }

            set
            {
                _targetHours = value;
                RaisePropertyChanged(() => TargetHours);
            }
        }

        public ICommand SourceCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(sourceCommand);
            }
        }

        public ICommand TargetCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(targetCommand);
            }
        }

        public ICommand ClearAllListCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(clearAllListCommand);
            }
        }

        public ICommand SelectedAllListCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(selectAllListCommand);
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

        public CreateBetweenClassHoursModel()
        {
            ClassHours = new ObservableCollection<UIClassHour>();
            TargetHours = new ObservableCollection<UIClassHour>();
        }

        void sourceCommand(object selectItem)
        {
            var model = selectItem as UIClassHour;
            if (model != null)
                this.AddClassHour(model);
        }

        void targetCommand(object selectItem)
        {
            var model = selectItem as UIClassHour;
            if (model != null)
                this.RemoveClassHour(model);
        }

        void clearAllListCommand()
        {
            this.TargetHours.ToList()?.ForEach((removeItem) =>
            {
                this.RemoveClassHour(removeItem);
            });
        }

        void selectAllListCommand()
        {
            this.ClassHours.ToList()?.ForEach((removeItem) =>
            {
                this.AddClassHour(removeItem);
            });
        }

        void save(object obj)
        {
            CreateBetweenClassHours window = obj as CreateBetweenClassHours;

            if (!this.Validate())
                return;

            if (this.OpratorEnum == OperatorEnum.Add)
            {
                window.Add = new UIClassHourRule
                {
                    UID = Guid.NewGuid().ToString(),
                    IsActive = this.IsActive,
                    ClassHours = this.TargetHours?.ToList(),
                    Weight = this.Weight,
                    MaxDays = this.MaxDays,
                    MinDays = this.MinDays,
                };
            }
            else
            {
                window.Modify = new UIClassHourRule
                {
                    UID = Guid.NewGuid().ToString(),
                    IsActive = this.IsActive,
                    ClassHours = this.TargetHours?.ToList(),
                    Weight = this.Weight,
                    MaxDays = this.MaxDays,
                    MinDays = this.MinDays,
                };
            }

            window.DialogResult = true;
        }

        void cancel(object obj)
        {
            CreateBetweenClassHours window = obj as CreateBetweenClassHours;
            window.Close();
        }

        void getBase(OperatorEnum opratorEnum, AdministrativeAlgoRuleEnum timeRule)
        {
            base.OpratorEnum = opratorEnum;
            this._timeRule = timeRule;
            this.TitleString = $"{ opratorEnum.GetLocalDescription()}-{timeRule.GetLocalDescription()}";

            this.RaisePropertyChanged(() => ShowRead);
            this.RaisePropertyChanged(() => ShowEdit);
            this.RaisePropertyChanged(() => ShowMax);
            this.RaisePropertyChanged(() => ShowMin);

            var cp = CommonDataManager.GetCPCase(base.LocalID);

            this.Sources = cp.GetClassHours(cp.ClassHours?.Select(ch => ch.ID)?.ToArray());
            this.ClassHours = new ObservableCollection<UIClassHour>(this.Sources);
        }

        void bind(UIClassHourRule receive)
        {
            this.Weight = receive.Weight;
            this.IsActive = receive.IsActive;
            this.MaxDays = receive.MaxDays;
            this.MinDays = receive.MinDays;
            this.UID = receive.UID;

            if (receive.ClassHours != null)
            {
                this.TargetHours = new ObservableCollection<UIClassHour>(receive.ClassHours);
            }
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

        public void AddClassHour(UIClassHour classHour)
        {
            if (this.TargetHours.Any(t=>t.ID== classHour.ID))
            {
                return;
            }
            else
            {
                this.TargetHours.Add(classHour);
            }
        }

        public void RemoveClassHour(UIClassHour classHour)
        {
            if (this.TargetHours.Any(t => t.ID == classHour.ID))
            {
                this.TargetHours.Remove(classHour);
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

            if (source != null)
            {
                ClassHours = new ObservableCollection<UIClassHour>(source);
            }
        }

        public void Initilize()
        {

        }

        public bool Validate()
        {
            if (this.TargetHours?.Count == 0)
            {
                this.ShowDialog("提示信息", "选择课时列表为空!", DialogSettingType.NoButton);
                return false;
            }

            return true;
        }
    }
}
