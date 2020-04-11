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
using XYKernel.OS.Common.Models.Administrative.Result;

namespace OSKernel.Presentation.Arranging.Administrative.Result
{
    /// <summary>
    /// 调整结果界面
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
            var sourceSP = e.Data.GetData(typeof(StackPanel)) as StackPanel;
            if (sourceSP != null)
            {
                VM?.CourseFrameDrop(sourceSP);
            }
        }

        private void Sp_MouseMove(object sender, MouseEventArgs e)
        {
            StackPanel stackPanel = sender as StackPanel;

            if (stackPanel != null && e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(stackPanel, stackPanel, DragDropEffects.Copy);
            }
        }

        private void Sp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            VM.Sp_MouseLeftButtonDown(sender, e);
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            VM.Sp_MouseLeftButtonDown(sender, e);
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
