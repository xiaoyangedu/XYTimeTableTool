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
    public class HoursNotOverlapViewModel : CommonViewModel, IInitilize
    {
        private ObservableCollection<UIClassHourRule> _rules;
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

        public HoursNotOverlapViewModel()
        {
            this.Rules = new ObservableCollection<UIClassHourRule>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            Messenger.Default.Register<HostView>(this, save);

            this.Comments = CommonDataManager.GetMixedAlgoComments(MixedAlgoRuleEnum.ClassHoursNotOverlap);

            this.BindData();
        }

        void createCommand()
        {
            CreateNoOverlap createWin = new CreateNoOverlap(Models.Enums.OperatorEnum.Add, Models.Enums.AdministrativeAlgoRuleEnum.ClassHoursNotOverlap);
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

            CreateNoOverlap createWin = new CreateNoOverlap(Models.Enums.OperatorEnum.Modify, Models.Enums.AdministrativeAlgoRuleEnum.ClassHoursNotOverlap, rule);
            createWin.Closed += (s, arg) =>
            {
                var modify = createWin.Modify;
                if (createWin.DialogResult.Value)
                {
                    rule.IsActive = modify.IsActive;
                    rule.Weight = modify.Weight;
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

        public void BindData()
        {
            var rule = base.GetCLAlgoRule(base.LocalID);

            var cl = base.GetClCase(base.LocalID);

            int no = 0;
            rule.ClassHoursNotOverlaps.ForEach(t =>
            {
                UIClassHourRule ui = new UIClassHourRule();
                ui.NO = ++no;
                ui.IsActive = t.Active;
                ui.Weight = t.Weight;
                ui.ClassHours = cl.GetClassHours(t.ID);

                this.Rules.Add(ui);
            });
        }

        void save(HostView host)
        {
            var rule = base.GetCLAlgoRule(base.LocalID);

            rule.ClassHoursNotOverlaps.Clear();

            foreach (var r in this.Rules)
            {
                var model = new ClassHoursNotOverlapRule()
                {
                    Active = r.IsActive,
                    Weight = r.Weight,
                    ID = r.ClassHours?.Select(ch => ch.ID)?.ToArray(),
                    UID = r.UID,
                };
                rule.ClassHoursNotOverlaps.Add(model);
            }

            base.SerializePatternAlgo(rule, base.LocalID);
            this.ShowDialog("提示信息", "保存成功", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
        }
    }
}
