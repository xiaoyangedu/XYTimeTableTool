using OSKernel.Presentation.Core;
using OSKernel.Presentation.Models.Enums;
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

namespace OSKernel.Presentation.Analysis.Result.Administrative
{
    /// <summary>
    /// 行政班方案
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
            this.Title = $"结果分析-{title}";
        }
    }
}
