using GalaSoft.MvvmLight.Messaging;
using OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Course.Model;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Enums;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Course
{
    public class MutexGroupViewModel : CommonViewModel, IInitilize
    {
        private ObservableCollection<UIMutextGroup> _rules;

        public ICommand UserControlCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<string>(userControlCommand);
            }
        }

        public ICommand CreateMutexGroupCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(createMutexGroup);
            }
        }

        public ICommand DeleteRuleCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(deleteRule);
            }
        }

        public ObservableCollection<UIMutextGroup> Rules
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

        public MutexGroupViewModel()
        {
            this.Rules = new ObservableCollection<UIMutextGroup>();

            base.Weights = new Dictionary<string, WeightTypeEnum>()
            {
                { "高", WeightTypeEnum.Hight},
                { "中", WeightTypeEnum.Medium},
                { "低", WeightTypeEnum.Low},
            };

            base.SelectWeight = WeightTypeEnum.Hight;

        }

        [InjectionMethod]
        public void Initilize()
        {
            Messenger.Default.Register<HostView>(this, save);

            this.Comments = CommonDataManager.GetAdminRuleComments(AdministrativeRuleEnum.MutexGroup);

            var cp = CommonDataManager.GetCPCase(base.LocalID);
            var rule = CommonDataManager.GetAminRule(base.LocalID);

            int index = 0;
            rule.Mutexes.ForEach(m =>
            {
                UIMutextGroup mutext = new UIMutextGroup();
                mutext.CourseIDs = m.CourseIDs;
                mutext.NO = ++index;
                mutext.Weight = (WeightTypeEnum)m.Weight;
                mutext.GroupName = (from c in cp.Courses from cc in m.CourseIDs where cc.Equals(c.ID) select c.Name)?.ToList()?.Parse();
                this.Rules.Add(mutext);
            });
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

        void createMutexGroup()
        {
            Dialog.CreateMutexGroup createWin = new Dialog.CreateMutexGroup();
            createWin.Closed += (s, arg) =>
            {
                // 如果窗口结果为True
                if (createWin.DialogResult.Value)
                {
                    var last = this.Rules.LastOrDefault();
                    var index = last == null ? 0 : last.NO;

                    var courseString = createWin.Courses.Select(c => c.Name)?.Parse();

                    var has = this.Rules.Any(r => r.GroupName.Equals(courseString));
                    if (!has)
                    {
                        this.Rules.Add(new UIMutextGroup()
                        {
                            NO = ++index,
                            CourseIDs = createWin.Courses.Select(c => c.ID)?.ToList(),
                            GroupName = createWin.Courses.Select(c => c.Name)?.Parse(),
                        });
                    }
                    else
                    {
                        this.ShowDialog("提示信息", "存在该组课程互斥规则!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                    }
                }
            };
            createWin.ShowDialog();
        }

        void deleteRule(object obj)
        {
            var result = this.ShowDialog("提示信息", "确认删除?", CustomControl.Enums.DialogSettingType.OkAndCancel, CustomControl.Enums.DialogType.Warning);
            if (result == CustomControl.Enums.DialogResultType.OK)
            {
                UIMutextGroup rule = obj as UIMutextGroup;
                this.Rules.Remove(rule);
            }
        }

        void save(HostView host)
        {
            var rule = CommonDataManager.GetAminRule(base.LocalID);
            var cp = CommonDataManager.GetCPCase(base.LocalID);

            // TODO 课程过滤班级（将所有班级规则插入到课程互斥组中）

            rule.Mutexes.Clear();

            foreach (var r in Rules)
            {
                var mutextGroup = new XYKernel.OS.Common.Models.Administrative.Rule.MutexGroupRule()
                {
                    CourseIDs = r.CourseIDs,
                    Weight = (int)r.Weight
                };

                rule.Mutexes.Add(mutextGroup);
            }

            rule.Serialize(base.LocalID);
            this.ShowDialog("提示信息", "保存成功", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
        }

        public override void BatchSetWeight(WeightTypeEnum weightEnum)
        {
            foreach (var r in this.Rules)
            {
                r.Weight = weightEnum;
            }
        }
    }
}
