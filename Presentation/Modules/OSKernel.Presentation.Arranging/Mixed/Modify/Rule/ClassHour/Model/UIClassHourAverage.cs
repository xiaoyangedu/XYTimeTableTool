using OSKernel.Presentation.Models.Base;
using OSKernel.Presentation.Models.Rule;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Rule.ClassHour.Model
{
    public class UIClassHourAverage : UIRuleBase
    {
        private int _minDay;

        /// <summary>
        /// 课程ID
        /// </summary>
        public string CourseID { get; set; }

        /// <summary>
        /// 层ID
        /// </summary>
        public string LevelID { get; set; }

        /// <summary>
        /// 班级ID
        /// </summary>
        public string ClassID { get; set; }

        /// <summary>
        /// 班级名称（层-班级名称）
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        public string Course { get; set; }

        /// <summary>
        /// 最小间隔天
        /// </summary>
        public int MinDay
        {
            get
            {
                return _minDay;
            }

            set
            {
                _minDay = value;
                RaisePropertyChanged(() => MinDay);
            }
        }
    }
}
