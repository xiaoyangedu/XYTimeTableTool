using OSKernel.Presentation.Models.Base;
using OSKernel.Presentation.Models.Rule;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Course.Model
{
    public class UIClassUnion : UIRuleBase
    {
        public string CourseID { get; set; }

        public string CourseName { get; set; }

        public List<UIClass> Classes { get; set; }

        public string Display
        {
            get
            {
                return Classes?.Select(s => s.Name)?.Parse();
            }
        }

        public UIClassUnion()
        {
            this.Classes = new List<UIClass>();
        }
    }
}
