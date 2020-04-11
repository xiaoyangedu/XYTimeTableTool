using OSKernel.Presentation.Models.Base;
using OSKernel.Presentation.Models.Rule;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Enums;

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Teacher.Model
{
    /// <summary>
    /// 师徒跟随规则
    /// </summary>
    public class UIMasterApprenttice : UIRuleBase
    {
        /// <summary>
        /// 课程ID
        /// </summary>
        public string CourseID { get; set; }

        public string Course { get; set; }

        public string MasterID { get; set; }

        public List<XYKernel.OS.Common.Models.Administrative.TeacherModel> ApprentticeID { get; set; }

        /// <summary>
        /// 师傅
        /// </summary>
        public string Master { get; set; }

        /// <summary>
        /// 徒弟
        /// </summary>
        public string Apprenttice
        {
            get
            {
                return ApprentticeID?.Select(a => a.Name)?.Parse();
            }
        }

        /// <summary>
        /// 跟随方式
        /// </summary>
        //public MasterApprenticeFollow FollowMode { get; set; }

        public UIMasterApprenttice()
        {
            this.ApprentticeID = new List<XYKernel.OS.Common.Models.Administrative.TeacherModel>();
        }
    }
}
