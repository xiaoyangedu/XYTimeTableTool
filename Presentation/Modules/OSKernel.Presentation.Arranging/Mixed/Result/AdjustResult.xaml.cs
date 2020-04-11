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
namespace OSKernel.Presentation.Arranging.Mixed.Result
{
    /// <summary>
    /// 调整界面逻辑
    /// </summary>
    public partial class AdjustResult
    {
        public AdjustResultViewModel VM
        {
            get
            {
                return this.DataContext as AdjustResultViewModel;
            }
        }

        public AdjustResult()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<AdjustResultViewModel>();
        }

        public AdjustResult(UIResult result) : this()
        {
            this.Title = $"{result.Name} 结果";
            VM?.GetData(result, this);
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            var sourceWP = e.Data.GetData(typeof(WrapPanel)) as WrapPanel;
            if (sourceWP != null)
            {
                VM?.CourseFrameDrop(sourceWP);
            }
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            VM.Wp_MouseLeftButtonDown(sender, e);
        }

        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Rectangle rectangle = sender as Rectangle;

                if (rectangle != null)
                    DragDrop.DoDragDrop(rectangle, rectangle, DragDropEffects.Copy);
            }
        }
    }
}
