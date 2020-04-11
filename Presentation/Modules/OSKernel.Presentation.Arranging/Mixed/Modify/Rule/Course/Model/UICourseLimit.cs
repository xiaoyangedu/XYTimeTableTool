using OSKernel.Presentation.Models.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Course.Model
{
    /// <summary>
    /// 同时开课限制
    /// </summary>
    public class UICourseLimit : UIRuleBase
    {
        private int _limit;

        private bool _unLimited = true;

        public string CourseID { get; set; }

        public string Course { get; set; }

        public int Limit
        {
            get
            {
                return _limit;
            }

            set
            {
                _limit = value;
                RaisePropertyChanged(() => Limit);
            }
        }

        public bool UnLimited
        {
            get
            {
                return _unLimited;
            }

            set
            {
                _unLimited = value;
                RaisePropertyChanged(() => UnLimited);
            }
        }

        public List<XYKernel.OS.Common.Models.Mixed.Rule.PeriodLimit> PeriodLimits { get; set; }

        public UICourseLimit()
        {
            this.PeriodLimits = new List<XYKernel.OS.Common.Models.Mixed.Rule.PeriodLimit>();
        }
    }
}
