using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Utilities
{
    /// <summary>
    /// 常见扩展类
    /// </summary>
    public static class Extend
    {
        public static string GetLocalDescription(this Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memberInfos = type.GetMember(en.ToString());
            if (memberInfos != null && memberInfos.Length > 0)
            {
                DescriptionAttribute[] attrs = memberInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
                if (attrs != null && attrs.Length > 0)
                {
                    return attrs[0].Description;
                }
            }
            return en.ToString();

        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="deleteAll"></param>
        public static void DeleteDirectory(this string directory, bool deleteAll = true)
        {
            if (System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.Delete(directory, true);
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="file"></param>
        public static void Delete(this string file)
        {
            if (File.Exists(file))
            {
                File.Delete(file);

                if (Directory.Exists(Path.GetDirectoryName(file)))
                {
                    Directory.Delete(Path.GetDirectoryName(file));
                }
            }
        }

        /// <summary>
        /// 是否存在文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool Exists(this string file)
        {
            return System.IO.File.Exists(file);
        }

        /// <summary>
        /// 根据指定字符串进行拼接
        /// </summary>
        /// <param name="list"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        public static string Parse(this IEnumerable<string> list, string sign)
        {
            return string.Join(sign, list ?? new List<string>());
        }

        /// <summary>
        /// 字符串默认用字符串拼接
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string Parse(this IEnumerable<string> list)
        {
            return string.Join(",", list ?? new List<string>());
        }

        /// <summary>
        /// 字符串默认用字符串拼接
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string Parse<T>(this IEnumerable<T> list)
        {
            return string.Join(",", list ?? new List<T>());
        }

        /// <summary>
        /// 验证字符串数组中的数据是否重复
        /// </summary>
        /// <param name="array">要检查的数组</param>
        /// <returns>是否重复</returns>
        public static List<string> IsRepeatHashSet(this List<string> array)
        {
            HashSet<string> hs = new HashSet<string>();

            List<string> repeats = new List<string>();

            for (int i = 0; i < array.Count; i++)
            {
                if (hs.Contains(array[i]))
                {
                    repeats.Add(array[i]);
                }
                else
                {
                    hs.Add(array[i]);
                }
            }

            return repeats;
        }
    }
}
