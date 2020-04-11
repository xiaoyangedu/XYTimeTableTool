using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models;
using XYKernel.OS.Common.Models.Mixed.Result;

namespace OSKernel.Presentation.Models.Result.Mixed
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
        /// 是否从课程框里面返回
        /// </summary>
        public bool IsFromCourseFrame { get; set; } = false;

        /// <summary>
        /// 当前位置
        /// </summary>
        public DayPeriodModel DayPeriod { get; set; }

        /// <summary>
        /// 结果数据
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

        #endregion

        public void RaiseStatus()
        {
            base.RaisePropertyChanged(() => ShowForbid);
        }
    }
}
