using OSKernel.Presentation.Core.Http.User;
using OSKernel.Presentation.Utilities;
using OSKernel.Presentation.Utilities.XY.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models.Mixed.Result;

namespace OSKernel.Presentation.Core.Http.Table
{
    public class WebAPI
    {
        private static readonly object _lock = new object();
        private static WebAPI _api;
        private HttpClient _httpclient;

        private long _partner;
        private string _version;
        private string _privateKey;
        private RSA _rsa;

        /// <summary>
        /// 排课地址
        /// </summary>
        public string Url { get; private set; }
        /// <summary>
        /// 登录地址
        /// </summary>
        public string LoginUrl { get; private set; }

        public static WebAPI Instance
        {
            get
            {
                if (_api == null)
                {
                    lock (_lock)
                    {
                        _api = new WebAPI();
                    }
                }

                return _api;
            }
        }

        #region Method

        /// <summary>
        /// 登录
        /// </summary>
        public const string Method_Login = "user.users.login";

        /// <summary>
        /// 设置签名
        /// </summary>
        public const string Method_Secrets = "auth.secrets.set";

        /// <summary>
        /// 获取价格
        /// </summary>
        public const string Method_GetPrice = "price.own.price";

        /// <summary>
        /// 创建教学班
        /// </summary>
        public const string Method_CreateJXB = "mission.jxb.create";

        /// <summary>
        /// 创建行政班
        /// </summary>
        public const string Method_CreateXZB = "mission.xzb.create";

        /// <summary>
        /// 停止排课任务
        /// </summary>
        public const string Method_Stop = "mission.own.stop";

        /// <summary>
        /// 取消排课任务
        /// </summary>
        public const string Method_Cancel = "mission.own.cancel";

        /// <summary>
        /// 获取进度
        /// </summary>
        public const string Method_Progress = "mission.own.progress";

        /// <summary>
        /// 获取结果
        /// </summary>
        public const string Method_GetResult = "mission.own.fruit";

        /// <summary>
        /// 获取任务列表
        /// </summary>
        public const string Method_GetTaskList = "mission.own.list";

        /// <summary>
        /// 获取任务状态
        /// </summary>
        public const string Method_GetTaskState = "mission.own.state";

        /// <summary>
        /// 获取困难信息
        /// </summary>
        public const string Method_GetDlog = "mission.own.dlog";

        /// <summary>
        /// 获取错误信息
        /// </summary>
        public const string Method_GetEinfo = "mission.own.einfo";

        /// <summary>
        /// 获取年费
        /// </summary>
        public const string Method_GetAnnual = "price.own.annual";

        /// <summary>
        /// 放弃任务
        /// </summary>
        public const string Method_AbandonTask = "mission.own.abandon";

        /// <summary>
        /// 确认任务
        /// </summary>
        public const string Method_ConfirmTask = "mission.own.confirm";

        /// <summary>
        /// 获取样本任务
        /// </summary>
        public const string Method_SampleTask = "mission.own.sample";

        /// <summary>
        /// 写入行政班结果
        /// </summary>
        public const string Method_WriteAdminResult = "mission.xzb.post";

        /// <summary>
        /// 写入行政班结果
        /// </summary>
        public const string Method_WriteMixedResult = "mission.jxb.post";

        #endregion

        public WebAPI()
        {
            _httpclient = new HttpClient();
        }

        #region Initilize

        public void SetLoginFactory(string url, string version)
        {
            this.LoginUrl = $"http://{url}/gateway?version={version}";
        }

        public void SetFactory(string url, string version, long partner, string sign_type = "MD5")
        {
            _partner = partner;
            _version = version;
            _privateKey = CacheManager.Instance.LoginUser.PrivateKey;
            _rsa = RSAUtil.FromPrivateKey(_privateKey);

            this.Url = $"http://{url}/gateway?version={version}&partner={partner}";
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

        public Tuple<bool, T, string> Post<T>(string url, string body)
        {
            HttpClient client = new HttpClient();
            HttpContent content = new StringContent(body);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            try
            {
                var responseTask = client.PostAsync(url, content);
                responseTask.Wait();

                if (responseTask.Status == TaskStatus.RanToCompletion)
                {
                    var response = responseTask.Result;

                    if (responseTask.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var resultTask = response.Content.ReadAsStringAsync();
                        resultTask.Wait();

                        var responseInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(resultTask.Result);
                        if (responseInfo.code == 20000)
                        {
                            var resultText = responseInfo.result == null ? string.Empty : responseInfo.result;
                            var jsonInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(resultText);
                            return Tuple.Create<bool, T, string>(true, jsonInfo, string.Empty);
                        }
                        else
                        {
                            var message = responseInfo.msg;
                            List<MessageInfo> messageInfo;
                            try
                            {
                                messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(message);
                                StringBuilder stringBuilder = new StringBuilder();
                                messageInfo?.ForEach(mi =>
                                {
                                    stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                                });
                                return Tuple.Create<bool, T, string>(false, default(T), stringBuilder.ToString());
                            }
                            catch
                            {
                                return Tuple.Create<bool, T, string>(false, default(T), $"{responseInfo.msg}");
                            }
                        }
                    }
                    else
                    {
                        return Tuple.Create<bool, T, string>(false, default(T), $"{response.StatusCode}");
                    }
                }
                else
                {
                    return Tuple.Create<bool, T, string>(false, default(T), string.Empty);
                }
            }
            catch (Exception ex)
            {
                return Tuple.Create<bool, T, string>(false, default(T), ex.Message);
            }
        }

        public Tuple<bool, string, string> PostString(string url, string body)
        {
            HttpClient client = new HttpClient();
            HttpContent content = new StringContent(body);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            try
            {
                var responseTask = client.PostAsync(url, content);
                responseTask.Wait();

                if (responseTask.Status == TaskStatus.RanToCompletion)
                {
                    var response = responseTask.Result;

                    if (responseTask.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var resultTask = response.Content.ReadAsStringAsync();
                        resultTask.Wait();

                        var responseInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(resultTask.Result);
                        if (responseInfo.code == 20000)
                        {
                            var resultText = responseInfo.result == null ? string.Empty : responseInfo.result;
                            return Tuple.Create<bool, string, string>(true, resultText, string.Empty);
                        }
                        else
                        {
                            var message = responseInfo.msg;
                            List<MessageInfo> messageInfo;
                            try
                            {
                                messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(message);
                                StringBuilder stringBuilder = new StringBuilder();
                                messageInfo?.ForEach(mi =>
                                {
                                    stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                                });
                                return Tuple.Create<bool, string, string>(false, string.Empty, stringBuilder.ToString());
                            }
                            catch
                            {
                                return Tuple.Create<bool, string, string>(false, string.Empty, $"{responseInfo.msg}");
                            }
                        }
                    }
                    else
                    {
                        return Tuple.Create<bool, string, string>(false, string.Empty, $"{response.StatusCode}");
                    }
                }
                else
                {
                    return Tuple.Create<bool, string, string>(false, string.Empty, string.Empty);
                }
            }
            catch (Exception ex)
            {
                return Tuple.Create<bool, string, string>(false, string.Empty, ex.Message);
            }
        }

        public Tuple<bool, bool, string> PostBool(string url, string body)
        {
            HttpClient client = new HttpClient();
            HttpContent content = new StringContent(body);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            try
            {
                var responseTask = client.PostAsync(url, content);
                responseTask.Wait();

                if (responseTask.Status == TaskStatus.RanToCompletion)
                {
                    var response = responseTask.Result;

                    if (responseTask.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var resultTask = response.Content.ReadAsStringAsync();
                        resultTask.Wait();

                        var responseInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(resultTask.Result);
                        if (responseInfo.code == 20000)
                        {
                            bool value = false;
                            var boolValue = bool.TryParse(responseInfo.result, out value);
                            return Tuple.Create<bool, bool, string>(boolValue, value, boolValue == true ? string.Empty : "返回值转换boolean 出错!");
                        }
                        else
                        {
                            var message = responseInfo.msg;
                            List<MessageInfo> messageInfo;
                            try
                            {
                                messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(message);
                                StringBuilder stringBuilder = new StringBuilder();
                                messageInfo?.ForEach(mi =>
                                {
                                    stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                                });
                                return Tuple.Create<bool, bool, string>(false,false, stringBuilder.ToString());
                            }
                            catch
                            {
                                return Tuple.Create<bool, bool, string>(false, false, $"{responseInfo.msg}");
                            }
                        }
                    }
                    else
                    {
                        return Tuple.Create<bool, bool, string>(false, false, $"{response.StatusCode}");
                    }
                }
                else
                {
                    return Tuple.Create<bool, bool, string>(false, false, string.Empty);
                }
            }
            catch (Exception ex)
            {
                return Tuple.Create<bool, bool, string>(false, false, ex.Message);
            }
        }

        public Tuple<bool, Models.Enums.MissionStateEnum, string> Post(string url, string body)
        {
            HttpClient client = new HttpClient();
            HttpContent content = new StringContent(body);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            try
            {
                var responseTask = client.PostAsync(url, content);
                responseTask.Wait();

                if (responseTask.Status == TaskStatus.RanToCompletion)
                {
                    var response = responseTask.Result;

                    if (responseTask.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var resultTask = response.Content.ReadAsStringAsync();
                        resultTask.Wait();

                        var responseInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseInfo>(resultTask.Result);
                        if (responseInfo.code == 20000)
                        {
                            if (Enum.TryParse<Models.Enums.MissionStateEnum>(responseInfo.result, true, out var outEnum))
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
                            var message = responseInfo.msg;
                            List<MessageInfo> messageInfo;
                            try
                            {
                                messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(message);
                                StringBuilder stringBuilder = new StringBuilder();
                                messageInfo?.ForEach(mi =>
                                {
                                    stringBuilder.AppendLine($"({mi.ErrorCode}){mi.Description}");
                                });
                                return Tuple.Create<bool, Models.Enums.MissionStateEnum, string>(false, Models.Enums.MissionStateEnum.Unknown, stringBuilder.ToString());
                            }
                            catch
                            {
                                return Tuple.Create<bool, Models.Enums.MissionStateEnum, string>(false, Models.Enums.MissionStateEnum.Unknown, $"{responseInfo.msg}");
                            }
                        }
                    }
                    else
                    {
                        return Tuple.Create<bool, Models.Enums.MissionStateEnum, string>(false, Models.Enums.MissionStateEnum.Unknown, $"{response.StatusCode}");
                    }
                }
                else
                {
                    return Tuple.Create<bool, Models.Enums.MissionStateEnum, string>(false, Models.Enums.MissionStateEnum.Unknown, string.Empty);
                }
            }
            catch (Exception ex)
            {
                return Tuple.Create<bool, Models.Enums.MissionStateEnum, string>(false, Models.Enums.MissionStateEnum.Unknown, ex.Message);
            }
        }

        #endregion

        #region 登录

        public Tuple<bool, PriceInfo, string> GetPrice(string userToken)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new PostNormal { partner = _partner });
            var url = this.GetPostUrl(Method_GetPrice, body);
            return this.Post<PriceInfo>(url, body);
        }

        public Tuple<bool, UserInfo, string> Login(LoginInfo loginInfo)
        {
            var url = $"{LoginUrl}&method={Method_Login}";
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(loginInfo);
            return this.Post<UserInfo>(url, body);
        }

        public Tuple<bool, string, string> SetSecret(KeyBodyInfo secretInfo, string token)
        {
            var url = $"{LoginUrl}&method={Method_Secrets}&access_token={token}";
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(secretInfo);
            return this.PostString(url, body);
        }

        #endregion

        #region 排课

        public Tuple<bool, long, string> CreateJXB(CLTransfer cl)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(cl);
            var url = this.GetPostUrl(Method_CreateJXB, body);
            return this.Post<long>(url, body);
        }

        public Tuple<bool, long, string> CreateXZB(CPTransfer cp)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(cp);
            var url = this.GetPostUrl(Method_CreateXZB, body);
            return this.Post<long>(url, body);
        }

        public Tuple<bool, string, string> Stop(long taskID)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultTransfer() { id = taskID });
            var url = this.GetPostUrl(Method_Stop, body);
            return this.Post<string>(url, body);
        }

        public Tuple<bool, string, string> Cancel(long taskID)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultTransfer() { id = taskID });
            var url = this.GetPostUrl(Method_Cancel, body);
            return this.Post<string>(url, body);
        }

        public Tuple<bool, ProgressInfo, string> Progress(long taskID)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultTransfer() { id = taskID });
            var url = this.GetPostUrl(Method_Progress, body);
            return this.Post<ProgressInfo>(url, body);
        }

        public Tuple<bool, ResultModel, string> GetMixedResult(long taskID)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultTransfer() { id = taskID });
            var url = this.GetPostUrl(Method_GetResult, body);
            return this.Post<ResultModel>(url, body);
        }

        public Tuple<bool, XYKernel.OS.Common.Models.Administrative.Result.ResultModel, string> GetAdminResult(long taskID)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultTransfer() { id = taskID });
            var url = this.GetPostUrl(Method_GetResult, body);
            return this.Post<XYKernel.OS.Common.Models.Administrative.Result.ResultModel>(url, body);
        }

        public Tuple<bool, List<TaskResult>, string> GetTaskList(TaskParam param)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(param);
            var url = this.GetPostUrl(Method_GetTaskList, body);
            return this.Post<List<TaskResult>>(url, body);
        }

        public Tuple<bool, Models.Enums.MissionStateEnum, string> GetStateByTaskID(long taskID)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultTransfer() { id = taskID });
            var url = this.GetPostUrl(Method_GetTaskState, body);
            return this.Post(url, body);
        }

        public Tuple<bool, string, string> GetDifficultLog(long taskID)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultTransfer() { id = taskID });
            var url = this.GetPostUrl(Method_GetDlog, body);
            return this.PostString(url, body);
        }

        public Tuple<bool, string, string> GetErroInfo(long taskID)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultTransfer() { id = taskID });
            var url = this.GetPostUrl(Method_GetEinfo, body);
            return this.PostString(url, body);
        }

        public Tuple<bool, bool, string> GetAnnual()
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new PostNormal { partner = _partner });
            var url = this.GetPostUrl(Method_GetAnnual, body);
            return this.PostBool(url, body);
        }

        public Tuple<bool, string, string> AbandonTask(long taskID)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultTransfer() { id = taskID });
            var url = this.GetPostUrl(Method_AbandonTask, body);
            return this.PostString(url, body);
        }

        public Tuple<bool, string, string> ConfirmTask(long taskID)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultTransfer() { id = taskID });
            var url = this.GetPostUrl(Method_ConfirmTask, body);
            return this.PostString(url, body);
        }

        public Tuple<bool, List<string>, string> SampleTask(long taskID)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultTransfer() { id = taskID });
            var url = this.GetPostUrl(Method_SampleTask, body);
            return this.Post<List<string>>(url, body);
        }

        public Tuple<bool, string, string> WriteBackResult(long taskID, XYKernel.OS.Common.Models.Administrative.Result.ResultAdjustmentModel resultAdjust)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultAdministrative() { id = taskID, fruit = resultAdjust });
            var url = this.GetPostUrl(Method_WriteAdminResult, body);
            return this.PostString(url, body);
        }

        public Tuple<bool, string, string> WriteBackResult(long taskID, XYKernel.OS.Common.Models.Mixed.Result.ResultAdjustmentModel resultAdjust)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new ResultMixed() { id = taskID, fruit = resultAdjust });
            var url = this.GetPostUrl(Method_WriteMixedResult, body);
            return this.PostString(url, body);
        }

        #endregion
    }
}
