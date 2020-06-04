using OSKernel.Presentation.Analysis.Data.Mixed.Models;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace OSKernel.Presentation.Analysis.Data.Mixed.Views
{
    public class SummerizeViewModel : CommonViewModel, IInitilize
    {
        private int _studentCount;
        private int _teacherCount;
        private int _totalClassHour;

        private List<GeneralTimeSlotAnalysisResult> _timeSlots;
        private List<GeneralRoomAnalysisResult> _roomAnalysis;
        private List<GeneralTeacherAnalysisResult> _teacherAnalysis;
        private List<GeneralStudentAnalysisResult> _studentAnalysis;

        private List<RuleAnalysisResult> _rules;

        /// <summary>
        /// 学生数
        /// </summary>
        public int StudentCount
        {
            get
            {
                return _studentCount;
            }

            set
            {
                _studentCount = value;
                RaisePropertyChanged(() => StudentCount);
            }
        }

        /// <summary>
        /// 教师数
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

        public List<GeneralTimeSlotAnalysisResult> TimeSlots
        {
            get
            {
                return _timeSlots;
            }

            set
            {
                _timeSlots = value;
                RaisePropertyChanged(() => TimeSlots);
            }
        }

        public List<GeneralRoomAnalysisResult> RoomAnalysis
        {
            get
            {
                return _roomAnalysis;
            }

            set
            {
                _roomAnalysis = value;
                RaisePropertyChanged(() => RoomAnalysis);
            }
        }

        public List<GeneralTeacherAnalysisResult> TeacherAnalysis
        {
            get
            {
                return _teacherAnalysis;
            }

            set
            {
                _teacherAnalysis = value;
                RaisePropertyChanged(() => TeacherAnalysis);
            }
        }

        public List<GeneralStudentAnalysisResult> StudentAnalysis
        {
            get
            {
                return _studentAnalysis;
            }

            set
            {
                _studentAnalysis = value;
                RaisePropertyChanged(() => StudentAnalysis);
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

        public SummerizeViewModel()
        {
            this.TimeSlots = new List<GeneralTimeSlotAnalysisResult>();
            this.RoomAnalysis = new List<GeneralRoomAnalysisResult>();
            this.TeacherAnalysis = new List<GeneralTeacherAnalysisResult>();
            this.StudentAnalysis = new List<GeneralStudentAnalysisResult>();
            this.Rules = new List<RuleAnalysisResult>();
        }

        [InjectionMethod]
        public void Initilize()
        {
            var cl = base.GetClCase(base.LocalID);
            var rule = base.GetClRule(base.LocalID);

            this.TimeSlots = new List<GeneralTimeSlotAnalysisResult>
            {
                DataAnalysis.GetTimeSlotAnalysisResult(cl)
            };

            this.RoomAnalysis = new List<GeneralRoomAnalysisResult>
            {
                DataAnalysis.GetRoomAnalysisResult(cl)
            };

            this.TeacherAnalysis = DataAnalysis.GetGeneralTeacherAnalysisResult(cl);
            if (this.TeacherAnalysis != null)
            {
                this.TeacherCount = this.TeacherAnalysis.Count;
            }

            this.StudentAnalysis = DataAnalysis.GetGeneralStudentAnalysisResult(cl);
            if (this.StudentAnalysis != null)
            {
                this.StudentCount = this.StudentAnalysis.Sum(s => s.StudentNumber);
                this.TotalClassHour = cl.GetAvaliablePositions();
            }

            var rules = DataAnalysis.GetRuleAnalysisResult(cl, rule);
            int no = 0;
            rules.ForEach(r =>
            {
                r.NO = ++no;
            });
            this.Rules = rules;

        }
    }
}
