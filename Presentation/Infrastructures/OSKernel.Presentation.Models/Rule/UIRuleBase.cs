using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using OSKernel.Presentation.Models.Enums;

namespace OSKernel.Presentation.Models.Rule
{
    public class UIRuleBase : GalaSoft.MvvmLight.ObservableObject
    {
        private bool isChecked;

        private WeightTypeEnum weight =  WeightTypeEnum.Hight;

        private bool _isActive;

        private Dictionary<string, WeightTypeEnum> _weights;

        public UIRuleBase()
        {
            this.Weights = new Dictionary<string, WeightTypeEnum>()
            {
                { "高", WeightTypeEnum.Hight},
                { "中", WeightTypeEnum.Medium},
                { "低", WeightTypeEnum.Low},
            };
        }

        /// <summary>
        /// 选中
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }

            set
            {
                isChecked = value;
                RaisePropertyChanged(() => IsChecked);
            }
        }

        /// <summary>
        /// 序号
        /// </summary>
        public int NO { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public WeightTypeEnum Weight
        {
            get
            {
                return weight;
            }

            set
            {
                weight = value;
                RaisePropertyChanged(() => Weight);
            }
        }

        /// <summary>
        /// 是否启动
        /// </summary>
        public bool IsActive
        {
            get
            {
                return _isActive;
            }

            set
            {
                _isActive = value;
                RaisePropertyChanged(() => IsActive);
            }
        }

        /// <summary>
        /// 权重
        /// </summary>
        public Dictionary<string, WeightTypeEnum> Weights
        {
            get
            {
                return _weights;
            }

            set
            {
                _weights = value;
                RaisePropertyChanged(() => Weights);
            }
        }
    }
}
