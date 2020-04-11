using OSKernel.Presentation.Core;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models.Mixed;
using XYKernel.OS.Common.Models.Mixed.AlgoRule;
using XYKernel.OS.Common.Models.Mixed.Rule;

namespace OSKernel.Presentation.Core
{
    /// <summary>
    /// 模式扩展类
    /// </summary>
    public static class PatternExtend
    {
        #region GetPath

        public static string GetPatternPath(string localID)
        {
            var path = Core.CacheManager.Instance.GetDataPath();
            var patternPath = $"{path}\\{localID}\\Pattern\\{localID}.pattern";

            return patternPath;
        }

        public static string GetRulePath(string localID)
        {
            var path = Core.CacheManager.Instance.GetDataPath();
            var patternPath = $"{path}\\{localID}\\Pattern\\{localID}.rule";

            return patternPath;
        }

        public static string GetAlgoPath(string localID)
        {
            var path = Core.CacheManager.Instance.GetDataPath();
            var patternPath = $"{path}\\{localID}\\Pattern\\{localID}.algo";

            return patternPath;
        }

        public static string GetCasePath(string localID)
        {
            var path = Core.CacheManager.Instance.GetDataPath();
            var patternPath = $"{path}\\{localID}\\Pattern\\{localID}.case";

            return patternPath;
        }

        #endregion

        public static void Serialize(this string localID, object model)
        {
            GetPatternPath(localID).SerializeObjectToJson(model);
        }

        public static T DeSerialize<T>(this string localID) where T : class
        {
            var path = GetPatternPath(localID);
            if (File.Exists(path))
            {
                return path.DeSerializeObjectFromJson<T>();
            }
            else
                return null;
        }

        public static void SerializePatternCase(this CLCase model, string localID)
        {
            string path = GetCasePath(localID);
            path.SerializeObjectToJson(model);
        }

        public static CLCase DeSerializePatternCase(this string localID)
        {
            string path = GetCasePath(localID);
            return path.DeSerializeObjectFromJson<CLCase>();
        }

        public static void SerializePatternAlgo(this AlgoRule model, string localID)
        {
            string path = GetAlgoPath(localID);
            path.SerializeObjectToJson(model);
        }

        public static AlgoRule DeSerializePatternAlgo(this string localID)
        {
            string path = GetAlgoPath(localID);
            return path.DeSerializeObjectFromJson<AlgoRule>();
        }

        public static void SerializePatternRule(this Rule model, string localID)
        {
            string path = GetRulePath(localID);
            path.SerializeObjectToJson(model);
        }

        public static Rule DeSerializePatternRule(this string localID)
        {
            string path = GetRulePath(localID);
            return path.DeSerializeObjectFromJson<Rule>();
        }

        public static void DeleteAllPatternData(this string localID)
        {
            var casePath = GetCasePath(localID);
            var rulePath = GetRulePath(localID);
            var algoPath = GetAlgoPath(localID);
            var patternPath = GetPatternPath(localID);

            if (File.Exists(casePath))
            {
                System.IO.File.Delete(casePath);
            }

            if (File.Exists(rulePath))
            {
                System.IO.File.Delete(rulePath);
            }

            if (File.Exists(algoPath))
            {
                System.IO.File.Delete(algoPath);
            }

            if (File.Exists(patternPath))
            {
                System.IO.File.Delete(patternPath);
            }
        }

    }
}
