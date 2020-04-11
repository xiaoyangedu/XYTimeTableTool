using GalaSoft.MvvmLight.Messaging;
using OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Teacher.Model;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Teacher
{
    public class TeacherPriorityBalanceViewModel : CommonViewModel, IInitilize
    {
        private List<UITeacherPriorityBalance> _rules;
        private bool _isAllChecked;

        public ICommand UserControlCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<string>(userControlCommand);
            }
        }

        public List<UITeacherPriorityBalance> Rules
        {
            get
            {
                return _rules;
            }

            set
            {
                _rules = value;
                RaisePropertyChanged(() => Rules);
            }
        }

        public bool IsAllChecked
        {
            get
            {
                return _isAllChecked;
            }

            set
            {
                _isAllChecked = value;
                RaisePropertyChanged(() => IsAllChecked);

                this.Rules?.ForEach(r =>
                {
                    r.IsChecked = value;
                });

            }
        }

        public TeacherPriorityBalanceViewModel()
        {
            this.Rules = new List<UITeacherPriorityBalance>();

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
            this.Comments = CommonDataManager.GetAdminRuleComments(AdministrativeRuleEnum.TeacherPriorityBalanceRule);

            Messenger.Default.Register<HostView>(this, save);
            var cp = CommonDataManager.GetCPCase(base.LocalID);

            // 绑定教师
            int no = 0;
            List<UITeacherPriorityBalance> rules = new List<UITeacherPriorityBalance>();
            cp.Teachers.ForEach(t =>
            {
                var classes = cp.GetClassesByTeacherID(t.ID);
                var courses = cp.GetCourses(t.ID);

                var groups = classes?.GroupBy(c => c.CourseID);
                if (groups != null)
                {
                    foreach (var g in groups)
                    {
                        var count = g.Count();
                        if (count > 1)
                        {
                            UITeacherPriorityBalance teacherRule = new UITeacherPriorityBalance()
                            {
                                CourseID = g.Key,
                                TeacherID = t.ID,
                                Teacher = t.Name,
                                Courses = courses,
                            };
                            teacherRule.Classes = g.ToList();
                            teacherRule.NO = ++no;
                            rules.Add(teacherRule);
                        }
                    }
                }
            });
            this.Rules = rules;

            // 绑定教师状态
            var rule = CommonDataManager.GetAminRule(base.LocalID);
            rule.ClassHourPriorityBalance.ForEach(h =>
            {
                // 1.绑定规则选中状态。
                var first = this.Rules.FirstOrDefault(r => r.TeacherID.Equals(h.TeacherID));
                if (first != null)
                {
                    first.Weight = (Models.Enums.WeightTypeEnum)h.Weight;

                    // 2.全部选中
                    var classes = (from c in h.ClassIDs from cl in first.Classes where c.Equals(cl.ID) select cl)?.ToList();
                    classes?.ForEach(c =>
                    {
                        c.IsChecked = true;
                    });
                }
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

        void save(HostView host)
        {
            var rule = CommonDataManager.GetAminRule(base.LocalID);
            rule.ClassHourPriorityBalance.Clear();

            var hasOneSelect = this.Rules.Any(r => r.Classes.Count(c => c.IsChecked) == 1);
            if (hasOneSelect)
            {
                this.ShowDialog("提示信息", "至少选择两个班级齐头", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);
                return;
            }

            var filters = this.Rules.Where(r => r.Classes.Count(c => c.IsChecked) >= 2);
            if (filters?.Count() > 0)
            {
                foreach (var f in filters)
                {
                    var balanceRule = new XYKernel.OS.Common.Models.Administrative.Rule.TeacherPriorityBalanceRule()
                    {
                        CourseID = f.CourseID,
                        TeacherID = f.TeacherID,
                        ClassIDs = f.Classes.Where(c => c.IsChecked)?.Select(s => s.ID)?.ToList(),
                        Weight = (int)f.Weight
                    };
                    rule.ClassHourPriorityBalance.Add(balanceRule);
                }
            }

            rule.Serialize(base.LocalID);
            this.ShowDialog("提示信息", "保存成功", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
        }

        public override void BatchSetWeight(WeightTypeEnum weightEnum)
        {
            this.Rules.ForEach(r =>
            {
                r.Weight = weightEnum;
            });
        }
    }
}
