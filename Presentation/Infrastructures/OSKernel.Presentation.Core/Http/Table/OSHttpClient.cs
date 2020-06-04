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
    public partial class OSHttpClient
    {
        private static readonly object _lock = new object();

        private static OSHttpClient _client;

        private HttpClient _easyHttpClient;

        public string Url { get; private set; }

        public string LoginUrl { get; private set; }

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

        public OSHttpClient()
        {
            _easyHttpClient = new EasyHttp.Http.HttpClient();
            _easyHttpClient.Request.Accept = HttpContentTypes.ApplicationJson;
            _easyHttpClient.ThrowExceptionOnHttpError = false;
        }

        public void SetFactory(string url, string version, long partner, string sign_type = "MD5")
        {
            _partner = partner;
            _version = version;
            _privateKey = CacheManager.Instance.LoginUser.PrivateKey;
            _rsa = RSAUtil.FromPrivateKey(_privateKey);

            this.Url = $"http://{url}/gateway?version={version}&partner={partner}";
        }

        public void SetLoginFactory(string url, string version)
        {
            this.LoginUrl = $"http://{url}/gateway?version={version}";
        }
    }
}
