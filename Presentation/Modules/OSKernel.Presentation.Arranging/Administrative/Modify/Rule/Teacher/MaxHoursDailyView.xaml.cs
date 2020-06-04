﻿using OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Teacher.Model;
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
namespace OSKernel.Presentation.Arranging.Administrative.Modify.Rule.Teacher
{
    /// <summary>
    /// 教师每天最大课时数
    /// </summary>
    public partial class MaxHoursDailyView : UserControl
    {
        public MaxHoursDailyView()
        {
            InitializeComponent();
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<MaxHoursDailyViewModel>();
        }

        private void Dg_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //MaxHours
            UIMaxHoursDaily cellEdit = e.Row.DataContext as UIMaxHoursDaily;
            if (cellEdit != null)
            {
                cellEdit.IsChecked = true;
            }
        }
    }
}
