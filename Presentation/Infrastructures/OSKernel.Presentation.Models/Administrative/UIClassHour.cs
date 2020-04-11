using OSKernel.Presentation.Models.Base;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models.Administrative;

namespace OSKernel.Presentation.Models.Administrative
{
    public class UIClassHour : GalaSoft.MvvmLight.ObservableObject
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int No { get; set; }

        /// <summary>
        /// 课时ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 班级ID
        /// </summary>
        public string ClassID { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// 课程ID
        /// </summary>
        public string CourseID { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        public string Course { get; set; }

        /// <summary>
        /// 教师列表
        /// </summary>
        public List<TeacherModel> Teachers { get; set; }

        /// <summary>
        /// 启动
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// 教师拼接名称
        /// </summary>
        private string _teacherString;
        public string TeacherString
        {
            get
            {
                return Teachers?.Select(s => s.Name)?.Parse();
            }

            set
            {
                _teacherString = value;
            }
        }

        public string Display
        {
            get
            {
                var teacherString = string.Empty;
                if (string.IsNullOrEmpty(TeacherString))
                {
                    teacherString = "无教师";
                }
                else
                {
                    teacherString = TeacherString;
                }
                return $"{ID}-{Course}-{Class}-{teacherString}";
            }
        }

        public UIClassHour()
        {
            Teachers = new List<TeacherModel>();
        }

        public void RaiseChanged()
        {
            this.RaisePropertyChanged(() => TeacherString);
        }
    }
}
