using OSKernel.Presentation.Analysis.Data.Administrative.Models;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace OSKernel.Presentation.Analysis.Data.Administrative.Views
{
    public class SummerizeViewModel : CommonViewModel, IInitilize
    {
        private List<ClassTimeSlotAnalysisResult> _classTimeSlots;
        private List<GeneralTeacherAnalysisResult> _teacherAnalysises;
        private List<RuleAnalysisResult> _rules;
        private int _totalClassHour;

        private int _teacherCount;
        private int _classCount;

        /// <summary>
        /// 教师数量
        /// </summary>
        public int TeacherCount
        {
            get
            {
                return _teacherCount;
            }

            set
            {
                _teacherCount = value;
                RaisePropertyChanged(() => TeacherCount);
            }
        }

        /// <summary>
        /// 班级数量
        /// </summary>
        public int ClassCount
        {
            get
            {
                return _classCount;
            }

            set
            {
                _classCount = value;
                RaisePropertyChanged(() => ClassCount);
            }
        }

        /// <summary>
        /// 课位间隔
        /// </summary>
        public List<ClassTimeSlotAnalysisResult> ClassTimeSlots
        {
            get
            {
                return _classTimeSlots;
            }

            set
            {
                _classTimeSlots = value;
                RaisePropertyChanged(() => ClassTimeSlots);
            }
        }

        /// <summary>
        /// 教师工作量统计
        /// </summary>
        public List<GeneralTeacherAnalysisResult> TeacherAnalysises
        {
            get
            {
                return _teacherAnalysises;
            }

            set
            {
                _teacherAnalysises = value;
                RaisePropertyChanged(() => TeacherAnalysises);
            }
        }

        /// <summary>
        /// 规则
        /// </summary>
        public List<RuleAnalysisResult> Rules
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

        /// <summary>
        /// 总课时数
        /// </summary>
        public int TotalClassHour
        {
            get
            {
                return _totalClassHour;
            }

            set
            {
                _totalClassHour = value;
                RaisePropertyChanged(() => TotalClassHour);
            }
        }

        public SummerizeViewModel()
        {
            this.ClassTimeSlots = new List<ClassTimeSlotAnalysisResult>();
            this.TeacherAnalysises = new List<GeneralTeacherAnalysisResult>();
            this.Rules = new List<RuleAnalysisResult>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            var cp = CommonDataManager.GetCPCase(base.LocalID);
            var rule = CommonDataManager.GetAminRule(base.LocalID);

            this.ClassTimeSlots = DataAnalysis.GetClassTimeSlotAnalysisResult(cp, rule);
            if (this.ClassTimeSlots != null)
            {
                this.ClassCount = this.ClassTimeSlots.Count;
                this.TotalClassHour = cp.GetAvaliablePositions();
            }

            this.TeacherAnalysises = DataAnalysis.GetGeneralTeacherAnalysisResult(cp);
            if (this.TeacherAnalysises != null)
            {
                this.TeacherCount = this.TeacherAnalysises.Count;
            }

            var rules = DataAnalysis.GetRuleAnalysisResult(cp, rule);

            int no = 0;
            rules.ForEach(r =>
            {
                r.NO = ++no;
            });

            this.Rules = rules;
        }
    }
}
