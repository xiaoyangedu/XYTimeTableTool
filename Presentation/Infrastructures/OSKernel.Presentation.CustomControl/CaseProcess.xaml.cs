using OSKernel.Presentation.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OSKernel.Presentation.CustomControl
{
    /// <summary>
    /// 方案进度
    /// </summary>
    public partial class CaseProcess : UserControl
    {
        public CaseProcess()
        {
            InitializeComponent();
        }

        private void cb_control_MouseMove(object sender, MouseEventArgs e)
        {
            CheckBox cb = sender as CheckBox;

            Case model = cb.DataContext as Case;

            Grid start = (Grid)cb_control.Template.FindName("CIRCLE___PLAY", cb_control);

            Grid stop = (Grid)cb_control.Template.FindName("CIRCLE___STOP", cb_control);

            if (cb.IsChecked == false)
            {
                stop.Visibility = Visibility.Hidden;
                start.Visibility = Visibility.Visible;
                return;
            }

            if (model.Task != null)
            {
                if (model.Task.TaskStatus == Models.Enums.MissionStateEnum.Completed
                    || model.Task.TaskStatus == Models.Enums.MissionStateEnum.Aborted
                    || model.Task.TaskStatus == Models.Enums.MissionStateEnum.Failed)
                {
                    start.Visibility = Visibility.Visible;
                    return;
                }
            }

            stop.Visibility = Visibility.Visible;
        }

        private void cb_control_MouseLeave(object sender, MouseEventArgs e)
        {
            CheckBox cb = sender as CheckBox;

            Case model = cb.DataContext as Case;

            Grid stop = (Grid)cb_control.Template.FindName("CIRCLE___STOP", cb_control);
            Grid start = (Grid)cb_control.Template.FindName("CIRCLE___PLAY", cb_control);

            stop.Visibility = Visibility.Hidden;
            start.Visibility = Visibility.Hidden;

            if (cb.IsChecked == false)
            {
                stop.Visibility = Visibility.Hidden;
                start.Visibility = Visibility.Visible;
            }
        }

        private void cb_control_Unchecked(object sender, RoutedEventArgs e)
        {
            Grid start = (Grid)cb_control.Template.FindName("CIRCLE___PLAY", cb_control);
            if (start.Visibility != Visibility.Visible)
            {
                start.Visibility = Visibility.Visible;
            }
        }
    }
}
