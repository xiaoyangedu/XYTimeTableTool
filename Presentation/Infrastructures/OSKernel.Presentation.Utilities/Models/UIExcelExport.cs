using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Utilities.Models
{
    /// <summary>
    /// 走班课表导出Excel模型
    /// </summary>
    public class UIExcelExport
    {
        /// <summary>
        /// 学生ID
        /// </summary>
        public string StudentID { get; set; }

        /// <summary>
        /// 学生姓名
        /// </summary>
        public string Student { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 表格数据
        /// </summary>
        public DataTable Table { get; set; }

        public string Title
        {
            get
            {
                return $"编号:{StudentID} 学生:{Student} 生成时间:{CreateTime.ToString("F")}";
            }
        }
    }
}
