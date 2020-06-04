﻿using OSKernel.Presentation.Core;
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
namespace OSKernel.Presentation.Analysis.Result.Administrative.Views
{
    /// <summary>
    /// 一般性评估
    /// </summary>
    public partial class GeneralAssessmentView : UserControl
    {
        public GeneralAssessmentView()
        {
            InitializeComponent();
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<GeneralAssessmentViewModel>();
        }
    }
}
