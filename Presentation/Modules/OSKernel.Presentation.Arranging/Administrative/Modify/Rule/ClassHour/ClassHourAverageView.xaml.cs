﻿using OSKernel.Presentation.Arranging.Administrative.Modify.Rule.ClassHour.Model;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unity;
namespace OSKernel.Presentation.Arranging.Administrative.Modify.Rule.ClassHour
{
    /// <summary>
    /// 课时分散
    /// </summary>
    public partial class ClassHourAverageView : UserControl
    {
        public ClassHourAverageView()
        {
            InitializeComponent();
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<ClassHourAverageViewModel>();
        }

        private void Dg_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            UIClassHourAverage classHour = e.Row.DataContext as UIClassHourAverage;
            if (classHour != null)
            {
                classHour.IsChecked = true;
            }
        }
    }
}
