using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models.Mixed;

namespace OSKernel.Presentation.Models.Mixed
{
    public class UIClassHour : GalaSoft.MvvmLight.ObservableObject
    {
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
        /// 层ID
        /// </summary>
        public string LevelID { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// 层
        /// </summary>
        public string Level { get; set; }

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

                if (string.IsNullOrEmpty(Level))
                {
                    return $"{ID}-{Course}-{Class}-{teacherString}";
                }
                else
                {
                    return $"{ID}-{Course}-{Level}-{Class}-{teacherString}";
                }
            }
        }

        /// <summary>
        /// 短显示
        /// </summary>
        public string ShortDisplay
        {
            get
            {
                if (string.IsNullOrEmpty(Level))
                {
                    return $"{Course}-{Class}";
                }
                else
                {
                    return $"{Course}-{Level}-{Class}";
                }
            }
        }

        public UIClassHour()
        {
            Teachers = new List<TeacherModel>();
            Tags = new List<string>();
        }

        public void RaiseChanged()
        {
            this.RaisePropertyChanged(() => TeacherString);
        }
    }
}
