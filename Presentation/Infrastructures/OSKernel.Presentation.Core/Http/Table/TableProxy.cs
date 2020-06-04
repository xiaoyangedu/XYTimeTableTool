using EasyHttp.Http;
using OSKernel.Presentation.Core.Http.User;
using OSKernel.Presentation.Utilities;
using OSKernel.Presentation.Utilities.XY.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models.Mixed.Result;

namespace OSKernel.Presentation.Core.Http
{
    /// <summary>
    /// 开源工具代理类
    /// </summary>
    public partial class OSHttpClient
    {
        #region 排课接口

        private long _partner;
        private string _version;
        private string _privateKey;
        private RSA _rsa;
        /// <summary>
        /// 创建走班任务
        /// </summary>
        /// <param name="cl"></param>
        /// <returns></returns>
        public Tuple<bool, long, string> CreateJXB(CLTransfer cl)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(cl);
            var postUrl = this.GetPostUrl(Method_CreateJXB, body);

            var response = _easyHttpClient.Post(postUrl, body, HttpContentTypes.ApplicationJson);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(response.RawText);
                if (result.code == 20000)
                {
                    var taskID = long.Parse(result.result);
                    return Tuple.Create<bool, long, string>(true, taskID, string.Empty);
                }
                else
                {
                    var message = result.msg;
                    List<MessageInfo> messageInfo;
                    try
                    {
                        messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(message);
                        StringBuilder stringBuilder = new StringBuilder();
                        messageInfo.ForEach(mi =>
                        {
                            stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                        });

                        return Tuple.Create<bool, long, string>(false, 0, stringBuilder.ToString());
                    }
                    catch
                    {
                        return Tuple.Create<bool, long, string>(false, 0, $"{result.msg}");
                    }
                }
            }
            else
            {
                return Tuple.Create<bool, long, string>(false, 0, $"{response.StatusCode}-{response.StatusDescription}");
            }
        }

        /// <summary>
        /// 创建行政班
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public Tuple<bool, long, string> CreateXZB(CPTransfer cp)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(cp);
            var postUrl = this.GetPostUrl(Method_CreateXZB, body);

            var response = _easyHttpClient.Post(postUrl, body, HttpContentTypes.ApplicationJson);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(response.RawText);
                if (result.code == 20000)
                {
                    var taskID = long.Parse(result.result);
                    return Tuple.Create<bool, long, string>(true, taskID, string.Empty);
                }
                else
                {
                    var message = result.msg;
                    List<MessageInfo> messageInfo;
                    try
                    {
                        messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(message);
                        StringBuilder stringBuilder = new StringBuilder();
                        messageInfo.ForEach(mi =>
                        {
                            stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                        });

                        return Tuple.Create<bool, long, string>(false, 0, stringBuilder.ToString());
                    }
                    catch
                    {
                        return Tuple.Create<bool, long, string>(false, 0, $"{result.msg}");
                    }

                }
            }
            else
            {
                return Tuple.Create<bool, long, string>(false, 0, $"{response.StatusCode}-{response.StatusDescription}");
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public Tuple<bool, string> Stop(long taskID)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultTransfer() { id = taskID });
            var postUrl = this.GetPostUrl(Method_Stop, body);

            var response = _easyHttpClient.Post(postUrl, body, HttpContentTypes.ApplicationJson);

            bool isSuccess = false;
            string message = string.Empty;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(response.RawText);
                if (result.code == 20000)
                {
                    isSuccess = true;
                }
                else
                {
                    try
                    {
                        var messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(result.msg);
                        StringBuilder stringBuilder = new StringBuilder();
                        messageInfo?.ForEach(mi =>
                        {
                            stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                        });
                        message = stringBuilder.ToString();
                    }
                    catch
                    {
                        message = $"{result.msg}";
                    }
                    isSuccess = false;
                }
            }
            else
            {
                isSuccess = false;
            }

            return Tuple.Create<bool, string>(isSuccess, message);
        }

        /// <summary>
        /// 取消任务
        /// </summary>
        /// <param name="cp">任务ID</param>
        /// <returns></returns>
        public Tuple<bool, string> Cancel(long taskID)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultTransfer() { id = taskID });
            var postUrl = this.GetPostUrl(Method_Cancel, body);

            var response = _easyHttpClient.Post(postUrl, body, HttpContentTypes.ApplicationJson);

            bool isSuccess = false;
            string message = string.Empty;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(response.RawText);
                if (result.code == 20000)
                {
                    isSuccess = true;
                }
                else
                {
                    try
                    {
                        var messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(result.msg);
                        StringBuilder stringBuilder = new StringBuilder();
                        messageInfo?.ForEach(mi =>
                        {
                            stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                        });
                        message = stringBuilder.ToString();
                    }
                    catch
                    {
                        message = $"{result.msg}";
                    }
                    isSuccess = false;
                }
            }
            else
            {
                isSuccess = false;
            }

            return Tuple.Create<bool, string>(isSuccess, message);
        }

        /// <summary>
        /// 获取进度
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <returns></returns>
        public Tuple<bool, ProgressInfo, string> Progress(long taskID)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultTransfer() { id = taskID });
            var postUrl = this.GetPostUrl(Method_Progress, body);

            var response = _easyHttpClient.Post(postUrl, body, HttpContentTypes.ApplicationJson);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(response.RawText);
                if (result.code == 20000)
                {
                    var progressInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<ProgressInfo>(result.result);
                    return Tuple.Create<bool, ProgressInfo, string>(true, progressInfo, string.Empty);
                }
                else
                {
                    List<MessageInfo> messageInfo;
                    try
                    {
                        messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(result.msg);

                        StringBuilder stringBuilder = new StringBuilder();
                        messageInfo?.ForEach(mi =>
                        {
                            stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                        });
                        return Tuple.Create<bool, ProgressInfo, string>(false, null, stringBuilder.ToString());
                    }
                    catch
                    {
                        return Tuple.Create<bool, ProgressInfo, string>(false, null, $"{result.msg}");
                    }
                }
            }
            else
            {
                return Tuple.Create<bool, ProgressInfo, string>(false, null, $"{response.StatusCode}-{response.StatusDescription}");
            }
        }

        /// <summary>
        /// 获取走班结果
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public Tuple<bool, ResultModel, string> GetMixedResult(long taskID)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultTransfer() { id = taskID });
            var postUrl = this.GetPostUrl(Method_GetResult, body);

            var response = _easyHttpClient.Post(postUrl, body, HttpContentTypes.ApplicationJson);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(response.RawText);
                if (result.code == 20000)
                {
                    var resultInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultModel>(result.result);
                    return Tuple.Create<bool, ResultModel, string>(true, resultInfo, string.Empty);
                }
                else
                {
                    List<MessageInfo> messageInfo;
                    try
                    {
                        messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(result.msg);

                        StringBuilder stringBuilder = new StringBuilder();
                        messageInfo?.ForEach(mi =>
                        {
                            stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                        });
                        return Tuple.Create<bool, ResultModel, string>(false, null, stringBuilder.ToString());
                    }
                    catch
                    {
                        return Tuple.Create<bool, ResultModel, string>(false, null, $"{result.msg}");
                    }
                }
            }
            else
            {
                return Tuple.Create<bool, ResultModel, string>(false, null, $"{response.StatusCode}-{response.StatusDescription}");
            }
        }

        /// <summary>
        /// 行政班获取结构
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public Tuple<bool, XYKernel.OS.Common.Models.Administrative.Result.ResultModel, string> GetAdminResult(long taskID)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultTransfer() { id = taskID });
            var postUrl = this.GetPostUrl(Method_GetResult, body);

            var response = _easyHttpClient.Post(postUrl, body, HttpContentTypes.ApplicationJson);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(response.RawText);
                if (result.code == 20000)
                {
                    var resultInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<XYKernel.OS.Common.Models.Administrative.Result.ResultModel>(result.result);
                    return Tuple.Create<bool, XYKernel.OS.Common.Models.Administrative.Result.ResultModel, string>(true, resultInfo, string.Empty);
                }
                else
                {
                    List<MessageInfo> messageInfo;
                    try
                    {
                        messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(result.msg);

                        StringBuilder stringBuilder = new StringBuilder();
                        messageInfo?.ForEach(mi =>
                        {
                            stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                        });
                        return Tuple.Create<bool, XYKernel.OS.Common.Models.Administrative.Result.ResultModel, string>(false, null, stringBuilder.ToString());
                    }
                    catch
                    {
                        return Tuple.Create<bool, XYKernel.OS.Common.Models.Administrative.Result.ResultModel, string>(false, null, $"{result.msg}");
                    }
                }
            }
            else
            {
                return Tuple.Create<bool, XYKernel.OS.Common.Models.Administrative.Result.ResultModel, string>(false, null, $"{response.StatusCode}-{response.StatusDescription}");
            }
        }

        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public Tuple<bool, List<TaskResult>, string> GetTaskList(TaskParam param)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(param);
            var postUrl = this.GetPostUrl(Method_GetTaskList, body);

            var response = _easyHttpClient.Post(postUrl, body, HttpContentTypes.ApplicationJson);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(response.RawText);
                if (result.code == 20000)
                {
                    var taskInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<TaskInfo>(result.result);
                    if (taskInfo != null)
                    {
                        var taskes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TaskResult>>(taskInfo.data);
                        return Tuple.Create<bool, List<TaskResult>, string>(true, taskes, string.Empty);
                    }
                    else
                    {
                        List<MessageInfo> messageInfo;
                        try
                        {
                            messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(result.msg);

                            StringBuilder stringBuilder = new StringBuilder();
                            messageInfo?.ForEach(mi =>
                            {
                                stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                            });
                            return Tuple.Create<bool, List<TaskResult>, string>(false, new List<TaskResult>(), stringBuilder.ToString());
                        }
                        catch
                        {
                            return Tuple.Create<bool, List<TaskResult>, string>(false, new List<TaskResult>(), $"{result.msg}");
                        }
                    }
                }
                else
                {
                    return Tuple.Create<bool, List<TaskResult>, string>(false, new List<TaskResult>(), $"{result.msg}");
                }
            }
            else
            {
                return Tuple.Create<bool, List<TaskResult>, string>(false, new List<TaskResult>(), $"{response.StatusCode}-{response.StatusDescription}");
            }
        }

        /// <summary>
        /// 获取任务状态
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <returns></returns>
        public Tuple<bool, Models.Enums.MissionStateEnum, string> GetStateByTaskID(long taskID)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultTransfer() { id = taskID });
            var postUrl = this.GetPostUrl(Method_GetTaskState, body);

            var response = _easyHttpClient.Post(postUrl, body, HttpContentTypes.ApplicationJson);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(response.RawText);
                if (result.code == 20000)
                {
                    if (Enum.TryParse<Models.Enums.MissionStateEnum>(result.result, true, out var outEnum))
                    {
                        return Tuple.Create<bool, Models.Enums.MissionStateEnum, string>(true, outEnum, string.Empty);
                    }
                    else
                    {

                        return Tuple.Create<bool, Models.Enums.MissionStateEnum, string>(false, Models.Enums.MissionStateEnum.Completed, "转换失败!");
                    }
                }
                else
                {
                    List<MessageInfo> messageInfo;
                    try
                    {
                        messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(result.msg);

                        StringBuilder stringBuilder = new StringBuilder();
                        messageInfo?.ForEach(mi =>
                        {
                            stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                        });

                        return Tuple.Create<bool, Models.Enums.MissionStateEnum, string>(false, Models.Enums.MissionStateEnum.Unknown, stringBuilder.ToString());
                    }
                    catch
                    {
                        return Tuple.Create<bool, Models.Enums.MissionStateEnum, string>(false, Models.Enums.MissionStateEnum.Unknown, $"{result.msg}");
                    }
                }
            }
            else
            {
                return Tuple.Create<bool, Models.Enums.MissionStateEnum, string>(false, Models.Enums.MissionStateEnum.Unknown, $"{response.StatusCode}-{response.StatusDescription}");
            }
        }

        /// <summary>
        /// 获取困难
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public Tuple<bool, string> GetDifficultLog(long taskID)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultTransfer() { id = taskID });
            var postUrl = this.GetPostUrl(Method_GetDlog, body);

            var response = _easyHttpClient.Post(postUrl, body, HttpContentTypes.ApplicationJson);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(response.RawText);
                if (result.code == 20000)
                {
                    return Tuple.Create<bool, string>(true, result.result);
                }
                else
                {
                    List<MessageInfo> messageInfo;
                    try
                    {
                        messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(result.msg);
                        StringBuilder stringBuilder = new StringBuilder();
                        messageInfo?.ForEach(mi =>
                        {
                            stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                        });
                        return Tuple.Create<bool, string>(false, stringBuilder.ToString());
                    }
                    catch
                    {
                        return Tuple.Create<bool, string>(false, $"{result.msg}");
                    }
                }
            }
            else
            {
                return Tuple.Create<bool, string>(false, $"{response.StatusCode}-{response.StatusDescription}");
            }
        }

        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public Tuple<bool, string> GetErroInfo(long taskID)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultTransfer() { id = taskID });
            var postUrl = this.GetPostUrl(Method_GetEinfo, body);

            var response = _easyHttpClient.Post(postUrl, body, HttpContentTypes.ApplicationJson);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(response.RawText);
                if (result.code == 20000)
                {
                    return Tuple.Create<bool, string>(true, result.result);
                }
                else
                {
                    List<MessageInfo> messageInfo;
                    try
                    {
                        messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(result.msg);
                        StringBuilder stringBuilder = new StringBuilder();
                        messageInfo?.ForEach(mi =>
                        {
                            stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                        });
                        return Tuple.Create<bool, string>(false, stringBuilder.ToString());
                    }
                    catch
                    {
                        return Tuple.Create<bool, string>(false, $"{result.msg}");
                    }
                }
            }
            else
            {
                return Tuple.Create<bool, string>(false, $"{response.StatusCode}-{response.StatusDescription}");
            }
        }

        /// <summary>
        /// 获取年预付费
        /// </summary>
        /// <returns>返回是否年付费</returns>
        public Tuple<bool, bool, string> GetAnnual()
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new PostNormal { partner = _partner });

            var postUrl = this.GetPostUrl(Method_GetAnnual, body);
            var response = _easyHttpClient.Post(postUrl, body, HttpContentTypes.ApplicationJson);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(response.RawText);
                if (result.code == 20000)
                {
                    var isAnnual = bool.Parse(result.result);
                    return Tuple.Create<bool, bool, string>(true, isAnnual, string.Empty);
                }
                else
                {
                    List<MessageInfo> messageInfo;
                    try
                    {
                        messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(result.msg);
                        StringBuilder stringBuilder = new StringBuilder();
                        messageInfo?.ForEach(mi =>
                        {
                            stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                        });
                        return Tuple.Create<bool, bool, string>(false, false, stringBuilder.ToString());
                    }
                    catch
                    {
                        return Tuple.Create<bool, bool, string>(false, false, $"{result.msg}");
                    }
                }
            }
            else
            {
                return Tuple.Create<bool, bool, string>(false, false, $"{response.StatusCode}-{response.StatusDescription}");
            }
        }

        /// <summary>
        /// 放弃使用任务
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <returns></returns>
        public Tuple<bool, string> AbandonTask(long taskID)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultTransfer() { id = taskID });
            var postUrl = this.GetPostUrl(Method_AbandonTask, body);

            var response = _easyHttpClient.Post(postUrl, body, HttpContentTypes.ApplicationJson);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(response.RawText);
                if (result.code == 20000)
                {
                    return Tuple.Create<bool, string>(true, result.result);
                }
                else
                {
                    List<MessageInfo> messageInfo;
                    try
                    {
                        messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(result.msg);
                        StringBuilder stringBuilder = new StringBuilder();
                        messageInfo?.ForEach(mi =>
                        {
                            stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                        });
                        return Tuple.Create<bool, string>(false, stringBuilder.ToString());
                    }
                    catch
                    {
                        return Tuple.Create<bool, string>(false, $"{result.msg}");
                    }
                }
            }
            else
            {
                return Tuple.Create<bool, string>(false, $"{response.StatusCode}-{response.StatusDescription}");
            }
        }

        /// <summary>
        /// 确认任务
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <returns></returns>
        public Tuple<bool, string> ConfirmTask(long taskID)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultTransfer() { id = taskID });
            var postUrl = this.GetPostUrl(Method_ConfirmTask, body);

            var response = _easyHttpClient.Post(postUrl, body, HttpContentTypes.ApplicationJson);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(response.RawText);
                if (result.code == 20000)
                {
                    return Tuple.Create<bool, string>(true, result.result);
                }
                else
                {
                    List<MessageInfo> messageInfo;
                    try
                    {
                        messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(result.msg);

                        StringBuilder stringBuilder = new StringBuilder();
                        messageInfo?.ForEach(mi =>
                        {
                            stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                        });
                        return Tuple.Create<bool, string>(false, stringBuilder.ToString());
                    }
                    catch
                    {
                        return Tuple.Create<bool, string>(false, $"{result.msg}");
                    }
                }
            }
            else
            {
                return Tuple.Create<bool, string>(false, $"{response.StatusCode}-{response.StatusDescription}");
            }
        }


        /// <summary>
        /// 获取任务样本
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <returns></returns>
        public Tuple<bool, List<string>, string> SampleTask(long taskID)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultTransfer() { id = taskID });
            var postUrl = this.GetPostUrl(Method_SampleTask, body);

            var response = _easyHttpClient.Post(postUrl, body, HttpContentTypes.ApplicationJson);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(response.RawText);
                if (result.code == 20000)
                {
                    var results = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(result.result);
                    return Tuple.Create<bool, List<string>, string>(true, results, result.result);
                }
                else
                {
                    List<MessageInfo> messageInfo;
                    try
                    {
                        messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(result.msg);
                        StringBuilder stringBuilder = new StringBuilder();
                        messageInfo?.ForEach(mi =>
                        {
                            stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                        });
                        return Tuple.Create<bool, List<string>, string>(false, null, stringBuilder.ToString());
                    }
                    catch
                    {
                        return Tuple.Create<bool, List<string>, string>(false, null, $"{result.msg}");
                    }
                }
            }
            else
            {
                return Tuple.Create<bool, List<string>, string>(false, null, $"{response.StatusCode}-{response.StatusDescription}");
            }
        }

        /// <summary>
        /// 回写行政班结果
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <param name="result">结果</param>
        /// <returns></returns>
        public Tuple<bool, string> WriteBackResult(long taskID, XYKernel.OS.Common.Models.Administrative.Result.ResultAdjustmentModel resultAdjust)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultAdministrative() { id = taskID, fruit = resultAdjust });
            var postUrl = this.GetPostUrl(Method_WriteAdminResult, body);

            var response = _easyHttpClient.Post(postUrl, body, HttpContentTypes.ApplicationJson);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(response.RawText);
                if (result.code == 20000)
                {
                    return Tuple.Create<bool, string>(true, result.result);
                }
                else
                {
                    List<MessageInfo> messageInfo;
                    try
                    {
                        messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(result.msg);
                        StringBuilder stringBuilder = new StringBuilder();
                        messageInfo?.ForEach(mi =>
                        {
                            stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                        });
                        return Tuple.Create<bool, string>(false, stringBuilder.ToString());
                    }
                    catch
                    {
                        return Tuple.Create<bool, string>(false, $"{result.code}-{result.msg}");
                    }
                }
            }
            else
            {
                return Tuple.Create<bool, string>(false, $"{response.StatusCode}-{response.StatusDescription}");
            }
        }

        /// <summary>
        /// 回写走班结果
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <param name="result">结果</param>
        /// <returns></returns>
        public Tuple<bool, string> WriteBackResult(long taskID, XYKernel.OS.Common.Models.Mixed.Result.ResultAdjustmentModel resultAdjust)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultMixed() { id = taskID, fruit = resultAdjust });
            var postUrl = this.GetPostUrl(Method_WriteMixedResult, body);

            var response = _easyHttpClient.Post(postUrl, body, HttpContentTypes.ApplicationJson);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(response.RawText);
                if (result.code == 20000)
                {
                    return Tuple.Create<bool, string>(true, result.result);
                }
                else
                {
                    List<MessageInfo> messageInfo;
                    try
                    {
                        messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(result.msg);
                        StringBuilder stringBuilder = new StringBuilder();
                        messageInfo?.ForEach(mi =>
                        {
                            stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                        });
                        return Tuple.Create<bool, string>(false, stringBuilder.ToString());
                    }
                    catch
                    {
                        return Tuple.Create<bool, string>(false, $"{result.msg}");
                    }
                }
            }
            else
            {
                return Tuple.Create<bool, string>(false, $"{response.StatusCode}-{response.StatusDescription}");
            }
        }

        public string GetPostUrl(string method, string body)
        {
            var bodyMD5 = HashUtil.MD5(body);

            var timeSpan = DateTime.Now.ToUnixTimestamp();

            var rsa2Data = $"biz_content={bodyMD5}&method={method}&partner={_partner}&timestamp={timeSpan}&version={_version}";

            var signData = _rsa.SignData(rsa2Data.GetBytes(), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            string sign = signData.ToHexString();

            return $"{Url}&method={method}&timestamp={timeSpan}&sign={sign}";
        }

        #endregion
    }
}
