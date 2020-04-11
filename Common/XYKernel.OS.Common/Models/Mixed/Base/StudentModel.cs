using System.Collections.Generic;
using System.Linq;

namespace XYKernel.OS.Common.Models.Mixed
{
    /// <summary>
    /// 学生
    /// </summary>
    public class StudentModel
    {
        /// <summary>
        /// 学生ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 学生名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 志愿列表
        /// </summary>
        public List<PreselectionModel> Preselections { get; set; }

        public StudentModel()
        {
            Preselections = new List<PreselectionModel>();
        }
    }
}
