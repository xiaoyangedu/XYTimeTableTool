using System;
using System.Linq;
using System.Collections.Generic;
using XYKernel.OS.Common.Models.Mixed;
using XYKernel.OS.Common.Models.Mixed.AlgoRule;
using XYKernel.OS.Common.Models.Mixed.Rule;

namespace OSKernel.Presentation.Arranging.Mixed.Pattern.Data
{
    public class ModelValidation
    {
        public static bool ValidateModel(CLCase cLCase, AlgoRule algoRule, Rule rule, out List<DataValidationResultInfo> mydvri)
        {
            bool result = true;
            mydvri = new List<DataValidationResultInfo>();

            Tuple<bool, List<DataValidationResultInfo>> VBasicResult = ValidateCaseModelBase(cLCase);

            if (!VBasicResult.Item1)
            {
                result = false;
                mydvri = VBasicResult.Item2;
                return result;
            }

            //检查基本数据
            Tuple<bool, List<DataValidationResultInfo>> VCMResult = ValidateCaseModel(cLCase, rule);

            //检查批量设置的规则
            Tuple<bool, List<DataValidationResultInfo>> VRResult = ValidateRuleModel(cLCase, rule);

            //检查算法规则
            Tuple<bool, List<DataValidationResultInfo>> VAResult = ValidateAlgoRuleModel(cLCase, algoRule);

            //检查班额
            Tuple<bool, List<DataValidationResultInfo>> VCapacityResult = ValidateClassCapacity(cLCase);

            if (!VCMResult.Item1 || !VRResult.Item1 || !VAResult.Item1 || !VCapacityResult.Item1)
            {
                result = false;
                mydvri = VCMResult.Item2.Union(VRResult.Item2).Union(VAResult.Item2).Union(VCapacityResult.Item2).ToList();

                return result;
            }
            else
            {
                return result;
            }
        }

        private static Tuple<bool, List<DataValidationResultInfo>> ValidateCaseModelBase(CLCase cLCase)
        {
            List<DataValidationResultInfo> validationInfo = new List<DataValidationResultInfo>();
            bool result = true;

            if (cLCase == null)
            {
                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr100.ToString(), Description = "基本数据模型为空!" });
            }
            else
            {
                #region 检查方案课位
                if (cLCase.Positions == null
                    || cLCase.Positions.Count() == 0
                    || cLCase.Positions.Where(x => x.IsSelected
                                              && x.Position != XYKernel.OS.Common.Enums.Position.Noon
                                              && x.Position != XYKernel.OS.Common.Enums.Position.AB
                                              && x.Position != XYKernel.OS.Common.Enums.Position.PB).Count() == 0)
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr101.ToString(), Description = "排课方案没有设定可用课位!" });
                }
                #endregion

                #region 检查课程
                if (cLCase.Courses == null || cLCase.Courses.Count() == 0)
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr102.ToString(), Description = "排课方案没有开设课程!" });
                }
                #endregion

                #region 检查班级
                if (cLCase.Classes == null || cLCase.Classes.Count() == 0)
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr104.ToString(), Description = "排课方案没有任何班级!" });
                }
                #endregion

                #region 检查课时
                if (cLCase.ClassHours == null || cLCase.ClassHours.Count() == 0)
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr105.ToString(), Description = "排课方案没有任何课时信息!" });
                }
                #endregion
            }

            if (validationInfo.Count > 0)
            {
                result = false;
            }

            return Tuple.Create(result, validationInfo);
        }

        private static Tuple<bool, List<DataValidationResultInfo>> ValidateCaseModel(CLCase cLCase, Rule rule)
        {
            List<DataValidationResultInfo> validationInfo = new List<DataValidationResultInfo>();
            bool result = true;

            #region 检查课位
            //检查课位中 DayPeriod 数据的有效性
            //检查每个都不为空且不重复
            cLCase.Positions.ForEach(x => {
                if (x == null)
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr101.ToString(), Description = $"方案的课位信息中存在空课时!" });
                }
                else
                {
                    if (x.DayPeriod == null)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr101.ToString(), Description = $"方案的课位信息中存在空课时!" });
                    }
                    else
                    {
                        int periodRepeatNumber = cLCase.Positions.Where(p => p.DayPeriod.Day == x.DayPeriod.Day && p.DayPeriod.Period == x.DayPeriod.Period).Count();
                        if (periodRepeatNumber > 1)
                        {
                            validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr101.ToString(), Description = $"方案的课位信息中存在重复课位（{ValidationUtility.GetTimeSlotInfo(x.DayPeriod)}）!" });
                        }
                    }
                }
            });

            int selectedPosition = GetAvailableTimeSlot(cLCase);
            //检查是否满足教师最大课位
            List<string> teachers = new List<string>();

            cLCase.Teachers?.ForEach(x => {

                int teacherClassHour = GetTotalClassHourNumberByTeacherID(cLCase, x.ID);

                if (teacherClassHour > selectedPosition)
                {
                    teachers.Add(x.ID);
                }
            });

            if (teachers.Count > 0)
            {
                foreach (var item in teachers)
                {
                    string teachername = cLCase?.Teachers?.FirstOrDefault(t => t.ID == item)?.Name ?? string.Empty;
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr103.ToString(), Description = $"教师 {teachername} 课时过多，超过了方案提供的可用课时!" });
                }
            }

            //检查是否满足学生最大课位
            List<string> students = new List<string>();
            cLCase?.Students?.ForEach(x => {

                int studentClassHour = GetTotalClassHourNumberByStudent(cLCase, x);

                if (studentClassHour > selectedPosition)
                {
                    students.Add(x.ID);
                }
            });

            if (students.Count > 0)
            {
                foreach (var item in students)
                {
                    string studentName = cLCase?.Students?.FirstOrDefault(t => t.ID == item)?.Name ?? string.Empty;
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr104.ToString(), Description = $"学生 {studentName} 课时过多，超过了方案提供的可用课时!" });
                }
            }
            #endregion

            #region 检查课程
            int errCourseID = cLCase.Courses.Where(x => string.IsNullOrEmpty(x.ID)).Count();
            if (errCourseID > 0)
            {
                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr102.ToString(), Description = "课程ID必须存在且不能为空!" });
            }

            int errCourseName = cLCase.Courses.Where(x => string.IsNullOrEmpty(x.Name)).Count();
            if (errCourseName > 0)
            {
                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr102.ToString(), Description = "课程名称必须存在且不能为空!" });
            }

            var errCourseIDUnique = cLCase.Courses.GroupBy(x => x.ID)
                                    ?.Where(grp => grp.Count() > 1)
                                    ?.Select(grp => grp.Key);

            if (errCourseIDUnique != null && errCourseIDUnique.Any())
            {
                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr102.ToString(), Description = "课程ID存在重复!" });
            }

            var errCourseNameUnique = cLCase.Courses.GroupBy(x => x.Name)
                                    ?.Where(grp => grp.Count() > 1)
                                    ?.Select(grp => grp.Key);

            if (errCourseNameUnique != null && errCourseNameUnique.Any())
            {
                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr102.ToString(), Description = "课程名称存在重复!" });
            }

            cLCase.Courses.ForEach(x => {
                x.Levels?.ForEach(le => {
                    if (le.Lessons < 1)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr102.ToString(), Description = $"课程{x.Name}{le.Name}上面设置的课时小于1!" });
                    }
                    else
                    {
                        cLCase.Classes.Where(c => c.CourseID == x.ID && c.LevelID == le.ID)?.ToList()?.ForEach(c => {
                            if (!string.IsNullOrEmpty(x.ID) && !string.IsNullOrEmpty(le.ID))
                            {
                                int classHourCount = cLCase.ClassHours.Where(ch => ch.ClassID == c.ID).Count();
                                if (le.Lessons != classHourCount && classHourCount > 0)
                                {
                                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr102.ToString(), Description = $"课程{x.Name}{le.Name}上面设置的课时数（{le.Lessons}）与实际生成的课时数（{classHourCount}）不相等!" });
                                }
                            }
                        });
                    }
                });
            });

            #endregion

            #region 检查教师
            int errTeacherID = cLCase?.Teachers?.Where(x => string.IsNullOrEmpty(x.ID)).Count() ?? 0;
            if (errTeacherID > 0)
            {
                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr103.ToString(), Description = "教师ID必须存在且不能为空!" });
            }

            int errTeacherName = cLCase?.Teachers?.Where(x => string.IsNullOrEmpty(x.Name)).Count() ?? 0;
            if (errTeacherName > 0)
            {
                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr103.ToString(), Description = "教师名称必须存在且不能为空!" });
            }

            var errTeacherIDUnique = cLCase?.Teachers
                                    ?.GroupBy(x => x.ID)
                                    ?.Where(grp => grp.Count() > 1)
                                    ?.Select(grp => grp.Key);

            if (errTeacherIDUnique.Any())
            {
                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr103.ToString(), Description = "教师ID存在重复!" });
            }
            #endregion

            #region 检查班级
            int errClassID = cLCase.Classes.Where(x => string.IsNullOrEmpty(x.ID)).Count();
            if (errClassID > 0)
            {
                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr104.ToString(), Description = "班级存在ID为空的不规范数据!" });
            }

            int errClassName = cLCase?.Classes?.Where(x => string.IsNullOrEmpty(x.Name)).Count() ?? 0;
            if (errClassName > 0)
            {
                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr104.ToString(), Description = "班级存在名称为空的不规范数据!" });
            }

            var errClassIDUnique = cLCase?.Classes
                                    ?.GroupBy(x => x.ID)
                                    ?.Where(grp => grp.Count() > 1)
                                    ?.Select(grp => grp.Key);

            if (errClassIDUnique != null && errClassIDUnique.Any())
            {
                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr104.ToString(), Description = "班级ID存在重复!" });
            }

            cLCase?.Classes?.ForEach(x => {

                if (!cLCase.Courses.Exists(c => c.ID == x.CourseID))
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr104.ToString(), Description = $"班级 {x.Name} 中ID={x.CourseID}的课程是无效数据，不存在此课程ID!" });
                }

                if (x.TeacherIDs != null)
                {
                    x.TeacherIDs.ForEach(t =>
                    {
                        if (!(cLCase.Teachers?.Exists(ts => ts.ID == t) ?? false))
                        {
                            validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr104.ToString(), Description = $"班级 {x.Name} 中ID={x.CourseID}的课程的教师设置异常，不存在ID={t} 的教师!" });
                        }
                    });
                }
            });
            #endregion

            #region 检查课时
            if (cLCase?.ClassHours != null)
            {
                int minClassHourId = cLCase.ClassHours.Min(x => x.ID);
                if (minClassHourId < 1)
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr105.ToString(), Description = "排课方案最小课时ID小于1!" });
                }
            }

            var errClassHourIDUnique = cLCase?.ClassHours
                                    ?.GroupBy(x => x.ID)
                                    ?.Where(grp => grp.Count() > 1)
                                    ?.Select(grp => grp.Key);

            if (errClassHourIDUnique != null && errClassHourIDUnique.Any())
            {
                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr105.ToString(), Description = "课时ID存在重复!" });
            }

            if (cLCase.ClassHours.Exists(x => string.IsNullOrEmpty(x.ClassID)))
            {
                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr105.ToString(), Description = "某些课时没有班级信息!" });
            }

            if (cLCase.ClassHours.Exists(x => string.IsNullOrEmpty(x.CourseID)))
            {
                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr105.ToString(), Description = "某些课时没有课程信息!" });
            }

            if (cLCase.ClassHours.Exists(x => !cLCase.Courses.Exists(co => co.ID == x.CourseID)))
            {
                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr105.ToString(), Description = "某些课时课程ID无效!" });
            }

            if (cLCase.ClassHours.Exists(x => !cLCase.Classes.Exists(co => co.ID == x.ClassID)))
            {
                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr105.ToString(), Description = "某些课时班级ID无效!" });
            }

            List<string> teacherids = cLCase?.Teachers?.Select(x => x.ID)?.ToList() ?? new List<string>();

            cLCase?.ClassHours?.ForEach(x => {
                if (x.TeacherIDs != null && x.TeacherIDs.Count > 0)
                {
                    var errClassHourTeacherIDUnique = x.TeacherIDs
                                    .GroupBy(t => t)
                                    .Where(grp => grp.Count() > 1)
                                    .Select(grp => grp.Key);

                    if (errClassHourTeacherIDUnique.Any())
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr105.ToString(), Description = $"ID= {x.ID} 的课时教师信息有误，教师重复添加!" });
                    }

                    List<string> validTeachers = x.TeacherIDs.Intersect(teacherids).ToList();
                    if (validTeachers.Count != x.TeacherIDs.Count)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr105.ToString(), Description = $"ID= {x.ID} 的课时教师信息有误，存在无效教师ID!" });
                    }
                }
            });
            #endregion

            #region 检查学生基本信息及志愿是否有问题
            var errStudentIDUnique = cLCase?.Students
                                    ?.GroupBy(x => x.ID)
                                    ?.Where(grp => grp.Count() > 1)
                                    ?.Select(grp => grp.Key);

            if (errStudentIDUnique != null && errStudentIDUnique.Any())
            {
                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr105.ToString(), Description = "学生ID存在重复!" });
            }

            cLCase.Students?.ForEach(x => {

                if (string.IsNullOrEmpty(x.ID))
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr105.ToString(), Description = "排课方案中学生ID不能为空!" });
                }

                if (string.IsNullOrEmpty(x.Name))
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr105.ToString(), Description = "排课方案中学生姓名不能为空!" });
                }

                if (!string.IsNullOrEmpty(x.ID) && !string.IsNullOrEmpty(x.Name))
                {
                    x.Preselections?.ForEach(p => {
                        if (p != null)
                        {
                            if (string.IsNullOrEmpty(p.CourseID) || string.IsNullOrEmpty(p.LevelID))
                            {
                                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr105.ToString(), Description = $"排课方案中学生{x.Name}志愿信息不完整（需要有科目层级信息）!" });
                            }
                            else
                            {
                                if (!(cLCase.Courses.FirstOrDefault(co => co.ID == p.CourseID)?.Levels?.Exists(le => le.ID == p.LevelID) ?? false))
                                {
                                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr105.ToString(), Description = $"排课方案中学生{x.Name}的志愿中课程ID({p.CourseID})与层ID({p.LevelID})组合无效!" });
                                }
                            }
                        }
                    });
                }
            });
            #endregion

            if (validationInfo.Count > 0)
            {
                result = false;
            }

            return Tuple.Create(result, validationInfo);
        }

        private static Tuple<bool, List<DataValidationResultInfo>> ValidateRuleModel(CLCase cLCase, Rule rule)
        {
            List<DataValidationResultInfo> validationInfo = new List<DataValidationResultInfo>();
            bool result = true;

            if (rule == null)
            {
                return Tuple.Create(true, validationInfo);
            }

            #region common
            int maxDays = cLCase.Positions?.Select(p => p.DayPeriod.Day)?.Distinct()?.ToList()?.Count ?? 0;
            #endregion

            #region 检查班级上下午最大占用课时规则
            rule.AmPmClassHours?.ForEach(x => {
                if (x.AmMax < 0)
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr215.ToString(), Description = "上下午课时规则中上午最大课时数不能小于0!" });
                }

                if (x.PmMax < 0)
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr215.ToString(), Description = "上下午课时规则中下午最大课时数不能小于0!" });
                }

                if (string.IsNullOrEmpty(x.ClassID))
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr215.ToString(), Description = "上下午课时规则中存在着无效的班级ID(ID为空)!" });
                }
                else
                {
                    if (!cLCase.Classes.Exists(c => c.ID == x.ClassID))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr215.ToString(), Description = $"上下午课时规则中存在着无效的班级ID({x.ClassID})!" });
                    }
                }

                if (string.IsNullOrEmpty(x.CourseID))
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr215.ToString(), Description = "上下午课时规则中存在着无效的课程ID(ID为空)!" });
                }
                else
                {
                    if (!cLCase.Courses.Exists(c => c.ID == x.CourseID))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr215.ToString(), Description = $"上下午课时规则中存在着无效的课程ID({x.CourseID})!" });
                    }
                }
            });
            #endregion

            #region 检查课时分散规则
            rule.ClassHourAverages?.ForEach(x => {

                if (string.IsNullOrEmpty(x.ClassID))
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr201.ToString(), Description = "课时分散规则中存在着无效的班级ID(ID为空)!" });
                }
                else
                {
                    if (!cLCase.Classes.Exists(c => c.ID == x.ClassID))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr201.ToString(), Description = $"课时分散规则中存在着无效的班级ID({x.ClassID})!" });
                    }
                }

                if (x.MinDay < 1)
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr201.ToString(), Description = $"课时分散规则中存在着无效的最小日期间隔({x.MinDay})，最小间隔不能小于1!" });
                }

                if (x.Weight < 0 || x.Weight > 100)
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr201.ToString(), Description = $"课时分散规则中权重值有错误({x.Weight})" });
                }
            });
            #endregion

            #region 检查多个班级同时开课规则
            rule.ClassHourSameOpens?.ForEach(x => {

                x.Details?.ForEach(d => {
                    if (d.Index < 0)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr212.ToString(), Description = $"多班级同时开课规则中存在着无效的课时索引({d.Index})，最小值应为0!" });
                    }

                    if (d.Classes == null || d.Classes.Count < 2)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr212.ToString(), Description = $"多班级同时开课规则中存在班级不足的情况({d.Classes?.Count ?? 0}个班)，最少需要2个班规则才有意义!" });
                    }

                    d.Classes?.ForEach(c => {
                        if (string.IsNullOrEmpty(c))
                        {
                            validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr212.ToString(), Description = "多班级同时开课规则中存在着无效的班级ID(ID为空)!" });
                        }
                        else
                        {
                            if (!cLCase.Classes.Exists(cl => cl.ID == c))
                            {
                                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr212.ToString(), Description = $"多班级同时开课规则中存在着无效的班级ID({c})!" });
                            }
                            else
                            {

                                if (cLCase.ClassHours.Where(ch => ch.ClassID == c).Count() <= d.Index)
                                {
                                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr212.ToString(), Description = $"多班级同时开课规则中指定课节超出有的班级课时(班级ID={c}，不存在第{d.Index + 1}个课时!" });
                                }
                            }
                        }
                    });
                });
            });
            #endregion

            #region 检查课程连排规则
            rule.ArrangeContinuous?.ForEach(x => {

                if (string.IsNullOrEmpty(x.ClassID))
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr217.ToString(), Description = "课程连排规则中存在着无效的班级ID(ID为空)!" });
                }
                else
                {
                    if (!cLCase.Classes.Exists(c => c.ID == x.ClassID))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr217.ToString(), Description = $"课程连排规则中存在着无效的班级ID({x.ClassID})!" });
                    }
                }

                if (x.Count < 1)
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr217.ToString(), Description = "课程连排规则中设置的连排数量必须大于0!" });
                }

                if (x.IsIntervalDay && (x.IntervalDayWeight < 0 || x.IntervalDayWeight > 100))
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr217.ToString(), Description = $"课程连排规则隔天连排权重应在0-100之间!" });
                }

                if (x.NoCrossingBreak && (x.NoCrossingBreakWeight < 0 || x.NoCrossingBreakWeight > 100))
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr217.ToString(), Description = $"课程连排规则不跨上下午大课间权重应在0-100之间!" });
                }

                if (x.Times != null && x.Times.Count > 0 && (x.TimesWeight < 0 || x.TimesWeight > 100))
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr217.ToString(), Description = $"课程连排规则连排优先时间权重应在0-100之间!" });
                }

                if (x.Times != null)
                {
                    //检查时间有效性
                    x.Times.ForEach(t => {
                        if (!cLCase.Positions.Exists(p => p.DayPeriod.Day == t.Day && p.DayPeriod.Period == t.Period
                                    && p.Position != XYKernel.OS.Common.Enums.Position.AB
                                    && p.Position != XYKernel.OS.Common.Enums.Position.Noon
                                    && p.Position != XYKernel.OS.Common.Enums.Position.PB))
                        {
                            validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr217.ToString(), Description = $"课程连排规则连排优先时间存在无效数据({ValidationUtility.GetTimeSlotInfo(t)})!" });
                        }
                    });
                }
            });
            #endregion

            #region 最大同时开课限制规则
            rule.CourseLimits?.ForEach(x => {

                if (string.IsNullOrEmpty(x.CourseID))
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr219.ToString(), Description = "最大同时开课限制规则中存在着无效的课程ID(ID为空)!" });
                }
                else
                {
                    if (!cLCase.Courses.Exists(c => c.ID == x.CourseID))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr219.ToString(), Description = $"最大同时开课限制规则中存在着无效的课程ID({x.CourseID})!" });
                    }
                }

                if (x.Limit < 1)
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr219.ToString(), Description = "最大同时开课限制规则中最大同时开课数不能小于1!" });
                }

                if (x.PeriodLimits != null && x.PeriodLimits.Count > 0)
                {
                    x.PeriodLimits.ForEach(p => {

                        if (p.Limit < 1)
                        {
                            validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr219.ToString(), Description = "最大同时开课限制规则中最大同时开课数不能小于1!" });
                        }

                        //检查课位有效性
                        if (!cLCase.Positions.Exists(pl => pl.DayPeriod.Day == p.DayPeriod.Day && pl.DayPeriod.Period == p.DayPeriod.Period
                                && pl.Position != XYKernel.OS.Common.Enums.Position.AB
                                && pl.Position != XYKernel.OS.Common.Enums.Position.PB
                                && pl.Position != XYKernel.OS.Common.Enums.Position.Noon))
                        {
                            validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr219.ToString(), Description = $"最大同时开课限制规则中课位存在无效数据({ValidationUtility.GetTimeSlotInfo(p.DayPeriod)})!" });
                        }
                    });
                }
            });
            #endregion

            #region 课程必须禁止时间规则
            rule.CourseTimes?.ForEach(x => {

                if (string.IsNullOrEmpty(x.ClassID))
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr218.ToString(), Description = "课程必须禁止时间规则中没有设置班级ID!" });
                }
                else
                {
                    if (!cLCase.Classes.Exists(c => c.ID == x.ClassID))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr218.ToString(), Description = $"课程必须禁止时间规则中存在着无效的班级ID({x.ClassID})!" });
                    }
                }

                x.MustTimes?.ForEach(mt => {
                    if (!cLCase.Positions.Exists(pl => pl.DayPeriod.Day == mt.Day && pl.DayPeriod.Period == mt.Period
                            && pl.Position != XYKernel.OS.Common.Enums.Position.AB
                            && pl.Position != XYKernel.OS.Common.Enums.Position.PB
                            && pl.Position != XYKernel.OS.Common.Enums.Position.Noon))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr218.ToString(), Description = $"课程必须禁止时间规则中存在着无效课位({ValidationUtility.GetTimeSlotInfo(mt)})!" });
                    }
                });

                x.ForbidTimes?.ForEach(bt => {
                    if (!cLCase.Positions.Exists(pl => pl.DayPeriod.Day == bt.Day && pl.DayPeriod.Period == bt.Period
                            && pl.Position != XYKernel.OS.Common.Enums.Position.AB
                            && pl.Position != XYKernel.OS.Common.Enums.Position.PB
                            && pl.Position != XYKernel.OS.Common.Enums.Position.Noon))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr218.ToString(), Description = $"课程必须禁止时间规则中存在着无效课位({ValidationUtility.GetTimeSlotInfo(bt)})!" });
                    }
                });
            });
            #endregion

            #region 教师每周最大工作天数
            rule.MaxDaysPerWeek?.ForEach(x => {
                if (x.MaxDay < 1)
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr207.ToString(), Description = $"教师每周最大工作天数规则中最大工作天数应该大于0!" });
                }

                if (x.Weight < 0 || x.Weight > 100)
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr207.ToString(), Description = $"教师每周最大工作天数规则中权重应在0-100之间!" });
                }

                if (string.IsNullOrEmpty(x.TeacherID))
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr207.ToString(), Description = "教师每周最大工作天数规则中存在着无效的教师ID(ID为空)!" });
                }
                else
                {
                    if (!(cLCase.Teachers?.Exists(c => c.ID == x.TeacherID) ?? false))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr207.ToString(), Description = $"教师每周最大工作天数规则中存在着无效的教师ID({x.TeacherID})!" });
                    }
                }

                if (x.MaxDay > maxDays)
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr207.ToString(), Description = $"教师每周最大工作天数规则中最大天数不应超过排课方案天数{GetTeacherInfo(cLCase, x.TeacherID)}!" });
                }
            });
            #endregion

            #region 教师每天最大课程间隔规则
            rule.MaxGapsPerDay?.ForEach(x => {
                if (x.MaxIntervel < 0)
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr208.ToString(), Description = $"教师每天最大课程间隔规则中最大课时间隔不应小于0!" });
                }

                if (x.Weight < 0 || x.Weight > 100)
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr208.ToString(), Description = $"教师每天最大课程间隔规则中权重应在0-100之间!" });
                }

                if (string.IsNullOrEmpty(x.TeacherID))
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr208.ToString(), Description = "教师每天最大课程间隔规则中存在着无效的教师ID(ID为空)!" });
                }
                else
                {
                    if (!(cLCase.Teachers?.Exists(c => c.ID == x.TeacherID) ?? false))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr208.ToString(), Description = $"教师每天最大课程间隔规则中存在着无效的教师ID({x.TeacherID})!" });
                    }
                }
            });
            #endregion

            #region 教师每天最大课时
            rule.MaxHoursDaily?.ForEach(x => {
                if (x.MaxHour < 1)
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr206.ToString(), Description = $"教师每天最大课时规则中最大课时不应小于1!" });
                }

                if (x.Weight < 0 || x.Weight > 100)
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr206.ToString(), Description = $"教师每天最大课时规则中权重应在0-100之间!" });
                }

                if (string.IsNullOrEmpty(x.TeacherID))
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr206.ToString(), Description = "教师每天最大课时规则中存在着无效的教师ID(ID为空)!" });
                }
                else
                {
                    if (!(cLCase.Teachers?.Exists(c => c.ID == x.TeacherID) ?? false))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr206.ToString(), Description = $"教师每天最大课时规则中存在着无效的教师ID({x.TeacherID})!" });
                    }
                }
            });
            #endregion

            #region 教师必须禁止时间规则
            rule.TeacherTimes?.ForEach(x => {
                if (string.IsNullOrEmpty(x.TeacherID))
                {
                    validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr205.ToString(), Description = "教师必须禁止时间规则中存在着无效的教师ID(ID为空)!" });
                }
                else
                {
                    if (!(cLCase.Teachers?.Exists(c => c.ID == x.TeacherID) ?? false))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr205.ToString(), Description = $"教师必须禁止时间规则中存在着无效的教师ID({x.TeacherID})!" });
                    }
                }

                x.MustTimes?.ForEach(mt => {
                    if (!cLCase.Positions.Exists(pl => pl.DayPeriod.Day == mt.Day && pl.DayPeriod.Period == mt.Period
                            && pl.Position != XYKernel.OS.Common.Enums.Position.AB
                            && pl.Position != XYKernel.OS.Common.Enums.Position.PB
                            && pl.Position != XYKernel.OS.Common.Enums.Position.Noon))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr205.ToString(), Description = $"教师必须禁止时间规则中存在着无效课位({ValidationUtility.GetTimeSlotInfo(mt)})!" });
                    }
                });

                x.ForbidTimes?.ForEach(bt => {
                    if (!cLCase.Positions.Exists(pl => pl.DayPeriod.Day == bt.Day && pl.DayPeriod.Period == bt.Period
                            && pl.Position != XYKernel.OS.Common.Enums.Position.AB
                            && pl.Position != XYKernel.OS.Common.Enums.Position.PB
                            && pl.Position != XYKernel.OS.Common.Enums.Position.Noon))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = RuleError.XYErr205.ToString(), Description = $"教师必须禁止时间规则中存在着无效课位({ValidationUtility.GetTimeSlotInfo(bt)})!" });
                    }
                });
            });
            #endregion

            if (validationInfo.Count > 0)
            {
                result = false;
            }
            else
            {
                result = true;
            }

            return Tuple.Create(result, validationInfo);
        }

        private static Tuple<bool, List<DataValidationResultInfo>> ValidateAlgoRuleModel(CLCase cLCase, AlgoRule algoRule)
        {
            List<DataValidationResultInfo> validationInfo = new List<DataValidationResultInfo>();
            bool result = true;

            if (algoRule == null)
            {
                return Tuple.Create(true, validationInfo);
            }

            #region common
            int maxDays = cLCase.Positions?.Select(p => p.DayPeriod.Day)?.Distinct()?.ToList()?.Count ?? 0;
            #endregion

            #region 单个课时有一个优先开始时间
            algoRule.ClassHourRequiredStartingTime?.ForEach(x => {
                if (x.Active)
                {
                    if (x.Weight < 0 || x.Weight > 100)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr301.ToString(), Description = $"单个课时有一个优先开始时间规则中权重应在0-100之间!" });
                    }

                    if (!cLCase.ClassHours.Exists(c =>/* c.Active && */c.ID == x.ID))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr301.ToString(), Description = $"单个课时有一个优先开始时间规则中课时ID无效(ID={x.ID})!" });
                    }

                    if (!cLCase.Positions.Exists(p => p.DayPeriod.Day == x.Period.Day && p.DayPeriod.Period == x.Period.Period
                            && p.Position != XYKernel.OS.Common.Enums.Position.AB
                            && p.Position != XYKernel.OS.Common.Enums.Position.Noon
                            && p.Position != XYKernel.OS.Common.Enums.Position.PB))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr301.ToString(), Description = $"单个课时有一个优先开始时间规则中课位信息无效({ValidationUtility.GetTimeSlotInfo(x.Period)})!" });
                    }
                }
            });
            #endregion

            #region 单个课时有多个优先开始时间
            algoRule.ClassHourRequiredStartingTimes?.ForEach(x => {
                if (x.Active)
                {
                    if (x.Weight < 0 || x.Weight > 100)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr302.ToString(), Description = $"单个课时有多个优先开始时间规则中权重应在0-100之间!" });
                    }

                    if (!cLCase.ClassHours.Exists(c =>/* c.Active && */c.ID == x.ID))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr302.ToString(), Description = $"单个课时有多个优先开始时间规则中课时ID无效(ID={x.ID})!" });
                    }

                    if (x.Times == null)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr302.ToString(), Description = $"单个课时有多个优先开始时间规则中没有设置优先时间!" });
                    }
                    else
                    {
                        x.Times.ForEach(t => {
                            if (!cLCase.Positions.Exists(p => p.DayPeriod.Day == t.Day && p.DayPeriod.Period == t.Period
                                    && p.Position != XYKernel.OS.Common.Enums.Position.AB
                                    && p.Position != XYKernel.OS.Common.Enums.Position.Noon
                                    && p.Position != XYKernel.OS.Common.Enums.Position.PB))
                            {
                                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr302.ToString(), Description = $"单个课时有多个优先开始时间规则中课位信息无效({ValidationUtility.GetTimeSlotInfo(t)})!" });
                            }
                        });
                    }
                }
            });
            #endregion

            #region 单个课时有多个优先课位
            algoRule.ClassHourRequiredTimes?.ForEach(x => {
                if (x.Active)
                {
                    if (x.Weight < 0 || x.Weight > 100)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr303.ToString(), Description = $"单个课时有多个优先课位规则中权重应在0-100之间!" });
                    }

                    if (!cLCase.ClassHours.Exists(c =>/* c.Active && */c.ID == x.ID))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr303.ToString(), Description = $"单个课时有多个优先课位规则中课时ID无效(ID={x.ID})!" });
                    }

                    if (x.Times == null)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr303.ToString(), Description = $"单个课时有多个优先课位规则中没有设置优先时间!" });
                    }
                    else
                    {
                        x.Times.ForEach(t => {
                            if (!cLCase.Positions.Exists(p => p.DayPeriod.Day == t.Day && p.DayPeriod.Period == t.Period
                                    && p.Position != XYKernel.OS.Common.Enums.Position.AB
                                    && p.Position != XYKernel.OS.Common.Enums.Position.Noon
                                    && p.Position != XYKernel.OS.Common.Enums.Position.PB))
                            {
                                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr303.ToString(), Description = $"单个课时有多个优先课位规则中课位信息无效({ValidationUtility.GetTimeSlotInfo(t)})!" });
                            }
                        });
                    }
                }
            });
            #endregion

            #region 在选定课位中设置多个课时的最大同时开课数量
            algoRule.ClassHoursMaxConcurrencyInSelectedTimes?.ForEach(x => {
                if (x.Active)
                {
                    if (x.ID == null || x.ID.Length < 1)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr304.ToString(), Description = $"在选定课位中设置多个课时的最大同时开课数量规则中课时ID无效(ID为空)!" });
                    }
                    else
                    {
                        for (int i = 0; i < x.ID.Length; i++)
                        {
                            if (!cLCase.ClassHours.Exists(c =>/* c.Active && */c.ID == x.ID[i]))
                            {
                                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr304.ToString(), Description = $"在选定课位中设置多个课时的最大同时开课数量规则中课时ID无效(ID={x.ID[i]})!" });
                            }
                        }
                    }

                    if (x.MaxConcurrencyNumber < 1)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr304.ToString(), Description = $"在选定课位中设置多个课时的最大同时开课数量规则中最大开课数量不能小于1!" });
                    }

                    if (x.Times == null)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr304.ToString(), Description = $"在选定课位中设置多个课时的最大同时开课数量规则中没有设置优先时间!" });
                    }
                    else
                    {
                        x.Times.ForEach(t => {
                            if (!cLCase.Positions.Exists(p => p.DayPeriod.Day == t.Day && p.DayPeriod.Period == t.Period
                                    && p.Position != XYKernel.OS.Common.Enums.Position.AB
                                    && p.Position != XYKernel.OS.Common.Enums.Position.Noon
                                    && p.Position != XYKernel.OS.Common.Enums.Position.PB))
                            {
                                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr304.ToString(), Description = $"在选定课位中设置多个课时的最大同时开课数量规则中课位信息无效({ValidationUtility.GetTimeSlotInfo(t)})!" });
                            }
                        });
                    }
                }
            });
            #endregion

            #region 多个课时不同时开课
            algoRule.ClassHoursNotOverlaps?.ForEach(x => {
                if (x.Active)
                {
                    if (x.ID == null || x.ID.Length < 1)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr305.ToString(), Description = $"多个课时不同时开课规则中课时ID无效(ID为空)!" });
                    }
                    else
                    {
                        for (int i = 0; i < x.ID.Length; i++)
                        {
                            if (!cLCase.ClassHours.Exists(c =>/* c.Active && */c.ID == x.ID[i]))
                            {
                                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr305.ToString(), Description = $"多个课时不同时开课规则中课时ID无效(ID={x.ID[i]})!" });
                            }
                        }
                    }

                    if (x.Weight < 0 || x.Weight > 100)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr305.ToString(), Description = $"多个课时不同时开课规则中权重应在0-100之间!" });
                    }
                }
            });
            #endregion

            #region 多个课时在选定课位中占用的最大数量
            algoRule.ClassHoursOccupyMaxTimeFromSelections?.ForEach(x => {
                if (x.Active)
                {
                    if (x.ID == null || x.ID.Length < 1)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr306.ToString(), Description = $"多个课时在选定课位中占用的最大数量规则中课时ID无效(ID为空)!" });
                    }
                    else
                    {
                        for (int i = 0; i < x.ID.Length; i++)
                        {
                            if (!cLCase.ClassHours.Exists(c =>/* c.Active && */c.ID == x.ID[i]))
                            {
                                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr306.ToString(), Description = $"多个课时在选定课位中占用的最大数量规则中课时ID无效(ID={x.ID[i]})!" });
                            }
                        }
                    }

                    if (x.MaxOccupyNumber < 1)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr306.ToString(), Description = $"多个课时在选定课位中占用的最大数量规则中最大占用数量不能小于1!" });
                    }

                    if (x.Times == null)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr306.ToString(), Description = $"多个课时在选定课位中占用的最大数量规则中没有设置选定课位!" });
                    }
                    else
                    {
                        x.Times.ForEach(t => {
                            if (!cLCase.Positions.Exists(p => p.DayPeriod.Day == t.Day && p.DayPeriod.Period == t.Period
                                    && p.Position != XYKernel.OS.Common.Enums.Position.AB
                                    && p.Position != XYKernel.OS.Common.Enums.Position.Noon
                                    && p.Position != XYKernel.OS.Common.Enums.Position.PB))
                            {
                                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr306.ToString(), Description = $"多个课时在选定课位中占用的最大数量规则中课位信息无效({ValidationUtility.GetTimeSlotInfo(t)})!" });
                            }
                        });
                    }
                }
            });
            #endregion

            #region 多个课时有多个优先开始时间
            algoRule.ClassHoursRequiredStartingTimes?.ForEach(x => {
                if (x.Active)
                {
                    if (x.Weight < 0 || x.Weight > 100)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr307.ToString(), Description = $"多个课时有多个优先开始时间规则中权重应在0-100之间!" });
                    }

                    if (x.Times == null)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr307.ToString(), Description = $"多个课时有多个优先开始时间规则中没有设置优先开始时间!" });
                    }
                    else
                    {
                        x.Times.ForEach(t => {
                            if (!cLCase.Positions.Exists(p => p.DayPeriod.Day == t.Day && p.DayPeriod.Period == t.Period
                                    && p.Position != XYKernel.OS.Common.Enums.Position.AB
                                    && p.Position != XYKernel.OS.Common.Enums.Position.Noon
                                    && p.Position != XYKernel.OS.Common.Enums.Position.PB))
                            {
                                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr307.ToString(), Description = $"多个课时有多个优先开始时间规则中课位信息无效({ValidationUtility.GetTimeSlotInfo(t)})!" });
                            }
                        });
                    }

                    if (!string.IsNullOrEmpty(x.CourseID) && !cLCase.Courses.Exists(c => c.ID == x.CourseID))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr307.ToString(), Description = $"多个课时有多个优先开始时间规则中存在着无效的课程ID({x.CourseID})!" });
                    }

                    if (!string.IsNullOrEmpty(x.TeacherID) && !(cLCase.Teachers?.Exists(c => c.ID == x.TeacherID) ?? false))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr307.ToString(), Description = $"多个课时有多个优先开始时间规则中存在着无效的教师ID({x.TeacherID})!" });
                    }

                    var qResult = cLCase.ClassHours.ToList();

                    if (!string.IsNullOrEmpty(x.CourseID) && cLCase.Courses.Exists(c => c.ID == x.CourseID))
                    {
                        qResult = qResult.Where(c => c.CourseID != null && c.CourseID == x.CourseID).ToList();
                    }

                    if (!string.IsNullOrEmpty(x.TeacherID) && (cLCase.Teachers?.Exists(c => c.ID == x.TeacherID) ?? false))
                    {
                        qResult = qResult.Where(c => c.TeacherIDs != null && c.TeacherIDs.Contains(x.TeacherID)).ToList();
                    }
                }
            });
            #endregion

            #region 多个课时有多个优先课位
            algoRule.ClassHoursRequiredTimes?.ForEach(x => {
                if (x.Active)
                {
                    if (x.Weight < 0 || x.Weight > 100)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr308.ToString(), Description = $"多个课时有多个优先课位规则中权重应在0-100之间!" });
                    }

                    if (x.Times == null)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr308.ToString(), Description = $"多个课时有多个优先课位规则中没有设置优先开始时间!" });
                    }
                    else
                    {
                        x.Times.ForEach(t => {
                            if (!cLCase.Positions.Exists(p => p.DayPeriod.Day == t.Day && p.DayPeriod.Period == t.Period
                                    && p.Position != XYKernel.OS.Common.Enums.Position.AB
                                    && p.Position != XYKernel.OS.Common.Enums.Position.Noon
                                    && p.Position != XYKernel.OS.Common.Enums.Position.PB))
                            {
                                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr308.ToString(), Description = $"多个课时有多个优先课位规则中课位信息无效({ValidationUtility.GetTimeSlotInfo(t)})!" });
                            }
                        });
                    }

                    if (!string.IsNullOrEmpty(x.CourseID) && !cLCase.Courses.Exists(c => c.ID == x.CourseID))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr308.ToString(), Description = $"多个课时有多个优先课位规则中存在着无效的课程ID({x.CourseID})!" });
                    }

                    if (!string.IsNullOrEmpty(x.TeacherID) && !(cLCase.Teachers?.Exists(c => c.ID == x.TeacherID) ?? false))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr308.ToString(), Description = $"多个课时有多个优先课位规则中存在着无效的教师ID({x.TeacherID})!" });
                    }

                    var qResult = cLCase.ClassHours.ToList();

                    if (!string.IsNullOrEmpty(x.CourseID) && cLCase.Courses.Exists(c => c.ID == x.CourseID))
                    {
                        qResult = qResult.Where(c => c.CourseID != null && c.CourseID == x.CourseID).ToList();
                    }

                    if (!string.IsNullOrEmpty(x.TeacherID) && (cLCase.Teachers?.Exists(c => c.ID == x.TeacherID) ?? false))
                    {
                        qResult = qResult.Where(c => c.TeacherIDs != null && c.TeacherIDs.Contains(x.TeacherID)).ToList();
                    }
                }
            });
            #endregion

            #region 多个课时有相同的开始日期（日期）
            algoRule.ClassHoursSameStartingDays?.ForEach(x => {
                if (x.Active)
                {
                    if (x.Weight < 0 || x.Weight > 100)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr309.ToString(), Description = $"多个课时有相同的开始日期规则中权重应在0-100之间!" });
                    }

                    if (x.ID == null || x.ID.Length < 2)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr309.ToString(), Description = $"多个课时有相同的开始日期规则中选择的课时数量不能少于2!" });
                    }
                    else
                    {
                        for (int i = 0; i < x.ID.Length; i++)
                        {
                            if (!cLCase.ClassHours.Exists(c => c.ID == x.ID[i]))
                            {
                                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr309.ToString(), Description = $"多个课时有相同的开始日期规则中存在无效课时ID(ID={x.ID[i]})!" });
                            }
                        }
                    }
                }
            });
            #endregion

            #region 多个课时有相同的开始课位（时间）
            algoRule.ClassHoursSameStartingHours?.ForEach(x => {
                if (x.Active)
                {
                    if (x.Weight < 0 || x.Weight > 100)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr310.ToString(), Description = $"多个课时有相同的开始课位（时间）规则中权重应在0-100之间!" });
                    }

                    if (x.ID == null || x.ID.Length < 2)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr310.ToString(), Description = $"多个课时有相同的开始课位（时间）规则中选择的课时数量不能少于2!" });
                    }
                    else
                    {
                        for (int i = 0; i < x.ID.Length; i++)
                        {
                            if (!cLCase.ClassHours.Exists(c => c.ID == x.ID[i]))
                            {
                                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr310.ToString(), Description = $"多个课时有相同的开始课位（时间）规则中存在无效课时ID(ID={x.ID[i]})!" });
                            }
                        }
                    }
                }
            });
            #endregion

            #region 多个课时有相同的开始时间（日期+时间）
            algoRule.ClassHoursSameStartingTimes?.ForEach(x => {
                if (x.Active)
                {
                    if (x.Weight < 0 || x.Weight > 100)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr311.ToString(), Description = $"多个课时有相同的开始时间（日期+时间）规则中权重应在0-100之间!" });
                    }

                    if (x.ID == null || x.ID.Length < 2)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr311.ToString(), Description = $"多个课时有相同的开始时间（日期+时间）规则中选择的课时数量不能少于2!" });
                    }
                    else
                    {
                        for (int i = 0; i < x.ID.Length; i++)
                        {
                            if (!cLCase.ClassHours.Exists(c => c.ID == x.ID[i]))
                            {
                                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr311.ToString(), Description = $"多个课时有相同的开始时间（日期+时间）规则中存在无效课时ID(ID={x.ID[i]})!" });
                            }
                        }
                    }
                }
            });
            #endregion

            #region 多个课时之间的最大间隔天数
            algoRule.MaxDaysBetweenClassHours?.ForEach(x => {
                if (x.Active)
                {
                    if (x.Weight < 0 || x.Weight > 100)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr312.ToString(), Description = $"多个课时之间的最大间隔天数规则中权重应在0-100之间!" });
                    }

                    if (x.MaxDays < 0)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr312.ToString(), Description = $"多个课时之间的最大间隔天数规则中最大间隔天数不应小于0!" });
                    }

                    if (x.ID == null || x.ID.Length < 2)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr312.ToString(), Description = $"多个课时之间的最大间隔天数规则中选择的课时数量不能少于2!" });
                    }
                    else
                    {
                        for (int i = 0; i < x.ID.Length; i++)
                        {
                            if (!cLCase.ClassHours.Exists(c => c.ID == x.ID[i]))
                            {
                                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr312.ToString(), Description = $"多个课时之间的最大间隔天数规则中存在无效课时ID(ID={x.ID[i]})!" });
                            }
                        }
                    }
                }
            });
            #endregion

            #region 多个课时间最小课程间隔天数
            algoRule.MinDaysBetweenClassHours?.ForEach(x => {
                if (x.Active)
                {
                    if (x.Weight < 0 || x.Weight > 100)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr313.ToString(), Description = $"多个课时间最小课程间隔天数规则中权重应在0-100之间!" });
                    }

                    if (x.MinDays < 0)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr313.ToString(), Description = $"多个课时间最小课程间隔天数规则中最小间隔天数不应小于0!" });
                    }

                    if (x.ID == null || x.ID.Length < 2)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr313.ToString(), Description = $"多个课时间最小课程间隔天数规则中选择的课时数量不能少于2!" });
                    }
                    else
                    {
                        for (int i = 0; i < x.ID.Length; i++)
                        {
                            if (!cLCase.ClassHours.Exists(c => c.ID == x.ID[i]))
                            {
                                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr313.ToString(), Description = $"多个课时间最小课程间隔天数规则中存在无效课时ID(ID={x.ID[i]})!" });
                            }
                        }
                    }
                }
            });
            #endregion

            #region 教师每周最大工作天数
            algoRule.TeacherMaxDaysPerWeeks?.ForEach(x => {
                if (x.Active)
                {
                    if (x.MaxDays < 1)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr314.ToString(), Description = $"教师每周最大工作天数规则中最大工作天数不应小于1!" });
                    }

                    if (string.IsNullOrEmpty(x.TeacherID))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr314.ToString(), Description = "教师每周最大工作天数规则中存在着无效的教师ID(ID为空)!" });
                    }
                    else
                    {
                        if (!(cLCase.Teachers?.Exists(c => c.ID == x.TeacherID) ?? false))
                        {
                            validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr314.ToString(), Description = $"教师每周最大工作天数规则中存在着无效的教师ID({x.TeacherID})!" });
                        }
                    }

                    if (x.MaxDays > maxDays)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr314.ToString(), Description = $"教师每周最大工作天数规则中最大天数不应超过排课方案天数{GetTeacherInfo(cLCase, x.TeacherID)}!" });
                    }
                }
            });
            #endregion

            #region 教师每天最大课程间隔
            algoRule.TeacherMaxGapsPerDays?.ForEach(x => {
                if (x.Active)
                {
                    if (x.MaxGaps < 0)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr315.ToString(), Description = $"教师每天最大课程间隔规则中最大间隔不应小于0!" });
                    }

                    if (string.IsNullOrEmpty(x.TeacherID))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr315.ToString(), Description = "教师每天最大课程间隔规则中存在着无效的教师ID(ID为空)!" });
                    }
                    else
                    {
                        if (!(cLCase.Teachers?.Exists(c => c.ID == x.TeacherID) ?? false))
                        {
                            validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr315.ToString(), Description = $"教师每天最大课程间隔规则中存在着无效的教师ID({x.TeacherID})!" });
                        }
                    }
                }
            });
            #endregion

            #region 教师每天最大课时数
            algoRule.TeacherMaxHoursDailys?.ForEach(x => {
                if (x.Active)
                {
                    if (x.MaxHours < 1)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr316.ToString(), Description = $"教师每天最大课时数规则中最大课时数不应小于1!" });
                    }

                    if (string.IsNullOrEmpty(x.TeacherID))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr316.ToString(), Description = "教师每天最大课时数规则中存在着无效的教师ID(ID为空)!" });
                    }
                    else
                    {
                        if (!(cLCase.Teachers?.Exists(c => c.ID == x.TeacherID) ?? false))
                        {
                            validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr316.ToString(), Description = $"教师每天最大课时数隔规则中存在着无效的教师ID({x.TeacherID})!" });
                        }
                    }
                }
            });
            #endregion

            #region 教师的不可用时间
            algoRule.TeacherNotAvailableTimes?.ForEach(x => {
                if (x.Active)
                {
                    if (x.Weight < 0 || x.Weight > 100)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr317.ToString(), Description = $"教师的不可用时间规则中权重应在0-100之间!" });
                    }

                    if (x.Times == null)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr317.ToString(), Description = $"教师的不可用时间规则中没有设置优先时间!" });
                    }
                    else
                    {
                        x.Times.ForEach(t => {
                            if (!cLCase.Positions.Exists(p => p.DayPeriod.Day == t.Day && p.DayPeriod.Period == t.Period
                                    && p.Position != XYKernel.OS.Common.Enums.Position.AB
                                    && p.Position != XYKernel.OS.Common.Enums.Position.Noon
                                    && p.Position != XYKernel.OS.Common.Enums.Position.PB))
                            {
                                validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr317.ToString(), Description = $"教师的不可用时间规则中课位信息无效({ValidationUtility.GetTimeSlotInfo(t)})!" });
                            }
                        });
                    }

                    if (string.IsNullOrEmpty(x.TeacherID))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr317.ToString(), Description = "教师的不可用时间规则中存在着无效的教师ID(ID为空)!" });
                    }
                    else
                    {
                        if (!(cLCase.Teachers?.Exists(c => c.ID == x.TeacherID) ?? false))
                        {
                            validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr317.ToString(), Description = $"教师的不可用时间规则中存在着无效的教师ID({x.TeacherID})!" });
                        }
                    }
                }
            });
            #endregion

            #region 所有教师每周最大工作天数
            algoRule.TeachersMaxDaysPerWeeks?.ForEach(x => {
                if (x.Active)
                {
                    if (x.MaxDays < 1)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr318.ToString(), Description = $"所有教师每周最大工作天数规则中最大工作天数不应小于1!" });
                    }

                    if (x.MaxDays > maxDays)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr318.ToString(), Description = $"所有教师每周最大工作天数规则中最大天数不应超过排课方案天数!" });
                    }
                }
            });
            #endregion

            #region 所有教师每天最大课程间隔
            algoRule.TeachersMaxGapsPerDays?.ForEach(x => {
                if (x.Active)
                {
                    if (x.MaxGaps < 0)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr319.ToString(), Description = $"所有教师每天最大课程间隔规则中最大间隔数不应小于0!" });
                    }
                }
            });
            #endregion

            #region 所有教师每天最大课时数
            algoRule.TeachersMaxHoursDailys?.ForEach(x => {
                if (x.Active)
                {
                    if (x.MaxHours < 1)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr320.ToString(), Description = $"所有教师每天最大课时数隔规则中最大间隔数不应小于1!" });
                    }
                }
            });
            #endregion

            #region 给3个课时分组
            algoRule.ThreeClassHoursGrouped?.ForEach(x => {
                if (x.Active)
                {
                    if (x.Weight < 0 || x.Weight > 100)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr321.ToString(), Description = $"给3个课时分组规则中权重应在0-100之间!" });
                    }

                    if (!cLCase.ClassHours.Exists(c => c.ID == x.FirstID))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr321.ToString(), Description = $"给3个课时分组规则中存在无效的课时ID（ID={x.FirstID}）!" });
                    }

                    if (!cLCase.ClassHours.Exists(c => c.ID == x.SecondID))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr321.ToString(), Description = $"给3个课时分组规则中存在无效的课时ID（ID={x.SecondID}）!" });
                    }

                    if (!cLCase.ClassHours.Exists(c => c.ID == x.ThirdID))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr321.ToString(), Description = $"给3个课时分组规则中存在无效的课时ID（ID={x.ThirdID}）!" });
                    }

                    if (x.FirstID == x.SecondID || x.FirstID == x.ThirdID || x.SecondID == x.ThirdID)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr321.ToString(), Description = $"给3个课时分组规则中3个课时存在重复情况（ID={x.FirstID},{x.SecondID},{x.ThirdID}）!" });
                    }
                }
            });
            #endregion

            #region 对2个课时设置连排
            algoRule.TwoClassHoursContinuous?.ForEach(x => {
                if (x.Active)
                {
                    if (x.Weight < 0 || x.Weight > 100)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr322.ToString(), Description = $"对2个课时设置连排规则中权重应在0-100之间!" });
                    }

                    if (!cLCase.ClassHours.Exists(c => c.ID == x.FirstID))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr322.ToString(), Description = $"对2个课时设置连排规则中存在无效的课时ID（ID={x.FirstID}）!" });
                    }

                    if (!cLCase.ClassHours.Exists(c => c.ID == x.SecondID))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr322.ToString(), Description = $"对2个课时设置连排规则中存在无效的课时ID（ID={x.SecondID}）!" });
                    }

                    if (x.FirstID == x.SecondID)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr322.ToString(), Description = $"对2个课时设置连排规则中2个课时存在重复情况（ID={x.FirstID},{x.SecondID}）!" });
                    }
                }
            });
            #endregion

            #region 给2个课时分组
            algoRule.TwoClassHoursGrouped?.ForEach(x => {
                if (x.Active)
                {
                    if (x.Weight < 0 || x.Weight > 100)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr323.ToString(), Description = $"给2个课时分组规则中权重应在0-100之间!" });
                    }

                    if (!cLCase.ClassHours.Exists(c => c.ID == x.FirstID))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr323.ToString(), Description = $"给2个课时分组规则中存在无效的课时ID（ID={x.FirstID}）!" });
                    }

                    if (!cLCase.ClassHours.Exists(c => c.ID == x.SecondID))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr323.ToString(), Description = $"给2个课时分组规则中存在无效的课时ID（ID={x.SecondID}）!" });
                    }

                    if (x.FirstID == x.SecondID)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr323.ToString(), Description = $"给2个课时分组规则中2个课时存在重复情况（ID={x.FirstID},{x.SecondID}）!" });
                    }
                }
            });
            #endregion

            #region 对2个课时排序
            algoRule.TwoClassHoursOrdered?.ForEach(x => {
                if (x.Active)
                {
                    if (x.Weight < 0 || x.Weight > 100)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr324.ToString(), Description = $"对2个课时排序规则中权重应在0-100之间!" });
                    }

                    if (!cLCase.ClassHours.Exists(c => c.ID == x.FirstID))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr324.ToString(), Description = $"对2个课时排序规则中存在无效的课时ID（ID={x.FirstID}）!" });
                    }

                    if (!cLCase.ClassHours.Exists(c => c.ID == x.SecondID))
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr324.ToString(), Description = $"对2个课时排序规则中存在无效的课时ID（ID={x.SecondID}）!" });
                    }

                    if (x.FirstID == x.SecondID)
                    {
                        validationInfo.Add(new DataValidationResultInfo() { ErrorCode = AlgoRuleError.XYErr324.ToString(), Description = $"对2个课时排序规则中2个课时存在重复情况（ID={x.FirstID},{x.SecondID}）!" });
                    }
                }
            });
            #endregion

            if (validationInfo.Count > 0)
            {
                result = false;
            }
            else
            {
                result = true;
            }

            return Tuple.Create(result, validationInfo);
        }

        private static Tuple<bool, List<DataValidationResultInfo>> ValidateClassCapacity(CLCase cLCase)
        {
            List<DataValidationResultInfo> validationInfo = new List<DataValidationResultInfo>();
            bool result = true;

            cLCase.Courses.ForEach(c => {
                c.Levels?.ForEach(cl => {
                    if (!string.IsNullOrEmpty(cl.ID))
                    {
                        int totalStudents = cLCase.Students?.Where(s => s.Preselections != null && s.Preselections.Exists(p => p.CourseID == c.ID && p.LevelID == cl.ID))?.Count() ?? 0;
                        int totalClassesCapacity = cLCase.Classes.Where(cs => cs.CourseID == c.ID && cs.LevelID == cl.ID).Sum(cs => cs.Capacity);

                        if (totalStudents > totalClassesCapacity)
                        {
                            string levelInfo = string.IsNullOrEmpty(cl.Name) ? "" : $"层 {cl.Name} ";
                            validationInfo.Add(new DataValidationResultInfo() { ErrorCode = CaseError.XYErr106.ToString(), Description = $"科目 { c.Name } {levelInfo}的班额不足!" });
                        }
                    }
                });
            });

            if (validationInfo.Count > 0)
            {
                result = false;
            }
            else
            {
                result = true;
            }

            return Tuple.Create(result, validationInfo);
        }

        /// <summary>
        /// 统计一个教师的课时
        /// </summary>
        /// <param name="cLCase"></param>
        /// <param name="TeacherID"></param>
        /// <returns></returns>
        private static int GetTotalClassHourNumberByTeacherID(CLCase cLCase, string TeacherID)
        {
            int classHourNumber = 0;

            if (!string.IsNullOrEmpty(TeacherID))
            {
                classHourNumber = cLCase.ClassHours?.Where(c => c.TeacherIDs != null && c.TeacherIDs.Contains(TeacherID)).Count() ?? 0;
            }

            return classHourNumber;
        }

        /// <summary>
        /// 统计一个班级的课时
        /// </summary>
        /// <param name="cLCase"></param>
        /// <param name="Student"></param>
        /// <returns></returns>
        private static int GetTotalClassHourNumberByStudent(CLCase cLCase, StudentModel Student)
        {
            int classHourNumber = 0;

            if (Student?.Preselections != null)
            {
                Student.Preselections.ForEach(x => {
                    if (!string.IsNullOrEmpty(x.CourseID) && !string.IsNullOrEmpty(x.LevelID))
                    {
                        classHourNumber = classHourNumber + (cLCase.Courses?.FirstOrDefault(co => co.ID == x.CourseID)?.Levels?.FirstOrDefault(le => le.ID == x.LevelID)?.Lessons ?? 0);
                    }
                });
            }

            return classHourNumber;
        }

        /// <summary>
        /// 统计方案可用课位数
        /// </summary>
        /// <param name="cLCase"></param>
        /// <returns></returns>
        private static int GetAvailableTimeSlot(CLCase cLCase)
        {
            int selectedPosition = cLCase?.Positions?.Where(x => x.IsSelected
            && x.Position != XYKernel.OS.Common.Enums.Position.AB
            && x.Position != XYKernel.OS.Common.Enums.Position.PB
            && x.Position != XYKernel.OS.Common.Enums.Position.Noon)?.Count() ?? 0;

            return selectedPosition;
        }

        private static string GetTeacherInfo(CLCase cLCase, string teacherID)
        {
            string teacherInfo = string.Empty;

            if (string.IsNullOrEmpty(teacherID))
            {
                teacherInfo = "(ID=, Name=)";
            }
            else
            {
                teacherInfo = $"(ID={teacherID}, Name={cLCase.Teachers?.FirstOrDefault(c => c.ID == teacherID)?.Name ?? string.Empty})";
            }

            return teacherInfo;
        }
    }
}
