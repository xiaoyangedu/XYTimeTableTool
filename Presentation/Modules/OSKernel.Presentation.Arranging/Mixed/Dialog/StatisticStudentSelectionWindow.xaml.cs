using OSKernel.Presentation.Core;
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
namespace OSKernel.Presentation.Arranging.Mixed.Dialog
{
    /// <summary>
    /// 统计学生志愿
    /// </summary>
    public partial class StatisticStudentSelectionWindow
    {
        public StatisticStudentSelectionWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<StatisticStudentSelectionWindowModel>();
        }

        public StatisticStudentSelectionWindow(List<string> bindDatas) : this()
        {
            (this.DataContext as StatisticStudentSelectionWindowModel).BindData(bindDatas);
        }
    }
}
