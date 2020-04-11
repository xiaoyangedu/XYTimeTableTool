using OSKernel.Presentation.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models.Administrative.Result;

namespace OSKernel.Presentation.Core.DataManager
{
    public interface IResultDataManager
    {
        /// <summary>
        /// 每个方案的排课结果列表
        /// </summary>
        Dictionary<string, List<UIResult>> Results { get; set; }

        /// <summary>
        /// 行政班
        /// </summary>
        Dictionary<string, ResultModel> TaskResults { get; set; }

        /// <summary>
        /// 走班
        /// </summary>
        Dictionary<string, XYKernel.OS.Common.Models.Mixed.Result.ResultModel> TaskCLResults { get; set; }

        /// <summary>
        /// 获取结果根据TaskID-行政班
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        XYKernel.OS.Common.Models.Mixed.Result.ResultModel GetResultCLModelsByTaskID(string key);

        /// <summary>
        /// 获取结果根据TaskID-行政班
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        ResultModel GetResultModelsByTaskID(string key);

        /// <summary>
        /// 添加结果模型-行政班
        /// </summary>
        /// <param name="key">TaskID</param>
        /// <param name="model">添加单个结果</param>
        void AddResultModel(string key, ResultModel model);

        /// <summary>
        /// 添加结果模型-走班
        /// </summary>
        /// <param name="key">TaskID</param>
        /// <param name="model">添加单个结果</param>
        void AddCLResultModel(string key, XYKernel.OS.Common.Models.Mixed.Result.ResultModel model);

        /// <summary>
        /// 根据TaskID删除-行政班
        /// </summary>
        /// <param name="key"></param>
        void DeleteResultModelByTaskID(string key);

        /// <summary>
        /// 根据TaskID删除-走班
        /// </summary>
        /// <param name="key"></param>
        void DeleteCLResultModelByTaskID(string key);

        /// <summary>
        /// 添加结果
        /// </summary>
        /// <param name="key"></param>
        /// <param name="result"></param>
        void AddResult(string key, UIResult result);

        /// <summary>
        /// 批量添加结果
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="results">结果</param>
        void AddRangeResult(string key, List<UIResult> results);

        /// <summary>
        /// 删除结果
        /// </summary>
        /// <param name="caseID">方案ID</param>
        /// <param name="taskID">任务ID</param>
        void RemoveResult(string caseID, long taskID);

        /// <summary>
        /// 获取结果列表
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        List<UIResult> GetResults(string key);
    }
}
