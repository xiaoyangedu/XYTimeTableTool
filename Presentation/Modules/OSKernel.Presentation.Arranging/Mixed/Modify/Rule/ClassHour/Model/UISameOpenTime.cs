using OSKernel.Presentation.Models.Mixed;
using OSKernel.Presentation.Models.Rule;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models.Mixed.Rule;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Rule.ClassHour.Model
{
    /// <summary>
    /// 同时开课
    /// </summary>
    public class UISameOpenTime : UIRuleBase
    {
        private string _display;

        public List<UIClass> Classes { get; set; }

        public List<SameOpenDetailsModel> Details { get; set; }

        /// <summary>
        /// 显示
        /// </summary>
        public string Display
        {
            get
            {
                return _display;
            }

            set
            {
                _display = value;
                RaisePropertyChanged(() => Display);
            }
        }

        public UISameOpenTime()
        {
            this.Classes = new List<UIClass>();
            this.Details = new List<SameOpenDetailsModel>();
        }
    }
}
