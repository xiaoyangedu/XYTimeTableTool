using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.Models.Administrative;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Views
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
            UIRule rule = obj as UIRule;

            var adminRule = CommonDataManager.GetAminRule(base.LocalID);

            HostView ruleWindow = new HostView(rule.RuleEnum);
            ruleWindow.Closed += (s, arg) =>
            {
                switch (rule.RuleEnum)
                {
                    case Models.Enums.AdministrativeRuleEnum.AmPmClassHour:
                        rule.Record = adminRule.AmPmClassHours.Count;
                        break;

                    case Models.Enums.AdministrativeRuleEnum.ArrangeContinuousPlanFlush:
                        rule.Record = adminRule.ContinuousPlanFlushes.Count;
                        break;

                    case Models.Enums.AdministrativeRuleEnum.ClassHourAverage:
                        rule.Record = adminRule.ClassHourAverages.Count;
                        break;

                    case Models.Enums.AdministrativeRuleEnum.ClassHourSameOpen:
                        rule.Record = adminRule.ClassHourSameOpens.Count;
                        break;

                    case Models.Enums.AdministrativeRuleEnum.ClassUnion:
                        rule.Record = adminRule.ClassUnions.Count;
                        break;

                    case Models.Enums.AdministrativeRuleEnum.CourseArrangeContinuous:
                        rule.Record = adminRule.CourseArranges.Count;
                        break;

                    case Models.Enums.AdministrativeRuleEnum.CourseLimit:
                        rule.Record = adminRule.CourseLimits.Count;
                        break;

                    case Models.Enums.AdministrativeRuleEnum.CourseTime:
                        rule.Record = adminRule.CourseTimes.Count;
                        break;

                    case Models.Enums.AdministrativeRuleEnum.MasterApprenttice:
                        rule.Record = adminRule.MasterApprenttices.Count;
                        break;

                    case Models.Enums.AdministrativeRuleEnum.MutexGroup:
                        rule.Record = adminRule.Mutexes.Count;
                        break;

                    case Models.Enums.AdministrativeRuleEnum.OddDualWeek:
                        rule.Record = adminRule.OddDualWeeks.Count;
                        break;

                    case Models.Enums.AdministrativeRuleEnum.TeacherAmPmNoContinues:
                        rule.Record = adminRule.AmPmNoContinues.Count;
                        break;

                    case Models.Enums.AdministrativeRuleEnum.TeacherHalfDayWorkRule:
                        rule.Record = adminRule.HalfDayWork.Count;
                        break;

                    case Models.Enums.AdministrativeRuleEnum.TeacherMaxDaysPerWeek:
                        rule.Record = adminRule.MaxDaysPerWeek.Count;
                        break;

                    case Models.Enums.AdministrativeRuleEnum.TeacherMaxGapsPerDay:
                        rule.Record = adminRule.MaxGapsPerDay.Count;
                        break;

                    case Models.Enums.AdministrativeRuleEnum.TeacherMaxHoursDaily:
                        rule.Record = adminRule.MaxHoursDaily.Count;
                        break;

                    case Models.Enums.AdministrativeRuleEnum.TeacherPriorityBalanceRule:
                        rule.Record = adminRule.ClassHourPriorityBalance.Count;
                        break;

                    case Models.Enums.AdministrativeRuleEnum.TeacherTime:
                        rule.Record = adminRule.TeacherTimes.Count;
                        break;

                    case Models.Enums.AdministrativeRuleEnum.TeachingPlanFlush:
                        rule.Record = adminRule.PlanFlushes.Count;
                        break;

                    case Models.Enums.AdministrativeRuleEnum.LockedCourse:
                        rule.Record = adminRule.TimeTableLockedTimes == null ? 0 : 1;
                        break;
                }
            };
            ruleWindow.ShowDialog();
        }

        void setAlgo(object obj)
        {
            var algoRule = CommonDataManager.GetAminAlgoRule(base.LocalID);

            UIAlgoRule algo = obj as UIAlgoRule;
            HostView algoWindow = new HostView(algo.RuleEnum);
            algoWindow.Closed += (s, arg) =>
            {
                switch (algo.RuleEnum)
                {
                    case Models.Enums.AdministrativeAlgoRuleEnum.ClassHourRequiredStartingTime:
                        algo.Record = algoRule.ClassHourRequiredStartingTime.Count;
                        break;

                    case Models.Enums.AdministrativeAlgoRuleEnum.ClassHourRequiredStartingTimes:
                        algo.Record = algoRule.ClassHourRequiredStartingTimes.Count;
                        break;

                    case Models.Enums.AdministrativeAlgoRuleEnum.ClassHourRequiredTimes:
                        algo.Record = algoRule.ClassHourRequiredTimes.Count;
                        break;

                    case Models.Enums.AdministrativeAlgoRuleEnum.ClassHoursMaxConcurrencyInSelectedTime:
                        algo.Record = algoRule.ClassHoursMaxConcurrencyInSelectedTimes.Count;
                        break;

                    case Models.Enums.AdministrativeAlgoRuleEnum.ClassHoursNotOverlap:
                        algo.Record = algoRule.ClassHoursNotOverlaps.Count;
                        break;

                    case Models.Enums.AdministrativeAlgoRuleEnum.ClassHoursOccupyMaxTimeFromSelection:
                        algo.Record = algoRule.ClassHoursOccupyMaxTimeFromSelections.Count;
                        break;

                    case Models.Enums.AdministrativeAlgoRuleEnum.ClassHoursRequiredStartingTimes:
                        algo.Record = algoRule.ClassHoursRequiredStartingTimes.Count;
                        break;

                    case Models.Enums.AdministrativeAlgoRuleEnum.ClassHoursRequiredTimes:
                        algo.Record = algoRule.ClassHoursRequiredTimes.Count;
                        break;

                    case Models.Enums.AdministrativeAlgoRuleEnum.ClassHoursSameStarting:
                        algo.Record = algoRule.ClassHoursSameStartingDays.Count;
                        break;

                    case Models.Enums.AdministrativeAlgoRuleEnum.ClassHoursSameStartingHour:
                        algo.Record = algoRule.ClassHoursSameStartingHours.Count;
                        break;

                    case Models.Enums.AdministrativeAlgoRuleEnum.ClassHoursSameStartingTime:
                        algo.Record = algoRule.ClassHoursSameStartingTimes.Count;
                        break;

                    case Models.Enums.AdministrativeAlgoRuleEnum.MaxDaysBetweenClassHours:
                        algo.Record = algoRule.MaxDaysBetweenClassHours.Count;
                        break;

                    case Models.Enums.AdministrativeAlgoRuleEnum.MinDaysBetweenClassHours:
                        algo.Record = algoRule.MinDaysBetweenClassHours.Count;
                        break;

                    case Models.Enums.AdministrativeAlgoRuleEnum.TeacherMaxDaysPerWeek:
                        algo.Record = algoRule.TeacherMaxDaysPerWeeks.Count;
                        break;

                    case Models.Enums.AdministrativeAlgoRuleEnum.TeacherMaxGapsPerDay:
                        algo.Record = algoRule.TeacherMaxGapsPerDays.Count;
                        break;

                    case Models.Enums.AdministrativeAlgoRuleEnum.TeacherMaxHoursDaily:
                        algo.Record = algoRule.TeacherMaxHoursDailys.Count;
                        break;

                    case Models.Enums.AdministrativeAlgoRuleEnum.TeacherNotAvailableTimes:
                        algo.Record = algoRule.TeacherNotAvailableTimes.Count;
                        break;

                    case Models.Enums.AdministrativeAlgoRuleEnum.TeachersMaxDaysPerWeek:
                        algo.Record = algoRule.TeachersMaxDaysPerWeeks.Count;
                        break;

                    case Models.Enums.AdministrativeAlgoRuleEnum.TeachersMaxGapsPerDay:
                        algo.Record = algoRule.TeachersMaxGapsPerDays.Count;
                        break;

                    case Models.Enums.AdministrativeAlgoRuleEnum.TeachersMaxHoursDaily:
                        algo.Record = algoRule.TeachersMaxHoursDailys.Count;
                        break;

                    case Models.Enums.AdministrativeAlgoRuleEnum.ThreeClassHoursGrouped:
                        algo.Record = algoRule.ThreeClassHoursGrouped.Count;
                        break;

                    case Models.Enums.AdministrativeAlgoRuleEnum.TwoClassHoursContinuous:
                        algo.Record = algoRule.TwoClassHoursContinuous.Count;
                        break;

                    case Models.Enums.AdministrativeAlgoRuleEnum.TwoClassHoursGrouped:
                        algo.Record = algoRule.TwoClassHoursGrouped.Count;
                        break;

                    case Models.Enums.AdministrativeAlgoRuleEnum.TwoClassHoursOrdered:
                        algo.Record = algoRule.TwoClassHoursOrdered.Count;
                        break;
                }
            };
            algoWindow.ShowDialog();
        }

        void loadRules()
        {
            var rule = CommonDataManager.GetAminRule(base.LocalID);
            var algo = CommonDataManager.GetAminAlgoRule(base.LocalID);

            #region 教师

            var rules = new List<UIRule>();
            rules.Add(new UIRule()
            {
                NO = 1,
                Name = "教师排课时间",
                Category = Models.Enums.RuleCategoryEnum.Teacher,
                RuleEnum = Models.Enums.AdministrativeRuleEnum.TeacherTime,
                Record = rule.TeacherTimes.Count,
                Comment = CommonDataManager.GetAdminRuleComments(Models.Enums.AdministrativeRuleEnum.TeacherTime)
            });

            rules.Add(
               new UIRule
               {
                   NO = 2,
                   Name = "教师每天最大课时数",
                   Category = Models.Enums.RuleCategoryEnum.Teacher,
                   RuleEnum = Models.Enums.AdministrativeRuleEnum.TeacherMaxHoursDaily,
                   Record = rule.MaxHoursDaily.Count,
                   Comment = CommonDataManager.GetAdminRuleComments(Models.Enums.AdministrativeRuleEnum.TeacherMaxHoursDaily)
               });

            rules.Add(
                new UIRule
                {
                    NO = 3,
                    Name = "教师每周最大工作天数",
                    Category = Models.Enums.RuleCategoryEnum.Teacher,
                    RuleEnum = Models.Enums.AdministrativeRuleEnum.TeacherMaxDaysPerWeek,
                    Record = rule.MaxDaysPerWeek.Count,
                    Comment = CommonDataManager.GetAdminRuleComments(Models.Enums.AdministrativeRuleEnum.TeacherMaxDaysPerWeek)
                });

            rules.Add(
                new UIRule
                {
                    NO = 4,
                    Name = "教师每天最大课时间隔",
                    Category = Models.Enums.RuleCategoryEnum.Teacher,
                    RuleEnum = Models.Enums.AdministrativeRuleEnum.TeacherMaxGapsPerDay,
                    Record = rule.MaxGapsPerDay.Count,
                    Comment = CommonDataManager.GetAdminRuleComments(Models.Enums.AdministrativeRuleEnum.TeacherMaxGapsPerDay)
                });

            rules.Add(
                new UIRule
                {
                    NO = 5,
                    Name = "教师上下午不连排",
                    Category = Models.Enums.RuleCategoryEnum.Teacher,
                    RuleEnum = Models.Enums.AdministrativeRuleEnum.TeacherAmPmNoContinues,
                    Record = rule.AmPmNoContinues.Count,
                    Comment = CommonDataManager.GetAdminRuleComments(Models.Enums.AdministrativeRuleEnum.TeacherAmPmNoContinues)
                });

            rules.Add(
                new UIRule
                {
                    NO = 6,
                    Name = "连排齐头",
                    Category = Models.Enums.RuleCategoryEnum.Teacher,
                    RuleEnum = Models.Enums.AdministrativeRuleEnum.ArrangeContinuousPlanFlush,
                    Record = rule.ContinuousPlanFlushes.Count,
                    Comment = CommonDataManager.GetAdminRuleComments(Models.Enums.AdministrativeRuleEnum.ArrangeContinuousPlanFlush)
                });

            rules.Add(
                new UIRule
                {
                    NO = 7,
                    Name = "教案齐头",
                    Category = Models.Enums.RuleCategoryEnum.Teacher,
                    RuleEnum = Models.Enums.AdministrativeRuleEnum.TeachingPlanFlush,
                    Record = rule.PlanFlushes.Count,
                    Comment = CommonDataManager.GetAdminRuleComments(Models.Enums.AdministrativeRuleEnum.TeachingPlanFlush)
                });

            rules.Add(
               new UIRule
               {
                   NO = 8,
                   Name = "教案平头",
                   Category = Models.Enums.RuleCategoryEnum.Teacher,
                   RuleEnum = Models.Enums.AdministrativeRuleEnum.TeacherPriorityBalanceRule,
                   Record = rule.ClassHourPriorityBalance.Count,
                   Comment = CommonDataManager.GetAdminRuleComments(Models.Enums.AdministrativeRuleEnum.TeacherPriorityBalanceRule)
               });

            rules.Add(
               new UIRule
               {
                   NO = 9,
                   Name = "教师半天上课",
                   Category = Models.Enums.RuleCategoryEnum.Teacher,
                   RuleEnum = Models.Enums.AdministrativeRuleEnum.TeacherHalfDayWorkRule,
                   Record = rule.HalfDayWork.Count,
                   Comment = CommonDataManager.GetAdminRuleComments(Models.Enums.AdministrativeRuleEnum.TeacherHalfDayWorkRule)
               });

            rules.Add(
                new UIRule
                {
                    NO = 10,
                    Name = "师徒跟随",
                    Category = Models.Enums.RuleCategoryEnum.Teacher,
                    RuleEnum = Models.Enums.AdministrativeRuleEnum.MasterApprenttice,
                    Record = rule.MasterApprenttices.Count,
                    Comment = CommonDataManager.GetAdminRuleComments(Models.Enums.AdministrativeRuleEnum.MasterApprenttice)
                });

            #endregion

            #region 课时

            rules.Add(
                new UIRule
                {
                    NO = 11,
                    Name = "课时分散",
                    Category = Models.Enums.RuleCategoryEnum.Course,
                    RuleEnum = Models.Enums.AdministrativeRuleEnum.ClassHourAverage,
                    Record = rule.ClassHourAverages.Count,
                    Comment = CommonDataManager.GetAdminRuleComments(Models.Enums.AdministrativeRuleEnum.ClassHourAverage)
                });

            rules.Add(
                new UIRule
                {
                    NO = 12,
                    Name = "同时开课",
                    Category = Models.Enums.RuleCategoryEnum.Course,
                    RuleEnum = Models.Enums.AdministrativeRuleEnum.ClassHourSameOpen,
                    Record = rule.ClassHourSameOpens.Count,
                    Comment = CommonDataManager.GetAdminRuleComments(Models.Enums.AdministrativeRuleEnum.ClassHourSameOpen)
                });

            #endregion

            #region 课程

            rules.Add(
            new UIRule
            {
                NO = 13,
                Name = "课程互斥",
                Category = Models.Enums.RuleCategoryEnum.Course,
                RuleEnum = Models.Enums.AdministrativeRuleEnum.MutexGroup,
                Record = rule.Mutexes.Count,
                Comment = CommonDataManager.GetAdminRuleComments(Models.Enums.AdministrativeRuleEnum.MutexGroup)
            });

            rules.Add(
             new UIRule
             {
                 NO = 14,
                 Name = "合班上课",
                 Category = Models.Enums.RuleCategoryEnum.Course,
                 RuleEnum = Models.Enums.AdministrativeRuleEnum.ClassUnion,
                 Record = rule.ClassUnions.Count,
                 Comment = CommonDataManager.GetAdminRuleComments(Models.Enums.AdministrativeRuleEnum.ClassUnion)
             });

            rules.Add(
            new UIRule
            {
                NO = 15,
                Name = "上下午课时",
                Category = Models.Enums.RuleCategoryEnum.Course,
                RuleEnum = Models.Enums.AdministrativeRuleEnum.AmPmClassHour,
                Record = rule.AmPmClassHours.Count,
                Comment = CommonDataManager.GetAdminRuleComments(Models.Enums.AdministrativeRuleEnum.AmPmClassHour)
            });

            rules.Add(
            new UIRule
            {
                NO = 16,
                Name = "单双周",
                Category = Models.Enums.RuleCategoryEnum.Course,
                RuleEnum = Models.Enums.AdministrativeRuleEnum.OddDualWeek,
                Record = rule.OddDualWeeks.Count,
                Comment = CommonDataManager.GetAdminRuleComments(Models.Enums.AdministrativeRuleEnum.OddDualWeek)
            });

            rules.Add(
            new UIRule
            {
                NO = 17,
                Name = "课程连排",
                Category = Models.Enums.RuleCategoryEnum.Course,
                RuleEnum = Models.Enums.AdministrativeRuleEnum.CourseArrangeContinuous,
                Record = rule.CourseArranges.Count,
                Comment = CommonDataManager.GetAdminRuleComments(Models.Enums.AdministrativeRuleEnum.CourseArrangeContinuous)
            });

            rules.Add(
            new UIRule
            {
                NO = 18,
                Name = "课程排课时间",
                Category = Models.Enums.RuleCategoryEnum.Course,
                RuleEnum = Models.Enums.AdministrativeRuleEnum.CourseTime,
                Record = rule.CourseTimes.Count,
                Comment = CommonDataManager.GetAdminRuleComments(Models.Enums.AdministrativeRuleEnum.CourseTime)
            });

            rules.Add(
            new UIRule
            {
                NO = 19,
                Name = "同时开课限制",
                Category = Models.Enums.RuleCategoryEnum.Course,
                RuleEnum = Models.Enums.AdministrativeRuleEnum.CourseLimit,
                Record = rule.CourseLimits.Count,
                Comment = CommonDataManager.GetAdminRuleComments(Models.Enums.AdministrativeRuleEnum.CourseLimit)
            });

            rules.Add(
            new UIRule
            {
                NO = 20,
                Name = "锁定课表",
                Category = Models.Enums.RuleCategoryEnum.Course,
                RuleEnum = Models.Enums.AdministrativeRuleEnum.LockedCourse,
                Record = 0,
                Comment = CommonDataManager.GetAdminRuleComments(Models.Enums.AdministrativeRuleEnum.LockedCourse)
            });

            #endregion

            this.Rules = rules;
        }

        void loadAlgoes()
        {
            var algo = CommonDataManager.GetAminAlgoRule(base.LocalID);

            #region 教师

            var algoRules = new List<UIAlgoRule>();
            algoRules.Add(new UIAlgoRule()
            {
                NO = 1,
                Name = "教师的不可用时间",
                Category = Models.Enums.RuleCategoryEnum.Teacher,
                RuleEnum = Models.Enums.AdministrativeAlgoRuleEnum.TeacherNotAvailableTimes,
                Record = algo.TeacherNotAvailableTimes.Count,
                Comment = CommonDataManager.GetAdminAlgoComments(Models.Enums.AdministrativeAlgoRuleEnum.TeacherNotAvailableTimes)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 2,
                Name = "教师每周最大工作天数",
                Category = Models.Enums.RuleCategoryEnum.Teacher,
                RuleEnum = Models.Enums.AdministrativeAlgoRuleEnum.TeacherMaxDaysPerWeek,
                Record = algo.TeacherMaxDaysPerWeeks.Count,
                Comment = CommonDataManager.GetAdminAlgoComments(Models.Enums.AdministrativeAlgoRuleEnum.TeacherMaxDaysPerWeek)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 3,
                Name = "教师每天最大课时数",
                Category = Models.Enums.RuleCategoryEnum.Teacher,
                RuleEnum = Models.Enums.AdministrativeAlgoRuleEnum.TeacherMaxHoursDaily,
                Record = algo.TeacherMaxHoursDailys.Count,
                Comment = CommonDataManager.GetAdminAlgoComments(Models.Enums.AdministrativeAlgoRuleEnum.TeacherMaxHoursDaily)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 4,
                Name = "教师每天最大课程间隔",
                Category = Models.Enums.RuleCategoryEnum.Teacher,
                RuleEnum = Models.Enums.AdministrativeAlgoRuleEnum.TeacherMaxGapsPerDay,
                Record = algo.TeacherMaxGapsPerDays.Count,
                Comment = CommonDataManager.GetAdminAlgoComments(Models.Enums.AdministrativeAlgoRuleEnum.TeacherMaxGapsPerDay)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 5,
                Name = "所有教师每周最大工作天数",
                Category = Models.Enums.RuleCategoryEnum.Teacher,
                RuleEnum = Models.Enums.AdministrativeAlgoRuleEnum.TeachersMaxDaysPerWeek,
                Record = algo.TeachersMaxDaysPerWeeks.Count,
                Comment = CommonDataManager.GetAdminAlgoComments(Models.Enums.AdministrativeAlgoRuleEnum.TeachersMaxDaysPerWeek)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 6,
                Name = "所有教师每天最大课程间隔",
                Category = Models.Enums.RuleCategoryEnum.Teacher,
                RuleEnum = Models.Enums.AdministrativeAlgoRuleEnum.TeachersMaxGapsPerDay,
                Record = algo.TeachersMaxGapsPerDays.Count,
                Comment = CommonDataManager.GetAdminAlgoComments(Models.Enums.AdministrativeAlgoRuleEnum.TeachersMaxGapsPerDay)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 7,
                Name = "所有教师每天最大课时数",
                Category = Models.Enums.RuleCategoryEnum.Teacher,
                RuleEnum = Models.Enums.AdministrativeAlgoRuleEnum.TeachersMaxHoursDaily,
                Record = algo.TeachersMaxHoursDailys.Count,
                Comment = CommonDataManager.GetAdminAlgoComments(Models.Enums.AdministrativeAlgoRuleEnum.TeachersMaxHoursDaily)
            });

            #endregion

            #region 课时

            algoRules.Add(new UIAlgoRule()
            {
                NO = 8,
                Name = "单个课时有多个优先开始时间",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.AdministrativeAlgoRuleEnum.ClassHourRequiredStartingTimes,
                Record = algo.ClassHourRequiredStartingTimes.Count,
                Comment = CommonDataManager.GetAdminAlgoComments(Models.Enums.AdministrativeAlgoRuleEnum.ClassHourRequiredStartingTimes)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 9,
                Name = "单个课时有多个优先课位",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.AdministrativeAlgoRuleEnum.ClassHourRequiredTimes,
                Record = algo.ClassHourRequiredTimes.Count,
                Comment = CommonDataManager.GetAdminAlgoComments(Models.Enums.AdministrativeAlgoRuleEnum.ClassHourRequiredTimes)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 10,
                Name = "单个课时有一个优先开始时间",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.AdministrativeAlgoRuleEnum.ClassHourRequiredStartingTime,
                Record = algo.ClassHourRequiredStartingTime.Count,
                Comment = CommonDataManager.GetAdminAlgoComments(Models.Enums.AdministrativeAlgoRuleEnum.ClassHourRequiredStartingTime)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 11,
                Name = "多个课时间最小间隔天数",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.AdministrativeAlgoRuleEnum.MinDaysBetweenClassHours,
                Record = algo.MinDaysBetweenClassHours.Count,
                Comment = CommonDataManager.GetAdminAlgoComments(Models.Enums.AdministrativeAlgoRuleEnum.MinDaysBetweenClassHours)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 12,
                Name = "多个课时间最大间隔天数",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.AdministrativeAlgoRuleEnum.MaxDaysBetweenClassHours,
                Record = algo.MaxDaysBetweenClassHours.Count,
                Comment = CommonDataManager.GetAdminAlgoComments(Models.Enums.AdministrativeAlgoRuleEnum.MaxDaysBetweenClassHours)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 13,
                Name = "多个课时有多个优先开始时间",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.AdministrativeAlgoRuleEnum.ClassHoursRequiredStartingTimes,
                Record = algo.ClassHoursRequiredStartingTimes.Count,
                Comment = CommonDataManager.GetAdminAlgoComments(Models.Enums.AdministrativeAlgoRuleEnum.ClassHoursRequiredStartingTimes)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 14,
                Name = "多个课时有多个优先课位",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.AdministrativeAlgoRuleEnum.ClassHoursRequiredTimes,
                Record = algo.ClassHoursRequiredTimes.Count,
                Comment = CommonDataManager.GetAdminAlgoComments(Models.Enums.AdministrativeAlgoRuleEnum.ClassHoursRequiredTimes)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 15,
                Name = "多个课时不同时开课",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.AdministrativeAlgoRuleEnum.ClassHoursNotOverlap,
                Record = algo.ClassHoursNotOverlaps.Count,
                Comment = CommonDataManager.GetAdminAlgoComments(Models.Enums.AdministrativeAlgoRuleEnum.ClassHoursNotOverlap)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 16,
                Name = "多个课时在选定课位中占用的最大数量",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.AdministrativeAlgoRuleEnum.ClassHoursOccupyMaxTimeFromSelection,
                Record = algo.ClassHoursOccupyMaxTimeFromSelections.Count,
                Comment = CommonDataManager.GetAdminAlgoComments(Models.Enums.AdministrativeAlgoRuleEnum.ClassHoursOccupyMaxTimeFromSelection)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 17,
                Name = "多个课时在选定课位中同时开课最大数量",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.AdministrativeAlgoRuleEnum.ClassHoursMaxConcurrencyInSelectedTime,
                Record = algo.ClassHoursMaxConcurrencyInSelectedTimes.Count,
                Comment = CommonDataManager.GetAdminAlgoComments(Models.Enums.AdministrativeAlgoRuleEnum.ClassHoursMaxConcurrencyInSelectedTime)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 18,
                Name = "多个课时有相同的开始日期（日期）",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.AdministrativeAlgoRuleEnum.ClassHoursSameStarting,
                Record = algo.ClassHoursSameStartingDays.Count,
                Comment = CommonDataManager.GetAdminAlgoComments(Models.Enums.AdministrativeAlgoRuleEnum.ClassHoursSameStarting)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 19,
                Name = "多个课时有相同的开始课位（时间）",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.AdministrativeAlgoRuleEnum.ClassHoursSameStartingHour,
                Record = algo.ClassHoursSameStartingHours.Count,
                Comment = CommonDataManager.GetAdminAlgoComments(Models.Enums.AdministrativeAlgoRuleEnum.ClassHoursSameStartingHour)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 20,
                Name = "多个课时有相同的开始时间（日期+时间）",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.AdministrativeAlgoRuleEnum.ClassHoursSameStartingTime,
                Record = algo.ClassHoursSameStartingTimes.Count,
                Comment = CommonDataManager.GetAdminAlgoComments(Models.Enums.AdministrativeAlgoRuleEnum.ClassHoursSameStartingTime)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 21,
                Name = "对2个课时设置连排",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.AdministrativeAlgoRuleEnum.TwoClassHoursContinuous,
                Record = algo.TwoClassHoursContinuous.Count,
                Comment = CommonDataManager.GetAdminAlgoComments(Models.Enums.AdministrativeAlgoRuleEnum.TwoClassHoursContinuous)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 22,
                Name = "对2个课时排序",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.AdministrativeAlgoRuleEnum.TwoClassHoursOrdered,
                Record = algo.TwoClassHoursOrdered.Count,
                Comment = CommonDataManager.GetAdminAlgoComments(Models.Enums.AdministrativeAlgoRuleEnum.TwoClassHoursOrdered)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 23,
                Name = "给2个课时分组",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.AdministrativeAlgoRuleEnum.TwoClassHoursGrouped,
                Record = algo.TwoClassHoursGrouped.Count,
                Comment = CommonDataManager.GetAdminAlgoComments(Models.Enums.AdministrativeAlgoRuleEnum.TwoClassHoursGrouped)
            });

            algoRules.Add(new UIAlgoRule()
            {
                NO = 24,
                Name = "给3个课时分组",
                Category = Models.Enums.RuleCategoryEnum.ClassHour,
                RuleEnum = Models.Enums.AdministrativeAlgoRuleEnum.ThreeClassHoursGrouped,
                Record = algo.ThreeClassHoursGrouped.Count,
                Comment = CommonDataManager.GetAdminAlgoComments(Models.Enums.AdministrativeAlgoRuleEnum.ThreeClassHoursGrouped)
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
    }
}
