using OSKernel.Presentation.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Models.Mixed
{
    public class UIRule : GalaSoft.MvvmLight.ObservableObject
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int NO { get; set; }

        /// <summary>
        /// 规则
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        public RuleCategoryEnum Category { get; set; }

        /// <summary>
        /// 规则类型
        /// </summary>
        public Enums.MixedRuleEnum RuleEnum { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comment { get; set; }

        private int record;
        /// <summary>
        /// 记录数
        /// </summary>
        public int Record
        {
            get
            {
                return record;
            }

            set
            {
                record = value;
                this.RaisePropertyChanged(() => Record);
                this.RaisePropertyChanged(() => ShowCount);
            }
        }

        /// <summary>
        /// 显示数量
        /// </summary>
        public bool ShowCount
        {
            get
            {
                if (Record > 0)
                    return true;
                else
                    return false;
            }
        }
    }
}
