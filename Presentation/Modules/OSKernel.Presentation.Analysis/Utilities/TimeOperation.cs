using System;
using System.Linq;
using System.Collections.Generic;
using XYKernel.OS.Common.Models;

namespace OSKernel.Presentation.Analysis.Utilities
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
            List<DayPeriodModel> result = new List<DayPeriodModel>() { };

            T1.ForEach(t1 => {
                result.Add(t1);
            });

            T2.ForEach(t2 =>
            {
                result.RemoveAll(t => t.Day == t2.Day && t.PeriodName == t2.PeriodName);
            });

            return result;
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
            List<DayPeriodModel> result = new List<DayPeriodModel>() { };

            T2.ForEach(t2 =>
            {
                if (!T1.Exists(t1 => t1.PeriodName == t2.PeriodName && t1.Day == t2.Day))
                {
                    result.Add(t2);
                }
            });

            T1.ForEach(t1 =>
            {
                result.Add(t1);
            });

            return result;
        }

        /// <summary>
        /// 获取日期中文表示
        /// </summary>
        /// <param name="dayPeriod"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取日期中文表示
        /// </summary>
        /// <param name="week"></param>
        /// <returns></returns>
        public static string GetWeekName(DayOfWeek week)
        {
            string weekName = "";
            switch (week)
            {
                case DayOfWeek.Friday:
                    weekName = "周五";
                    break;
                case DayOfWeek.Monday:
                    weekName = "周一";
                    break;
                case DayOfWeek.Saturday:
                    weekName = "周六";
                    break;
                case DayOfWeek.Sunday:
                    weekName = "周日";
                    break;
                case DayOfWeek.Thursday:
                    weekName = "周四";
                    break;
                case DayOfWeek.Tuesday:
                    weekName = "周二";
                    break;
                case DayOfWeek.Wednesday:
                    weekName = "周三";
                    break;
                default:
                    break;
            }
            return weekName;
        }

        /// <summary>
        /// 获取数字表示的可用日期
        /// </summary>
        /// <param name="dayPeriods"></param>
        /// <returns></returns>
        public static List<DayOfWeek> GetFormattedAvailableTims(List<DayPeriodModel> dayPeriods)
        {
            List<DayOfWeek> dow = dayPeriods?.Select(c => c.Day)?.Distinct()?.ToList() ?? new List<DayOfWeek>();

            return dow;
        }

        /// <summary>
        /// 日期转换成周一为0，而不是周日为0的表示
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public static List<int> ConvertDayOfWeek(List<DayOfWeek> days)
        {
            List<int> result = new List<int>();
            foreach (var item in days)
            {
                switch (item)
                {
                    case DayOfWeek.Friday:
                        result.Add(5);
                        break;
                    case DayOfWeek.Monday:
                        result.Add(1);
                        break;
                    case DayOfWeek.Saturday:
                        result.Add(6);
                        break;
                    case DayOfWeek.Sunday:
                        result.Add(7);
                        break;
                    case DayOfWeek.Thursday:
                        result.Add(4);
                        break;
                    case DayOfWeek.Tuesday:
                        result.Add(2);
                        break;
                    case DayOfWeek.Wednesday:
                        result.Add(3);
                        break;
                    default:
                        break;
                }
            }

            return result;
        }
    }
}
