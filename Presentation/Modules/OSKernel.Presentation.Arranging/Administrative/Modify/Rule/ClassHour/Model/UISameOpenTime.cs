using OSKernel.Presentation.Models.Base;
using OSKernel.Presentation.Models.Rule;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models.Administrative.Rule;

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Rule.ClassHour.Model
{
    /// <summary>
    /// 同时开课
    /// </summary>
    public class UISameOpenTime : UIRuleBase
    {
        public string Display
        {
            get
            {
                return Classes?.Select(c => c.Display)?.Parse();
            }
        }

        public List<UIClass> Classes { get; set; }

        public List<SameOpenDetailsModel> Details { get; set; }

        public UISameOpenTime()
        {
            this.Classes = new List<UIClass>();
        }
    }
}
