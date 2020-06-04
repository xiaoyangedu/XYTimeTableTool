using OSKernel.Presentation.Core;
using OSKernel.Presentation.Models.Base;
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
namespace OSKernel.Presentation.Arranging.Mixed.Dialog
{
    public partial class SetStudentPreselectionWindow
    {
        /// <summary>
        /// 选择
        /// </summary>
        public List<UIPreselection> Preselections { get; set; }

        public SetStudentPreselectionWindow()
        {
            InitializeComponent();

            this.Owner = Application.Current.MainWindow;
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<SetStudentPreselectionWindowModel>();
        }

        public SetStudentPreselectionWindow(UIStudent student) : this()
        {
            this.Title = $"设置学生：{student.Name} 志愿";

            (this.DataContext as SetStudentPreselectionWindowModel).BindData(student);
        }
    }
}
