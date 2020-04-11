using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Models.Rule
{
    /// <summary>
    /// 课程设置
    /// </summary>
    public class UICourseSetting : GalaSoft.MvvmLight.ObservableObject
    {
        private bool isChecked;

        private int value;

        private int weight;

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

        public string ID { get; set; }

        public string Name { get; set; }

        public int Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
                RaisePropertyChanged(() => Value);
            }
        }

        public int Weight
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
    }
}
