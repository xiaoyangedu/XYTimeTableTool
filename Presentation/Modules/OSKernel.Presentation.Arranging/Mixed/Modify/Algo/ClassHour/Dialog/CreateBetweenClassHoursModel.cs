using OSKernel.Presentation.Arranging.Mixed.Modify.Algo.ClassHour.Model;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.CustomControl.Enums;
using OSKernel.Presentation.Models.Enums;
using OSKernel.Presentation.Models.Mixed;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Algo.ClassHour.Dialog
{
    public class CreateBetweenClassHoursModel : BaseWindowModel, IInitilize
    {
        public int MaxDays { get; set; } = 6;

        public int MinDays { get; set; } = 1;

        public bool ShowMax
        {
            get
            {
                if (_timeRule == MixedAlgoRuleEnum.MaxDaysBetweenClassHours)
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
                if (_timeRule == MixedAlgoRuleEnum.MinDaysBetweenClassHours)
                    return true;
                else return false;
            }
        }

        private MixedAlgoRuleEnum _timeRule;

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

                if (_timeRule == MixedAlgoRuleEnum.MaxDaysBetweenClassHours)
                {
                    window.Add.MaxAndMinDayString = $" 最大天数:{window.Add.MaxDays}";
                }
                else if (_timeRule == MixedAlgoRuleEnum.MinDaysBetweenClassHours)
                {
                    window.Add.MaxAndMinDayString = $" 最小天数:{window.Add.MinDays}";
                }
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

                if (_timeRule == MixedAlgoRuleEnum.MaxDaysBetweenClassHours)
                {
                    window.Add.MaxAndMinDayString = $" 最大天数:{window.Add.MaxDays}";
                }
                else if (_timeRule == MixedAlgoRuleEnum.MinDaysBetweenClassHours)
                {
                    window.Add.MaxAndMinDayString = $" 最小天数:{window.Add.MinDays}";
                }
            }

            window.DialogResult = true;
        }

        void cancel(object obj)
        {
            CreateBetweenClassHours window = obj as CreateBetweenClassHours;
            window.Close();
        }

        void getBase(OperatorEnum opratorEnum, MixedAlgoRuleEnum timeRule)
        {
            base.OpratorEnum = opratorEnum;
            this._timeRule = timeRule;
            this.TitleString = $"{ opratorEnum.GetLocalDescription()}-{timeRule.GetLocalDescription()}";

            this.RaisePropertyChanged(() => ShowRead);
            this.RaisePropertyChanged(() => ShowEdit);
            this.RaisePropertyChanged(() => ShowMax);
            this.RaisePropertyChanged(() => ShowMin);

            var cl = base.GetClCase(base.LocalID);

            this.Sources = cl.GetClassHours(cl.ClassHours?.Select(ch => ch.ID)?.ToArray());

            this.Search();
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

        public void SetValue(OperatorEnum opratorEnum, MixedAlgoRuleEnum timeRule)
        {
            this.getBase(opratorEnum, timeRule);
        }

        public void SetValue(OperatorEnum opratorEnum, MixedAlgoRuleEnum timeRule, UIClassHourRule rule)
        {
            this.getBase(opratorEnum, timeRule);
            this.bind(rule);
        }

        public void AddClassHour(UIClassHour classHour)
        {
            if (this.TargetHours.Any(t => t.ID == classHour.ID))
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

            if (!string.IsNullOrEmpty(base.SelectMixedTeacher?.ID))
            {
                source = source.Where(s => s.Teachers.Any(t => t.ID.Equals(base.SelectMixedTeacher?.ID)))?.ToList();
            }

            if (!string.IsNullOrEmpty(base.SelectMixedCourse?.ID))
            {
                source = source.Where(s => s.CourseID.Equals(base.SelectMixedCourse?.ID))?.ToList();
            }

            if (!string.IsNullOrEmpty(base.SelectMixedClass?.ID))
            {
                source = source.Where(s => s.ClassID.Equals(base.SelectMixedClass?.ID))?.ToList();
            }

            if (!string.IsNullOrEmpty(base.SelectMixedTag?.ID))
            {
                source = source.Where(s =>
                {
                    if (s.Tags == null)
                    {
                        return false;
                    }
                    else
                    {
                        return s.Tags.Contains(base.SelectMixedTag?.ID);
                    }

                })?.ToList();
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
