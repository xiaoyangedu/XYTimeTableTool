using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using OSKernel.Presentation.CustomControl.Enums;
using System.ComponentModel;

namespace OSKernel.Presentation.CustomControl
{
    /// <summary>
    /// DialogWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DialogWindow : INotifyPropertyChanged
    {
        private bool showIcon;

        private BitmapImage _iconSource;

        /// <summary>
        /// 窗体关闭返回类型
        /// </summary>
        public new DialogResultType DialogResult = DialogResultType.None;

        /// <summary>
        /// 选中状态
        /// </summary>
        public bool Checked;

        public event PropertyChangedEventHandler PropertyChanged;

        public void PropertyChangedNotify(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// 描述部分，主要信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 备注显示信息
        /// </summary>
        public string Mark { get; set; }

        /// <summary>
        /// 显示图标
        /// </summary>
        public bool ShowIcon
        {
            get
            {
                return showIcon;
            }

            set
            {
                showIcon = value;
                PropertyChangedNotify("ShowIcon");
            }
        }

        /// <summary>
        /// 图标数据源
        /// </summary>
        public BitmapImage IconSource
        {
            get
            {
                return _iconSource;
            }

            set
            {
                _iconSource = value;
                PropertyChangedNotify("IconSource");
            }
        }

        public DialogWindow()
        {
            InitializeComponent();
            init();
            this.Closed += DialogWindow_Closed;
        }

        public DialogWindow(Window owner)
        {
            InitializeComponent();
            if (owner != null && owner.IsLoaded)
                this.Owner = owner;
            else
                init();
        }

        private void DialogWindow_Closed(object sender, EventArgs e)
        {
            if (Owner != null && Owner.IsLoaded)
            {
                Owner.Activate();
            }
        }

        private void CloseCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            switch (btn.Name)
            {
                case "Btn_Ok":
                    DialogResult = DialogResultType.OK;
                    break;
                case "Btn_Cancel":
                    DialogResult = DialogResultType.Cancel;
                    break;
            }

            this.Checked = cb_checked.IsChecked.Value;
            this.Close();
        }

        System.Windows.Threading.DispatcherTimer _timer;

        private void init()
        {
            var mainwindow = Application.Current.MainWindow;
            if (mainwindow != null && mainwindow.IsLoaded)
                Owner = mainwindow;

            this.IconSource = (BitmapImage)FindResource("aler_success");

            _timer = new System.Windows.Threading.DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 显示一个窗口
        /// </summary>
        /// <returns></returns>
        public new DialogResultType ShowDialog()
        {
            base.ShowDialog();
            return DialogResult;
        }

        /// <summary>
        /// 显示同步对话框
        /// </summary>
        /// <param name="description">主要信息描述</param>
        /// <param name="mark">备注信息</param>
        /// <param name="SettingType">按钮显示类型</param>
        /// <param name="width">对话框宽度</param>
        /// <param name="height">对话框高度</param>
        /// <returns></returns>
        public DialogResultType ShowDialog(string Caption, string mark = "", bool showChecked = false, string checkString = "", DialogSettingType SettingType = DialogSettingType.OkAndCancel, DialogType WindowType = DialogType.Warning, double? width = null, double? height = null)
        {
            SettingWindow(Caption, mark, showChecked, checkString, SettingType, WindowType, width, height);
            return ShowDialog();
        }

        /// <summary>
        /// 设置对话框的展示信息，按钮显示，宽度高度
        /// </summary>
        /// <param name="description">主要信息描述</param>
        /// <param name="mark">备注信息</param>
        /// <param name="SettingType">按钮显示类型</param>
        /// <param name="width">对话框宽度</param>
        /// <param name="height">对话框高度</param>
        public void SettingWindow(string Caption, string mark, bool showChecked = false, string checkString = "", DialogSettingType SettingType = DialogSettingType.OkAndCancel, DialogType WindowType = DialogType.Warning, double? width = null, double? height = null)
        {
            this.Title = Caption;
            TxtB_Mark.Text = mark;
            if (width.HasValue)
                this.Width = (double)width;
            if (height.HasValue)
                this.Height = (double)height;
            SettingButtons(SettingType);

            if (showChecked)
            {
                cb_checked.Visibility = Visibility.Visible;
                cb_checked.Content = checkString;
            }

            if (WindowType == DialogType.None)
            {
                _timer.Start();
                this.ShowIcon = false;
            }
            else if (WindowType == DialogType.Error)
            {
                this.ShowIcon = true;
                this.IconSource = (BitmapImage)FindResource("alert_error");
            }
            else if (WindowType == DialogType.Warning)
            {
                this.ShowIcon = true;
                this.IconSource = (BitmapImage)FindResource("alert_warning");
            }
            else if (WindowType == DialogType.Success)
            {
                this.ShowIcon = true;
                this.IconSource = (BitmapImage)FindResource("aler_success");
            }
        }

        public void SettingWindow(DialogWindowSettings WindowSetting)
        {
            if (WindowSetting != null)
            {
                SettingWindow(WindowSetting.Title, WindowSetting.Message, WindowSetting.ShowChecked, WindowSetting.CheckBoxString, WindowSetting.SettingType, WindowSetting.WindowType, WindowSetting.Width, WindowSetting.Height);
                SettingButtonContent(WindowSetting.OkbtnContent, WindowSetting.CancelContent, WindowSetting.IgnoreContent);
                SettingButtonStyle(WindowSetting.Okstyle, WindowSetting.CancelStyle, WindowSetting.IgnoreStyle);
            }
        }

        /// <summary>
        /// 设置显示按钮
        /// </summary>
        /// <param name="SettingType"></param>
        public void SettingButtons(DialogSettingType SettingType)
        {
            if (SettingType == DialogSettingType.OkAndCancel)
            {
                return;
            }
            if (SettingType == DialogSettingType.NoButton)
            {
                Rows_btns.Height = new GridLength(0);
                return;
            }
            if (SettingType == DialogSettingType.OnlyOkButton)
            {
                Btn_Cancel.Visibility = Visibility.Collapsed;
                return;
            }
        }

        public new void Close()
        {
            base.Close();
        }

        public void SettingButtonContent(string okcontent = null, string cancelcontent = null, string ignorecontent = null)
        {
            if (!string.IsNullOrEmpty(okcontent))
            {
                Btn_Ok.Content = okcontent;
            }
            if (!string.IsNullOrEmpty(cancelcontent))
            {
                Btn_Cancel.Content = cancelcontent;
            }
            if (!string.IsNullOrEmpty(ignorecontent))
            {
                Btn_Cancel.Content = ignorecontent;
            }
        }

        public void SettingButtonStyle(Style OkStyle = null, Style CancelStyle = null, Style IgnoreStyle = null)
        {
            if (OkStyle != null)
            {
                Btn_Ok.Style = OkStyle;
            }
            if (CancelStyle != null)
            {
                Btn_Cancel.Style = CancelStyle;
            }
            if (IgnoreStyle != null)
            {
                Btn_Cancel.Style = IgnoreStyle;
            }
        }

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Escape))
            {
                DialogResult = DialogResultType.Cancel;
                e.Handled = true;
                this.Close();
            }
        }

        private void CopyCommandExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            StringBuilder str = new StringBuilder();
            str.Append(TxtB_Mark.Text);
            Clipboard.SetDataObject(str.ToString(), true);

        }
    }
}
