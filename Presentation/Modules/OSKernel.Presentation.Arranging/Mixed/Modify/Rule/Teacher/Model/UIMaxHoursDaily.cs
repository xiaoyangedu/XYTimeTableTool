using OSKernel.Presentation.Models.Mixed;
using OSKernel.Presentation.Models.Rule;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Teacher.Model
{
    public class UIMaxHoursDaily : UIRuleBase
    {
        private int _maxHours;

        /// <summary>
        /// 课程
        /// </summary>
        public List<UICourse> Courses { get; set; }

        /// <summary>
        /// 课程字符串
        /// </summary>
        public string CourseString
        {
            get
            {
                return Courses?.Select(c => c.Name)?.Parse();
            }
        }

        /// <summary>
        /// 教师ID
        /// </summary>
        public string TeacherID { get; set; }

        /// <summary>
        /// 教师
        /// </summary>
        public string Teacher { get; set; }

        /// <summary>
        /// 最大课时数
        /// </summary>
        public int MaxHours
        {
            get
            {
                return _maxHours;
            }

            set
            {
                _maxHours = value;
                RaisePropertyChanged(() => MaxHours);
            }
        }
    }
}
