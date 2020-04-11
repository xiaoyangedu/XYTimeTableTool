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
            Messenger.Default.Register<HostView>(this, save);

            this.Comments = CommonDataManager.GetMixedRuleComments(MixedRuleEnum.TeacherMaxHoursDaily);

            var cl = base.GetClCase(base.LocalID);
            // 绑定教师
            int no = 0;
            List<UIMaxHoursDaily> rules = new List<UIMaxHoursDaily>();
            cl.Teachers.ForEach(t =>
            {
                UIMaxHoursDaily teacherRule = new UIMaxHoursDaily()
                {
                    MaxHours = 1,
                    TeacherID = t.ID,
                    Teacher = t.Name,
                    Courses = cl.GetCourses(t.ID),
                    NO = ++no,
                };
                rules.Add(teacherRule);
            });
            this.Rules = rules;

            // 绑定课程
            List<UICourseSetting> courses = new List<UICourseSetting>();
            cl.Courses.ForEach(c =>
            {
                UICourseSetting setting = new UICourseSetting();
                setting.ID = c.ID;
                setting.Name = c.Name;
                setting.Value = 1;
                setting.Weight = 100;
                courses.Add(setting);
            });

            // 绑定教师状态
            var rule = base.GetClRule(base.LocalID);
            rule.MaxHoursDaily.ForEach(h =>
            {
                var first = this.Rules.FirstOrDefault(r => r.TeacherID.Equals(h.TeacherID));
                if (first != null)
                {
                    first.MaxHours = h.MaxHour;
                    first.Weight = (Models.Enums.WeightTypeEnum)h.Weight;
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
            var rule = base.GetClRule(base.LocalID);
            rule.MaxHoursDaily.Clear();

            this.Rules.Where(r => r.IsChecked)?.ToList()?.ForEach(r =>
            {
                var dailyRule = new XYKernel.OS.Common.Models.Mixed.Rule.TeacherMaxHoursDailyRule()
                {
                    TeacherID = r.TeacherID,
                    MaxHour = r.MaxHours,
                    Weight = (int)r.Weight
                };
                rule.MaxHoursDaily.Add(dailyRule);
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
