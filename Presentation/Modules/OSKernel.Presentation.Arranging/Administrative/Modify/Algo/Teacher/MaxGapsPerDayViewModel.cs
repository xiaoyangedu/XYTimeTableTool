using GalaSoft.MvvmLight.Messaging;
using OSKernel.Presentation.Arranging.Administrative.Modify.Algo.Teacher.Dialog;
using OSKernel.Presentation.Arranging.Administrative.Modify.Algo.Teacher.Model;
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

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Algo.Teacher
{
    public class MaxGapsPerDayViewModel : CommonViewModel, IInitilize
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

        private AdministrativeAlgoRuleEnum currentRuleEnum;

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

        public MaxGapsPerDayViewModel()
        {
            this.Rules = new ObservableCollection<UITeacherRule>();
        }

        void createCommand()
        {
            CreateMaxGapsPerDay createWin = new CreateMaxGapsPerDay(Models.Enums.OperatorEnum.Add, this.GetCurrentRuleEnum());
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

            CreateMaxGapsPerDay createWin = new CreateMaxGapsPerDay(Models.Enums.OperatorEnum.Modify, this.GetCurrentRuleEnum(), rule);
            createWin.Closed += (s, arg) =>
            {
                var modify = createWin.Modify;
                if (createWin.DialogResult.Value)
                {
                    rule.IsActive = modify.IsActive;
                    rule.Value = modify.Value;
                    rule.Weight = modify.Weight;
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
        }

        public void BindData(AdministrativeAlgoRuleEnum ruleEnum)
        {
            this.SetCurrentRuleEnum(ruleEnum);

            var rule = CommonDataManager.GetAminAlgoRule(base.LocalID);
            if (rule == null)
                return;

            var cp = CommonDataManager.GetCPCase(base.LocalID);

            int no = 0;
            if (this.GetCurrentRuleEnum() == AdministrativeAlgoRuleEnum.TeacherMaxGapsPerDay)
            {
                rule.TeacherMaxGapsPerDays.ForEach(t =>
                {
                    var teacher = cp.Teachers.FirstOrDefault(f => f.ID.Equals(t.TeacherID));

                    UITeacherRule ui = new UITeacherRule();
                    ui.NO = ++no;
                    ui.TeacherID = teacher?.ID;
                    ui.Name = teacher?.Name;
                    ui.IsActive = t.Active;
                    ui.Value = t.MaxGaps;

                    this.Rules.Add(ui);
                });
            }
            else
            {
                rule.TeachersMaxGapsPerDays?.ForEach(t =>
                {
                    UITeacherRule ui = new UITeacherRule();
                    ui.NO = ++no;
                    ui.Name = "所有教师";
                    ui.IsActive = t.Active;
                    ui.Value = t.MaxGaps;
                    ui.TeacherID = string.Empty;

                    this.Rules.Add(ui);
                });
            }
        }

        void save(HostView host)
        {
            var rule = CommonDataManager.GetAminAlgoRule(base.LocalID);

            if (this.GetCurrentRuleEnum() == AdministrativeAlgoRuleEnum.TeacherMaxGapsPerDay)
            {
                rule.TeacherMaxGapsPerDays.Clear();
                foreach (var r in Rules)
                {
                    var gaps = new TeacherMaxGapsPerDayRule()
                    {
                        Active = r.IsActive,
                        MaxGaps = r.Value,
                        TeacherID = r.TeacherID,
                        UID = r.UID,
                    };
                    rule.TeacherMaxGapsPerDays.Add(gaps);
                }
            }
            else
            {
                rule.TeachersMaxGapsPerDays?.Clear();
                foreach (var r in Rules)
                {
                    var perweeks = new TeachersMaxGapsPerDayRule()
                    {
                        Active = r.IsActive,
                        MaxGaps = r.Value,
                        UID = r.UID,
                    };
                    rule.TeachersMaxGapsPerDays?.Add(perweeks);
                }
            }

            rule.Serialize(base.LocalID);

            this.ShowDialog("提示信息", "保存成功", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
        }
    }
}
