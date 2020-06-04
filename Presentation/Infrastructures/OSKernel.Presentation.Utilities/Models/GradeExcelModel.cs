using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Utilities.Models
{
    /// <summary>
    /// 行政班年级导出Excel模型
    /// </summary>
    public class GradeExcelModel
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateDateString { get; set; }

        /// <summary>
        /// 标题名称
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// 星期
        /// </summary>
        public List<string> Weeks { get; private set; }

        /// <summary>
        /// 节次
        /// </summary>
        public List<string> Periods { get; private set; }

        /// <summary>
        /// 班级字典（班级名称-对应其它列）从第一节一直到最后一节课程。
        /// </summary>
        public Dictionary<string, List<string>> ClassesDictionary { get; set; }

        public GradeExcelModel()
        {
            this.Header = "年级课表";

            this.CreateDateString = $"报表生成时间：{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}";

            // 星期
            this.Weeks = new List<string>();

            // 节次
            this.Periods = new List<string>();

            // 班级字典
            this.ClassesDictionary = new Dictionary<string, List<string>>();
        }

        /// <summary>
        /// 设置星期
        /// </summary>
        /// <param name="headers"></param>
        public void SetWeeks(List<string> headers)
        {
            this.Weeks = headers;
        }

        /// <summary>
        /// 设置节次
        /// </summary>
        /// <param name="headers"></param>
        public void SetPeriods(List<string> headers)
        {
            this.Periods = headers;
        }

        public void AddClassesDictionary(string key, List<string> values)
        {
            if (!this.ClassesDictionary.ContainsKey(key))
            {
                this.ClassesDictionary.Add(key, values);
            }
        }
    }
}
