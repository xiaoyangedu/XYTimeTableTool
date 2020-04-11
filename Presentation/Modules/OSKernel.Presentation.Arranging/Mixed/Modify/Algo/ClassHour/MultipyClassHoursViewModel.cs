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
    public class MultipyClassHoursViewModel : CommonViewModel, IInitilize
    {
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

        public MultipyClassHoursViewModel()
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
        }

        void createCommand()
        {
            CreateMultipyClassHours createWin = new CreateMultipyClassHours(Models.Enums.OperatorEnum.Add, this.GetCurrentRuleEnum());
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

            CreateMultipyClassHours createWin = new CreateMultipyClassHours(Models.Enums.OperatorEnum.Modify, this.GetCurrentRuleEnum(), rule);
            createWin.Closed += (s, arg) =>
            {
                var modify = createWin.Modify;
                if (createWin.DialogResult.Value)
                {
                    rule.IsActive = modify.IsActive;
                    rule.Weight = modify.Weight;
                    rule.ClassHours = modify.ClassHours;
                    rule.FirstID = modify.FirstID;
                    rule.SecondID = modify.SecondID;
                    rule.ThirdID = modify.ThirdID;
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

            var cl = base.GetClCase(base.LocalID);

            int no = 0;
            if (this.GetCurrentRuleEnum() == MixedAlgoRuleEnum.ThreeClassHoursGrouped)
            {
                rule.ThreeClassHoursGrouped.ForEach(t =>
                {
                    UIClassHourRule ui = new UIClassHourRule();
                    ui.NO = ++no;
                    ui.FirstID = t.FirstID;
                    ui.SecondID = t.SecondID;
                    ui.ThirdID = t.ThirdID;
                    ui.IsActive = t.Active;
                    ui.Weight = t.Weight;

                    var hours = new List<int>() { t.FirstID, t.SecondID, t.ThirdID };
                    ui.ClassHours = cl.GetClassHours(hours.ToArray());

                    this.Rules.Add(ui);
                });
            }
            else if (this.GetCurrentRuleEnum() == MixedAlgoRuleEnum.TwoClassHoursContinuous)
            {
                rule.TwoClassHoursContinuous?.ForEach(t =>
                {
                    UIClassHourRule ui = new UIClassHourRule();
                    ui.NO = ++no;
                    ui.FirstID = t.FirstID;
                    ui.SecondID = t.SecondID;
                    ui.IsActive = t.Active;
                    ui.Weight = t.Weight;

                    var hours = new List<int>() { t.FirstID, t.SecondID };
                    ui.ClassHours = cl.GetClassHours(hours.ToArray());

                    this.Rules.Add(ui);
                });
            }
            else if (this.GetCurrentRuleEnum() == MixedAlgoRuleEnum.TwoClassHoursGrouped)
            {
                rule.TwoClassHoursGrouped?.ForEach(t =>
                {
                    UIClassHourRule ui = new UIClassHourRule();
                    ui.NO = ++no;
                    ui.FirstID = t.FirstID;
                    ui.SecondID = t.SecondID;
                    ui.IsActive = t.Active;
                    ui.Weight = t.Weight;

                    var hours = new List<int>() { t.FirstID, t.SecondID };
                    ui.ClassHours = cl.GetClassHours(hours.ToArray());

                    this.Rules.Add(ui);
                });
            }
            else if (this.GetCurrentRuleEnum() == MixedAlgoRuleEnum.TwoClassHoursOrdered)
            {
                rule.TwoClassHoursOrdered?.ForEach(t =>
                {
                    UIClassHourRule ui = new UIClassHourRule();
                    ui.NO = ++no;
                    ui.FirstID = t.FirstID;
                    ui.SecondID = t.SecondID;
                    ui.IsActive = t.Active;
                    ui.Weight = t.Weight;

                    var hours = new List<int>() { t.FirstID, t.SecondID };
                    ui.ClassHours = cl.GetClassHours(hours.ToArray());

                    this.Rules.Add(ui);
                });
            }
        }

        void save(HostView host)
        {
            var rule = base.GetCLAlgoRule(base.LocalID);

            if (this.GetCurrentRuleEnum() == MixedAlgoRuleEnum.ThreeClassHoursGrouped)
            {
                rule.ThreeClassHoursGrouped.Clear();
                foreach (var r in Rules)
                {
                    var model = new ThreeClassHoursGroupedRule()
                    {
                        UID = r.UID,
                        Active = r.IsActive,
                        Weight = r.Weight,
                        FirstID = r.FirstID,
                        SecondID = r.SecondID,
                        ThirdID = r.ThirdID
                    };
                    rule.ThreeClassHoursGrouped.Add(model);
                }
            }
            else if (this.GetCurrentRuleEnum() == MixedAlgoRuleEnum.TwoClassHoursContinuous)
            {
                rule.TwoClassHoursContinuous?.Clear();
                foreach (var r in Rules)
                {
                    var model = new TwoClassHoursContinuousRule()
                    {
                        UID = r.UID,
                        Active = r.IsActive,
                        Weight = r.Weight,
                        FirstID = r.FirstID,
                        SecondID = r.SecondID
                    };
                    rule.TwoClassHoursContinuous?.Add(model);
                }
            }
            else if (this.GetCurrentRuleEnum() == MixedAlgoRuleEnum.TwoClassHoursGrouped)
            {
                rule.TwoClassHoursGrouped?.Clear();
                foreach (var r in Rules)
                {
                    var model = new TwoClassHoursGroupedRule()
                    {
                        UID = r.UID,
                        Active = r.IsActive,
                        Weight = r.Weight,
                        FirstID = r.FirstID,
                        SecondID = r.SecondID,
                    };
                    rule.TwoClassHoursGrouped?.Add(model);
                }
            }
            else if (this.GetCurrentRuleEnum() == MixedAlgoRuleEnum.TwoClassHoursOrdered)
            {
                rule.TwoClassHoursOrdered?.Clear();
                foreach (var r in Rules)
                {
                    var model = new TwoClassHoursOrderedRule()
                    {
                        UID = r.UID,
                        Active = r.IsActive,
                        Weight = r.Weight,
                        FirstID = r.FirstID,
                        SecondID = r.SecondID,
                    };
                    rule.TwoClassHoursOrdered?.Add(model);
                }
            }

            base.SerializePatternAlgo(rule,base.LocalID);
            this.ShowDialog("提示信息", "保存成功", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
        }
    }
}
