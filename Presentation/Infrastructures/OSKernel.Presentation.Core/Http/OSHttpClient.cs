using EasyHttp.Http;
using OSKernel.Presentation.Core.Http.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using XY.Common;
using XY.Common.Utilities;
using XYKernel.OS.Common.Models.Mixed.Result;

namespace OSKernel.Presentation.Core.Http
{
    /// <summary>
    /// 开源工具 Http请求客户端
    /// </summary>
    public class OSHttpClient
    {
        private static readonly object _lock = new object();

        private static OSHttpClient _client;

        public string Url { get; private set; }

        public string LoginUrl { get; private set; }

        /// <summary>
        /// 单例
        /// </summary>
        public static OSHttpClient Instance
        {
            get
            {
                if (_client == null)
                {
                    lock (_lock)
                    {
                        _client = new OSHttpClient();
                    }
                }

                return _client;
            }
        }

        public EasyHttp.Http.HttpClient _easyHttpClient;
        public OSHttpClient()
        {
            _easyHttpClient = new EasyHttp.Http.HttpClient();
            _easyHttpClient.Request.Accept = HttpContentTypes.ApplicationJson;
            _easyHttpClient.ThrowExceptionOnHttpError = true;
        }

        public void SetFactory(string url, string version, long partner, string sign_type = "MD5")
        {
            Url = $"http://{url}/gateway?version={version}&partner={partner}";
            _partner = partner;
            _version = version;
            _privateKey = CacheManager.Instance.LoginUser.PrivateKey;
            //_rsa = RSAUtil.FromPrivateKey(CacheManager.Instance.LoginUser.PrivateKey);
            _rsa = RSAUtil.FromPrivateKey(_privateKey);
        }

        public void SetLoginFactory(string url, string version)
        {
            LoginUrl = $"http://{url}/gateway?version={version}";
        }

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

            var postUrl = this.GetPostUrl("mission.jxb.create", body);

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
                        messageInfo.ForEach(mi => {
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
            var postUrl = this.GetPostUrl("mission.xzb.create", body);

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
                        messageInfo.ForEach(mi => {
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
            var postUrl = this.GetPostUrl("mission.own.stop", body);
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
                    var messageString = result.msg;
                    try
                    {
                        var messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(message);
                        StringBuilder stringBuilder = new StringBuilder();
                        messageInfo?.ForEach(mi => {
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
            var postUrl = this.GetPostUrl("mission.own.cancel", body);

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
                    var messageString = result.msg;
                    try
                    {
                        var messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(message);
                        StringBuilder stringBuilder = new StringBuilder();
                        messageInfo?.ForEach(mi => {
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
            var postUrl = this.GetPostUrl("mission.own.progress", body);

            var result = _easyHttpClient.Post(postUrl, body, HttpContentTypes.ApplicationJson);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var response = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(result.RawText);
                if (response.code == 20000)
                {
                    var progressInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<ProgressInfo>(response.result);
                    return Tuple.Create<bool, ProgressInfo, string>(true, progressInfo, string.Empty);
                }
                else
                {
                    var message = response.msg;
                    List<MessageInfo> messageInfo;
                    try
                    {
                        messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(message);
                        StringBuilder stringBuilder = new StringBuilder();
                        messageInfo?.ForEach(mi => {
                            stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                        });
                        return Tuple.Create<bool, ProgressInfo, string>(false, null, stringBuilder.ToString());
                    }
                    catch
                    {
                        return Tuple.Create<bool, ProgressInfo, string>(false, null, $"{response.msg}");
                    }
                }
            }
            else
            {
                return Tuple.Create<bool, ProgressInfo, string>(false, null, $"{result.StatusCode}-{result.StatusDescription}");
            }
        }

        /// <summary>
        /// 获取走班结果
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public Tuple<bool, ResultModel, string> GetResult(long taskID)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultTransfer() { id = taskID });
            var postUrl = this.GetPostUrl("mission.own.fruit", body);

            var result = _easyHttpClient.Post(postUrl, body, HttpContentTypes.ApplicationJson);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var response = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(result.RawText);
                if (response.code == 20000)
                {
                    var resultInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultModel>(response.result);
                    return Tuple.Create<bool, ResultModel, string>(true, resultInfo, string.Empty);
                }
                else
                {
                    var message = response.msg;
                    List<MessageInfo> messageInfo;
                    try
                    {
                        messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(message);
                        StringBuilder stringBuilder = new StringBuilder();
                        messageInfo?.ForEach(mi => {
                            stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                        });
                        return Tuple.Create<bool, ResultModel, string>(false, null, stringBuilder.ToString());
                    }
                    catch
                    {
                        return Tuple.Create<bool, ResultModel, string>(false, null, $"{response.msg}");
                    }
                }
            }
            else
            {
                return Tuple.Create<bool, ResultModel, string>(false, null, $"{result.StatusCode}-{result.StatusDescription}");
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
            var postUrl = this.GetPostUrl("mission.own.fruit", body);

            var result = _easyHttpClient.Post(postUrl, body, HttpContentTypes.ApplicationJson);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var response = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(result.RawText);
                if (response.code == 20000)
                {
                    var resultInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<XYKernel.OS.Common.Models.Administrative.Result.ResultModel>(response.result);
                    return Tuple.Create<bool, XYKernel.OS.Common.Models.Administrative.Result.ResultModel, string>(true, resultInfo, string.Empty);
                }
                else
                {
                    var message = response.msg;
                    List<MessageInfo> messageInfo;
                    try
                    {
                        messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(message);
                        StringBuilder stringBuilder = new StringBuilder();
                        messageInfo?.ForEach(mi => {
                            stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                        });
                        return Tuple.Create<bool, XYKernel.OS.Common.Models.Administrative.Result.ResultModel, string>(false, null, stringBuilder.ToString());
                    }
                    catch
                    {
                        return Tuple.Create<bool, XYKernel.OS.Common.Models.Administrative.Result.ResultModel, string>(false, null, $"{response.msg}");
                    }
                }
            }
            else
            {
                return Tuple.Create<bool, XYKernel.OS.Common.Models.Administrative.Result.ResultModel, string>(false, null, $"{result.StatusCode}-{result.StatusDescription}");
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
            var postUrl = this.GetPostUrl("mission.own.list", body);

            var result = _easyHttpClient.Post(postUrl, body, HttpContentTypes.ApplicationJson);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var response = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(result.RawText);
                if (response.code == 20000)
                {
                    var taskInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<TaskInfo>(response.result);
                    if (taskInfo != null)
                    {
                        var taskes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TaskResult>>(taskInfo.data);
                        return Tuple.Create<bool, List<TaskResult>, string>(true, taskes, string.Empty);
                    }
                    else
                    {
                        var message = response.msg;
                        List<MessageInfo> messageInfo;
                        try
                        {
                            messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(message);
                            StringBuilder stringBuilder = new StringBuilder();
                            messageInfo?.ForEach(mi => {
                                stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                            });
                            return Tuple.Create<bool, List<TaskResult>, string>(false, new List<TaskResult>(), stringBuilder.ToString());
                        }
                        catch
                        {
                            return Tuple.Create<bool, List<TaskResult>, string>(false, new List<TaskResult>(), $"{response.msg}");
                        }
                    }
                }
                else
                {
                    return Tuple.Create<bool, List<TaskResult>, string>(false, new List<TaskResult>(), $"{response.msg}");
                }
            }
            else
            {
                return Tuple.Create<bool, List<TaskResult>, string>(false, new List<TaskResult>(), $"{result.StatusCode}-{result.StatusDescription}");
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
            var postUrl = this.GetPostUrl("mission.own.state", body);

            var result = _easyHttpClient.Post(postUrl, body, HttpContentTypes.ApplicationJson);

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var response = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(result.RawText);
                if (response.code == 20000)
                {
                    int enumIndex = 0;
                    var parseStatus = Int32.TryParse(response.result, out enumIndex);
                    if (parseStatus)
                    {
                        var stateEnum = (Models.Enums.MissionStateEnum)enumIndex;
                        return Tuple.Create<bool, Models.Enums.MissionStateEnum, string>(true, stateEnum, string.Empty);
                    }
                    else
                    {
                        return Tuple.Create<bool, Models.Enums.MissionStateEnum, string>(false, Models.Enums.MissionStateEnum.Unknown, "状态转换失败!");
                    }
                }
                else
                {
                    var message = response.msg;
                    List<MessageInfo> messageInfo;
                    try
                    {
                        messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(message);
                        StringBuilder stringBuilder = new StringBuilder();
                        messageInfo?.ForEach(mi => {
                            stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                        });

                        return Tuple.Create<bool, Models.Enums.MissionStateEnum, string>(false, Models.Enums.MissionStateEnum.Unknown, stringBuilder.ToString());
                    }
                    catch
                    {
                        return Tuple.Create<bool, Models.Enums.MissionStateEnum, string>(false, Models.Enums.MissionStateEnum.Unknown, $"{response.msg}");
                    }
                }
            }
            else
            {
                return Tuple.Create<bool, Models.Enums.MissionStateEnum, string>(false, Models.Enums.MissionStateEnum.Unknown, $"{result.StatusCode}-{result.StatusDescription}");
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
            var postUrl = this.GetPostUrl("mission.own.dlog", body);

            var result = _easyHttpClient.Post(postUrl, body, HttpContentTypes.ApplicationJson);

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var response = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(result.RawText);
                if (response.code == 20000)
                {
                    return Tuple.Create<bool, string>(true, response.result);
                }
                else
                {
                    var message = response.msg;
                    List<MessageInfo> messageInfo;
                    try
                    {
                        messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(message);
                        StringBuilder stringBuilder = new StringBuilder();
                        messageInfo?.ForEach(mi => {
                            stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                        });
                        return Tuple.Create<bool, string>(false, stringBuilder.ToString());
                    }
                    catch
                    {
                        return Tuple.Create<bool, string>(false, $"{response.msg}");
                    }
                }
            }
            else
            {
                return Tuple.Create<bool, string>(false, $"{result.StatusCode}-{result.StatusDescription}");
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
            var postUrl = this.GetPostUrl("mission.own.einfo", body);

            var result = _easyHttpClient.Post(postUrl, body, HttpContentTypes.ApplicationJson);

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var response = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(result.RawText);
                if (response.code == 20000)
                {
                    return Tuple.Create<bool, string>(true, response.result);
                }
                else
                {
                    var message = response.msg;
                    List<MessageInfo> messageInfo;
                    try
                    {
                        messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(message);
                        StringBuilder stringBuilder = new StringBuilder();
                        messageInfo?.ForEach(mi => {
                            stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                        });
                        return Tuple.Create<bool, string>(false, stringBuilder.ToString());
                    }
                    catch
                    {
                        return Tuple.Create<bool, string>(false, $"{response.msg}");
                    }
                }
            }
            else
            {
                return Tuple.Create<bool, string>(false, $"{result.StatusCode}-{result.StatusDescription}");
            }
        }

        public Tuple<bool, string> WriteBackResult(long taskID, XYKernel.OS.Common.Models.Administrative.Result.ResultAdjustmentModel result)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultAdministrative() { id = taskID, fruit = result });
            var postUrl = this.GetPostUrl("mission.xzb.post", body);

            var response = _easyHttpClient.Post(postUrl, body, HttpContentTypes.ApplicationJson);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(response.RawText);
                if (responseInfo.code == 20000)
                {
                    return Tuple.Create<bool, string>(true, responseInfo.result);
                }
                else
                {
                    var message = responseInfo.msg;
                    List<MessageInfo> messageInfo;
                    try
                    {
                        messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(message);
                        StringBuilder stringBuilder = new StringBuilder();
                        messageInfo?.ForEach(mi => {
                            stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                        });
                        return Tuple.Create<bool, string>(false, stringBuilder.ToString());
                    }
                    catch
                    {
                        return Tuple.Create<bool, string>(false, $"{responseInfo.code}-{responseInfo.msg}");
                    }
                }
            }
            else
            {
                return Tuple.Create<bool, string>(false, $"{response.StatusCode}-{response.StatusDescription}");
            }
        }

        public Tuple<bool, string> WriteBackResult(long taskID, XYKernel.OS.Common.Models.Mixed.Result.ResultAdjustmentModel result)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultMixed() { id = taskID, fruit = result });
            var postUrl = this.GetPostUrl("mission.jxb.post", body);

            var response = _easyHttpClient.Post(postUrl, body, HttpContentTypes.ApplicationJson);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(response.RawText);
                if (responseInfo.code == 20000)
                {
                    return Tuple.Create<bool, string>(true, responseInfo.result);
                }
                else
                {
                    var message = responseInfo.msg;
                    List<MessageInfo> messageInfo;
                    try
                    {
                        messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(message);
                        StringBuilder stringBuilder = new StringBuilder();
                        messageInfo?.ForEach(mi => {
                            stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                        });
                        return Tuple.Create<bool, string>(false, stringBuilder.ToString());
                    }
                    catch
                    {
                        return Tuple.Create<bool, string>(false, $"{responseInfo.msg}");
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

        #region 登录接口

        public Tuple<bool, PriceInfo, string> GetPrice(string userToken)
        {
            var priceUrl = $"{LoginUrl}&method=price.price.get&access_token={userToken}";
            try
            {
                var response = _easyHttpClient.Get(priceUrl);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(response.RawText);
                    if (responseInfo.code == 20000)
                    {
                        var priceInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<PriceInfo>(responseInfo.result);
                        return Tuple.Create<bool, PriceInfo, string>(true, priceInfo, string.Empty);
                    }
                    else
                    {
                        var message = responseInfo.msg;
                        List<MessageInfo> messageInfo;
                        try
                        {
                            messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(message);
                            StringBuilder stringBuilder = new StringBuilder();
                            messageInfo?.ForEach(mi => {
                                stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                            });
                            return Tuple.Create<bool, PriceInfo, string>(false, null, stringBuilder.ToString());
                        }
                        catch
                        {
                            return Tuple.Create<bool, PriceInfo, string>(false, null, $"{responseInfo.msg}");
                        }
                    }
                }
                else
                {
                    return Tuple.Create<bool, PriceInfo, string>(false, null, $"{response.StatusCode}-{response.StatusDescription}");
                }
            }
            catch (Exception ex)
            {
                return Tuple.Create<bool, PriceInfo, string>(false, null, ex.Message);
            }
        }

        public Tuple<bool, UserInfo, string> Login(string username, string password)
        {
            var loginUrl = $"{LoginUrl}&method=user.users.login";
            try
            {
                var loginInfo = new User.LoginInfo()
                {
                    user_name = username,
                    password = password
                };
                var message = Newtonsoft.Json.JsonConvert.SerializeObject(loginInfo);
                var response = _easyHttpClient.Post(loginUrl, message, HttpContentTypes.ApplicationJson);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(response.RawText);
                    if (responseInfo.code == 20000)
                    {
                        var userInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfo>(responseInfo.result);
                        return Tuple.Create<bool, UserInfo, string>(true, userInfo, string.Empty);
                    }
                    else
                    {
                        var messageString = responseInfo.msg;
                        List<MessageInfo> messageInfo;
                        try
                        {
                            messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(messageString);
                            StringBuilder stringBuilder = new StringBuilder();
                            messageInfo?.ForEach(mi => {
                                stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                            });
                            return Tuple.Create<bool, UserInfo, string>(false, null, stringBuilder.ToString());
                        }
                        catch
                        {
                            return Tuple.Create<bool, UserInfo, string>(false, null, $"{responseInfo.msg}");
                        }
                    }
                }
                else
                {
                    return Tuple.Create<bool, UserInfo, string>(false, null, $"{response.StatusCode}-{response.StatusDescription}");
                }
            }
            catch (Exception ex)
            {
                return Tuple.Create<bool, UserInfo, string>(false, null, ex.Message);
            }
        }

        /// <summary>
        /// RSA2=0,RSA=1,MD5=2
        /// </summary>
        /// <param name="key"></param>
        /// <param name="secrettype"></param>
        /// <returns></returns>
        public Tuple<bool, string> SetSecret(string key, byte secrettype, string token)
        {
            var loginUrl = $"{LoginUrl}&method=auth.secrets.set&access_token={token}";
            var keyInfo = new User.KeyBodyInfo()
            {
                public_key = key,
                secret_type = secrettype
            };
            var message = Newtonsoft.Json.JsonConvert.SerializeObject(keyInfo);
            var response = _easyHttpClient.Post(loginUrl, message, HttpContentTypes.ApplicationJson);

            string result = null;
            bool resultBool = false;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(response.RawText);
                if (responseInfo.code == 20000)
                {
                    resultBool = true;
                }
                else
                {
                    var messageString = responseInfo.msg;
                    List<MessageInfo> messageInfo;
                    try
                    {
                        messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(messageString);
                        StringBuilder stringBuilder = new StringBuilder();
                        messageInfo?.ForEach(mi => {
                            stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                        });

                        result = stringBuilder.ToString();
                    }
                    catch
                    {
                        result = $"{responseInfo.msg}";
                    }
                    resultBool = false;
                }
            }
            else
            {
                resultBool = false;
            }
            return Tuple.Create<bool, string>(resultBool, result);
        }

        #endregion
    }
}
