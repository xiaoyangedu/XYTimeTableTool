using OSKernel.Presentation.Core;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Arranging
{
    /// <summary>
    /// 存储规则数据到本地
    /// </summary>
    public static class RuleExtend
    {
        /// <summary>
        /// 规则序列化函数
        /// </summary>
        /// <param name="rule">规则</param>
        /// <param name="localID">方案ID</param>
        /// <param name="model">模型</param>
        public static void RuleSerialize(this OSKernel.Presentation.Models.Enums.AdministrativeRuleEnum rule, string localID, object model)
        {
            string path = CacheManager.Instance.GetDataPath();
            string rulePath = $"{path}\\{localID}\\{rule.ToString()}\\{localID}.rule";

            rulePath.SerializeObjectToJson(model);
        }

        /// <summary>
        /// 规则反序列化类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rule">规则</param>
        /// <param name="localID">方案ID</param>
        /// <returns>反序列化对象</returns>
        public static T RuleDeSerialize<T>(this OSKernel.Presentation.Models.Enums.AdministrativeRuleEnum rule, string localID) where T : class
        {
            string path = CacheManager.Instance.GetDataPath();
            string rulePath = $"{path}\\{localID}\\{rule.ToString()}\\{localID}.rule";

            if (File.Exists(rulePath))
            {
                return rulePath.DeSerializeObjectFromJson<T>();
            }
            else
                return null;
        }

        public static void RuleSerialize(this OSKernel.Presentation.Models.Enums.MixedRuleEnum rule, string localID, object model)
        {
            string path = CacheManager.Instance.GetDataPath();
            string rulePath = $"{path}\\{localID}\\{rule.ToString()}\\{localID}.rule";

            rulePath.SerializeObjectToJson(model);
        }

        /// <summary>
        /// 规则反序列化类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rule">规则</param>
        /// <param name="localID">方案ID</param>
        /// <returns>反序列化对象</returns>
        public static T RuleDeSerialize<T>(this OSKernel.Presentation.Models.Enums.MixedRuleEnum rule, string localID) where T : class
        {
            string path = CacheManager.Instance.GetDataPath();
            string rulePath= $"{path}\\{localID}\\{rule.ToString()}\\{localID}.rule";

            if (File.Exists(rulePath))
            {
                return rulePath.DeSerializeObjectFromJson<T>();
            }
            else
                return null;
        }

        public static void DeleteRule(this OSKernel.Presentation.Models.Enums.MixedRuleEnum rule, string localID)
        {
            string path = CacheManager.Instance.GetDataPath();
            string rulePath = $"{path}\\{localID}\\{rule.ToString()}\\{localID}.rule";

            if (File.Exists(rulePath))
            {
                System.IO.File.Delete(rulePath);
            }
        }
    }
}
