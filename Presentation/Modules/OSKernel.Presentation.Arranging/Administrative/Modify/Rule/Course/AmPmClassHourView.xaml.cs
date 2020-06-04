﻿using OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Course.Model;
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
namespace OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Course
{
    /// <summary>
    /// 上下午课时
    /// </summary>
    public partial class AmPmClassHourView : UserControl
    {
        public AmPmClassHourView()
        {
            InitializeComponent();
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<AmPmClassHourViewModel>();
        }

        private void Dg_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            UIAmPm ampm = e.Row.DataContext as UIAmPm;
            if (ampm != null)
            {
                ampm.IsChecked = true;
            }
        }
    }
}
