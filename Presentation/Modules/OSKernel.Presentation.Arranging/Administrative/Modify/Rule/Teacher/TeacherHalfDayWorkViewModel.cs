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
    /// <summary>
    /// 教师半天上课
    /// </summary>
    public class TeacherHalfDayWorkViewModel : CommonViewModel, IInitilize
    {
        private List<UITeacherHalfDayWork> _rules;

        private bool _isAllChecked;

        public ICommand UserControlCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<string>(userControlCommand);
            }
        }

        public List<UITeacherHalfDayWork> Rules
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

        public TeacherHalfDayWorkViewModel()
        {
            this.Rules = new List<UITeacherHalfDayWork>();

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
            this.Comments = CommonDataManager.GetAdminRuleComments(AdministrativeRuleEnum.TeacherHalfDayWorkRule);

            Messenger.Default.Register<HostView>(this, save);

            var cp = CommonDataManager.GetCPCase(base.LocalID);
            // 绑定教师
            int no = 0;
            List<UITeacherHalfDayWork> rules = new List<UITeacherHalfDayWork>();
            cp.Teachers.ForEach(t =>
            {
                UITeacherHalfDayWork teacherRule = new UITeacherHalfDayWork()
                {
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
            rule.HalfDayWork.ForEach(h =>
            {
                var first = this.Rules.FirstOrDefault(r => r.TeacherID.Equals(h.TeacherID));
                if (first != null)
                {
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
            rule.HalfDayWork.Clear();

            this.Rules.Where(r => r.IsChecked)?.ToList()?.ForEach(r =>
            {
                var halfDay = new XYKernel.OS.Common.Models.Administrative.Rule.TeacherHalfDayWorkRule()
                {
                    TeacherID = r.TeacherID,
                    Weight = (int)r.Weight
                };
                rule.HalfDayWork.Add(halfDay);
            });

            rule.Serialize(base.LocalID);
            this.ShowDialog("提示信息", "保存成功", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.None);
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
