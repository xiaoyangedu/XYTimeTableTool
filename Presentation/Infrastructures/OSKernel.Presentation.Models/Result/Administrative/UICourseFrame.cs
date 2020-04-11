using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using XYKernel.OS.Common.Models;

namespace OSKernel.Presentation.Models.Result.Administrative
{
    /// <summary>
    /// 课程框
    /// </summary>
    public class UICourseFrame
    {
        /// <summary>
        /// 班级
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// 拖拽项
        /// </summary>
        public List<UIDragItem> DragItems { get; set; }

    }
}
