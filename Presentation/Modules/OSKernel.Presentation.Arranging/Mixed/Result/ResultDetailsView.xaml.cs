using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.Http;
using OSKernel.Presentation.Models.Result;
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
namespace OSKernel.Presentation.Arranging.Mixed.Result
{
    /// <summary>
    /// 结果详细界面
    /// </summary>
    public partial class ResultDetailsView
    {
        public ResultDetailsView()
        {
            InitializeComponent();
            this.Owner = System.Windows.Application.Current.MainWindow;
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<ResultDetailsViewModel>();
        }

        public ResultDetailsView(UIResult result) : this()
        {
            // 设置数据源
            (this.DataContext as ResultDetailsViewModel).GetData(result,this);
        }
    }
}
