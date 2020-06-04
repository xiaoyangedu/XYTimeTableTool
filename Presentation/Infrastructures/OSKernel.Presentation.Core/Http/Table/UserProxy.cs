using EasyHttp.Http;
using OSKernel.Presentation.Core.Http.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Core.Http
{
    /// <summary>
    /// 用户部分类
    /// </summary>
    public partial class OSHttpClient
    {
        public Tuple<bool, PriceInfo, string> GetPrice(string userToken)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new PostNormal { partner = _partner });

            var postUrl = this.GetPostUrl(Method_GetPrice, body);
            try
            {
                var response = _easyHttpClient.Post(postUrl, body, HttpContentTypes.ApplicationJson);
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
                            messageInfo?.ForEach(mi =>
                            {
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

        public Tuple<bool, UserInfo, string> Login(LoginInfo loginInfo)
        {
            var loginUrl = $"{LoginUrl}&method={Method_Login}";
            try
            {
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
                            messageInfo?.ForEach(mi =>
                            {
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

        public Tuple<bool, string> SetSecret(KeyBodyInfo secretInfo, string token)
        {
            var url = $"{LoginUrl}&method={Method_Secrets}&access_token={token}";

            var message = Newtonsoft.Json.JsonConvert.SerializeObject(secretInfo);

            var response = _easyHttpClient.Post(url, message, HttpContentTypes.ApplicationJson);

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
                    List<MessageInfo> messageInfo;
                    try
                    {
                        messageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageInfo>>(responseInfo.msg);
                        StringBuilder stringBuilder = new StringBuilder();
                        messageInfo?.ForEach(mi =>
                        {
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
    }
}
