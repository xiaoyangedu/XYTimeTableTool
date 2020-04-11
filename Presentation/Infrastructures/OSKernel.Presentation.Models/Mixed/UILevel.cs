using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Models.Mixed
{
    /// <summary>
    /// 层
    /// </summary>
    public class UILevel : ObservableObject
    {
        private string _name;

        /// <summary>
        /// 层ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 层
        /// </summary>
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
        /// 课程ID
        /// </summary>
        public string CourseID { get; set; }

        /// <summary>
        /// 课时
        /// </summary>
        public int Lessons { get; set; }
    }
}
