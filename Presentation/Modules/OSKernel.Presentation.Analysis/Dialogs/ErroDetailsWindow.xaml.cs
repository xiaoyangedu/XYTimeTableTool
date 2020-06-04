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

namespace OSKernel.Presentation.Analysis.Dialogs
{
    /// <summary>
    /// 错误详细信息窗口
    /// </summary>
    public partial class ErroDetailsWindow
    {
        public ErroDetailsWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
        }

        public ErroDetailsWindow(List<string> erros) : this()
        {
            dg_erro.ItemsSource = erros;
        }
    }
}
