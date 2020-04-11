using OSKernel.Presentation.Arranging.Administrative.Modify.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models;
using XYKernel.OS.Common.Models.Administrative.Result;

namespace OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Course.Model
{
    /// <summary>
    /// 界面锁定对象
    /// </summary>
    public class UILockedItem : GalaSoft.MvvmLight.ObservableObject
    {
        private bool _locked;

        /// <summary>
        /// 当前位置
        /// </summary>
        public DayPeriodModel DayPeriod { get; set; }

        /// <summary>
        /// 结果（如果有两条为单双周类型，普通的只有一个）
        /// </summary>
        public List<ResultDetailModel> Details { get; set; }

        /// <summary>
        /// 是否锁定
        /// </summary>
        public bool Locked
        {
            get
            {
                return _locked;
            }

            set
            {
                _locked = value;
                RaisePropertyChanged(() => Locked);
            }
        }

        /// <summary>
        /// 班级
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// 所有课程
        /// </summary>
        public List<string> Courses { get; set; }

        /// <summary>
        /// 所有教师
        /// </summary>
        public List<string> Teachers { get; set; }

        public UILockedItem()
        {
            this.Details = new List<ResultDetailModel>();
        }
    }
}
