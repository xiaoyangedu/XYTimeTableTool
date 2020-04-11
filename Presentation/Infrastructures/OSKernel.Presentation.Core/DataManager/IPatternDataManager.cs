using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models.Mixed;
using XYKernel.OS.Common.Models.Mixed.AlgoRule;
using XYKernel.OS.Common.Models.Mixed.Rule;

namespace OSKernel.Presentation.Core.DataManager
{
    /// <summary>
    /// 模式数据管理接口
    /// </summary>
    public interface IPatternDataManager
    {
        #region Variate

        /// <summary>
        /// 模式-参数
        /// </summary>
        Dictionary<string, object> Patterns { get; set; }

        /// <summary>
        /// 模式方案
        /// </summary>
        Dictionary<string, CLCase> Cases { get; set; }

        /// <summary>
        /// 排课规则
        /// </summary>
        Dictionary<string, Rule> Rules { get; set; }

        /// <summary>
        /// 算法规则
        /// </summary>
        Dictionary<string, AlgoRule> Algoes { get; set; }

        #endregion

        #region Get

        /// <summary>
        /// 获取排课模式规则
        /// </summary>
        /// <param name="localID"></param>
        /// <returns></returns>
        Rule GetRule(string localID);

        /// <summary>
        /// 获取算法规则数
        /// </summary>
        /// <param name="localID"></param>
        /// <returns></returns>
        AlgoRule GetAlgoRule(string localID);

        /// <summary>
        /// 获取当前模型参数内容
        /// </summary>
        /// <param name="localID"></param>
        /// <returns></returns>
        object GetPatternParam(string localID);

        /// <summary>
        /// 获取方案
        /// </summary>
        /// <param name="localID"></param>
        /// <returns></returns>
        CLCase GetCase(string localID);

        #endregion

        #region Add

        void AddAlgoRule(string localID, AlgoRule model);

        void AddRule(string localID, Rule model);

        void AddCase(string localID, CLCase model);

        void AddPatternParam(string localID, object model);

        #endregion

        #region Clear

        void RemoveAll(string localID);

        #endregion
    }
}
