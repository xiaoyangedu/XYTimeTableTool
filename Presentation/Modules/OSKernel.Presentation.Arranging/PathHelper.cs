using OSKernel.Presentation.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Arranging
{
    /// <summary>
    /// 文件路径
    /// </summary>
    public static class PathHelper
    {
        #region 基础

        /// <summary>
        /// 走班-班级基础文件
        /// </summary>
        /// <param name="localID"></param>
        /// <returns></returns>
        public static string MixedClassFile(this string localID)
        {
            var path = CacheManager.Instance.GetDataPath();
            return $"{path}\\{localID}\\Mixed\\Class\\{localID}.rule";
        }

        /// <summary>
        /// 获取模式目录
        /// </summary>
        /// <param name="localID">本地方案ID</param>
        /// <returns></returns>
        public static string GetPatternDirectory(this string localID)
        {
            // 获取模式目录
            var path = CacheManager.Instance.GetDataPath();
            return $"{path}\\{localID}\\Pattern";
        }

        /// <summary>
        /// 根据文件扩展名删除文件
        /// </summary>
        /// <param name="localID">方案ID</param>
        /// <param name="extension">扩展名</param>
        public static void DeleteFileByExtension(this string localID, string extension)
        {
            // 文件路径
            var path = CacheManager.Instance.GetDataPath();

            // 删除文件
            var file = $"{path}\\{localID}\\{localID}{extension}";
            if (System.IO.File.Exists(file))
            {
                System.IO.File.Delete(file);
            }
        }

        public static void DeleteTaskAdjustRecord(this string localID, long taskID, string extension,string customize="")
        {
            // 文件路径
            var path = CacheManager.Instance.GetDataPath();

            // 删除文件
            var file = $"{path}\\{localID}\\Tasks\\{taskID}\\{taskID}{customize}{extension}";
            if (System.IO.File.Exists(file))
            {
                System.IO.File.Delete(file);
            }
        }

        #endregion
    }
}
