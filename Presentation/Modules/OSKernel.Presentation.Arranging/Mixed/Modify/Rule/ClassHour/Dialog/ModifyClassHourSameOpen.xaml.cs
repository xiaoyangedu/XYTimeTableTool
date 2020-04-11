using OSKernel.Presentation.Arranging.Mixed.Modify.Rule.Teacher.Model;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Models.Mixed;
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
using XYKernel.OS.Common.Models.Mixed.Rule;

namespace OSKernel.Presentation.Arranging.Mixed.Modify.Rule.ClassHour.Dialog
{
    public partial class ModifyClassHourSameOpen
    {
        public List<UIClass> Classes { get; set; }

        public ModifyClassHourSameOpen()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<ModifyClassHourSameOpenModel>();
        }

        public ModifyClassHourSameOpen(List<UIClass> classes) : this()
        {
            // 班级列表
            (this.DataContext as ModifyClassHourSameOpenModel).SetClasses(classes);
        }
    }
}
