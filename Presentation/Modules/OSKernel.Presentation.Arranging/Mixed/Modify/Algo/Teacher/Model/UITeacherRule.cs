using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.Models.Mixed;
using OSKernel.Presentation.Models.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Algo.Teacher.Model
{
    /// <summary>
    /// 用来承载教师规则
    /// </summary>
    public class UITeacherRule : UIAlgoBase
    {
        private int _value;

        public string UID { get; set; }

        public string TeacherID { get; set; }

        public string Name { get; set; }

        public int Value
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;
                RaisePropertyChanged(() => Value);
            }
        }

        public List<DayPeriodModel> ForbidTimes { get; set; }
    }
}
