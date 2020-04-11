using GalaSoft.MvvmLight.Messaging;
using OSKernel.Presentation.Arranging.Mixed.Modify.Algo.Teacher.Dialog;
using OSKernel.Presentation.Arranging.Mixed.Modify.Algo.Teacher.Model;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Time;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Algo.Teacher
{
    public class NotAvailableTimesViewModel : CommonViewModel, IInitilize
    {
        private ObservableCollection<UITeacherRule> _rules;

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

        public ObservableCollection<UITeacherRule> Rules
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

        public NotAvailableTimesViewModel()
        {
            this.Rules = new ObservableCollection<UITeacherRule>();
        }

        void createCommand()
        {
            CreateTeacherNoAvaliable createWin = new CreateTeacherNoAvaliable(Models.Enums.OperatorEnum.Add, Models.Enums.MixedAlgoRuleEnum.TeacherNotAvailableTimes);
            createWin.Closed += (s, arg) =>
            {
                if (createWin.DialogResult.Value)
                {
                    var has = this.Rules.Any(r => r.TeacherID.Equals(createWin.Add.TeacherID));
                    if (has)
                    {
                        this.ShowDialog("提示信息", "该教师存在规则", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                        return;
                    }

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
            UITeacherRule rule = obj as UITeacherRule;

            CreateTeacherNoAvaliable createWin = new CreateTeacherNoAvaliable(Models.Enums.OperatorEnum.Modify, Models.Enums.MixedAlgoRuleEnum.TeacherNotAvailableTimes, rule);
            createWin.Closed += (s, arg) =>
            {
                var modify = createWin.Modify;
                if (createWin.DialogResult.Value)
                {
                    rule.IsActive = modify.IsActive;
                    rule.Value = modify.Value;
                    rule.Weight = modify.Weight;
                    rule.ForbidTimes = modify.ForbidTimes;
                }
            };
            createWin.ShowDialog();
        }

        void deleteCommand(object obj)
        {
            UITeacherRule rule = obj as UITeacherRule;

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

            this.Comments = CommonDataManager.GetMixedAlgoComments(Models.Enums.MixedAlgoRuleEnum.TeacherNotAvailableTimes);

            var rule = base.GetCLAlgoRule(base.LocalID);

            var cl = base.GetClCase(base.LocalID);

            int no = 0;
            rule.TeacherNotAvailableTimes.ForEach(n =>
            {
                var newRule = new UITeacherRule()
                {
                    NO = ++no,
                    UID = n.UID,
                    ForbidTimes = n.Times,
                    IsActive = n.Active,
                    Weight = n.Weight,
                };

                var teacher = cl.Teachers.FirstOrDefault(t => t.ID.Equals(n.TeacherID));
                newRule.TeacherID = teacher?.ID;
                newRule.Name = teacher?.Name;

                this.Rules.Add(newRule);
            });
        }

        void save(HostView host)
        {
            var rule = base.GetCLAlgoRule(base.LocalID);
            rule.TeacherNotAvailableTimes.Clear();

            foreach (var r in this.Rules)
            {
                rule.TeacherNotAvailableTimes.Add(new XYKernel.OS.Common.Models.Mixed.AlgoRule.TeacherNotAvailableTimesRule()
                {
                    Active = r.IsActive,
                    TeacherID = r.TeacherID,
                    Times = r.ForbidTimes,
                    UID = r.UID,
                    Weight = r.Weight
                });
            }

            base.SerializePatternAlgo(rule, base.LocalID);
            this.ShowDialog("提示信息", "保存成功", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
        }
    }
}
