using System;
using System.Collections.Generic;
using XYKernel.OS.Common.Models;

namespace OSKernel.Presentation.Arranging.Utilities
{
    public static class TimeOperation
    {
        /// <summary>
        /// 求2个时间列表的交集
        /// </summary>
        /// <param name="T1"></param>
        /// <param name="T2"></param>
        /// <returns></returns>
        public static List<DayPeriodModel> TimeSlotInterSect(List<DayPeriodModel> T1, List<DayPeriodModel> T2)
        {
            T1 = T1 ?? new List<DayPeriodModel>() { };
            T2 = T2 ?? new List<DayPeriodModel>() { };
            List<DayPeriodModel> result = new List<DayPeriodModel>() { };

            T1.ForEach(t1 =>
            {
                if (T2.Exists(t2 => t2.PeriodName == t1.PeriodName && t2.Day == t1.Day))
                {
                    result.Add(t1);
                }
            });

            return result;
        }

        /// <summary>
        /// 求2个时间列表的差集 T1-T2
        /// </summary>
        /// <param name="T1"></param>
        /// <param name="T2"></param>
        /// <returns></returns>
        public static List<DayPeriodModel> TimeSlotDiff(List<DayPeriodModel> T1, List<DayPeriodModel> T2)
        {
            T1 = T1 ?? new List<DayPeriodModel>() { };
            T2 = T2 ?? new List<DayPeriodModel>() { };

            T2.ForEach(t2 =>
            {
                T1.RemoveAll(t => t.Day == t2.Day && t.PeriodName == t2.PeriodName);
            });

            return T1;
        }

        /// <summary>
        /// 求2个时间列表的合集 T1+T2
        /// </summary>
        /// <param name="T1"></param>
        /// <param name="T2"></param>
        /// <returns></returns>
        public static List<DayPeriodModel> TimeSlotUnion(List<DayPeriodModel> T1, List<DayPeriodModel> T2)
        {
            T1 = T1 ?? new List<DayPeriodModel>() { };
            T2 = T2 ?? new List<DayPeriodModel>() { };

            T2.ForEach(t2 =>
            {
                if (!T1.Exists(t1 => t1.PeriodName == t2.PeriodName && t1.Day == t2.Day))
                {
                    T1.Add(t2);
                }
            });

            return T1;
        }

        public static string GetDateInfo(DayPeriodModel dayPeriod)
        {
            string dayPeriodString = string.Empty;

            if (dayPeriod != null)
            {
                switch (dayPeriod.Day)
                {
                    case DayOfWeek.Sunday:
                        dayPeriodString = "星期日";
                        break;
                    case DayOfWeek.Monday:
                        dayPeriodString = "星期一";
                        break;
                    case DayOfWeek.Tuesday:
                        dayPeriodString = "星期二";
                        break;
                    case DayOfWeek.Wednesday:
                        dayPeriodString = "星期三";
                        break;
                    case DayOfWeek.Thursday:
                        dayPeriodString = "星期四";
                        break;
                    case DayOfWeek.Friday:
                        dayPeriodString = "星期五";
                        break;
                    case DayOfWeek.Saturday:
                        dayPeriodString = "星期六";
                        break;
                    default:
                        break;
                }

                dayPeriodString = dayPeriodString + dayPeriod.PeriodName;
            }

            return dayPeriodString;
        }
    }
}
