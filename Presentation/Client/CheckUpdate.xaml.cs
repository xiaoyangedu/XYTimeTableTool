using System.Windows;
using System.Windows.Input;

namespace Client
{
    /// <summary>
    /// 检查更新
    /// </summary>
    public partial class CheckUpdate : Window
    {
        public CheckUpdate()
        {
            InitializeComponent();
            this.DataContext = new CheckUpdateModel();
            this.MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;
        }

        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
