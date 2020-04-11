using GalaSoft.MvvmLight.Messaging;
using OSKernel.Presentation.Arranging.Mixed.Modify.Algo.ClassHour.Dialog;
using OSKernel.Presentation.Arranging.Mixed.Modify.Algo.ClassHour.Model;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;
using XYKernel.OS.Common.Models.Mixed.AlgoRule;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Algo.ClassHour
{
    public class BetweenClassHoursViewModel : CommonViewModel, IInitilize
    {
        private bool _showMaxDay = true;
        private bool _showMinDay = true;

        private ObservableCollection<UIClassHourRule> _rules;

        private MixedAlgoRuleEnum currentRuleEnum;

        public ICommand CreateCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(createCommand);
            }
        }

        public ICommand ModifyCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(modifyCommand);
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(deleteCommand);
            }
        }

        public ICommand UserControlCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<string>(userControlCommand);
            }
        }

        public ObservableCollection<UIClassHourRule> Rules
        {
            get
            {
                return _rules;
            }

            set
            {
                _rules = value;
            }
        }

        /// <summary>
        /// 显示最大天数
        /// </summary>
        public bool ShowMaxDay
        {
            get
            {
                return _showMaxDay;
            }

            set
            {
                _showMaxDay = value;
                RaisePropertyChanged(() => ShowMaxDay);
            }
        }

        /// <summary>
        /// 显示最小天数
        /// </summary>
        public bool ShowMinDay
        {
            get
            {
                return _showMinDay;
            }

            set
            {
                _showMinDay = value;
                RaisePropertyChanged(() => ShowMinDay);
            }
        }

        public BetweenClassHoursViewModel()
        {
            this.Rules = new ObservableCollection<UIClassHourRule>();
        }

        /// <summary>
        /// 当前显示界面：规则或高级
        /// </summary>
        public MixedAlgoRuleEnum GetCurrentRuleEnum()
        {
            return currentRuleEnum;
        }

        /// <summary>
        /// 当前显示界面：规则或高级
        /// </summary>
        public void SetCurrentRuleEnum(MixedAlgoRuleEnum value)
        {
            currentRuleEnum = value;

            this.Comments = CommonDataManager.GetMixedAlgoComments(value);


            if (currentRuleEnum == MixedAlgoRuleEnum.MinDaysBetweenClassHours)
            {
                this.ShowMaxDay = true;
            }
            else if (currentRuleEnum == MixedAlgoRuleEnum.MaxDaysBetweenClassHours)
            {
                this.ShowMinDay = true;
            }
        }

        void createCommand()
        {
            CreateBetweenClassHours createWin = new CreateBetweenClassHours(Models.Enums.OperatorEnum.Add, this.GetCurrentRuleEnum());
            createWin.Closed += (s, arg) =>
            {
                if (createWin.DialogResult.Value)
                {
                    // 1.向UI中插入
                    var last = this.Rules.LastOrDefault();
                    var index = last == null ? 0 : Convert.ToInt32(last.NO);

                    // 2.创建对象
                    createWin.Add.NO = index + 1;

                    // 3.更新界面
                    this.Rules.Add(createWin.Add);
                }
            };
            createWin.ShowDialog();
        }

        void modifyCommand(object obj)
        {
            UIClassHourRule rule = obj as UIClassHourRule;

            CreateBetweenClassHours createWin = new CreateBetweenClassHours(Models.Enums.OperatorEnum.Modify, this.GetCurrentRuleEnum(), rule);
            createWin.Closed += (s, arg) =>
            {
                var modify = createWin.Modify;
                if (createWin.DialogResult.Value)
                {
                    rule.IsActive = modify.IsActive;
                    rule.Weight = modify.Weight;
                    rule.ClassHours = modify.ClassHours;
                    rule.MaxDays = modify.MaxDays;
                    rule.MinDays = modify.MinDays;
                    rule.RaiseChanged();
                }
            };
            createWin.ShowDialog();
        }

        void deleteCommand(object obj)
        {
            UIClassHourRule rule = obj as UIClassHourRule;

            var result = this.ShowDialog("提示信息", "确认删除?", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
            if (result == CustomControl.Enums.DialogResultType.OK)
            {
                this.Rules.Remove(rule);
            }
        }

        void userControlCommand(string parms)
        {
            if (parms.Equals("loaded"))
            {

            }
            else if (parms.Equals("unloaded"))
            {
                Messenger.Default.Unregister<HostView>(this, save);
            }
        }

        [InjectionMethod]
        public void Initilize()
        {
            Messenger.Default.Register<HostView>(this, save);
        }

        public void BindData(MixedAlgoRuleEnum ruleEnum)
        {
            this.SetCurrentRuleEnum(ruleEnum);

            var rule = base.GetCLAlgoRule(base.LocalID);
            if (rule == null)
                return;

            var cl = base.GetClCase(base.LocalID);

            int no = 0;
            if (this.GetCurrentRuleEnum() == MixedAlgoRuleEnum.MinDaysBetweenClassHours)
            {
                rule.MinDaysBetweenClassHours.ForEach(t =>
                {
                    UIClassHourRule ui = new UIClassHourRule();
                    ui.UID = t.UID;
                    ui.NO = ++no;
                    ui.IsActive = t.Active;
                    ui.Weight = t.Weight;
                    ui.MinDays = t.MinDays;
                    ui.ClassHours = cl.GetClassHours(t.ID);
                    ui.MaxAndMinDayString = $" 最小天数:{t.MinDays}";
                    this.Rules.Add(ui);
                });

                this.ShowMaxDay = false;
            }
            else if (this.GetCurrentRuleEnum() == MixedAlgoRuleEnum.MaxDaysBetweenClassHours)
            {
                rule.MaxDaysBetweenClassHours?.ForEach(t =>
                {
                    UIClassHourRule ui = new UIClassHourRule();
                    ui.UID = t.UID;
                    ui.NO = ++no;
                    ui.IsActive = t.Active;
                    ui.Weight = t.Weight;
                    ui.MaxDays = t.MaxDays;
                    ui.ClassHours = cl.GetClassHours(t.ID);
                    ui.MaxAndMinDayString = $" 最大天数:{t.MaxDays}";
                    this.Rules.Add(ui);
                });

                this.ShowMinDay = false;
            }
        }

        void save(HostView host)
        {
            var rule = base.GetCLAlgoRule(base.LocalID);

            if (this.GetCurrentRuleEnum() == MixedAlgoRuleEnum.MinDaysBetweenClassHours)
            {
                rule.MinDaysBetweenClassHours.Clear();
                foreach (var r in Rules)
                {
                    var model = new MinDaysBetweenClassHoursRule()
                    {
                        Active = r.IsActive,
                        Weight = r.Weight,
                        ID = r.ClassHours?.Select(ch => ch.ID)?.ToArray(),
                        MinDays = r.MinDays,
                        UID = r.UID,
                    };
                    rule.MinDaysBetweenClassHours.Add(model);
                }
            }
            else if (this.GetCurrentRuleEnum() == MixedAlgoRuleEnum.MaxDaysBetweenClassHours)
            {
                rule.MaxDaysBetweenClassHours?.Clear();
                foreach (var r in Rules)
                {
                    var model = new MaxDaysBetweenClassHoursRule()
                    {
                        Active = r.IsActive,
                        Weight = r.Weight,
                        ID = r.ClassHours?.Select(ch => ch.ID)?.ToArray(),
                        MaxDays = r.MaxDays,
                        UID = r.UID,
                    };
                    rule.MaxDaysBetweenClassHours?.Add(model);
                }
            }

            base.SerializePatternAlgo(rule, base.LocalID);
            this.ShowDialog("提示信息", "保存成功", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
        }
    }
}
