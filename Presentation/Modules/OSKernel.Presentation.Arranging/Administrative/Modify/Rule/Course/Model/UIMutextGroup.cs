using OSKernel.Presentation.Models.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Course.Model
{
    public class UIMutextGroup : UIRuleBase
    {
        /// <summary>
        /// 互斥组
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 课程ID集合
        /// </summary>
        public List<string> CourseIDs { get; set; }

        public UIMutextGroup()
        {
            this.CourseIDs = new List<string>();
        }
    }
}
