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

namespace OSKernel.Presentation.Login.Summary
{
    /// <summary>
    /// 用户详细信息
    /// </summary>
    public partial class UserInfoView : UserControl
    {
        public UserInfoView()
        {
            InitializeComponent();
            this.DataContext = new UserInfoViewModel();
        }
    }
}
