using GalaSoft.MvvmLight.Messaging;
using OSKernel.Presentation.Arranging.Administrative.Modify.Algo.ClassHour.Dialog;
using OSKernel.Presentation.Arranging.Administrative.Modify.Algo.ClassHour.Model;
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
using XYKernel.OS.Common.Models.Administrative.AlgoRule;

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Algo.ClassHour
{
    public class ClassHoursTimesValueModel : CommonViewModel, IInitilize
    {
        private ObservableCollection<UIClassHourRule> _rules;

        private AdministrativeAlgoRuleEnum currentRuleEnum;

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

        public ClassHoursTimesValueModel()
        {
            this.Rules = new ObservableCollection<UIClassHourRule>();
        }

        /// <summary>
        /// 当前显示界面：规则或高级
        /// </summary>
        public AdministrativeAlgoRuleEnum GetCurrentRuleEnum()
        {
            return currentRuleEnum;
        }

        /// <summary>
        /// 当前显示界面：规则或高级
        /// </summary>
        public void SetCurrentRuleEnum(AdministrativeAlgoRuleEnum value)
        {
            currentRuleEnum = value;
            this.Comments = CommonDataManager.GetAdminAlgoComments(value);
        }

        void createCommand()
        {
            CreateClassHoursTimesValue createWin = new CreateClassHoursTimesValue(Models.Enums.OperatorEnum.Add, this.GetCurrentRuleEnum());
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

            CreateClassHoursTimesValue createWin = new CreateClassHoursTimesValue(Models.Enums.OperatorEnum.Modify, this.GetCurrentRuleEnum(), rule);
            createWin.Closed += (s, arg) =>
            {
                var modify = createWin.Modify;
                if (createWin.DialogResult.Value)
                {
                    rule.IsActive = modify.IsActive;
                    rule.Weight = modify.Weight;
                    rule.Periods = modify.Periods;
                    rule.MaxConcurrency = modify.MaxConcurrency;
                    rule.MaxOccupy = modify.MaxOccupy;
                    rule.ClassHours = modify.ClassHours;
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

        public void BindData(AdministrativeAlgoRuleEnum ruleEnum)
        {
            this.SetCurrentRuleEnum(ruleEnum);

            var rule = CommonDataManager.GetAminAlgoRule(base.LocalID);
            if (rule == null)
                return;

            var cp = CommonDataManager.GetCPCase(base.LocalID);
            int no = 0;
            if (this.GetCurrentRuleEnum() == AdministrativeAlgoRuleEnum.ClassHoursMaxConcurrencyInSelectedTime)
            {
                rule.ClassHoursMaxConcurrencyInSelectedTimes?.ForEach(t =>
                {
                    UIClassHourRule ui = new UIClassHourRule();
                    ui.NO = ++no;
                    ui.UID = t.UID;
                    ui.IsActive = t.Active;
                    ui.ClassHours = cp.GetClassHours(t.ID);
                    ui.MaxConcurrency = t.MaxConcurrencyNumber;
                    ui.Periods = t.Times;
                    this.Rules.Add(ui);
                });
            }
            else if (this.GetCurrentRuleEnum() == AdministrativeAlgoRuleEnum.ClassHoursOccupyMaxTimeFromSelection)
            {
                rule.ClassHoursOccupyMaxTimeFromSelections?.ForEach(t =>
                {
                    UIClassHourRule ui = new UIClassHourRule();
                    ui.NO = ++no;
                    ui.UID = t.UID;
                    ui.IsActive = t.Active;
                    ui.ClassHours = cp.GetClassHours(t.ID);
                    ui.MaxOccupy = t.MaxOccupyNumber;
                    ui.Periods = t.Times;
                    this.Rules.Add(ui);
                });
            }
        }

        void save(HostView host)
        {
            var rule = CommonDataManager.GetAminAlgoRule(base.LocalID);

            if (this.GetCurrentRuleEnum() == AdministrativeAlgoRuleEnum.ClassHoursMaxConcurrencyInSelectedTime)
            {
                rule.ClassHoursMaxConcurrencyInSelectedTimes.Clear();
                foreach (var r in Rules)
                {
                    var model = new ClassHoursMaxConcurrencyInSelectedTimeRule
                    {
                        UID = r.UID,
                        Active = r.IsActive,
                        ID = r.ClassHours?.Select(ch => ch.ID)?.ToArray(),
                        MaxConcurrencyNumber = r.MaxConcurrency,
                        Times = r.Periods,
                    };
                    rule.ClassHoursMaxConcurrencyInSelectedTimes.Add(model);
                }
            }

            else if (this.GetCurrentRuleEnum() == AdministrativeAlgoRuleEnum.ClassHoursOccupyMaxTimeFromSelection)
            {
                rule.ClassHoursOccupyMaxTimeFromSelections?.Clear();
                foreach (var r in Rules)
                {
                    var model = new ClassHoursOccupyMaxTimeFromSelectionRule()
                    {
                        UID = r.UID,
                        Times = r.Periods,
                        Active = r.IsActive,
                        ID = r.ClassHours?.Select(ch => ch.ID)?.ToArray(),
                        MaxOccupyNumber = r.MaxOccupy,
                    };
                    rule.ClassHoursOccupyMaxTimeFromSelections?.Add(model);
                }
            }

            rule.Serialize(base.LocalID);
            this.ShowDialog("提示信息", "保存成功", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
        }
    }
}
