using GalaSoft.MvvmLight.Messaging;
using OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Teacher.Model;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Enums;
using OSKernel.Presentation.Models.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Teacher
{
    public class MaxDaysPerWeekViewModel : CommonViewModel, IInitilize
    {
        private List<UIMaxDaysPerWeek> _rules;

        private int _batchMaxDays;

        private bool _isAllChecked;

        public ICommand UserControlCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<string>(userControlCommand);
            }
        }

        public List<UIMaxDaysPerWeek> Rules
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

        /// <summary>
        /// 最大天数批量调整
        /// </summary>
        public int BatchMaxDays
        {
            get
            {
                return _batchMaxDays;
            }

            set
            {
                _batchMaxDays = value;
                RaisePropertyChanged(() => BatchMaxDays);

                this.Rules.ForEach(r => r.MaxDays = value);
            }
        }

        public MaxDaysPerWeekViewModel()
        {
            this.Rules = new List<UIMaxDaysPerWeek>();

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

            this.Comments = CommonDataManager.GetMixedRuleComments(MixedRuleEnum.TeacherMaxDaysPerWeek);

            var cl = base.GetClCase(base.LocalID);

            // 绑定教师
            int no = 0;
            List<UIMaxDaysPerWeek> rules = new List<UIMaxDaysPerWeek>();
            cl.Teachers.ForEach(t =>
            {
                UIMaxDaysPerWeek teacherRule = new UIMaxDaysPerWeek()
                {
                    NO = ++no,
                    CourseID = string.Empty,
                    MaxDays = 1,
                    TeacherID = t.ID,
                    Teacher = t.Name,
                    Courses = cl.GetCourses(t.ID),
                };
                rules.Add(teacherRule);
            });
            this.Rules = rules;

            // 绑定教师状态
            var rule = base.GetClRule(base.LocalID);
            if (rule != null)
            {
                rule.MaxDaysPerWeek.ForEach(h =>
                {
                    var first = this.Rules.FirstOrDefault(r => r.TeacherID.Equals(h.TeacherID));
                    if (first != null)
                    {
                        first.Weight = (Models.Enums.WeightTypeEnum)h.Weight;
                        first.MaxDays = h.MaxDay;
                        first.IsChecked = true;
                    }
                });
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

        void save(HostView host)
        {
            var rule = base.GetClRule(base.LocalID);

            rule.MaxDaysPerWeek.Clear();

            this.Rules.Where(r => r.IsChecked)?.ToList()?.ForEach(r =>
            {
                var maxDayRule = new XYKernel.OS.Common.Models.Mixed.Rule.TeacherMaxDaysPerWeekRule()
                {
                    TeacherID = r.TeacherID,
                    Weight = (int)r.Weight,
                    MaxDay = r.MaxDays
                };
                rule.MaxDaysPerWeek.Add(maxDayRule);
            });

            base.SerializePatternRule(rule, base.LocalID);
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
