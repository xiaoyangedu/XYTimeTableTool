using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using XYKernel.OS.Common.Models;
using XYKernel.OS.Common.Models.Administrative;
using XYKernel.OS.Common.Models.Mixed;

namespace OSKernel.Presentation.Analysis.Utilities
{
    public static class Miscellaneous
    {
        public const int MaxLessonEveryDay = 100;

		/// <summary>
		/// 获取枚举备注名
		/// </summary>
		/// <param name="en"></param>
		/// <returns></returns>
		public static string GetLocalDescription(this Enum en)
		{
			Type type = en.GetType();
			MemberInfo[] memberInfos = type.GetMember(en.ToString());
			if (memberInfos != null && memberInfos.Length > 0)
			{
				DescriptionAttribute[] attrs = memberInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
				if (attrs != null && attrs.Length > 0)
				{
					return attrs[0].Description;
				}
			}
			return en.ToString();
		}

        public static Tuple<List<int>, List<int>> GetCPCasePeriodMapping(List<XYKernel.OS.Common.Models.Administrative.CoursePositionModel> coursePosition)
        {
            #region 构造映射表
            List<int> mappingPeriodOld = new List<int>();
            List<int> mappingPeriodNew = new List<int>();

            int tempI = 0;

            coursePosition?.Where(p => p.DayPeriod.Day == DayOfWeek.Monday)?.ToList()?.OrderBy(p => p.PositionOrder)?.ToList()?.ForEach(p => {
                    if (p.Position != XYKernel.OS.Common.Enums.Position.AB && p.Position != XYKernel.OS.Common.Enums.Position.PB && p.Position != XYKernel.OS.Common.Enums.Position.Noon)
                    {
                        mappingPeriodOld.Add(p.DayPeriod.Period);
                        mappingPeriodNew.Add(tempI);
                        tempI++;
                    }
            });
            #endregion

            return Tuple.Create(mappingPeriodOld, mappingPeriodNew);
        }

        public static Tuple<List<int>, List<int>> GetCLCasePeriodMapping(List<XYKernel.OS.Common.Models.Mixed.CoursePositionModel> coursePosition)
        {
            #region 构造映射表
            List<int> mappingPeriodOld = new List<int>();
            List<int> mappingPeriodNew = new List<int>();

            int tempI = 0;

            coursePosition?.Where(p => p.DayPeriod.Day == DayOfWeek.Monday)?.ToList()?.OrderBy(p => p.PositionOrder)?.ToList()?.ForEach(p => {
                if (p.Position != XYKernel.OS.Common.Enums.Position.AB && p.Position != XYKernel.OS.Common.Enums.Position.PB && p.Position != XYKernel.OS.Common.Enums.Position.Noon)
                {
                    mappingPeriodOld.Add(p.DayPeriod.Period);
                    mappingPeriodNew.Add(tempI);
                    tempI++;
                }
            });
            #endregion

            return Tuple.Create(mappingPeriodOld, mappingPeriodNew);
        }

        public static List<int> GetNewMappedPeriods(Tuple<List<int>, List<int>> mappingInfo, List<int> oldPeriods)
        {
            List<int> newMappedPeriods = new List<int>();

            if (oldPeriods != null && mappingInfo != null)
            {
                for (int i = 0; i < oldPeriods.Count; i++)
                {
                    int oldPeriod = oldPeriods[i];
                    if (mappingInfo.Item1.Contains(oldPeriod))
                    {
                        int iIndex = mappingInfo.Item1.IndexOf(oldPeriod);
                        int newPeriod = mappingInfo.Item2[iIndex];
                        newMappedPeriods.Add(newPeriod);
                    }
                }
            }

            if (newMappedPeriods.Count != oldPeriods.Count)
            {
                newMappedPeriods.Clear();
            }

            return newMappedPeriods;
        }

        /// <summary>
        /// 计算一系列数字中的最大间隔
        /// </summary>
        /// <param name="periodList"></param>
        /// <returns></returns>
        public static int MaxGaps(List<int> periodList)
        {
            int maxGaps = 0;

            if (periodList.Count > 1)
            {
                periodList = (periodList.OrderBy(x => x)).ToList();

                for (int i = 0; i < periodList.Count; i++)
                {
                    if (i + 1 < periodList.Count)
                    {
                        if (periodList[i+1] - periodList[i] - 1 > maxGaps)
                        {
                            maxGaps = periodList[i + 1] - periodList[i] - 1;
                        }
                    }
                }
            }

            return maxGaps;
        }

        /// <summary>
        /// 计算一系列数字中的最大连续次数
        /// </summary>
        /// <param name="periodList"></param>
        /// <returns></returns>
        public static int MaxContinous(List<int> periodList)
        {
            List<int> tempResult = new List<int>();
            int maxContinous = 1;

            if (periodList.Count > 1)
            {
                periodList = (periodList.OrderBy(x => x)).ToList();

                for (int i = 0; i < periodList.Count; i++)
                {
                    if (i + 1 == periodList.Count)
                    {
                        break;
                    }
                    if (periodList[i+1] - periodList[i] == 1)
                    {
                        maxContinous += 1;
                    }
                    else
                    {
                        tempResult.Add(maxContinous);
                        maxContinous = 1;
                    }
                }
            }

            return maxContinous;
        }

        /// <summary>
        /// 根据课位信息返回星期数组，每天的课时数组，以及包含全部课位的数组
        /// </summary>
        /// <param name="globalDayPeriod"></param>
        /// <returns></returns>
        public static Tuple<int[], int[], int[]> GetGlobalTimeTableSlot(List<DayPeriodModel> globalDayPeriod)
        {
            //构造全局课表位置信息
            var dayNumbers = globalDayPeriod?.Select(p => (int)p.Day)?.Distinct()?.ToList() ?? new List<int>();

            var periodNumbers = globalDayPeriod?.Where(p => (int)p.Day == dayNumbers.FirstOrDefault())?
                                .Select(p => p.Period)?.Distinct()?.ToList() ?? new List<int> { };

            //对排课结果进行转换，强制星期日为7，而非0
            int[] globalWeekDays = dayNumbers.Select(rc => rc == 0 ? 7 : rc).ToArray();
            int[] globalDayPeriods = periodNumbers.OrderBy(p => p).ToArray();
            int[] globalTimeTableSlot = new int[globalWeekDays.Length * globalDayPeriods.Length];

            for (int i = 0; i < globalWeekDays.Length; i++)
            {
                for (int j = 0; j < globalDayPeriods.Length; j++)
                {
                    int dpPosition = i * globalDayPeriods.Length + j;
                    globalTimeTableSlot[dpPosition] = globalWeekDays[i] * MaxLessonEveryDay + globalDayPeriods[j];
                }
            }

            return Tuple.Create(globalWeekDays, globalDayPeriods, globalTimeTableSlot);
        }

        /// <summary>
        /// 基于教师课表判定是否（宽松）齐头
        /// </summary>
        /// <param name="teacherTimeTableSlot">教师课表（以第几教案形式表示）</param>
        /// <param name="globalWeekDays">每周天</param>
        /// <param name="globalDayPeriods">每天课时</param>
        /// <returns></returns>
        public static bool CalculateTeacherTeachingPlan(int[] teacherTimeTableSlot, int[] globalWeekDays, int[] globalDayPeriods)
        {
            /* 判断教师课表是否教案齐头 */
            bool resultFlag = true;
            /* 原则1. 教师教案不能反复 */
            int initialTeachingPlan = 1;
            for (int i = 0; i < teacherTimeTableSlot.Length; i++)
            {
                if (teacherTimeTableSlot[i] > 0)
                {
                    if (teacherTimeTableSlot[i] < initialTeachingPlan)
                    {
                        resultFlag = false;
                        break;
                    }
                    else
                    {
                        initialTeachingPlan = teacherTimeTableSlot[i];
                    }
                }
            }
            /* 原则2. 不能同天换教案 */
            for (int i = 0; i < globalWeekDays.Length; i++)
            {
                int begin = i * globalDayPeriods.Length;
                int end = (i + 1) * globalDayPeriods.Length - 1;
                List<int> oneDayLesson = new List<int>();
                for (int j = begin; j <= end; j++)
                {
                    if (teacherTimeTableSlot[j] != 0)
                    {
                        oneDayLesson.Add(teacherTimeTableSlot[j]);
                    }
                }

                if (oneDayLesson.Distinct().Count() > 1)
                {
                    resultFlag = false;
                }
            }

            return resultFlag;
        }

        /// <summary>
        /// 不考虑间隔天数情况下计算课时分散（宽松）
        /// </summary>
        /// <param name="classTimeTableSlot">班级课表</param>
        /// <param name="globalDayPeriods">每天课时数量</param>
        /// <returns></returns>
        public static bool CalculateLessonsEvenlyDistributedForSuggest(int[] classTimeTableSlot, int[] globalDayPeriods)
        {
            bool result = true;

            int lessonCount = classTimeTableSlot.Where(ct => ct > 1).Count();

            if (lessonCount > 1 && globalDayPeriods.Length > 1)
            {
                List<int> allDayLesson = new List<int>() { };

                for (int i = 0; i < classTimeTableSlot.Length / globalDayPeriods.Length; i++)
                {
                    List<int> everyDayLesson = new List<int>() { };

                    for (int j = i * globalDayPeriods.Length; j < (i+1) * globalDayPeriods.Length; j++)
                    {
                        if (classTimeTableSlot[j] > 0)
                        {
                            everyDayLesson.Add(j);
                        }
                    }

                    switch (everyDayLesson.Count)
                    {
                        case 0:
                        case 1:
                            allDayLesson.Add(1);
                            break;
                        case 2:
                            int first = everyDayLesson[0];
                            int second = everyDayLesson[1];
                            if (Math.Abs(second - first) == 1)
                            {
                                allDayLesson.Add(1);
                            }
                            else
                            {
                                allDayLesson.Add(2);
                            }
                            break;
                        default:
                            allDayLesson.Add(2);
                            break;
                    }
                }

                if (allDayLesson.Distinct().Count() > 1)
                {
                    result = false;
                }
            }

            return result;        
        }

        /// <summary>
        /// 判断教师的课表是否齐头(考虑了连排)
        /// </summary>
        /// <param name="courseInfo">教师在各个班级的课时数据</param>
        /// <param name="globalDayPeriods">每天课时数据</param>
        /// <returns></returns>
        public static bool CalculateTeacherTeachingPlansForSuggest(Dictionary<string, int[]> courseInfo, int[] globalDayPeriods)
        {
            bool result = true;
            List<int> classLessonNumber = new List<int>() { };

            if (globalDayPeriods.Length <= 1)
            {
                return true;
            }

            foreach (var item in courseInfo)
            {
                classLessonNumber.Add(item.Value.Where(va => va > 0).Count());
            }

            //课时都相同且课时 > 1, 才能进行比较
            if (classLessonNumber.Distinct().Count() == 1)
            {
                if (classLessonNumber.FirstOrDefault() > 1)
                {
                    int courseLesson = classLessonNumber.FirstOrDefault();
                    //遍历循环判断每个班级的第N课时
                    for (int i = 0; i < courseLesson; i++)
                    {
                        int minDay = 10000;
                        int maxDay = -1;
                        List<int> nextValue = new List<int>();
                        bool currentIsContinous = false;

                        //循环各班
                        foreach (var item in courseInfo)
                        {
                            int lessonPosition = item.Value.Where(va => va > 0).Skip(i).Take(1).FirstOrDefault();
                            int cDay = Array.IndexOf(item.Value, lessonPosition);
                            cDay = (int)Math.Ceiling((decimal)(cDay+1) / globalDayPeriods.Length);

                            if (cDay < minDay)
                            {
                                minDay = cDay;
                            }

                            if (cDay > maxDay)
                            {
                                maxDay = cDay;
                            }

                            if (i < courseLesson - 1)
                            {
                                int iIndex = Array.IndexOf(item.Value, lessonPosition);
                                if ((iIndex + 1) % globalDayPeriods.Length != 0)
                                {
                                    nextValue.Add(item.Value[iIndex + 1]);
                                }
                                else
                                {
                                    //若当前课时是当天最后一节，则下一个课时位置为0
                                    nextValue.Add(0);
                                }
                            }
                            else
                            {
                                nextValue.Add(0);
                            }
                        }

                        if (!nextValue.Any(nv => nv == 0))
                        {
                            currentIsContinous = true;
                        }

                        List<int> classLessonCount = new List<int>();

                        foreach (var item in courseInfo)
                        {
                            int lessonCount = 0;

                            for (int j = (minDay-1) * globalDayPeriods.Length; j < maxDay * globalDayPeriods.Length; j++)
                            {
                                if (item.Value[j] != 0)
                                {
                                    lessonCount++;
                                }
                            }

                            classLessonCount.Add(lessonCount);
                        }

                        //如果不是连排课时
                        if (!currentIsContinous)
                        {
                            //若各个班级最大位置和最小位置天内课时数等于班级数，则当前课时齐头，否则不齐头
                            if (!(classLessonCount.Sum() == courseInfo.Count && classLessonCount.Distinct().FirstOrDefault() == 1))
                            {
                                result = false;
                            }
                        }
                        else
                        {
                            if (!(classLessonCount.Sum() == courseInfo.Count * 2 && classLessonCount.Distinct().FirstOrDefault() == 2))
                            {
                                result = false;
                            }
                        }

                        if (currentIsContinous)
                        {
                            i++;
                        }

                        if (!result)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    result = true;
                }
            }
            else
            {
                result = false;
            }
            
            return result;
        }

        /// <summary>
        /// 获取部分课位在全课表中的位置信息
        /// </summary>
        /// <param name="globalDayPeriod"></param>
        /// <param name="partDayPeriod"></param>
        /// <returns></returns>
        public static int[] GetPartTimeTableSlot(int[] globalTimeTableSlot, List<DayPeriodModel> partDayPeriod)
        {
            int[] partTimeTableSlot = new int[globalTimeTableSlot.Length];

            partDayPeriod?.ForEach(dp => {
                int day = dp.Day == 0 ? 7 : (int)dp.Day;
                int period = dp.Period;
                int dpPosition = Array.IndexOf(globalTimeTableSlot, day * MaxLessonEveryDay + period);

                if (dpPosition >= 0 && dpPosition < globalTimeTableSlot.Length)
                {
                    partTimeTableSlot[dpPosition] = globalTimeTableSlot[dpPosition];
                }
            });

            return partTimeTableSlot;
        }

        /// <summary>
        /// 基于课时课表返回连排个数，以及连排所在天
        /// </summary>
        /// <param name="partTimeTableSlot"></param>
        /// <param name="globalWeekDays"></param>
        /// <param name="globalDayPeriods"></param>
        /// <returns></returns>
        public static Tuple<int , List<int>> ComputeContinousNumberByTimeTable(int[] partTimeTableSlot, int[] globalWeekDays, int[] globalDayPeriods)
        {
            int continousNumber = 0;
            List<int> continousDays = new List<int>();

            for (int i = 0; i < globalWeekDays.Length; i++)
            {
                int begin = i * globalDayPeriods.Length;
                int end = (i + 1) * globalDayPeriods.Length - 1;

                for (int j = begin; j < end; j++)
                {
                    if (partTimeTableSlot[j] != 0)
                    {
                        if (partTimeTableSlot[j+1] != 0)
                        {
                            continousNumber += 1;
                            continousDays.Add(globalWeekDays[i]);
                        }

                        j += 1;
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            return Tuple.Create(continousNumber, continousDays);
        }

        /// <summary>
        /// 获取行政班上下午大课间和午休关联的课位信息
        /// </summary>
        /// <param name="coursePosition"></param>
        /// <returns></returns>
        public static Tuple<List<DayPeriodModel>, List<DayPeriodModel>, List<DayPeriodModel>> GetCPAMPMNoonBreak(List<XYKernel.OS.Common.Models.Administrative.CoursePositionModel> coursePosition)
        {
            List<DayPeriodModel> aMBreakTimes = new List<DayPeriodModel>();
            List<DayPeriodModel> noonBreakTimes = new List<DayPeriodModel>();
            List<DayPeriodModel> pMBreakTimes = new List<DayPeriodModel>();

            if (coursePosition != null)
            {
                //Get AM Break Times
                if (coursePosition.Exists(p => p.Position == XYKernel.OS.Common.Enums.Position.AB))
                {
                    int abOrder = coursePosition.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.AB).FirstOrDefault().PositionOrder;
                    int abPreviousOrder = coursePosition.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.AM && p.PositionOrder < abOrder)?.OrderByDescending(p => p.PositionOrder)?.Take(1)?.FirstOrDefault()?.PositionOrder ?? -1;
                    int abNextOrder = coursePosition.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.AM && p.PositionOrder > abOrder)?.OrderBy(p => p.PositionOrder)?.Take(1)?.FirstOrDefault()?.PositionOrder ?? -1;

                    if (abPreviousOrder >= 0 && abNextOrder >= 0)
                    {
                        aMBreakTimes = coursePosition.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.AM && (p.PositionOrder == abPreviousOrder || p.PositionOrder == abNextOrder))?.Select(p => p.DayPeriod)?.ToList() ?? new List<DayPeriodModel>() { };
                    }
                }

                //Get Noon Break Times
                if (coursePosition.Exists(p => p.Position == XYKernel.OS.Common.Enums.Position.Noon))
                {
                    int noonOrder = coursePosition.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.Noon).FirstOrDefault().PositionOrder;
                    int noonPreviousOrder = coursePosition.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.AM && p.PositionOrder < noonOrder)?.OrderByDescending(p => p.PositionOrder)?.Take(1)?.FirstOrDefault()?.PositionOrder ?? -1;
                    int noonNextOrder = coursePosition.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.PM && p.PositionOrder > noonOrder)?.OrderBy(p => p.PositionOrder)?.Take(1)?.FirstOrDefault()?.PositionOrder ?? -1;

                    if (noonPreviousOrder >= 0 && noonNextOrder >= 0)
                    {
                        noonBreakTimes = coursePosition.Where(p => (p.Position == XYKernel.OS.Common.Enums.Position.AM && p.PositionOrder == noonPreviousOrder) || (p.Position == XYKernel.OS.Common.Enums.Position.PM && p.PositionOrder == noonNextOrder))?.Select(p => p.DayPeriod)?.ToList() ?? new List<DayPeriodModel>() { };
                    }
                }

                //Get PM Break Times
                if (coursePosition.Exists(p => p.Position == XYKernel.OS.Common.Enums.Position.PB))
                {
                    int pbOrder = coursePosition.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.PB).FirstOrDefault().PositionOrder;
                    int pbPreviousOrder = coursePosition.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.PM && p.PositionOrder < pbOrder)?.OrderByDescending(p => p.PositionOrder)?.Take(1)?.FirstOrDefault()?.PositionOrder ?? -1;
                    int pbNextOrder = coursePosition.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.PM && p.PositionOrder > pbOrder)?.OrderBy(p => p.PositionOrder)?.Take(1)?.FirstOrDefault()?.PositionOrder ?? -1;

                    if (pbPreviousOrder >= 0 && pbNextOrder >= 0)
                    {
                        pMBreakTimes = coursePosition.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.PM && (p.PositionOrder == pbPreviousOrder || p.PositionOrder == pbNextOrder))?.Select(p => p.DayPeriod)?.ToList() ?? new List<DayPeriodModel>() { };
                    }
                }
            }

            return Tuple.Create(aMBreakTimes, noonBreakTimes, pMBreakTimes);
        }

        /// <summary>
        /// 获取走班上下午大课间和午休关联的课位信息
        /// </summary>
        /// <param name="coursePosition"></param>
        /// <returns></returns>
        public static Tuple<List<DayPeriodModel>, List<DayPeriodModel>, List<DayPeriodModel>> GetCLAMPMNoonBreak(List<XYKernel.OS.Common.Models.Mixed.CoursePositionModel> coursePosition)
        {
            List<DayPeriodModel> aMBreakTimes = new List<DayPeriodModel>();
            List<DayPeriodModel> noonBreakTimes = new List<DayPeriodModel>();
            List<DayPeriodModel> pMBreakTimes = new List<DayPeriodModel>();

            if (coursePosition != null)
            {
                //Get AM Break Times
                if (coursePosition.Exists(p => p.Position == XYKernel.OS.Common.Enums.Position.AB))
                {
                    int abOrder = coursePosition.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.AB).FirstOrDefault().PositionOrder;
                    int abPreviousOrder = coursePosition.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.AM && p.PositionOrder < abOrder)?.OrderByDescending(p => p.PositionOrder)?.Take(1)?.FirstOrDefault()?.PositionOrder ?? -1;
                    int abNextOrder = coursePosition.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.AM && p.PositionOrder > abOrder)?.OrderBy(p => p.PositionOrder)?.Take(1)?.FirstOrDefault()?.PositionOrder ?? -1;

                    if (abPreviousOrder >= 0 && abNextOrder >= 0)
                    {
                        aMBreakTimes = coursePosition.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.AM && (p.PositionOrder == abPreviousOrder || p.PositionOrder == abNextOrder))?.Select(p => p.DayPeriod)?.ToList() ?? new List<DayPeriodModel>() { };
                    }
                }

                //Get Noon Break Times
                if (coursePosition.Exists(p => p.Position == XYKernel.OS.Common.Enums.Position.Noon))
                {
                    int noonOrder = coursePosition.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.Noon).FirstOrDefault().PositionOrder;
                    int noonPreviousOrder = coursePosition.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.AM && p.PositionOrder < noonOrder)?.OrderByDescending(p => p.PositionOrder)?.Take(1)?.FirstOrDefault()?.PositionOrder ?? -1;
                    int noonNextOrder = coursePosition.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.PM && p.PositionOrder > noonOrder)?.OrderBy(p => p.PositionOrder)?.Take(1)?.FirstOrDefault()?.PositionOrder ?? -1;

                    if (noonPreviousOrder >= 0 && noonNextOrder >= 0)
                    {
                        noonBreakTimes = coursePosition.Where(p => (p.Position == XYKernel.OS.Common.Enums.Position.AM && p.PositionOrder == noonPreviousOrder) || (p.Position == XYKernel.OS.Common.Enums.Position.PM && p.PositionOrder == noonNextOrder))?.Select(p => p.DayPeriod)?.ToList() ?? new List<DayPeriodModel>() { };
                    }
                }

                //Get PM Break Times
                if (coursePosition.Exists(p => p.Position == XYKernel.OS.Common.Enums.Position.PB))
                {
                    int pbOrder = coursePosition.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.PB).FirstOrDefault().PositionOrder;
                    int pbPreviousOrder = coursePosition.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.PM && p.PositionOrder < pbOrder)?.OrderByDescending(p => p.PositionOrder)?.Take(1)?.FirstOrDefault()?.PositionOrder ?? -1;
                    int pbNextOrder = coursePosition.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.PM && p.PositionOrder > pbOrder)?.OrderBy(p => p.PositionOrder)?.Take(1)?.FirstOrDefault()?.PositionOrder ?? -1;

                    if (pbPreviousOrder >= 0 && pbNextOrder >= 0)
                    {
                        pMBreakTimes = coursePosition.Where(p => p.Position == XYKernel.OS.Common.Enums.Position.PM && (p.PositionOrder == pbPreviousOrder || p.PositionOrder == pbNextOrder))?.Select(p => p.DayPeriod)?.ToList() ?? new List<DayPeriodModel>() { };
                    }
                }
            }

            return Tuple.Create(aMBreakTimes, noonBreakTimes, pMBreakTimes);
        }


        /// <summary>
        /// 判断数字集合能容纳多少连排
        /// </summary>
        /// <returns></returns>
        public static int ContinuousNumber(List<int> numList)
        {
            numList.Sort((x, y) => -x.CompareTo(y));//降序
            int result = 0;

            for (int i = 0; i < numList.Count() - 1; i++)
            {
                if (numList[i] - numList[i + 1] == 1)
                {
                    result = result + 1;

                    if (i + 2 <= numList.Count - 1)
                    {
                        i = i + 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 根据给定课位，课时数，间隔日期判断是否能课时分散
        /// </summary>
        /// <param name="availableTimes"></param>
        /// <param name="lesson"></param>
        /// <param name="minDay"></param>
        /// <returns></returns>
        public static bool DaysEnoughForEvenlyDistributed(List<DayPeriodModel> availableTimes, int lesson, int minDay)
        {
            //1 2 3 4 5
            //1-2 - - 5 不满足 lesson 3 minDay=2
            //1 - 3 - 5   满足 lesson 3 minDay=2
            //

            bool result = true;

            if (lesson > 1 && minDay > 0)
            {
                List<int> availableDays = availableTimes?.Select(at => (int)at.Day == 0 ? 7 : (int)at.Day)?.Distinct()?.ToList() ?? new List<int>() { };

                availableDays.Sort();
                int canHoldLesson = 0;
                int lastDay = 0;

                for (int i = 0; i < availableDays.Count; i++)
                {
                    if (i == 0)
                    {
                        canHoldLesson += 1;
                        lastDay = availableDays[i];
                    }
                    else
                    {
                        if (availableDays[i] - lastDay >= minDay)
                        {
                            canHoldLesson += 1;
                            lastDay = availableDays[i];
                        } 
                    }
                }

                if (canHoldLesson < lesson)
                {
                    result = false;
                }
            }

            return result;
        }

        #region 获得行政班班级课程名称
        public static string GetCPClassCourseInfo(CPCase cPCase, string classID, string courseID)
        {
            string classCourseInfo = string.Empty;

            if (cPCase != null && !string.IsNullOrEmpty(classID) && !string.IsNullOrEmpty(courseID))
            {
                string className = cPCase.Classes?.FirstOrDefault(cl => cl.ID == classID).Name ?? string.Empty;
                string courseName = cPCase.Courses?.FirstOrDefault(co => co.ID == courseID)?.Name ?? string.Empty;

                classCourseInfo = $"{className}{courseName}";
            }

            return classCourseInfo;
        }
        #endregion

        #region 获得走班班级名称
        public static string GetCLClassCourseInfo(CLCase cLCase, string classID)
        {
            string classCourseInfo = string.Empty;

            if (cLCase != null && !string.IsNullOrEmpty(classID))
            {
                var classObj = cLCase.Classes?.FirstOrDefault(cl => cl.ID == classID);

                string className = classObj?.Name ?? string.Empty;
                string courseName = cLCase.Courses?.FirstOrDefault(co => co.ID == classObj.CourseID)?.Name ?? string.Empty;
                string levelName = cLCase.Courses?.FirstOrDefault(co => co.ID == classObj.CourseID)?.Levels?.FirstOrDefault(le => le.ID == classObj.LevelID)?.Name ?? string.Empty;

                classCourseInfo = $"{courseName}{levelName}{className}";
            }

            return classCourseInfo;
        }
        #endregion
    }
}
