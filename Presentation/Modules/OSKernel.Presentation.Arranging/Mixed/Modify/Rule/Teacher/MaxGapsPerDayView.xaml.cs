﻿using OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Teacher.Model;
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
namespace OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Teacher
{
    /// <summary>
    /// 教师每天最大课时间隔
    /// </summary>
    public partial class MaxGapsPerDayView : UserControl
    {
        public MaxGapsPerDayView()
        {
            InitializeComponent();
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<MaxGapsPerDayViewModel>();
        }

        private void Dg_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            UIMaxGapsPerDay maxGaps = e.Row.DataContext as UIMaxGapsPerDay;
            if (maxGaps != null)
            {
                maxGaps.IsChecked = true;
            }
        }
    }
}
