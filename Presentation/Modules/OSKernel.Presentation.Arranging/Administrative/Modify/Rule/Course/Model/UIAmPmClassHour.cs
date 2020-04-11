using OSKernel.Presentation.Models.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Course.Model
{
    public class UIAmPmClassHour : UIRuleBase
    {
        /// <summary>
        /// 课程ID
        /// </summary>
        public string CourseID { get; set; }

        /// <summary>
        /// 班级ID
        /// </summary>
        public string ClassID { get; set; }

        /// <summary>
        /// 班级名称
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 上午最大课时
        /// </summary>
        public int AmMax { get; set; }

        /// <summary>
        /// 下午最大课时
        /// </summary>
        public int PmMax { get; set; }
    }
}
