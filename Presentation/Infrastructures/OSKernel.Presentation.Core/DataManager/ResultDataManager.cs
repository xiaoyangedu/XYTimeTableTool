using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSKernel.Presentation.Models.Result;
using XYKernel.OS.Common.Models.Administrative.Result;
using XYKernel.OS.Common.Models.Mixed.Result;

namespace OSKernel.Presentation.Core.DataManager
{
    public class ResultDataManager : IResultDataManager
    {
        public Dictionary<string, XYKernel.OS.Common.Models.Administrative.Result.ResultModel> TaskResults
        {
            get; set;
        }

        public Dictionary<string, XYKernel.OS.Common.Models.Mixed.Result.ResultModel> TaskCLResults
        {
            get; set;
        }

        public Dictionary<string, List<UIResult>> Results
        {
            get; set;
        }

        public ResultDataManager()
        {
            this.TaskResults = new Dictionary<string, XYKernel.OS.Common.Models.Administrative.Result.ResultModel>();
            this.TaskCLResults = new Dictionary<string, XYKernel.OS.Common.Models.Mixed.Result.ResultModel>();
            this.Results = new Dictionary<string, List<UIResult>>();
        }

        public void AddCLResultModel(string key, XYKernel.OS.Common.Models.Mixed.Result.ResultModel model)
        {
            if (!this.TaskCLResults.ContainsKey(key))
            {
                this.TaskCLResults.Add(key, model);
            }
            else
            {
                this.TaskCLResults[key] = model;
            }
        }

        public void AddResultModel(string key, XYKernel.OS.Common.Models.Administrative.Result.ResultModel model)
        {
            if (!this.TaskResults.ContainsKey(key))
            {
                this.TaskResults.Add(key, model);
            }
            else
            {
                this.TaskResults[key] = model;
            }
        }

        public void DeleteCLResultModelByTaskID(string key)
        {
            if (this.TaskCLResults.ContainsKey(key))
            {
                this.TaskCLResults.Remove(key);
            }
        }

        public void DeleteResultModelByTaskID(string key)
        {
            if (this.TaskResults.ContainsKey(key))
            {
                this.TaskResults.Remove(key);
            }
        }

        public XYKernel.OS.Common.Models.Mixed.Result.ResultModel GetResultCLModelsByTaskID(string key)
        {
            if (this.TaskCLResults.ContainsKey(key))
            {
                return this.TaskCLResults[key];
            }
            else
                return null;
        }

        public XYKernel.OS.Common.Models.Administrative.Result.ResultModel GetResultModelsByTaskID(string key)
        {
            if (this.TaskResults.ContainsKey(key))
            {
                return this.TaskResults[key];
            }
            else
                return null;
        }

        public void AddResult(string key, UIResult result)
        {
            var has = this.Results.ContainsKey(key);
            if (has)
            {
                this.Results[key].Add(result);
            }
            else
            {
                this.Results.Add(key, new List<UIResult> { result });
            }
        }

        public void RemoveResult(string caseID, long taskID)
        {
            var has = this.Results.ContainsKey(caseID);
            if (has)
            {
                this.Results[caseID].RemoveAll(t => t.TaskID.Equals(taskID));
            }
        }

        public List<UIResult> GetResults(string key)
        {
            var has = this.Results.ContainsKey(key);
            if (!has)
            {
                this.Results.Add(key, new List<UIResult>());
            }
            return this.Results[key];
        }

        public void AddRangeResult(string key, List<UIResult> results)
        {
            var has = this.Results.ContainsKey(key);
            if (!has)
            {
                this.Results.Add(key, results);
            }
            else
            {
                this.Results[key] = results;
            }
        }
    }
}
