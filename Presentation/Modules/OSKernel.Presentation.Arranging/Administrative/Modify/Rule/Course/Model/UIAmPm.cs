using OSKernel.Presentation.Models.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Course.Model
{
    /// <summary>
    /// 上下午UI模型
    /// </summary>
    public class UIAmPm : UIRuleBase
    {
        private int _amMax;
        private int _pmMax;

        public int AmMax
        {
            get
            {
                return _amMax;
            }

            set
            {
                _amMax = value;
                RaisePropertyChanged(() => AmMax);
            }
        }

        public int PmMax
        {
            get
            {
                return _pmMax;
            }

            set
            {
                _pmMax = value;
                RaisePropertyChanged(() => PmMax);
            }
        }

        public string ClassID { get; set; }

        public string CourseID { get; set; }

        public string Course { get; set; }

        public string ClassName { get; set; }
    }
}
