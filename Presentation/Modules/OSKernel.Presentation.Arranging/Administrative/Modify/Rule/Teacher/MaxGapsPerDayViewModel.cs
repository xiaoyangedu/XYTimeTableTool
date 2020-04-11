using GalaSoft.MvvmLight.Messaging;
using OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Teacher.Model;
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

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Teacher
{
    public class MaxGapsPerDayViewModel : CommonViewModel, IInitilize
    {
        private List<UIMaxGapsPerDay> _rules;

        private int _batchMaxGaps;

        private bool _isAllChecked;

        public ICommand UserControlCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<string>(userControlCommand);
            }
        }

        public List<UIMaxGapsPerDay> Rules
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
        /// 课时批量调整
        /// </summary>
        public int BatchMaxGaps
        {
            get
            {
                return _batchMaxGaps;
            }

            set
            {
                _batchMaxGaps = value;
                RaisePropertyChanged(() => BatchMaxGaps);
                this.Rules.ForEach(r => r.MaxGaps = value);
            }
        }

        public MaxGapsPerDayViewModel()
        {
            this.Rules = new List<UIMaxGapsPerDay>();

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
            this.Comments = CommonDataManager.GetAdminRuleComments(AdministrativeRuleEnum.TeacherMaxGapsPerDay);

            Messenger.Default.Register<HostView>(this, save);

            var cp = CommonDataManager.GetCPCase(base.LocalID);
            // 绑定教师
            int no = 0;
            List<UIMaxGapsPerDay> rules = new List<UIMaxGapsPerDay>();
            cp.Teachers.ForEach(t =>
            {
                UIMaxGapsPerDay teacherRule = new UIMaxGapsPerDay()
                {
                    CourseID = string.Empty,
                    MaxGaps = 1,
                    TeacherID = t.ID,
                    Teacher = t.Name,
                    Courses = cp.GetCourses(t.ID),
                    NO = ++no,
                };
                rules.Add(teacherRule);
            });
            this.Rules = rules;

            // 绑定教师状态
            var rule = CommonDataManager.GetAminRule(base.LocalID);
            rule.MaxGapsPerDay.ForEach(h =>
            {
                var first = this.Rules.FirstOrDefault(r => r.TeacherID.Equals(h.TeacherID));
                if (first != null)
                {
                    first.IsChecked = true;
                    first.MaxGaps = h.MaxIntervel;
                    first.Weight = (Models.Enums.WeightTypeEnum)h.Weight;
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
            rule.MaxGapsPerDay.Clear();

            this.Rules.Where(r => r.IsChecked)?.ToList()?.ForEach(r =>
            {
                var gapsRule = new XYKernel.OS.Common.Models.Administrative.Rule.TeacherMaxGapsPerDayRule()
                {
                    TeacherID = r.TeacherID,
                    MaxIntervel = r.MaxGaps,
                    Weight = (int)r.Weight
                };
                rule.MaxGapsPerDay.Add(gapsRule);
            });

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
