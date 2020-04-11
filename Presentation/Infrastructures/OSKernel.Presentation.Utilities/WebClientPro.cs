using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Utilities
{
    public class WebClientPro : WebClient
    {
        /// <summary>
        /// 过期时间
        /// </summary>
        public int Timeout { get; set; }

        public WebClientPro(int timeout = 30000)
        {
            Timeout = timeout;
        }

        /// <summary>
        /// 重写GetWebRequest,添加WebRequest对象超时时间
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            request.Timeout = Timeout;
            request.ReadWriteTimeout = Timeout;
            return request;
        }
    }
}