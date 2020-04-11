using OSKernel.Presentation.Arranging.Administrative.Modify;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.Models.Mixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Views
{
    public class RuleViewModel : CommonViewModel, IInitilize
    {
        private List<UIRule> _rules;
        private List<UIAlgoRule> _algoRules;

        public List<UIRule> Rules
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
        public List<UIAlgoRule> AlgoRules
        {
            get
            {
                return _algoRules;
            }

            set
            {
                _algoRules = value;
                RaisePropertyChanged(() => AlgoRules);
            }
        }

        public ICommand SetRuleCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(setRule);
            }
        }

        public ICommand SetAlgoRuleCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(setAlgo);
            }
        }

        public ICommand WindowCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<string>(windowCommand);
            }
        }

        public RuleViewModel()
        {
            this.Rules = new List<UIRule>();
            this.AlgoRules = new List<UIAlgoRule>();
        }

        public void Initilize()
        {
            this.loadRules();

            this.loadAlgoes();
        }

        void setRule(object obj)
        {
            var mixRule = CommonDataManager.GetMixedRule(base.LocalID);

            UIRule rule = obj as UIRule;
            HostView ruleWindow = new HostView(rule.RuleEnum);
            ruleWindow.Closed += (s, arg) =>
            {
                switch (rule.RuleEnum)
                {
                    case Models.Enums.MixedRuleEnum.AmPmClassHour:
                        rule.Record = mixRule.AmPmClassHours.Count;
                        break;

                    case Models.Enums.MixedRuleEnum.ClassHourAverage:
                        rule.Record = mixRule.ClassHourAverages.Count;
                        break;

                    case Models.Enums.MixedRuleEnum.ClassHourSameOpen:
                        rule.Record = mixRule.ClassHourSameOpens.Count;
                        break;

                    case Models.Enums.MixedRuleEnum.CourseArrangeContinuous:
                        rule.Record = mixRule.ArrangeContinuous.Count;
                        break;

                    case Models.Enums.MixedRuleEnum.CourseLimit:
                        rule.Record = mixRule.CourseLimits.Count;
                        break;

                    case Models.Enums.MixedRuleEnum.CourseTime:
                        rule.Record = mixRule.CourseTimes.Count;
                        break;

                    case Models.Enums.MixedRuleEnum.TeacherMaxDaysPerWeek:
                        rule.Record = mixRule.MaxDaysPerWeek.Count;
                        break;

                    case Models.Enums.MixedRuleEnum.TeacherMaxGapsPerDay:
                        rule.Record = mixRule.MaxGapsPerDay.Count;
                        break;

                    case Models.Enums.MixedRuleEnum.TeacherMaxHoursDaily:
                        rule.Record = mixRule.MaxHoursDaily.Count;
                        break;

                    case Models.Enums.MixedRuleEnum.TeacherTime:
                        rule.Record = mixRule.TeacherTimes.Count;
                        break;
                }
            };
            ruleWindow.ShowDialog();
        }

        void setAlgo(object obj)
        {
            // 算法规则
            var algoRule = CommonDataManager.GetMixedAlgoRule(base.LocalID);

            UIAlgoRule algo = obj as UIAlgoRule;

            HostView algoWindow = new HostView(algo.RuleEnum);
            algoWindow.Closed += (s, arg) =>
            {
                switch (algo.RuleEnum)
                {
                    case Models.Enums.MixedAlgoRuleEnum.ClassHourRequiredStartingTime:
                        algo.Record = algoRule.ClassHourRequiredStartingTime.Count;
                        break;

                    case Models.Enums.MixedAlgoRuleEnum.ClassHourRequiredStartingTimes:
                        algo.Record = algoRule.ClassHourRequiredStartingTimes.Count;
                        break;

                    case Models.Enums.MixedAlgoRuleEnum.ClassHourRequiredTimes:
                        algo.Record = algoRule.ClassHourRequiredTimes.Count;
                        break;

                    case Models.Enums.MixedAlgoRuleEnum.ClassHoursMaxConcurrencyInSelectedTime:
                        algo.Record = algoRule.ClassHoursMaxConcurrencyInSelectedTimes.Count;
                        break;

                    case Models.Enums.MixedAlgoRuleEnum.ClassHoursNotOverlap:
                        algo.Record = algoRule.ClassHoursNotOverlaps.Count;
                        break;

                    case Models.Enums.MixedAlgoRuleEnum.ClassHoursOccupyMaxTimeFromSelection:
                        algo.Record = algoRule.ClassHoursOccupyMaxTimeFromSelections.Count;
                        break;

                    case Models.Enums.MixedAlgoRuleEnum.ClassHoursRequiredStartingTimes:
                        algo.Record = algoRule.ClassHoursRequiredStartingTimes.Count;
                        break;

                    case Models.Enums.MixedAlgoRuleEnum.ClassHoursRequiredTimes:
                        algo.Record = algoRule.ClassHoursRequiredTimes.Count;
                        break;

                    case Models.Enums.MixedAlgoRuleEnum.ClassHoursSameStarting:
                        algo.Record = algoRule.ClassHoursSameStartingDays.Count;
                        break;

                    case Models.Enums.MixedAlgoRuleEnum.ClassHoursSameStartingHour:
                        algo.Record = algoRule.ClassHoursSameStartingHours.Count;
                        break;

                    case Models.Enums.MixedAlgoRuleEnum.ClassHoursSameStartingTime:
                        algo.Record = algoRule.ClassHoursSameStartingTimes.Count;
                        break;

                    case Models.Enums.MixedAlgoRuleEnum.MaxDaysBetweenClassHours:
                        algo.Record = algoRule.MaxDaysBetweenClassHours.Count;
                        break;

                    case Models.Enums.MixedAlgoRuleEnum.MinDaysBetweenClassHours:
                        algo.Record = algoRule.MinDaysBetweenClassHours.Count;
                        break;

                    case Models.Enums.MixedAlgoRuleEnum.TeacherMaxDaysPerWeek:
                        algo.Record = algoRule.TeacherMaxDaysPerWeeks.Count;
                        break;

                    case Models.Enums.MixedAlgoRuleEnum.TeacherMaxGapsPerDay:
                        algo.Record = algoRule.TeacherMaxGapsPerDays.Count;
                        break;

                    case Models.Enums.MixedAlgoRuleEnum.TeacherMaxHoursDaily:
                        algo.Record = algoRule.TeacherMaxHoursDailys.Count;
                        break;

                    case Models.Enums.MixedAlgoRuleEnum.TeacherNotAvailableTimes:
                        algo.Record = algoRule.TeacherNotAvailableTimes.Count;
                        break;

                    case Models.Enums.MixedAlgoRuleEnum.TeachersMaxDaysPerWeek:
                        algo.Record = algoRule.TeachersMaxDaysPerWeeks.Count;
                        break;

                    case Models.Enums.MixedAlgoRuleEnum.TeachersMaxGapsPerDay:
                        algo.Record = algoRule.TeachersMaxGapsPerDays.Count;
                        break;

                    case Models.Enums.MixedAlgoRuleEnum.TeachersMaxHoursDaily:
                        algo.Record = algoRule.TeachersMaxHoursDailys.Count;
                        break;

                    case Models.Enums.MixedAlgoRuleEnum.ThreeClassHoursGrouped:
                        algo.Record = algoRule.ThreeClassHoursGrouped.Count;
                        break;

                    case Models.Enums.MixedAlgoRuleEnum.TwoClassHoursContinuous:
                        algo.Record = algoRule.TwoClassHoursContinuous.Count;
                        break;

                    case Models.Enums.MixedAlgoRuleEnum.TwoClassHoursGrouped:
                        algo.Record = algoRule.TwoClassHoursGrouped.Count;
                        break;

                    case Models.Enums.MixedAlgoRuleEnum.TwoClassHoursOrdered:
                        algo.Record = algoRule.TwoClassHoursOrdered.Count;
                        break;
                }
            };
            algoWindow.ShowDialog();
        }

        void loadRules()
        {
            var rule = base.GetClRule(base.LocalID);

            #region 教师

            var rules = new List<UIRule>();

            rules.Add(
                new UIRule
                {
                    NO = 1,
                    Name = "教师排课时间",
                    Category = Models.Enums.RuleCategoryEnum.Teacher,
                    RuleEnum = Models.Enums.MixedRuleEnum.TeacherTime,
                    Record = rule.TeacherTimes.Count,
                    Comment = CommonDataManager.GetMixedRuleComments(Models.Enums.MixedRuleEnum.TeacherTime)
                });

            rules.Add(
               new UIRule
               {
                   NO = 2,
                   Name = "教师每天最大课时数",
                   Category = Models.Enums.RuleCategoryEnum.Teacher,
                   RuleEnum = Models.Enums.MixedRuleEnum.TeacherMaxHoursDaily,
                   Record = rule.MaxHoursDaily.Count,
                   Comment = CommonDataManager.GetMixedRuleComments(Models.Enums.MixedRuleEnum.TeacherMaxHoursDaily)
               });

            rules.Add(
                new UIRule
                {
                    NO = 3,
                    Name = "教师每周最大工作天数",
                    Category = Models.Enums.RuleCategoryEnum.Teacher,
                    RuleEnum = Models.Enums.MixedRuleEnum.TeacherMaxDaysPerWeek,
                    Record = rule.MaxDaysPerWeek.Count,
                    Comment = CommonDataManager.GetMixedRuleComments(Models.Enums.MixedRuleEnum.TeacherMaxDaysPerWeek)
                });

            rules.Add(
                new UIRule
                {
                    NO = 4,
                    Name = "教师每天最大课时间隔",
                    Category = Models.Enums.RuleCategoryEnum.Teacher,
                    RuleEnum = Models.Enums.MixedRuleEnum.TeacherMaxGapsPerDay,
                    Record = rule.MaxGapsPerDay.Count,
                    Comment = CommonDataManager.GetMixedRuleComments(Models.Enums.MixedRuleEnum.TeacherMaxGapsPerDay)
                });

            #endregion

            #region 课时

            rules.Add(
                new UIRule
                {
                    NO = 5,
                    Name = "课时分散",
                    Category = Models.Enums.RuleCategoryEnum.Course,
                    RuleEnum = Models.Enums.MixedRuleEnum.ClassHourAverage,
                    Record = rule.ClassHourAverages.Count,
                    Comment = CommonDataManager.GetMixedRuleComments(Models.Enums.MixedRuleEnum.ClassHourAverage)
                });

            rules.Add(
                new UIRule
                {
                    NO = 6,
                    Name = "同时开课",
                    Category = Models.Enums.RuleCategoryEnum.Course,
                    RuleEnum = Models.Enums.MixedRuleEnum.ClassHourSameOpen,
                    Record = rule.ClassHourSameOpens.Count,
                    Comment = CommonDataManager.GetMixedRuleComments(Models.Enums.MixedRuleEnum.ClassHourSameOpen)
                });

            #endregion

            #region 课程

            rules.Add(
            new UIRule
            {
                NO = 7,
                Name = "上下午课时",
                Category = Models.Enums.RuleCategoryEnum.Course,
                RuleEnum = Models.Enums.MixedRuleEnum.AmPmClassHour,
                Record = rule.AmPmClassHours.Count,
                Comment = CommonDataManager.GetMixedRuleComments(Models.Enums.MixedRuleEnum.AmPmClassHour)
            });

            rules.Add(
            new UIRule
            {
                NO = 8,
                Name = "课程连排",
                Category = Models.Enums.RuleCategoryEnum.Course,
                RuleEnum = Models.Enums.MixedRuleEnum.CourseArrangeContinuous,
                Record = rule.ArrangeContinuous.Count,
                Comment = CommonDataManager.GetMixedRuleComments(Models.Enums.MixedRuleEnum.CourseArrangeContinuous)
            });

            rules.Add(
            new UIRule
            {
                NO = 9,
                Name = "课程排课时间",
                Category = Models.Enums.RuleCategoryEnum.Course,
                RuleEnum = Models.Enums.MixedRuleEnum.CourseTime,
                Record = rule.CourseTimes.Count,
                Comment = CommonDataManager.GetMixedRuleComments(Models.Enums.MixedRuleEnum.CourseTime)
            });

            rules.Add(
            new UIRule
            {
                NO = 10,
                Name = "同时开课限制",
                Category = Models.Enums.RuleCategoryEnum.Course,
                RuleEnum = Models.Enums.MixedRuleEnum.CourseLimit,
                Record = rule.CourseLimits.Count,
                Comment = CommonDataManager.GetMixedRuleComments(Models.Enums.MixedRuleEnum.CourseLimit)
            });

            rules.Add(
            new UIRule
            {
                NO = 11,
                Name = "可用教室数",
                Category = Models.Enums.RuleCategoryEnum.Room,
                RuleEnum = Models.Enums.MixedRuleEnum.AvailableRoom,
                Comment = CommonDataManager.GetMixedRuleComments(Models.Enums.MixedRuleEnum.AvailableRoom)
            });

            #endregion

            this.Rules = rules;
        }

        void loadAlgoes()
        {
            var mixedRule = base.GetCLAlgoRule(base.LocalID);

            var algoRules = new List<UIAlgoRule>();

            #region 教师

            algoRules.Add(new UIAlgoRule()
            {
                NO = 1,
                Name = "教师的不可用时间",
                Category = Models.Enums.RuleCategoryEnum.Teacher,
                RuleEnum = Models.Enums.MixedAlgoRuleEnum.TeacherNotAvailableTimes,
                Record = mixedRule.TeacherNotAvailableTimes.Count,
                Comment = CommonDataManager.GetMixedAlgoComments(Models.Enums.MixedAlgoRuleEnum.TeacherNotAvailableTimes)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 2,
                Name = "教师每周最大工作天数",
                Category = Models.Enums.RuleCategoryEnum.Teacher,
                RuleEnum = Models.Enums.MixedAlgoRuleEnum.TeacherMaxDaysPerWeek,
                Record = mixedRule.TeacherMaxDaysPerWeeks.Count,
                Comment = CommonDataManager.GetMixedAlgoComments(Models.Enums.MixedAlgoRuleEnum.TeacherMaxDaysPerWeek)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 3,
                Name = "教师每天最大课时数",
                Category = Models.Enums.RuleCategoryEnum.Teacher,
                RuleEnum = Models.Enums.MixedAlgoRuleEnum.TeacherMaxHoursDaily,
                Record = mixedRule.TeacherMaxHoursDailys.Count,
                Comment = CommonDataManager.GetMixedAlgoComments(Models.Enums.MixedAlgoRuleEnum.TeacherMaxHoursDaily)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 4,
                Name = "教师每天最大课程间隔",
                Category = Models.Enums.RuleCategoryEnum.Teacher,
                RuleEnum = Models.Enums.MixedAlgoRuleEnum.TeacherMaxGapsPerDay,
                Record = mixedRule.TeacherMaxGapsPerDays.Count,
                Comment = CommonDataManager.GetMixedAlgoComments(Models.Enums.MixedAlgoRuleEnum.TeacherMaxGapsPerDay)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 5,
                Name = "所有教师每周最大工作天数",
                Category = Models.Enums.RuleCategoryEnum.Teacher,
                RuleEnum = Models.Enums.MixedAlgoRuleEnum.TeachersMaxDaysPerWeek,
                Record = mixedRule.TeachersMaxDaysPerWeeks.Count,
                Comment = CommonDataManager.GetMixedAlgoComments(Models.Enums.MixedAlgoRuleEnum.TeachersMaxDaysPerWeek)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 6,
                Name = "所有教师每天最大课程间隔",
                Category = Models.Enums.RuleCategoryEnum.Teacher,
                RuleEnum = Models.Enums.MixedAlgoRuleEnum.TeachersMaxGapsPerDay,
                Record = mixedRule.TeachersMaxGapsPerDays.Count,
                Comment = CommonDataManager.GetMixedAlgoComments(Models.Enums.MixedAlgoRuleEnum.TeachersMaxGapsPerDay)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 7,
                Name = "所有教师每天最大课时数",
                Category = Models.Enums.RuleCategoryEnum.Teacher,
                RuleEnum = Models.Enums.MixedAlgoRuleEnum.TeachersMaxHoursDaily,
                Record = mixedRule.TeachersMaxHoursDailys.Count,
                Comment = CommonDataManager.GetMixedAlgoComments(Models.Enums.MixedAlgoRuleEnum.TeachersMaxHoursDaily)
            });

            #endregion

            #region 课时

            algoRules.Add(new UIAlgoRule()
            {
                NO = 8,
                Name = "单个课时有多个优先开始时间",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.MixedAlgoRuleEnum.ClassHourRequiredStartingTimes,
                Record = mixedRule.ClassHourRequiredStartingTimes.Count,
                Comment = CommonDataManager.GetMixedAlgoComments(Models.Enums.MixedAlgoRuleEnum.ClassHourRequiredStartingTimes)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 9,
                Name = "单个课时有多个优先课位",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.MixedAlgoRuleEnum.ClassHourRequiredTimes,
                Record = mixedRule.ClassHourRequiredTimes.Count,
                Comment = CommonDataManager.GetMixedAlgoComments(Models.Enums.MixedAlgoRuleEnum.ClassHourRequiredTimes)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 10,
                Name = "单个课时有一个优先开始时间",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.MixedAlgoRuleEnum.ClassHourRequiredStartingTime,
                Record = mixedRule.ClassHourRequiredStartingTime.Count,
                Comment = CommonDataManager.GetMixedAlgoComments(Models.Enums.MixedAlgoRuleEnum.ClassHourRequiredStartingTime)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 11,
                Name = "多个课时间最小间隔天数",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.MixedAlgoRuleEnum.MinDaysBetweenClassHours,
                Record = mixedRule.MinDaysBetweenClassHours.Count,
                Comment = CommonDataManager.GetMixedAlgoComments(Models.Enums.MixedAlgoRuleEnum.MinDaysBetweenClassHours)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 12,
                Name = "多个课时间最大间隔天数",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.MixedAlgoRuleEnum.MaxDaysBetweenClassHours,
                Record = mixedRule.MaxDaysBetweenClassHours.Count,
                Comment = CommonDataManager.GetMixedAlgoComments(Models.Enums.MixedAlgoRuleEnum.MaxDaysBetweenClassHours)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 13,
                Name = "多个课时有多个优先开始时间",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.MixedAlgoRuleEnum.ClassHoursRequiredStartingTimes,
                Record = mixedRule.ClassHourRequiredStartingTimes.Count,
                Comment = CommonDataManager.GetMixedAlgoComments(Models.Enums.MixedAlgoRuleEnum.ClassHoursRequiredStartingTimes)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 14,
                Name = "多个课时有多个优先课位",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.MixedAlgoRuleEnum.ClassHoursRequiredTimes,
                Record = mixedRule.ClassHoursRequiredTimes.Count,
                Comment = CommonDataManager.GetMixedAlgoComments(Models.Enums.MixedAlgoRuleEnum.ClassHoursRequiredTimes)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 15,
                Name = "多个课时不同时开课",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.MixedAlgoRuleEnum.ClassHoursNotOverlap,
                Record = mixedRule.ClassHoursNotOverlaps.Count,
                Comment = CommonDataManager.GetMixedAlgoComments(Models.Enums.MixedAlgoRuleEnum.ClassHoursNotOverlap)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 16,
                Name = "多个课时在选定课位中占用的最大数量",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.MixedAlgoRuleEnum.ClassHoursOccupyMaxTimeFromSelection,
                Record = mixedRule.ClassHoursOccupyMaxTimeFromSelections.Count,
                Comment = CommonDataManager.GetMixedAlgoComments(Models.Enums.MixedAlgoRuleEnum.ClassHoursOccupyMaxTimeFromSelection)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 17,
                Name = "多个课时在选定课位中同时开课最大数量",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.MixedAlgoRuleEnum.ClassHoursMaxConcurrencyInSelectedTime,
                Record = mixedRule.ClassHoursMaxConcurrencyInSelectedTimes.Count,
                Comment = CommonDataManager.GetMixedAlgoComments(Models.Enums.MixedAlgoRuleEnum.ClassHoursMaxConcurrencyInSelectedTime)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 18,
                Name = "多个课时有相同的开始日期（日期）",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.MixedAlgoRuleEnum.ClassHoursSameStarting,
                Record = mixedRule.ClassHoursSameStartingDays.Count,
                Comment = CommonDataManager.GetMixedAlgoComments(Models.Enums.MixedAlgoRuleEnum.ClassHoursSameStarting)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 19,
                Name = "多个课时有相同的开始课位（时间）",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.MixedAlgoRuleEnum.ClassHoursSameStartingHour,
                Record = mixedRule.ClassHoursSameStartingHours.Count,
                Comment = CommonDataManager.GetMixedAlgoComments(Models.Enums.MixedAlgoRuleEnum.ClassHoursSameStartingHour)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 20,
                Name = "多个课时有相同的开始时间（日期+时间）",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.MixedAlgoRuleEnum.ClassHoursSameStartingTime,
                Record = mixedRule.ClassHoursSameStartingTimes.Count,
                Comment = CommonDataManager.GetMixedAlgoComments(Models.Enums.MixedAlgoRuleEnum.ClassHoursSameStartingTime)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 21,
                Name = "对2个课时设置连排",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.MixedAlgoRuleEnum.TwoClassHoursContinuous,
                Record = mixedRule.TwoClassHoursContinuous.Count,
                Comment = CommonDataManager.GetMixedAlgoComments(Models.Enums.MixedAlgoRuleEnum.TwoClassHoursContinuous)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 22,
                Name = "对2个课时排序",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.MixedAlgoRuleEnum.TwoClassHoursOrdered,
                Record = mixedRule.TwoClassHoursOrdered.Count,
                Comment = CommonDataManager.GetMixedAlgoComments(Models.Enums.MixedAlgoRuleEnum.TwoClassHoursOrdered)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 23,
                Name = "给2个课时分组",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.MixedAlgoRuleEnum.TwoClassHoursGrouped,
                Record = mixedRule.TwoClassHoursGrouped.Count,
                Comment = CommonDataManager.GetMixedAlgoComments(Models.Enums.MixedAlgoRuleEnum.TwoClassHoursGrouped)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 24,
                Name = "给3个课时分组",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.MixedAlgoRuleEnum.ThreeClassHoursGrouped,
                Record = mixedRule.ThreeClassHoursGrouped.Count,
                Comment = CommonDataManager.GetMixedAlgoComments(Models.Enums.MixedAlgoRuleEnum.ThreeClassHoursGrouped)
            });

            #endregion

            this.AlgoRules = algoRules;
        }

        void windowCommand(object obj)
        {
            var param = obj as string;

            if (param.Equals("loaded"))
            {
                Initilize();
            }
            else if (param.Equals("unloaded"))
            {

            }
            else if (param.Equals("closed"))
            {

            }
        }

        public void Refresh()
        {
            this.Initilize();
        }
    }
}
