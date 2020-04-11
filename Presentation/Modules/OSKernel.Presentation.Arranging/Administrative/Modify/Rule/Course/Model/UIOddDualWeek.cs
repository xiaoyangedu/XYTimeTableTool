using OSKernel.Presentation.Models.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Course.Model
{
    /// <summary>
    /// 单双周课程
    /// </summary>
    public class UIOddDualWeek : UIRuleBase
    {
        public string ClassID { get; set; }

        public string ClassName { get; set; }

        public string OddCourseID { get; set; }

        public string DualCourseID { get; set; }

        public string OddCourse { get; set; }

        public string DualCourse { get; set; }
    }
}
