using OSKernel.Presentation.Models;
using OSKernel.Presentation.Utilities;
using Unity;

namespace OSKernel.Presentation.Core
{
    /// <summary>
    /// 系统全局缓存类
    /// </summary>
    public class CacheManager
    {
        private static readonly object _lock = new object();

        private static CacheManager _cacheManager;

        /// <summary>
        /// 单例
        /// </summary>
        public static CacheManager Instance
        {
            get
            {
                if (_cacheManager == null)
                {
                    lock (_lock)
                    {
                        _cacheManager = new CacheManager();
                    }
                }

                return _cacheManager;
            }
        }

        /// <summary>
        /// 容器
        /// </summary>
        public UnityContainer UnityContainer { get; set; }

        /// <summary>
        /// 当前登录用户
        /// </summary>
        public User LoginUser { get; set; }

        /// <summary>
        /// 版本信息
        /// </summary>
        public ClientUpdate Version { get; set; }

        /// <summary>
        /// 获得当前数据路径
        /// </summary>
        /// <returns></returns>
        public string GetDataPath()
        {
            return CommonPath.DefaultData.CombineCurrentDirectory();

            //if (CacheManager.Instance.LoginUser.UserName.Equals("未知用户"))
            //{
            //    return CommonPath.DefaultData.CombineCurrentDirectory();
            //}
            //else
            //{
            //    return $"{CommonPath.Data}\\{CacheManager.Instance.LoginUser.UserName}\\";
            //}
        }
    }
}
