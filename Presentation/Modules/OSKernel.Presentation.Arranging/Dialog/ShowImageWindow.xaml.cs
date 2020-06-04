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

namespace OSKernel.Presentation.Arranging.Dialog
{
    /// <summary>
    /// 显示图片窗口
    /// </summary>
    public partial class ShowImageWindow
    {
        public ShowImageWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
        }

        public ShowImageWindow(UIPrechargeResult precharge) : this()
        {
            this.Title = $"{precharge.Name}";
            image_show.Source = new BitmapImage(new Uri(precharge.URL));
        }
    }
}
