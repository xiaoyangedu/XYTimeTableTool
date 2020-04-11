using OSKernel.Presentation.Core;
using System;
using Unity;
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
namespace OSKernel.Presentation.Arranging.Dialog
{
    /// <summary>
    /// 选择教师窗口
    /// </summary>
    public partial class ChooseTeacherWindow
    {
        /// <summary>
        /// 选中教师
        /// </summary>
        public List<Models.Base.UITeacher> Teachers { get; set; }

        public ChooseTeacherWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<ChooseTeacherWindowModel>();
        }

        public ChooseTeacherWindow(List<Models.Base.UITeacher> current, List<Models.Base.UITeacher> all) : this()
        {
            (this.DataContext as ChooseTeacherWindowModel).BindData(current, all);
        }
    }
}
