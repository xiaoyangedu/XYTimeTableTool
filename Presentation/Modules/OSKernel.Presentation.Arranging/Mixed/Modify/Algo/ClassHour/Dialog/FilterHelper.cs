using OSKernel.Presentation.Models.Mixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Algo.ClassHour.Dialog
{
    public class FilterHelper
    {
        public static List<UIClass> Classes { get; set; }

        public static UIClass GetClassById(string classID)
        {
            return Classes?.FirstOrDefault(C => C.ID.Equals(classID));
        }
    }
}
