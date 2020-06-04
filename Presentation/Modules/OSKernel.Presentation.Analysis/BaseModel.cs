using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Analysis.Data
{
    public class BaseModel : GalaSoft.MvvmLight.ObservableObject
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int NO { get; set; }
    }
}
