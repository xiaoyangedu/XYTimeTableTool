using GalaSoft.MvvmLight.Messaging;
using OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Course.Model;
using OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Teacher.Dialog;
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
    public class CourseLimitViewModel : CommonViewModel, IInitilize
    {
        private List<UICourseLimit> _courseLimits;

        public ICommand SetCourseCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(setCourseCommand);
            }
        }

        public ICommand UserControlCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<string>(userControlCommand);
            }
        }

        public List<UICourseLimit> CourseLimits
        {
            get
            {
                return _courseLimits;
            }

            set
            {
                _courseLimits = value;
                RaisePropertyChanged(() => CourseLimits);
            }
        }

        public CourseLimitViewModel()
        {
            this.CourseLimits = new List<UICourseLimit>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            Messenger.Default.Register<HostView>(this, save);

            this.Comments = CommonDataManager.GetMixedRuleComments(MixedRuleEnum.CourseLimit);

            List<UICourseLimit> courseLimits = new List<UICourseLimit>();

            var rule = base.GetClRule(base.LocalID);
            var cl = base.GetClCase(base.LocalID);

            this.CourseLimits = cl.Courses.Select(c =>
              {
                  return new UICourseLimit()
                  {
                      CourseID = c.ID,
                      Course = c.Name,
                  };
              })?.ToList();

            rule.CourseLimits.ForEach(c =>
            {
                var course = this.CourseLimits.FirstOrDefault(cc => cc.CourseID.Equals(c.CourseID));
                if (course != null)
                {
                    course.IsChecked = true;
                    course.Limit = c.Limit;
                    course.PeriodLimits = c.PeriodLimits;
                    course.Weight = (Models.Enums.WeightTypeEnum)c.Weight;
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

        void setCourseCommand(object obj)
        {
            UICourseLimit model = obj as UICourseLimit;

            CourseLimitPosition window = new CourseLimitPosition(model);
            window.Closed += (s, arg) =>
            {
                if (window.DialogResult.Value)
                {
                    model.PeriodLimits = window.PeriodLimits;
                    model.Weight = window.WeightType;
                }
            };
            window.ShowDialog();
        }

        void save(HostView host)
        {
            var rule = base.GetClRule(base.LocalID);

            this.CourseLimits.Where(c => c.IsChecked)?.ToList()?.ForEach(c =>
              {
                  var courseRule = rule.CourseLimits.FirstOrDefault(cl => cl.CourseID.Equals(c.CourseID));
                  if (courseRule != null)
                  {
                      courseRule.Limit = c.Limit;
                      courseRule.PeriodLimits = c.PeriodLimits;
                  }
                  else
                  {
                      var limitRule = new XYKernel.OS.Common.Models.Mixed.Rule.CourseLimitRule()
                      {
                          CourseID = c.CourseID,
                          Limit = c.Limit,
                          PeriodLimits = c.PeriodLimits,
                          Weight = (int)c.Weight
                      };
                      rule.CourseLimits.Add(limitRule);
                  }
              });

            this.CourseLimits.Where(c => !c.IsChecked)?.ToList()?.ForEach(c =>
            {
                var courseRule = rule.CourseLimits.FirstOrDefault(cl => cl.CourseID.Equals(c.CourseID));
                if (courseRule != null)
                {
                    courseRule.Limit = c.Limit;
                    courseRule.PeriodLimits = c.PeriodLimits;
                    rule.CourseLimits.Remove(courseRule);
                }
            });
            base.SerializePatternRule(rule, base.LocalID);

            this.ShowDialog("提示信息", "保存成功", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
        }
    }
}
