using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Models;
using System.IO;
using OSKernel.Presentation.Utilities;
using XYKernel.OS.Common.Models.Mixed;
using OSKernel.Presentation.Core.DataManager;
using Unity;

namespace OSKernel.Presentation.Core
{
    /// <summary>
    /// 方案扩展类
    /// </summary>
    public static class CaseExtend
    {
        /// <summary>
        /// 保存方案
        /// </summary>
        /// <param name="model"></param>
        public static void Serialize(this Case model)
        {
            string path = CacheManager.Instance.GetDataPath();
            string localPath = $"{path}\\{model.LocalID}\\{model.LocalID}.local";

            model.LocalPath = $"{CommonPath.Data}{model.LocalID}\\{model.LocalID}.local";

            localPath.SerializeObjectToJson(model);
        }

        public static Case DeSerialize(this string localID)
        {
            string path = CacheManager.Instance.GetDataPath();
            string localPath = $"{path}\\{localID}\\{localID}.local";

            if (System.IO.File.Exists(localPath))
            {
                return localPath.DeSerializeObjectFromJson<Case>();
            }
            else
                return null;

        }

        public static void Serialize(this XYKernel.OS.Common.Models.Administrative.CPCase model, string localID)
        {
            string path = CacheManager.Instance.GetDataPath();
            string casePath = $"{path}\\{localID}\\{localID}.case";

            casePath.SerializeObjectToJson(model);
        }

        public static XYKernel.OS.Common.Models.Administrative.CPCase DeSerializeCP(this string localID)
        {
            string path = CacheManager.Instance.GetDataPath();
            string casePath = $"{path}\\{localID}\\{localID}.case";

            if (System.IO.File.Exists(casePath))
            {
                return casePath.DeSerializeObjectFromJson<XYKernel.OS.Common.Models.Administrative.CPCase>();
            }
            else
                return null;

        }

        public static void Serialize(this XYKernel.OS.Common.Models.Administrative.Rule.Rule model, string localID)
        {
            string path = CacheManager.Instance.GetDataPath();
            string rulePath = $"{path}\\{localID}\\{localID}.rule";

            rulePath.SerializeObjectToJson(model);
        }

        public static XYKernel.OS.Common.Models.Administrative.Rule.Rule DeSerializeRule(this string localID)
        {
            string path = CacheManager.Instance.GetDataPath();
            string rulePath = $"{path}\\{localID}\\{localID}.rule";

            if (System.IO.File.Exists(rulePath))
            {
                return rulePath.DeSerializeObjectFromJson<XYKernel.OS.Common.Models.Administrative.Rule.Rule>();
            }
            else
                return null;
        }

        public static void Serialize(this XYKernel.OS.Common.Models.Administrative.AlgoRule.AlgoRule model, string localID)
        {
            string path = CacheManager.Instance.GetDataPath();
            string algoPath = $"{path}\\{localID}\\{localID}.algo";

            algoPath.SerializeObjectToJson(model);
        }

        public static XYKernel.OS.Common.Models.Administrative.AlgoRule.AlgoRule DeSerializeAlgo(this string localID)
        {
            string path = CacheManager.Instance.GetDataPath();
            string algoPath = $"{path}\\{localID}\\{localID}.algo";

            if (System.IO.File.Exists(algoPath))
            {
                return algoPath.DeSerializeObjectFromJson<XYKernel.OS.Common.Models.Administrative.AlgoRule.AlgoRule>();
            }
            else
                return null;
        }

        public static void Serialize(this XYKernel.OS.Common.Models.Mixed.Rule.Rule model, string localID)
        {
            string path = CacheManager.Instance.GetDataPath();
            string rulePath = $"{path}\\{localID}\\{localID}.rule";

            rulePath.SerializeObjectToJson(model);
        }

        public static XYKernel.OS.Common.Models.Mixed.Rule.Rule DeSerializeMixedRule(this string localID)
        {
            string path = CacheManager.Instance.GetDataPath();
            string rulePath = $"{path}\\{localID}\\{localID}.rule";

            if (System.IO.File.Exists(rulePath))
            {
                return rulePath.DeSerializeObjectFromJson<XYKernel.OS.Common.Models.Mixed.Rule.Rule>();
            }
            else
                return null;
        }

        public static void Serialize(this XYKernel.OS.Common.Models.Mixed.AlgoRule.AlgoRule model, string localID)
        {
            string path = CacheManager.Instance.GetDataPath();
            string algoPath = $"{path}\\{localID}\\{localID}.algo";

            algoPath.SerializeObjectToJson(model);
        }

        public static XYKernel.OS.Common.Models.Mixed.AlgoRule.AlgoRule DeSerializeMixedAlgo(this string localID)
        {
            string path = CacheManager.Instance.GetDataPath();
            string algoPath = $"{path}\\{localID}\\{localID}.algo";

            if (System.IO.File.Exists(algoPath))
            {
                return algoPath.DeSerializeObjectFromJson<XYKernel.OS.Common.Models.Mixed.AlgoRule.AlgoRule>();
            }
            else
                return null;
        }

        public static void Serialize(this XYKernel.OS.Common.Models.Mixed.CLCase model, string localID)
        {
            string path = CacheManager.Instance.GetDataPath();
            string casePath = $"{path}\\{localID}\\{localID}.case";

            casePath.SerializeObjectToJson(model);
        }

        public static XYKernel.OS.Common.Models.Mixed.CLCase DeSerializeCL(this string localID)
        {
            string path = CacheManager.Instance.GetDataPath();
            string casePath = $"{path}\\{localID}\\{localID}.case";

            if (System.IO.File.Exists(casePath))
            {
                return casePath.DeSerializeObjectFromJson<XYKernel.OS.Common.Models.Mixed.CLCase>();
            }
            else
                return null;
        }

        /// <summary>
        /// 序列化结果
        /// </summary>
        /// <param name="model">走班</param>
        /// <param name="results">结果</param>
        public static void Serizlize(this Case model, List<Models.Result.UIResult> results)
        {
            string path = CacheManager.Instance.GetDataPath();
            string resultPath = $"{path}\\{model.LocalID}\\{model.LocalID}.result";

            resultPath.SerializeObjectToJson(results);
        }

        public static List<Models.Result.UIResult> DeSerializeResult(this string localID)
        {
            string path = CacheManager.Instance.GetDataPath();
            string resultPath = $"{path}\\{localID}\\{localID}.result";

            if (System.IO.File.Exists(resultPath))
            {
                return resultPath.DeSerializeObjectFromJson<List<Models.Result.UIResult>>();
            }
            else
                return null;
        }

        /// <summary>
        /// 删除方案所在文件
        /// </summary>
        public static void DeleteFile(this Case model)
        {
            string path = CacheManager.Instance.GetDataPath();
            string deletePath = $"{path}\\{model.LocalID}";

            var has = System.IO.Directory.Exists(deletePath);
            if (has)
            {
                System.IO.Directory.Delete(deletePath, true);
            }
        }

        public static void Serialize(this XYKernel.OS.Common.Models.Administrative.Result.ResultModel result, string localID)
        {
            string path = CacheManager.Instance.GetDataPath();
            string resultPath = $"{path}\\{localID}\\{localID}.result";

            resultPath.SerializeObjectToJson(result);
        }

        public static void Serialize(this XYKernel.OS.Common.Models.Mixed.Result.ResultModel result, string localID)
        {
            string path = CacheManager.Instance.GetDataPath();
            string resultPath = $"{path}\\{localID}\\{localID}.result";

            resultPath.SerializeObjectToJson(result);
        }

        public static XYKernel.OS.Common.Models.Mixed.Result.ResultModel DeSerializeLocalResultCL(this string localID)
        {
            string path = CacheManager.Instance.GetDataPath();
            string resultPath = $"{path}\\{localID}\\{localID}.result";

            if (System.IO.File.Exists(resultPath))
            {
                return resultPath.DeSerializeObjectFromJson<XYKernel.OS.Common.Models.Mixed.Result.ResultModel>();
            }
            else
                return null;
        }


        #region Administrative

        public static T DeSerializeLocalResult<T>(this string localID, long taskID) where T : class
        {
            string path = CacheManager.Instance.GetDataPath();
            string resultPath = $"{path}\\{localID}\\Tasks\\{taskID}\\{taskID}.localResult";

            if (System.IO.File.Exists(resultPath))
            {
                return resultPath.DeSerializeObjectFromJson<T>();
            }
            else
                return null;
        }

        public static List<T> DeSerializeCourseFrame<T>(this string localID, long taskID,string className="") where T : class
        {
            string path = CacheManager.Instance.GetDataPath();
            string resultPath = $"{path}\\{localID}\\Tasks\\{taskID}\\{taskID}{className}.adjustCourseFrame";

            if (System.IO.File.Exists(resultPath))
            {
                return resultPath.DeSerializeObjectFromJson<List<T>>();
            }
            else
                return null;
        }

        public static void SerializeCourseFrame<T>(this List<T> courseFrames, string localID, long taskID,string className="")
        {
            string path = CacheManager.Instance.GetDataPath();
            string resultPath = $"{path}\\{localID}\\Tasks\\{taskID}\\{taskID}{className}.adjustCourseFrame";

            resultPath.SerializeObjectToJson(courseFrames);
        }

        public static void SerializeLocalResult<T>(this T result, string localID, long taskID)
        {
            string path = CacheManager.Instance.GetDataPath();
            string resultPath = $"{path}\\{localID}\\Tasks\\{taskID}\\{taskID}.localResult";

            resultPath.SerializeObjectToJson(result);
        }

        public static void Serialize<T>(this T result, string localID, long taskID)
        {
            string path = CacheManager.Instance.GetDataPath();
            string resultPath = $"{path}\\{localID}\\Tasks\\{taskID}\\{taskID}.adjust";

            resultPath.SerializeObjectToJson(result);
        }

        public static T DeSerializeAdjustRecord<T>(this string localID, long taskID) where T : class
        {
            string path = CacheManager.Instance.GetDataPath();
            string resultPath = $"{path}\\{localID}\\Tasks\\{taskID}\\{taskID}.adjust";

            if (System.IO.File.Exists(resultPath))
            {
                return resultPath.DeSerializeObjectFromJson<T>();
            }
            else
                return null;
        }

        #endregion
    }
}
