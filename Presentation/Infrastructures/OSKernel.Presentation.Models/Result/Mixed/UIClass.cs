using OSKernel.Presentation.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models;

namespace OSKernel.Presentation.Models.Result.Mixed
{
    /// <summary>
    /// 分析界面模型
    /// </summary>
    public class UIClass : GalaSoft.MvvmLight.ObservableObject
    {
        /// <summary>
        /// 班级ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 班级名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 学生
        /// </summary>
        public List<UIStudent> Students { get; set; }

        /// <summary>
        /// 学生人数
        /// </summary>
        public int Count
        {
            get
            {
                return this.Students.Count;
            }
        }

        /// <summary>
        /// 班额
        /// </summary>
        public int Capacity
        {
            get; set;
        }

        /// <summary>
        /// 显示
        /// </summary>
        public string Display { get; set; }

        /// <summary>
        ///  位置
        /// </summary>
        public List<DayPeriodModel> Positions { get; set; }


        public UIClass()
        {
            this.Students = new List<UIStudent>();
        }

        /// <summary>
        /// 刷新属性
        /// </summary>
        public void RaiseProperty()
        {
            this.RaisePropertyChanged(() => Count);
        }
    }
}
