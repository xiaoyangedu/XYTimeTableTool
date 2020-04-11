using OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Course.Model;
using OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Teacher.Model;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Unity;
using XYKernel.OS.Common.Models.Mixed.Rule;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Teacher.Dialog
{
    /// <summary>
    /// 课程限制课位设置
    /// </summary>
    public partial class CourseLimitPosition
    {
        public List<PeriodLimit> PeriodLimits { get; set; }

        public Models.Enums.WeightTypeEnum WeightType { get; set; }

        public CourseLimitPosition()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<CourseLimitPositionModel>();
        }

        public CourseLimitPosition(UICourseLimit model) : this()
        {
            (this.DataContext as CourseLimitPositionModel).Bind(model);
        }
    }
}
