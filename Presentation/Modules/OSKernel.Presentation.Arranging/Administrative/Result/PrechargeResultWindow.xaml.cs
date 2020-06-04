using OSKernel.Presentation.Arranging.Dialog;
using OSKernel.Presentation.Core;
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
namespace OSKernel.Presentation.Arranging.Administrative.Result
{
    /// <summary>
    /// 预充值模式
    /// </summary>
    public partial class PrechargeResultWindow
    {
        /// <summary>
        /// 是否使用结果
        /// </summary>
        public bool IsUseResult;

        public PrechargeResultWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<PrechargeResultWindowModel>();
        }

        public PrechargeResultWindow(string tile, List<string> urls) : this()
        {
            this.Title = $"结果预览-{tile}";

            (this.DataContext as PrechargeResultWindowModel).Initilize(urls);
        }

        private void Image_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 图片对象
            Image image = sender as Image;
            UIPrechargeResult precharge = image.DataContext as UIPrechargeResult;

            ShowImageWindow showImage = new ShowImageWindow(precharge);
            showImage.ShowDialog();
        }
    }
}
