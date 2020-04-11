using GalaSoft.MvvmLight.Messaging;
using OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Course.Model;
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

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Course
{
    public class AmPmClassHourViewModel : CommonViewModel, IInitilize
    {
        private List<UIAmPm> _rules;

        private bool _isCheckedAll;

        private int _allAmMax;

        private int _allPmMax;

        public bool IsCheckedAll
        {
            get
            {
                return _isCheckedAll;
            }

            set
            {
                _isCheckedAll = value;
                RaisePropertyChanged(() => IsCheckedAll);

                this.Rules.ForEach(r =>
                {
                    r.IsChecked = value;
                });
            }
        }

        public List<UIAmPm> Rules
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

        public ICommand UserControlCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<string>(userControlCommand);
            }
        }

        /// <summary>
        /// 所有上午最大数
        /// </summary>
        public int AllAmMax
        {
            get
            {
                return _allAmMax;
            }

            set
            {
                _allAmMax = value;
                RaisePropertyChanged(() => AllAmMax);

                this.Rules.ForEach(r =>
                {
                    r.AmMax = value;
                });
            }
        }

        /// <summary>
        /// 所有下午最大数
        /// </summary>
        public int AllPmMax
        {
            get
            {
                return _allPmMax;
            }

            set
            {
                _allPmMax = value;
                RaisePropertyChanged(() => AllPmMax);

                this.Rules.ForEach(r =>
                {
                    r.PmMax = value;
                });
            }
        }

        public AmPmClassHourViewModel()
        {
            this.Rules = new List<UIAmPm>();

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

            this.Comments = CommonDataManager.GetMixedRuleComments(MixedRuleEnum.AmPmClassHour);


            var rule = base.GetClRule(base.LocalID);

            var cl = base.GetClCase(base.LocalID);

            // 班级和课程的联合
            var crouseClass = cl.GetClasses();

            int index = 0;
            var rules = new List<UIAmPm>();
            crouseClass?.ToList()?.ForEach(cc =>
            {
                UIAmPm model = new UIAmPm()
                {
                    NO = ++index,
                    ClassID = cc.ID,
                    ClassName = cc.Name,
                    CourseID = cc.CourseID,
                    LevelID = cc.LevelID,
                    Course = cc.Course,
                };
                rules.Add(model);
            });
            this.Rules = rules;

            // 绑定选中状态
            rule.AmPmClassHours.ForEach(r =>
            {
                var first = this.Rules.FirstOrDefault(ro => ro.CourseID.Equals(r.CourseID) && ro.ClassID.Equals(r.ClassID) && ro.LevelID.Equals(r.LevelID));
                if (first != null)
                {
                    first.AmMax = r.AmMax;
                    first.PmMax = r.PmMax;
                    first.IsChecked = true;
                    first.Weight = (WeightTypeEnum)r.Weight;
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
            var rule = base.GetClRule(base.LocalID);

            var cl = base.GetClCase(base.LocalID);

            rule.AmPmClassHours.Clear();

            var results = Rules.Where(r => r.IsChecked);
            if (results != null)
            {
                foreach (var r in results)
                {
                    var ampm = new XYKernel.OS.Common.Models.Mixed.Rule.AmPmClassHourRule()
                    {
                        ClassID = r.ClassID,
                        CourseID = r.CourseID,
                        LevelID = r.LevelID,
                        AmMax = r.AmMax,
                        PmMax = r.PmMax,
                        Weight = (int)r.Weight
                    };
                    rule.AmPmClassHours.Add(ampm);
                }

                base.SerializePatternRule(rule, base.LocalID);
                this.ShowDialog("提示信息", "保存成功", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
            }
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
