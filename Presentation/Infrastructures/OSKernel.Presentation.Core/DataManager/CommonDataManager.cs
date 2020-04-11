using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSKernel.Presentation.Models;
using OSKernel.Presentation.Models.Enums;
using XYKernel.OS.Common.Models.Administrative;
using XYKernel.OS.Common.Models.Mixed;
using XYKernel.OS.Common.Models.Mixed.AlgoRule;
using XYKernel.OS.Common.Models.Mixed.Rule;

namespace OSKernel.Presentation.Core.DataManager
{
    public class CommonDataManager : ICommonDataManager
    {
        public IList<Case> LocalCases
        {
            get; set;
        }

        public Dictionary<string, CPCase> CPCases
        {
            get; set;
        }

        public Dictionary<string, CLCase> CLCases
        {
            get; set;
        }

        public Dictionary<string, XYKernel.OS.Common.Models.Administrative.Rule.Rule> AdminRules
        {
            get; set;
        }

        public Dictionary<string, XYKernel.OS.Common.Models.Mixed.Rule.Rule> MixedRules
        {
            get; set;
        }

        public Dictionary<string, XYKernel.OS.Common.Models.Administrative.AlgoRule.AlgoRule> AdminAlgoes
        {
            get; set;
        }

        public Dictionary<string, XYKernel.OS.Common.Models.Mixed.AlgoRule.AlgoRule> MixedAlgoes
        {
            get; set;
        }

        public string LocalID
        {
            get; set;
        }

        public Dictionary<AdministrativeAlgoRuleEnum, string> AdministrativeAlgoComments
        {
            get; set;
        }

        public Dictionary<MixedAlgoRuleEnum, string> MixedAlgoComments
        {
            get; set;
        }

        public Dictionary<AdministrativeRuleEnum, string> AdministrativeRuleComments
        {
            get; set;
        }

        public Dictionary<MixedRuleEnum, string> MixedRuleComments
        {
            get; set;
        }

        public CommonDataManager()
        {
            LocalCases = new List<Case>();

            CPCases = new Dictionary<string, CPCase>();
            AdminRules = new Dictionary<string, XYKernel.OS.Common.Models.Administrative.Rule.Rule>();
            AdminAlgoes = new Dictionary<string, XYKernel.OS.Common.Models.Administrative.AlgoRule.AlgoRule>();

            CLCases = new Dictionary<string, CLCase>();
            MixedRules = new Dictionary<string, Rule>();
            MixedAlgoes = new Dictionary<string, AlgoRule>();

            AdministrativeAlgoComments = new Dictionary<AdministrativeAlgoRuleEnum, string>()
            {
                {AdministrativeAlgoRuleEnum.TeacherNotAvailableTimes,"用于禁止把某位教师的课排在某些位置上；" },
                {AdministrativeAlgoRuleEnum.TeacherMaxDaysPerWeek,"用于限制某位教师每周可以排课的天数；" },
                {AdministrativeAlgoRuleEnum.TeacherMaxHoursDaily,"用于限制某位教师每天可以上课的最大课时数；" },
                {AdministrativeAlgoRuleEnum.TeacherMaxGapsPerDay,"用于限制教师课时天内集中或分散；" },
                {AdministrativeAlgoRuleEnum.TeachersMaxDaysPerWeek,"用于统一设置所有教师每周可以排课的天数；" },
                {AdministrativeAlgoRuleEnum.TeachersMaxGapsPerDay,"用于统一设置所有教师一天内课时集中或分散；" },
                {AdministrativeAlgoRuleEnum.TeachersMaxHoursDaily,"用于统一设置所有教师每天可以上课的最大课时数；" },
                {AdministrativeAlgoRuleEnum.ClassHourRequiredStartingTimes,"用于设置某一个课时必须排在哪些区域中；多用于连排的2个课时限定第一个课时上课的优先区域；" },
                {AdministrativeAlgoRuleEnum.ClassHourRequiredTimes,"用于设置某一个课时必须排在哪些位置上；多用于固定某课时的上课位置；" },
                {AdministrativeAlgoRuleEnum.ClassHourRequiredStartingTime,"用于设置某位教师的其中一个课时固定排在某个位置上；" },
                {AdministrativeAlgoRuleEnum.MinDaysBetweenClassHours,"用于设置同课程或不同课程的某些课时最小间隔天数；" },
                {AdministrativeAlgoRuleEnum.MaxDaysBetweenClassHours,"用于设置同课程或不同课程的某些课时最大间隔天数；" },
                {AdministrativeAlgoRuleEnum.ClassHoursRequiredStartingTimes,"用于设置某个课程必须排在哪些区域中；" },
                {AdministrativeAlgoRuleEnum.ClassHoursRequiredTimes,"用于设置某个课程必须排在哪些位置上；" },
                {AdministrativeAlgoRuleEnum.ClassHoursNotOverlap,"可用于设置同课程的多个课时不同时上课，或不同课程的多个课时不同时上课；" },
                {AdministrativeAlgoRuleEnum.ClassHoursOccupyMaxTimeFromSelection,"用于限制一门或多门课程在选定位置上的最多开课节数；比如限制教师第一节和最后一节不同时出现；" },
                {AdministrativeAlgoRuleEnum.ClassHoursMaxConcurrencyInSelectedTime,"用于限制一门课程或多门课程在同一位置上的最多开课节数；比如限制同一时段只能开1节微机课；" },
                {AdministrativeAlgoRuleEnum.ClassHoursSameStarting,"用于设置同课程或多个课程在同一天排课；" },
                {AdministrativeAlgoRuleEnum.ClassHoursSameStartingHour,"用于设置同课程或多个课程在同一个位置排课；" },
                {AdministrativeAlgoRuleEnum.ClassHoursSameStartingTime,"用于设置同课程或多个课程在同一天、同一个位置排课；" },
                {AdministrativeAlgoRuleEnum.TwoClassHoursContinuous,"用于指定任意科目的2个课时进行连排；" },
                {AdministrativeAlgoRuleEnum.TwoClassHoursOrdered,"用于设置指定2个课时的先后顺序，第一课时会排在第二课时前面；" },
                {AdministrativeAlgoRuleEnum.TwoClassHoursGrouped,"用于指定任意科目的2个课时进行连排，无分先后；" },
                {AdministrativeAlgoRuleEnum.ThreeClassHoursGrouped,"用于指定任意科目的3个课时进行连排，无分先后；" },
            };

            MixedAlgoComments = new Dictionary<MixedAlgoRuleEnum, string>()
            {
                {MixedAlgoRuleEnum.TeacherNotAvailableTimes,"用于禁止把某位教师的课排在某些位置上；" },
                {MixedAlgoRuleEnum.TeacherMaxDaysPerWeek,"用于限制某位教师每周可以排课的天数；" },
                {MixedAlgoRuleEnum.TeacherMaxHoursDaily,"用于限制某位教师每天可以上课的最大课时数；" },
                {MixedAlgoRuleEnum.TeacherMaxGapsPerDay,"用于限制教师课时天内集中或分散；" },
                {MixedAlgoRuleEnum.TeachersMaxDaysPerWeek,"用于统一设置所有教师每周可以排课的天数；" },
                {MixedAlgoRuleEnum.TeachersMaxGapsPerDay,"用于统一设置所有教师一天内课时集中或分散；" },
                {MixedAlgoRuleEnum.TeachersMaxHoursDaily,"用于统一设置所有教师每天可以上课的最大课时数；" },
                {MixedAlgoRuleEnum.ClassHourRequiredStartingTimes,"用于设置某一个课时必须排在哪些区域中；多用于连排的2个课时限定第一个课时上课的优先区域；" },
                {MixedAlgoRuleEnum.ClassHourRequiredTimes,"用于设置某一个课时必须排在哪些位置上；多用于固定某课时的上课位置；" },
                {MixedAlgoRuleEnum.ClassHourRequiredStartingTime,"用于设置某位教师的其中一个课时固定排在某个位置上；" },
                {MixedAlgoRuleEnum.MinDaysBetweenClassHours,"用于设置同课程或不同课程的某些课时最小间隔天数；" },
                {MixedAlgoRuleEnum.MaxDaysBetweenClassHours,"用于设置同课程或不同课程的某些课时最大间隔天数；" },
                {MixedAlgoRuleEnum.ClassHoursRequiredStartingTimes,"用于设置某个课程必须排在哪些区域中；" },
                {MixedAlgoRuleEnum.ClassHoursRequiredTimes,"用于设置某个课程必须排在哪些位置上；" },
                {MixedAlgoRuleEnum.ClassHoursNotOverlap,"可用于设置同课程的多个课时不同时上课，或不同课程的多个课时不同时上课；" },
                {MixedAlgoRuleEnum.ClassHoursOccupyMaxTimeFromSelection,"用于限制一门或多门课程在选定位置上的最多开课节数；比如限制教师第一节和最后一节不同时出现；" },
                {MixedAlgoRuleEnum.ClassHoursMaxConcurrencyInSelectedTime,"用于限制一门课程或多门课程在同一位置上的最多开课节数；比如限制同一时段只能开1节微机课；" },
                {MixedAlgoRuleEnum.ClassHoursSameStarting,"用于设置同课程或多个课程在同一天排课；" },
                {MixedAlgoRuleEnum.ClassHoursSameStartingHour,"用于设置同课程或多个课程在同一个位置排课；" },
                {MixedAlgoRuleEnum.ClassHoursSameStartingTime,"用于设置同课程或多个课程在同一天、同一个位置排课；" },
                {MixedAlgoRuleEnum.TwoClassHoursContinuous,"用于指定任意科目的2个课时进行连排；" },
                {MixedAlgoRuleEnum.TwoClassHoursOrdered,"用于设置指定2个课时的先后顺序，第一课时会排在第二课时前面；" },
                {MixedAlgoRuleEnum.TwoClassHoursGrouped,"用于指定任意科目的2个课时进行连排，无分先后；" },
                {MixedAlgoRuleEnum.ThreeClassHoursGrouped,"用于指定任意科目的3个课时进行连排，无分先后；" },
            };

            AdministrativeRuleComments = new Dictionary<AdministrativeRuleEnum, string>()
            {
                {AdministrativeRuleEnum.TeacherTime,"用于禁止或固定把某位教师的课排在某些位置上；" },
                {AdministrativeRuleEnum.TeacherMaxHoursDaily,"用于限制教师每天可以上课的最大课时数；双击每位教师的最大课时数可单独修改；" },
                {AdministrativeRuleEnum.TeacherMaxDaysPerWeek,"用于限制教师每周可以排课的天数；双击每位教师的最大天数可单独修改；" },
                {AdministrativeRuleEnum.TeacherMaxGapsPerDay,"用于设置教师一天内课时的最大间隔；双击每位教师的最大课时间隔可单独修改；" },
                {AdministrativeRuleEnum.TeacherAmPmNoContinues,"用于限制同一教师上午最后一节和下午第一节不连排；" },
                {AdministrativeRuleEnum.ArrangeContinuousPlanFlush,"用于控制同一教师多个班的连排课，教案同进度；双击每位教师的齐头天数可单独修改；" },
                {AdministrativeRuleEnum.TeachingPlanFlush,"用于控制同一教师多个班的课，教案同进度；双击每位教师的齐头天数可单独修改；" },
                {AdministrativeRuleEnum.TeacherPriorityBalanceRule,"用于控制同一教师多个班的课，先后交替上课；比如上课顺序是星期一：1、2班，星期二：2、1班；" },
                {AdministrativeRuleEnum.TeacherHalfDayWorkRule,"用于设置同一教师当天所有课时集中在半天内上课；" },
                {AdministrativeRuleEnum.MasterApprenttice,"用于设置创建师徒关系后的相关老师上课时间是错开的；" },
                {AdministrativeRuleEnum.ClassHourAverage,"用于设置教师的周内课时均衡；双击每门课程的最小间隔天数可单独修改；" },
                {AdministrativeRuleEnum.ClassHourSameOpen,"用于设置课程同时上课，设置后相应班级的课程会排在相同的课位；" },
                {AdministrativeRuleEnum.MutexGroup,"创建互斥的课程不排在同一天；" },
                {AdministrativeRuleEnum.ClassUnion,"指定某些班级的某门课合班上课，合班上课的课程会排在相同的课位；" },
                {AdministrativeRuleEnum.AmPmClassHour,"用于设置一周内某课程排在上午的课时数和排在下午的课时数；" },
                {AdministrativeRuleEnum.OddDualWeek,"用于设置需要单周双周交替上课的课程；" },
                {AdministrativeRuleEnum.CourseArrangeContinuous,"用于指定课程每周连排次数以及需要排在哪些课位；" },
                {AdministrativeRuleEnum.CourseTime,"用于禁止或固定把课程排在某些位置上；" },
                {AdministrativeRuleEnum.CourseLimit,"用于限制同一时间最多几个班可以上某门课；" },
                {AdministrativeRuleEnum.LockedCourse,"用于将上一版排课结果的某部分课表锁定，锁定后此位置不参与本次排课，其他位置重排；" },
            };

            MixedRuleComments = new Dictionary<MixedRuleEnum, string>()
            {
                {MixedRuleEnum.TeacherTime,"用于禁止或固定把某位教师的课排在某些位置上；" },
                {MixedRuleEnum.TeacherMaxHoursDaily,"用于限制教师每天可以上课的最大课时数；双击每位教师的最大课时数可单独修改；" },
                {MixedRuleEnum.TeacherMaxDaysPerWeek,"用于限制教师每周可以排课的天数；双击每位教师的最大天数可单独修改；" },
                {MixedRuleEnum.TeacherMaxGapsPerDay,"用于设置教师一天内课时的最大间隔；双击每位教师的最大课时间隔可单独修改；" },
                {MixedRuleEnum.MasterApprenttice,"用于设置创建师徒关系后的相关老师上课时间是错开的；" },
                {MixedRuleEnum.ClassHourAverage,"用于设置教师的周内课时均衡；双击每门课程的最小间隔天数可单独修改；" },
                {MixedRuleEnum.ClassHourSameOpen,"用于设置课程同时上课，设置后相应班级的课程会排在相同的课位；" },
                {MixedRuleEnum.AmPmClassHour,"用于设置一周内某课程排在上午的课时数和排在下午的课时数；" },
                {MixedRuleEnum.CourseArrangeContinuous,"用于指定课程每周连排次数以及需要排在哪些课位；" },
                {MixedRuleEnum.CourseTime,"用于禁止或固定把课程排在某些位置上；" },
                {MixedRuleEnum.CourseLimit,"用于限制同一时间最多几个班可以上某门课；" },
                {MixedRuleEnum.AvailableRoom,"可用教室数量；" },
            };
        }

        public XYKernel.OS.Common.Models.Administrative.Rule.Rule GetAminRule(string localID)
        {
            if (AdminRules.ContainsKey(localID))
                return AdminRules[localID];
            else
            {
                var rule = new XYKernel.OS.Common.Models.Administrative.Rule.Rule();
                AdminRules.Add(localID, rule);
                return rule;
            }
        }

        public XYKernel.OS.Common.Models.Mixed.Rule.Rule GetMixedRule(string localID)
        {
            if (MixedRules.ContainsKey(localID))
                return MixedRules[localID];
            else
            {
                var rule = new XYKernel.OS.Common.Models.Mixed.Rule.Rule();
                MixedRules.Add(localID, rule);
                return rule;
            }
        }

        public XYKernel.OS.Common.Models.Administrative.AlgoRule.AlgoRule GetAminAlgoRule(string localID)
        {
            if (AdminAlgoes.ContainsKey(localID))
                return AdminAlgoes[localID];
            else
            {
                var newAlgo = new XYKernel.OS.Common.Models.Administrative.AlgoRule.AlgoRule();
                AdminAlgoes.Add(localID, newAlgo);

                return newAlgo;
            }
        }

        public XYKernel.OS.Common.Models.Mixed.AlgoRule.AlgoRule GetMixedAlgoRule(string localID)
        {
            if (MixedAlgoes.ContainsKey(localID))
                return MixedAlgoes[localID];
            else
            {
                var algoRule = new XYKernel.OS.Common.Models.Mixed.AlgoRule.AlgoRule();
                MixedAlgoes.Add(localID, algoRule);

                return algoRule;
            }
        }

        public void AddAdminRule(string localID, XYKernel.OS.Common.Models.Administrative.Rule.Rule model)
        {
            if (AdminRules.ContainsKey(localID))
            {
                AdminRules[localID] = model;
            }
            else
            {
                AdminRules.Add(localID, model);
            }
        }

        public void AddMixedRule(string localID, XYKernel.OS.Common.Models.Mixed.Rule.Rule model)
        {
            if (MixedAlgoes.ContainsKey(localID))
            {
                MixedRules[localID] = model;
            }
            else
            {
                MixedRules.Add(localID, model);
            }
        }

        public void AddAminAlgoRule(string localID, XYKernel.OS.Common.Models.Administrative.AlgoRule.AlgoRule model)
        {
            if (AdminAlgoes.ContainsKey(localID))
            {
                AdminAlgoes[localID] = model;
            }
            else
            {
                AdminAlgoes.Add(localID, model);
            }
        }

        public void AddMixedAlgoRule(string localID, XYKernel.OS.Common.Models.Mixed.AlgoRule.AlgoRule model)
        {
            if (MixedAlgoes.ContainsKey(localID))
            {
                MixedAlgoes[localID] = model;
            }
            else
            {
                MixedAlgoes.Add(localID, model);
            }
        }

        public void AddAdminCase(string localID, CPCase model)
        {
            if (CPCases.ContainsKey(localID))
            {
                CPCases[localID] = model;
            }
            else
            {
                CPCases.Add(localID, model);
            }
        }

        public void AddMixedCase(string localID, CLCase model)
        {
            if (CLCases.ContainsKey(localID))
            {
                CLCases[localID] = model;
            }
            else
            {
                CLCases.Add(localID, model);
            }
        }

        public CLCase GetCLCase(string localID)
        {
            if (CLCases.ContainsKey(localID))
            {
                return CLCases[localID];
            }
            else
            {
                CLCase newCase = new CLCase();
                CLCases.Add(localID, newCase);
                return newCase;
            }
        }

        public CPCase GetCPCase(string localID)
        {
            if (CPCases.ContainsKey(localID))
            {
                return CPCases[localID];
            }
            else
            {
                CPCase newCase = new CPCase();
                newCase.IsTeacherClassBalance = true;
                newCase.IsTeacherPositionBalance = true;
                newCase.IsTwoClassHourLimit = true;
                newCase.IsThreeClassHourLimit = true;
                newCase.IsMajorCourseSameDay = true;

                CPCases.Add(localID, newCase);
                return newCase;
            }
        }

        public Case GetLocalCase(string localID)
        {
            return LocalCases.FirstOrDefault(lc => lc.LocalID.Equals(localID));
        }

        public void RemoveFullCase(string localID)
        {
            var has = CPCases.Any(cp => cp.Key.Equals(localID));
            if (has)
            {
                CPCases.Remove(localID);
                AdminAlgoes.Remove(localID);
                AdminRules.Remove(localID);
            }
            else
            {
                CLCases.Remove(localID);
                MixedAlgoes.Remove(localID);
                MixedRules.Remove(localID);
            }

            var localClass = LocalCases.FirstOrDefault(lc => lc.LocalID.Equals(localID));
            if (localClass != null)
            {
                LocalCases.Remove(localClass);
            }
        }

        public string GetAdminAlgoComments(AdministrativeAlgoRuleEnum algoEnum)
        {
            return this.AdministrativeAlgoComments[algoEnum];
        }

        public string GetMixedAlgoComments(MixedAlgoRuleEnum algoEnum)
        {
            return this.MixedAlgoComments[algoEnum];
        }

        public string GetAdminRuleComments(AdministrativeRuleEnum algoEnum)
        {
            return this.AdministrativeRuleComments[algoEnum];
        }

        public string GetMixedRuleComments(MixedRuleEnum algoEnum)
        {
            return this.MixedRuleComments[algoEnum];
        }
    }
}
