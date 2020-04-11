using GalaSoft.MvvmLight;
using OSKernel.Presentation.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models;

namespace OSKernel.Presentation.Models.Base
{
    /// <summary>
    /// 教师
    /// </summary>
    public class UITeacher : ObservableObject
    {
        private bool _isChecked;
        private string _name;
        private bool hasOperation;

        private Models.Enums.WeightTypeEnum _weight = WeightTypeEnum.Hight;

        private Dictionary<string, WeightTypeEnum> _weights;

        public string ID { get; set; }

        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }

            set
            {
                _isChecked = value;
                RaisePropertyChanged(() => IsChecked);
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        /// <summary>
        /// 是否有操作
        /// </summary>
        public bool HasOperation
        {
            get
            {
                return hasOperation;
            }

            set
            {
                hasOperation = value;
                RaisePropertyChanged(() => HasOperation);
            }
        }

        public List<DayPeriodModel> ForbidTimes { get; set; }

        public WeightTypeEnum Weight
        {
            get
            {
                return _weight;
            }

            set
            {
                _weight = value;
                RaisePropertyChanged(() => Weight);
            }
        }

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

        #region 排课结果使用

        public List<int> ClassHourIDs { get; set; }

        #endregion

        public UITeacher()
        {
            this.Weights = new Dictionary<string, WeightTypeEnum>()
            {
                { "高", WeightTypeEnum.Hight},
                { "中", WeightTypeEnum.Medium},
                { "低", WeightTypeEnum.Low},
            };

            this.ClassHourIDs = new List<int>();
        }
    }
}
