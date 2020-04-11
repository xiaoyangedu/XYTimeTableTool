using OSKernel.Presentation.Models.Administrative;
using OSKernel.Presentation.Models.Rule;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models;

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Algo.ClassHour.Model
{
    public class UIClassHourRule : UIAlgoBase
    {
        public string UID { get; set; }

        public List<UIClassHour> ClassHours { get; set; }

        public int ClassHourCount
        {
            get
            {
                if (ClassHours == null)
                    return 0;
                else
                    return ClassHours.Count;
            }
        }

        public string ClassHourString
        {
            get
            {
                return ClassHours?.Select(c => c.ID)?.Parse();
            }
        }

        public int FirstID { get; set; }

        public int SecondID { get; set; }

        public int ThirdID { get; set; }

        /// <summary>
        /// 节次
        /// </summary>
        public DayPeriodModel Period { get; set; }

        /// <summary>
        /// 星期
        /// </summary>
        public DayOfWeek Day { get; set; }

        /// <summary>
        /// 课位集合信息
        /// </summary>
        public List<DayPeriodModel> Periods { get; set; }

        public string PeriodString
        {
            get
            {
                return this.Period?.PeriodName;
            }
        }

        public string DayString
        {
            get
            {
                if (Day == DayOfWeek.Friday)
                {
                    return "星期五";
                }
                else if (Day == DayOfWeek.Thursday)
                {
                    return "星期四";
                }
                else if (Day == DayOfWeek.Wednesday)
                {
                    return "星期三";
                }
                else if (Day == DayOfWeek.Tuesday)
                {
                    return "星期二";
                }
                else if (Day == DayOfWeek.Monday)
                {
                    return "星期一";
                }
                else if (Day == DayOfWeek.Saturday)
                {
                    return "星期六";
                }
                else
                {
                    return "星期日";
                }
            }
        }

        public int MaxDays { get; set; }

        public int MinDays { get; set; }

        public string CourseID { get; set; }

        public string ClassID { get; set; }

        public string TeacherID { get; set; }

        private int _maxConcurrency;
        public int MaxConcurrency
        {
            get
            {
                return _maxConcurrency;
            }

            set
            {
                _maxConcurrency = value;
                RaisePropertyChanged(() => MaxConcurrency);
            }
        }


        private int _maxOccupy;
        public int MaxOccupy
        {
            get
            {
                return _maxOccupy;
            }

            set
            {
                _maxOccupy = value;
                RaisePropertyChanged(() => MaxOccupy);
            }
        }

        public void RaiseChanged()
        {
            this.RaisePropertyChanged(() => ClassHourCount);
            this.RaisePropertyChanged(() => ClassHourString);
            this.RaisePropertyChanged(() => PeriodString);
            this.RaisePropertyChanged(() => DayString);
        }
    }
}
