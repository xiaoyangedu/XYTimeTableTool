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
    public class MaxHoursDailyViewModel : CommonViewModel, IInitilize
    {
        private List<UIMaxHoursDaily> _rules;

        private int _batchClassHour;

        private bool _isAllChecked;

        public ICommand UserControlCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<string>(userControlCommand);
            }
        }

        public List<UIMaxHoursDaily> Rules
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
        public int BatchClassHour
        {
            get
            {
                return _batchClassHour;
            }

            set
            {
                _batchClassHour = value;
                RaisePropertyChanged(() => BatchClassHour);

                this.Rules.ForEach(r => r.MaxHours = value);
            }
        }

        public MaxHoursDailyViewModel()
        {
            this.Rules = new List<UIMaxHoursDaily>();

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
            this.Comments = CommonDataManager.GetAdminRuleComments(AdministrativeRuleEnum.TeacherMaxHoursDaily);

            Messenger.Default.Register<HostView>(this, save);

            var cp = CommonDataManager.GetCPCase(base.LocalID);
            // 绑定教师
            int no = 0;
            List<UIMaxHoursDaily> rules = new List<UIMaxHoursDaily>();
            cp.Teachers.ForEach(t =>
            {
                UIMaxHoursDaily teacherRule = new UIMaxHoursDaily()
                {
                    MaxHours = 1,
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
            rule.MaxHoursDaily.ForEach(h =>
            {
                var first = this.Rules.FirstOrDefault(r => r.TeacherID.Equals(h.TeacherID));
                if (first != null)
                {
                    first.MaxHours = h.MaxHour;
                    first.Weight = (WeightTypeEnum)h.Weight;
                    first.IsChecked = true;
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
            rule.MaxHoursDaily.Clear();

            this.Rules.Where(r => r.IsChecked)?.ToList()?.ForEach(r =>
            {
                var dailyRule = new XYKernel.OS.Common.Models.Administrative.Rule.TeacherMaxHoursDailyRule()
                {
                    TeacherID = r.TeacherID,
                    MaxHour = r.MaxHours,
                    Weight = (int)r.Weight
                };
                rule.MaxHoursDaily.Add(dailyRule);
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
