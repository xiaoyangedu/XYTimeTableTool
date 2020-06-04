using System;
using System.Linq;
using System.Collections.Generic;
using XYKernel.OS.Common.Enums;
using XYKernel.OS.Common.Models.Mixed;
using XYKernel.OS.Common.Models.Mixed.AlgoRule;
using XYKernel.OS.Common.Models.Mixed.Rule;
using XYKernel.OS.Common.Models.Pattern.Base;
using XYKernel.OS.Common.Models.Mixed.Result;
using XYKernel.OS.Common.Models.Pattern.Extend;

namespace OSKernel.Presentation.Arranging.Mixed.Pattern.Data
{
    public class DataProcessing
    {
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 仅排教师
        /// </summary>
        /// <param name="cLCase"></param>
        /// <param name="rule"></param>
        /// <param name="algoRule"></param>
        /// <returns></returns>
        public Tuple<CLCase, bool, List<DataValidationResultInfo>> GetModelWithoutStudents(CLCase cLCase, Rule rule, AlgoRule algoRule)
        {
            List<DataValidationResultInfo> mydvri = new List<DataValidationResultInfo>() { };
            bool checkResult = ModelValidation.ValidateModel(cLCase, algoRule, rule, out mydvri);
            if (!checkResult)
            {
                return Tuple.Create(cLCase, false, mydvri);
            }

            try
            {
                //remove all students
                cLCase?.Students?.Clear();

                cLCase?.Classes?.ForEach(x => {
                    x.StudentIDs.Clear();
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return Tuple.Create(cLCase, true, mydvri);
        }

        /// <summary>
        /// 仅排教师和分到班的学生，删除所有班级未固定的学生
        /// </summary>
        /// <param name="cLCase"></param>
        /// <param name="rule"></param>
        /// <param name="algoRule"></param>
        /// <returns></returns>
        public Tuple<CLCase, bool, List<DataValidationResultInfo>> GetModelWithOnlyStudentsAssignedToClass(CLCase cLCase, Rule rule, AlgoRule algoRule)
        {
            List<DataValidationResultInfo> mydvri = new List<DataValidationResultInfo>() { };
            bool checkResult = ModelValidation.ValidateModel(cLCase, algoRule, rule, out mydvri);
            if (!checkResult)
            {
                return Tuple.Create(cLCase, false, mydvri);
            }

            //仅排教师和分到班的学生，删除所有班级未固定的学生
            cLCase?.Courses?.ForEach(x => {
                x.Levels?.ForEach(le => {
                    int classNumber = cLCase?.Classes?.Where(c => c.CourseID == x.ID && c.LevelID == le.ID)?.ToList()?.Count ?? 0;

                    if (classNumber > 1)
                    {
                        cLCase?.Students?.ForEach(s => {
                            s.Preselections.RemoveAll(p => p.CourseID == x.ID && p.LevelID == le.ID);
                        });
                    }
                });
            });

            return Tuple.Create(cLCase, true, mydvri);
        }

        /// <summary>
        /// 常规排课模式
        /// </summary>
        /// <param name="cLCase"></param>
        /// <param name="rule"></param>
        /// <param name="algoRule"></param>
        /// <param name="normalModel"></param>
        /// <returns></returns>
        public Tuple<CLCase, bool, List<DataValidationResultInfo>> GetModelByNormal(CLCase cLCase, Rule rule, AlgoRule algoRule, NormalModel normalModel)
        {
            List<DataValidationResultInfo> mydvri = new List<DataValidationResultInfo>() { };
            bool checkResult = ModelValidation.ValidateModel(cLCase, algoRule, rule, out mydvri);
            if (!checkResult)
            {
                return Tuple.Create(cLCase, false, mydvri);
            }

            if (cLCase != null && normalModel != null)
            {
                //remove selected combinations
                if (normalModel.RemovedCombination != null)
                {
                    cLCase = Utility.GetCaseByRemovedCombination(cLCase, normalModel.RemovedCombination);
                }

                //Update Capacity
                if (normalModel.ClassCapacity != null)
                {
                    normalModel.ClassCapacity.ForEach(x => {
                        cLCase?.Classes?.Where(c => c.ID == x.ClassId)?.ToList()?.ForEach(c => {
                            c.Capacity = x.Capacity;
                        });
                    });
                }

                //Update Positions
                if (normalModel.Positions != null)
                {
                    cLCase.Positions = normalModel.Positions;
                }
            }

            return Tuple.Create(cLCase, true, mydvri);
        }

        /// <summary>
        /// 学生抽样，仅根据设定保留部分学生
        /// </summary>
        /// <param name="cLCase"></param>
        /// <param name="rule"></param>
        /// <param name="algoRule"></param>
        /// <param name="studentExtractionModel"></param>
        /// <returns></returns>
        public Tuple<CLCase, bool, List<DataValidationResultInfo>> GetModelByStudentExtraction(CLCase cLCase, Rule rule, AlgoRule algoRule, StudentExtractionModel studentExtractionModel)
        {
            List<DataValidationResultInfo> mydvri = new List<DataValidationResultInfo>() { };
            bool checkResult = ModelValidation.ValidateModel(cLCase, algoRule, rule, out mydvri);
            if (!checkResult)
            {
                return Tuple.Create(cLCase, false, mydvri);
            }

            if (cLCase != null && studentExtractionModel != null)
            {
                //update Positions
                if (studentExtractionModel.Positions != null)
                {
                    cLCase.Positions = studentExtractionModel.Positions;
                }

                //remove combination
                if (studentExtractionModel.RemovedCombination != null)
                {
                    cLCase = Utility.GetCaseByRemovedCombination(cLCase, studentExtractionModel.RemovedCombination);
                }

                if (studentExtractionModel.ExtractionRatio >= 1 && studentExtractionModel.ExtractionRatio <= 100)
                {
                    cLCase = Utility.GetCaseByExtractionRatio(cLCase, studentExtractionModel.ExtractionRatio);
                }

                //Increase Capacity
                if (studentExtractionModel.IncreasedCapacity > 0)
                {
                    cLCase.Classes?.ForEach(x => {
                        x.Capacity = x.Capacity + studentExtractionModel.IncreasedCapacity;
                    });
                }
            }

            return Tuple.Create(cLCase, true, mydvri);
        }

        /// <summary>
        /// 抽样排课后的优化步骤
        /// </summary>
        /// <param name="cLCase"></param>
        /// <param name="algoRule"></param>
        /// <param name="normalModel"></param>
        /// <param name="resultModel"></param>
        /// <param name="IncreasedCapacity"></param>
        /// <returns></returns>
        public Tuple<CLCase, AlgoRule, bool, List<DataValidationResultInfo>> GetModelByFixedClassTimeTable(CLCase cLCase, Rule rule, AlgoRule algoRule, NormalModel normalModel, ResultModel resultModel, int IncreasedCapacity)
        {
            List<DataValidationResultInfo> mydvri = new List<DataValidationResultInfo>() { };
            bool checkResult = ModelValidation.ValidateModel(cLCase, algoRule, rule, out mydvri);
            if (!checkResult)
            {
                return Tuple.Create(cLCase, algoRule, false, mydvri);
            }

            if (cLCase != null && resultModel != null)
            {
                //remove selected combinations
                if (normalModel?.RemovedCombination != null)
                {
                    cLCase = Utility.GetCaseByRemovedCombination(cLCase, normalModel.RemovedCombination);
                }

                //Update Capacity
                if (normalModel?.ClassCapacity != null)
                {
                    normalModel.ClassCapacity.ForEach(x => {
                        cLCase?.Classes?.Where(c => c.ID == x.ClassId)?.ToList()?.ForEach(c => {
                            c.Capacity = c.Capacity + x.Capacity;
                        });
                    });
                }

                if (IncreasedCapacity > 0)
                {
                    cLCase?.Classes?.ForEach(x => {
                        x.Capacity = x.Capacity + IncreasedCapacity;
                    });
                }

                //Update Positions
                if (normalModel?.Positions != null)
                {
                    cLCase.Positions = normalModel.Positions;
                }

                //Analysis ResultModel and add rule to algoRule
                if (resultModel.ResultClasses != null)
                {
                    if (algoRule == null)
                    {
                        algoRule = new AlgoRule();
                    }

                    if (algoRule.ClassHourRequiredStartingTime == null)
                    {
                        algoRule.ClassHourRequiredStartingTime = new List<ClassHourRequiredStartingTimeRule>() { };
                    }

                    resultModel.ResultClasses?.ToList()?.ForEach(x => {
                        x.ResultDetails?.ToList()?.ForEach(rd => {

                            algoRule.ClassHourRequiredStartingTime.Add(new ClassHourRequiredStartingTimeRule()
                            {
                                ID = rd.ClassHourId,
                                Period = rd.DayPeriod,
                                Weight = 100
                            });
                        });
                    });
                }
            }

            return Tuple.Create(cLCase, algoRule, true, mydvri);
        }

        /// <summary>
        /// 课位压缩，根据设定参数压缩课位
        /// </summary>
        /// <param name="cLCase"></param>
        /// <param name="rule"></param>
        /// <param name="algoRule"></param>
        /// <param name="timeCompressionModel"></param>
        /// <returns></returns>
        public Tuple<CLCase, Rule, AlgoRule, bool, List<DataValidationResultInfo>> GetModelByTimeCompression(CLCase cLCase, Rule rule, AlgoRule algoRule, TimeCompressionModel timeCompressionModel)
        {
            List<DataValidationResultInfo> mydvri = new List<DataValidationResultInfo>() { };
            bool checkResult = ModelValidation.ValidateModel(cLCase, algoRule, rule, out mydvri);
            if (!checkResult)
            {
                return Tuple.Create(cLCase, rule, algoRule, false, mydvri);
            }

            if (cLCase != null && timeCompressionModel != null)
            {
                if (timeCompressionModel.ClassCapacity != null)
                {
                    timeCompressionModel.ClassCapacity.ForEach(x => {
                        cLCase?.Classes?.Where(c => c.ID == x.ClassId)?.ToList()?.ForEach(c => {
                            c.Capacity = x.Capacity;
                        });
                    });
                }

                if (timeCompressionModel.RemovedCombination != null)
                {
                    cLCase = Utility.GetCaseByRemovedCombination(cLCase, timeCompressionModel.RemovedCombination);
                }

                if (timeCompressionModel.CompressionRatio > 1)
                {
                    //调整课时
                    cLCase = Utility.GetCaseClassHourUpdateByTimeCompressionRatio(cLCase, timeCompressionModel.CompressionRatio);

                    //调整课位
                    Tuple<CLCase, AlgoRule> cLCaseAndAlgoRule = Utility.GetCasePositionsUpdateByTimeCompressionRatio(cLCase, algoRule, timeCompressionModel.CompressionRatio);
                    cLCase = cLCaseAndAlgoRule.Item1;
                    algoRule = cLCaseAndAlgoRule.Item2;

                    //Add Tags
                    if (cLCase.Tags == null)
                    {
                        cLCase.Tags = new List<TagModel>();
                    }

                    if (!cLCase.Tags.Exists(t => t.ID == SystemTag.XYTagN.ToString()))
                    {
                        cLCase.Tags.Add(new TagModel() { ID = SystemTag.XYTagN.ToString(), Name = SystemTag.XYTagN.ToString() });
                    }

                    if (!cLCase.Tags.Exists(t => t.ID == SystemTag.XYTag1.ToString()))
                    {
                        cLCase.Tags.Add(new TagModel() { ID = SystemTag.XYTag1.ToString(), Name = SystemTag.XYTag1.ToString() });
                    }
                }
            }

            return Tuple.Create(cLCase, new Rule() { }, algoRule, true, mydvri);
        }

        /// <summary>
        /// 课位压缩排课后的优化步骤
        /// </summary>
        /// <param name="cLCase"></param>
        /// <param name="rule"></param>
        /// <param name="algoRule"></param>
        /// <param name="resultModel"></param>
        /// <returns></returns>
        public Tuple<CLCase, Rule, AlgoRule, bool, List<DataValidationResultInfo>> GetModelByStudentsClassificationResult(CLCase cLCase, Rule rule, AlgoRule algoRule, ResultModel resultModel)
        {
            List<DataValidationResultInfo> mydvri = new List<DataValidationResultInfo>() { };
            bool checkResult = ModelValidation.ValidateModel(cLCase, algoRule, rule, out mydvri);
            if (!checkResult)
            {
                return Tuple.Create(cLCase, rule, algoRule, false, mydvri);
            }

            if (cLCase != null && resultModel != null)
            {
                resultModel.ResultClasses?.ToList()?.ForEach(x => {
                    var targetClass = cLCase.Classes?.FirstOrDefault(c => c.ID == x.ClassID);
                    if (targetClass != null)
                    {
                        if (targetClass.StudentIDs == null)
                        {
                            targetClass.StudentIDs = (x.ResultStudents?.ToList() ?? new List<string>());
                        }
                        else
                        {
                            targetClass.StudentIDs.AddRange(x.ResultStudents?.ToList() ?? new List<string>());
                        }
                    }
                });
            }

            //Check And add some rules if it's a auto schedule
            if (cLCase.IsAuto)
            {
                //
            }

            return Tuple.Create(cLCase, rule, algoRule, true, mydvri);
        }

        public int GetCompressionRatio(CLCase cLCase)
        {
            int compressionRatio = 2;

            List<int> lessons = cLCase?.Courses?.SelectMany(c => c.Levels)?.Select(le => le.Lessons)?.ToList() ?? new List<int>();
            if (lessons.Max() == lessons.Min())
            {
                compressionRatio = lessons.Min();
            }
            else if (lessons.Max() > 1)
            {
                compressionRatio = 2;
            }

            return Math.Max(1, compressionRatio);
        }
    }

    /// <summary>
    /// Help Class
    /// </summary>
    class Utility
    {
        /// <summary>
        /// Get selection road by SelectionModel
        /// </summary>
        /// <param name="SelectionCombination"></param>
        /// <returns></returns>
        public static string GetStringFormatBySelectionCombination(List<SelectionModel> SelectionCombination)
        {
            string result = string.Empty;

            if (SelectionCombination != null)
            {
                List<string> tempResult = new List<string>();

                SelectionCombination.ForEach(x => {
                    tempResult.Add($"{x.CourseId}_{x.LevelId}");
                });

                tempResult = tempResult.OrderBy(x => x).ToList();
                result = string.Join(",", tempResult);
            }

            return result;
        }

        /// <summary>
        /// Get selection roads by SelectionModels
        /// </summary>
        /// <param name="SelectionCombinations"></param>
        /// <returns></returns>
        public static List<string> GetStringFormatBySelectionCombination(List<SelectionCombinationModel> SelectionCombinations)
        {
            List<string> result = new List<string>();
            if (SelectionCombinations != null)
            {
                SelectionCombinations.ForEach(s => {
                    result.Add(GetStringFormatBySelectionCombination(s.SelectionCombination));
                });
            }

            return result;
        }

        /// <summary>
        /// Get selection road by student preselection
        /// </summary>
        /// <param name="Preselection"></param>
        /// <returns></returns>
        public static string GetStringFormatByStudentPreselection(List<PreselectionModel> Preselection)
        {
            string result = string.Empty;

            if (Preselection != null)
            {
                List<string> tempResult = new List<string>();

                Preselection.ForEach(x => {
                    tempResult.Add($"{x.CourseID}_{x.LevelID}");
                });

                tempResult = tempResult.OrderBy(x => x).ToList();
                result = string.Join(",", tempResult);
            }

            return result;
        }

        /// <summary>
        /// Remove Students who's selection in RemovedCombination
        /// </summary>
        /// <param name="cLCase"></param>
        /// <param name="RemovedCombination"></param>
        /// <returns></returns>
        public static CLCase GetCaseByRemovedCombination(CLCase cLCase, List<SelectionCombinationModel> RemovedCombination)
        {
            List<string> selectionRoads = GetStringFormatBySelectionCombination(RemovedCombination);
            List<string> studentsToRemoved = new List<string>();

            cLCase.Students.ForEach(x => {

                string preselectionRoad = GetStringFormatByStudentPreselection(x.Preselections);

                if (selectionRoads.Contains(preselectionRoad))
                {
                    studentsToRemoved.Add(x.ID);
                }
            });

            studentsToRemoved.ForEach(x =>
            {
                var studentToRemove = cLCase.Students.SingleOrDefault(s => s.ID == x);

                if (studentToRemove != null)
                {
                    cLCase.Students.Remove(studentToRemove);
                }
            });

            return cLCase;
        }

        /// <summary>
        /// 对模型中的学生进行抽样
        /// </summary>
        /// <param name="cLCase"></param>
        /// <param name="extractionRatio"></param>
        /// <returns></returns>
        public static CLCase GetCaseByExtractionRatio(CLCase cLCase, int extractionRatio)
        {
            List<string> leftStudentId = new List<string>();
            List<string> allStudentId = new List<string>();
            List<string> removedStudentId = new List<string>();

            List<StudentRoad> studentsRoad = new List<StudentRoad>();

            cLCase.Students.ForEach(x => {
                string preselectionRoad = GetStringFormatByStudentPreselection(x.Preselections);
                studentsRoad.Add(new StudentRoad { StudentId = x.ID, SelectionRoad = preselectionRoad });
            });

            List<string> allRoads = studentsRoad.GroupBy(x => x.SelectionRoad)?.Select(x => x.Key)?.ToList() ?? new List<string>();

            //每种选择都按照相同的百分比取学生
            allRoads.ForEach(x => {
                int roadStudents = studentsRoad.Where(s => s.SelectionRoad == x)?.Count() ?? 0;
                int takeStudentNumber = (int)Math.Ceiling((roadStudents * extractionRatio) / 100.0);
                leftStudentId.AddRange(studentsRoad.Where(sr => sr.SelectionRoad == x).Take(takeStudentNumber).Select(sr => sr.StudentId).ToList());
            });

            leftStudentId = leftStudentId.Distinct().ToList();
            allStudentId = studentsRoad.Select(x => x.StudentId).Distinct().ToList();
            removedStudentId = allStudentId.Except(leftStudentId).ToList();

            //删除保留之外的学生
            removedStudentId.ForEach(x => {
                var studentToRemove = cLCase.Students.SingleOrDefault(s => s.ID == x);

                if (studentToRemove != null)
                {
                    cLCase.Students.Remove(studentToRemove);
                }
            });

            //更新班额
            cLCase.Courses.ForEach(co => {
                co.Levels.ForEach(le => {
                    int leftStudents = cLCase.Students.Where(s => s.Preselections.Exists(p => p.CourseID == co.ID && p.LevelID == le.ID)).Count();
                    int classNumber = cLCase.Classes.Where(c => c.CourseID == co.ID && c.LevelID == le.ID).Count();
                    int avgCapacity = classNumber > 0 ? (int)Math.Ceiling(leftStudents / (decimal)classNumber) : 0;

                    cLCase.Classes.Where(c => c.CourseID == co.ID && c.LevelID == le.ID)?.ToList()?.ForEach(c => {
                        c.Capacity = avgCapacity;
                    });
                });
            });

            return cLCase;
        }

        /// <summary>
        /// 对模型中的课时进行压缩
        /// </summary>
        /// <param name="cLCase"></param>
        /// <param name="CompressionRatio"></param>
        /// <returns></returns>
        public static CLCase GetCaseClassHourUpdateByTimeCompressionRatio(CLCase cLCase, int CompressionRatio)
        {
            #region 更新层上的课时及班级中的课时
            cLCase.Courses.ForEach(x => {
                x.Levels.ForEach(le => {
                    int quotient = le.Lessons / CompressionRatio;
                    int remainder = le.Lessons % CompressionRatio;
                    le.Lessons = quotient + remainder;

                    cLCase.Classes.Where(c => c.CourseID == x.ID && c.LevelID == le.ID).ToList().ForEach(cl => {
                        /* add tags and inactive all useless classhour */
                        int iIndex = 0;
                        cLCase.ClassHours?.Where(c => c.CourseID == x.ID && c.LevelID == le.ID && c.ClassID == cl.ID)?.ToList()?.ForEach(c => {

                            if (c.TagIDs == null)
                            {
                                c.TagIDs = new List<string>() { };
                            }

                            switch (iIndex)
                            {
                                case int n when (n < quotient):
                                    c.TagIDs.Add(SystemTag.XYTagN.ToString());
                                    break;
                                case int n when (n >= quotient && n < quotient + remainder):
                                    c.TagIDs.Add(SystemTag.XYTag1.ToString());
                                    break;
                                case int n when (n >= quotient + remainder):
                                    break;
                                default:
                                    break;
                            }

                            iIndex++;
                        });
                    });
                });
            });
            #endregion

            //重新删除所有无效课时
            cLCase.ClassHours.RemoveAll(x => x.TagIDs == null || x.TagIDs.Count == 0);

            return cLCase;
        }

        public static Tuple<CLCase, AlgoRule> GetCasePositionsUpdateByTimeCompressionRatio(CLCase cLCase, AlgoRule algoRule, int CompressionRatio)
        {
            List<TeacherTagClassHour> teacherTagClassHour = new List<TeacherTagClassHour>();
            List<StudentTagClassHour> studentTagClassHour = new List<StudentTagClassHour>();
            List<StudentTagClassHour> studentTagClassHourBK = new List<StudentTagClassHour>();

            //统计教师的课时(压缩与非压缩)
            cLCase.Teachers.ForEach(x => {
                cLCase.ClassHours?.Where(c => c.TeacherIDs != null && c.TagIDs != null && c.TeacherIDs.Contains(x.ID))
                                         ?.Select(c => new { TeacherId = x.ID, Tags = c.TagIDs, c.ClassID })
                                         ?.ToList()?.ForEach(c => {
                                             if (c.Tags.Contains(SystemTag.XYTagN.ToString()))
                                             {
                                                 teacherTagClassHour.Add(new TeacherTagClassHour() { ClassID = c.ClassID, TagType = SystemTag.XYTagN.ToString(), TeacherId = x.ID });
                                             }
                                             else if (c.Tags.Contains(SystemTag.XYTag1.ToString()))
                                             {
                                                 teacherTagClassHour.Add(new TeacherTagClassHour() { ClassID = c.ClassID, TagType = SystemTag.XYTag1.ToString(), TeacherId = x.ID });
                                             }
                                         });
            });

            //统计学生的课时(压缩与非压缩)
            cLCase.Students?.ForEach(x => {
                x.Preselections.ForEach(s => {
                    studentTagClassHourBK.Add(new StudentTagClassHour() { StudentId = x.ID, CourseID = s.CourseID, LevelID = s.LevelID, TagType = "" });
                });
            });

            cLCase.Courses.ForEach(co => {
                co.Levels.ForEach(le => {

                    ClassModel classInfo = cLCase.Classes.FirstOrDefault(cl => cl.CourseID == co.ID && cl.LevelID == le.ID);

                    if (classInfo != null)
                    {
                        string ClassID = cLCase.Classes.First(cl => cl.CourseID == co.ID && cl.LevelID == le.ID).ID;
                        var classHours = cLCase.ClassHours.Where(c => c.ClassID == ClassID);
                        int tagNCount = classHours.Where(x => x.TagIDs != null && x.TagIDs.Contains(SystemTag.XYTagN.ToString())).Count();
                        int tag1Count = classHours.Where(x => x.TagIDs != null && x.TagIDs.Contains(SystemTag.XYTag1.ToString())).Count();

                        for (int i = 0; i < tagNCount; i++)
                        {
                            studentTagClassHourBK.Where(x => x.CourseID == co.ID && x.LevelID == le.ID).ToList().ForEach(x => {
                                studentTagClassHour.Add(new StudentTagClassHour() { StudentId = x.StudentId, CourseID = x.CourseID, LevelID = x.LevelID, TagType = SystemTag.XYTagN.ToString() });
                            });
                        }

                        for (int i = 0; i < tag1Count; i++)
                        {
                            studentTagClassHourBK.Where(x => x.CourseID == co.ID && x.LevelID == le.ID).ToList().ForEach(x => {
                                studentTagClassHour.Add(new StudentTagClassHour() { StudentId = x.StudentId, CourseID = x.CourseID, LevelID = x.LevelID, TagType = SystemTag.XYTag1.ToString() });
                            });
                        }
                    }
                });
            });

            //如果班级都没有安排教师，且没有学生怎么办: 按照教学班统计最大课时
            var noTS = cLCase.ClassHours.Where(x => (x.TeacherIDs == null || x.TeacherIDs.Count == 0));
            int noTSMaxN = 0;

            if (noTS.Where(x => x.TagIDs != null && x.TagIDs.Contains(SystemTag.XYTagN.ToString())).Any())
            {
                noTSMaxN = noTS.Where(x => x.TagIDs != null && x.TagIDs.Contains(SystemTag.XYTagN.ToString()))
                .Select(x => new { x.ClassID })
                .GroupBy(x => new { x.ClassID })
                .Select(x => new { x.Key.ClassID, Count = x.Count() }).Max(x => x.Count);
            }

            int noTSMax1 = 0;
            if (noTS.Where(x => x.TagIDs != null && x.TagIDs.Contains(SystemTag.XYTag1.ToString())).Any())
            {
                noTSMax1 = noTS.Where(x => x.TagIDs != null && x.TagIDs.Contains(SystemTag.XYTag1.ToString()))
                .Select(x => new { x.ClassID })
                .GroupBy(x => new { x.ClassID })
                .Select(x => new { x.Key.ClassID, Count = x.Count() }).Max(x => x.Count);
            }

            //计算需要最大的压缩课位和非压缩课位
            int teacherMaxN = 0;
            if (teacherTagClassHour.Where(x => x.TagType == SystemTag.XYTagN.ToString()).Any())
            {
                teacherMaxN = teacherTagClassHour.Where(x => x.TagType == SystemTag.XYTagN.ToString())
                .GroupBy(x => new { x.TeacherId, x.TagType })
                .Select(x => new { x.Key.TeacherId, x.Key.TagType, Count = x.Count() }).Max(x => x.Count);
            }

            int teacherMax1 = 0;
            if (teacherTagClassHour.Where(x => x.TagType == SystemTag.XYTag1.ToString()).Any())
            {
                teacherMax1 = teacherTagClassHour.Where(x => x.TagType == SystemTag.XYTag1.ToString())
                .GroupBy(x => new { x.TeacherId, x.TagType })
                .Select(x => new { x.Key.TeacherId, x.Key.TagType, Count = x.Count() }).Max(x => x.Count);
            }

            //计算需要最大的压缩课位和非压缩课位
            int studentMaxN = 0;
            if (studentTagClassHour.Where(x => x.TagType == SystemTag.XYTagN.ToString()).Any())
            {
                studentMaxN = studentTagClassHour.Where(x => x.TagType == SystemTag.XYTagN.ToString())
                .GroupBy(x => new { x.StudentId, x.TagType })
                .Select(x => new { x.Key.StudentId, x.Key.TagType, Count = x.Count() }).Max(x => x.Count);
            }

            int studentMax1 = 0;
            if (studentTagClassHour.Where(x => x.TagType == SystemTag.XYTag1.ToString()).Any())
            {
                studentMax1 = studentTagClassHour.Where(x => x.TagType == SystemTag.XYTag1.ToString())
                .GroupBy(x => new { x.StudentId, x.TagType })
                .Select(x => new { x.Key.StudentId, x.Key.TagType, Count = x.Count() }).Max(x => x.Count);
            }

            int TSmaxN = Math.Max(teacherMaxN, studentMaxN);
            int TSmax1 = Math.Max(teacherMax1, studentMax1);

            int maxN = Math.Max(noTSMaxN, TSmaxN);
            int max1 = Math.Max(noTSMax1, TSmax1);

            //初始化课位
            cLCase.Positions.ForEach(x => { x.IsSelected = false; });

            //设定新的排课课位
            //TagN TimeSlot
            ClassHoursRequiredTimesRule tagNRules = new ClassHoursRequiredTimesRule();
            tagNRules.Times = new List<XYKernel.OS.Common.Models.DayPeriodModel>();
            tagNRules.Active = true;
            tagNRules.Weight = 100;
            tagNRules.TagID = SystemTag.XYTagN.ToString();

            for (int i = 0; i < maxN; i++)
            {
                var coursePosition = cLCase.Positions.Where(x => x.IsSelected == false && x.Position != Position.AB && x.Position != Position.Noon && x.Position != Position.PB && x.DayPeriod.Day > 0)
                                     .OrderBy(x => x.DayPeriod.Day).ThenBy(x => x.DayPeriod.Period).First();
                coursePosition.IsSelected = true;
                tagNRules.Times.Add(coursePosition.DayPeriod);
            }

            //Tag1 TimeSlot
            ClassHoursRequiredTimesRule tag1Rules = new ClassHoursRequiredTimesRule();
            tag1Rules.Times = new List<XYKernel.OS.Common.Models.DayPeriodModel>();
            tag1Rules.Active = true;
            tag1Rules.Weight = 100;
            tag1Rules.TagID = SystemTag.XYTag1.ToString();

            for (int i = 0; i < max1; i++)
            {
                var coursePosition = cLCase.Positions.Where(x => x.IsSelected == false && x.Position != Position.AB && x.Position != Position.Noon && x.Position != Position.PB && x.DayPeriod.Day == 0)
                                    .OrderBy(x => x.DayPeriod.Day).ThenBy(x => x.DayPeriod.Period).FirstOrDefault();
                if (coursePosition == null)
                {
                    coursePosition = cLCase.Positions.Where(x => x.IsSelected == false && x.Position != Position.AB && x.Position != Position.Noon && x.Position != Position.PB && x.DayPeriod.Day > 0)
                                    .OrderByDescending(x => x.DayPeriod.Day).ThenBy(x => x.DayPeriod.Period).FirstOrDefault();
                }

                if (coursePosition != null)
                {
                    coursePosition.IsSelected = true;
                    tag1Rules.Times.Add(coursePosition.DayPeriod);
                }
            }

            //将两种时间分别记录到规则中，形成约束
            algoRule = new AlgoRule();

            if (algoRule.ClassHoursRequiredTimes == null)
            {
                algoRule.ClassHoursRequiredTimes = new List<ClassHoursRequiredTimesRule>();
            }

            if (tag1Rules.Times.Count > 0)
            {
                algoRule.ClassHoursRequiredTimes.Add(tag1Rules);
            }
            if (tagNRules.Times.Count > 0)
            {
                algoRule.ClassHoursRequiredTimes.Add(tagNRules);
            }

            return Tuple.Create(cLCase, algoRule);
        }
    }

    class StudentRoad
    {
        public string StudentId { get; set; }
        public string SelectionRoad { get; set; }
    }

    class TeacherTagClassHour
    {
        public string TeacherId { get; set; }
        public string TagType { get; set; }
        public string ClassID { get; set; }
    }

    class StudentTagClassHour
    {
        public string StudentId { get; set; }
        public string TagType { get; set; }
        public string CourseID { get; set; }
        public string LevelID { get; set; }
    }
}
