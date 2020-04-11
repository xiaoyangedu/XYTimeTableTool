using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Models
{
    /// <summary>
    /// UI绑定课时数
    /// </summary>
    public class UIClassHourCount : GalaSoft.MvvmLight.ObservableObject
    {
        private int _lessons;

        /// <summary>
        /// 课时数
        /// </summary>
        public int Lessons
        {
            get
            {
                return _lessons;
            }

            set
            {
                _lessons = value;
                RaisePropertyChanged(() => Lessons);
            }
        }

        /// <summary>
        /// UI显示
        /// </summary>
        public string LessonString
        {
            get
            {
                return $"{Lessons} 课时";
            }
        }
    }
}
