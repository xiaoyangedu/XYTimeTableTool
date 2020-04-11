using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models;
using XYKernel.OS.Common.Models.Administrative.Result;

namespace OSKernel.Presentation.Models.Result.Administrative
{
    /// <summary>
    /// 行政班-拖拽对象
    /// </summary>
    public class UIDragItem : GalaSoft.MvvmLight.ObservableObject
    {
        /// <summary>
        /// 是否可以拖拽（非空可拖拽）
        /// </summary>
        public bool CanDrag { get; set; } = false;

        /// <summary>
        /// 是否可以拖放
        /// </summary>
        public bool CanDrop { get; set; } = true;

        /// <summary>
        /// 是否常规（否则为单双周）
        /// </summary>
        public bool IsNormal { get; set; } = true;

        /// <summary>
        /// 是否从课程框里面返回
        /// </summary>
        public bool IsFromCourseFrame { get; set; } = false;

        /// <summary>
        /// 当前位置
        /// </summary>
        public DayPeriodModel DayPeriod { get; set; }

        /// <summary>
        /// 结果（如果有两条为单双周类型，普通的只有一个）
        /// </summary>
        public List<ResultDetailModel> Details { get; set; }

        /// <summary>
        /// 显示禁止
        /// </summary>
        public bool ShowForbid
        {
            get
            {
                return !CanDrop;
            }
        }

        #region Details

        // TODO
        /// <summary>
        /// 班级ID
        /// </summary>
        public string D_ClassID { get; set; }
        /// <summary>
        /// 班级名称
        /// </summary>
        public string D_ClassName { get; set; }

        /// <summary>
        /// 颜色字符串
        /// </summary>
        public string D_Color { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public double D_Width { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        public double D_Height { get; set; }

        /// <summary>
        /// 教师名称
        /// </summary>
        public string D_Teacher { get; set; }

        /// <summary>
        /// 课程详细
        /// </summary>
        public List<string> D_Courses { get; set; }

        /// <summary>
        /// 课程拼接名称
        /// </summary>
        public string D_Course
        {
            get
            {
                return D_Courses?.Parse();
            }
        }

        #endregion

        public void RaiseStatus()
        {
            base.RaisePropertyChanged(() => ShowForbid);
        }
    }
}
