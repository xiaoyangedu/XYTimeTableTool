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

namespace OSKernel.Presentation.Analysis.Data.Mixed
{
    /// <summary>
    /// 走班数据分析
    /// </summary>
    public partial class HostWindow
    {
        public HostWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
        }
        public HostWindow(string title) : this()
        {
            this.Title = $"数据分析-{title}";
        }
    }
}
