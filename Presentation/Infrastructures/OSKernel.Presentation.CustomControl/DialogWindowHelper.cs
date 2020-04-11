using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using OSKernel.Presentation.CustomControl.Enums;

namespace OSKernel.Presentation.CustomControl
{
    public static class DialogWindowHelper
    {
        /// <summary>
        /// 显示同步对话框
        /// </summary>
        /// <param name="description">主要信息描述</param>
        /// <param name="mark">备注信息</param>
        /// <param name="SettingType">按钮显示类型</param>
        /// <param name="width">对话框宽度</param>
        /// <param name="height">对话框高度</param>
        public static DialogResultType ShowDialog(string Caption, string mark = "", DialogSettingType SettingType = DialogSettingType.OkAndCancel, DialogType WindowType = DialogType.Warning, double? width = null, double? height = null, bool showChecked = false, string checkedString = "")
        {
            DialogResultType result = DialogResultType.None;
            DispatcherHelper.UIDispatcher.Invoke(() =>
            {
                DialogWindow dialogwin = new DialogWindow();
                result = dialogwin.ShowDialog(Caption, mark, showChecked, checkedString, SettingType, WindowType, width, height);
            });
            return result;
        }

        /// <summary>
        /// 显示同步对话框
        /// </summary>
        /// <param name="Owner">对话框所属窗体,默认是MainWindow</param>
        /// <param name="description">主要信息描述</param>
        /// <param name="mark">备注信息</param>
        /// <param name="SettingType">按钮显示类型</param>
        /// <param name="width">对话框宽度</param>
        /// <param name="height">对话框高度</param>
        public static DialogResultType ShowDialog(this Window Owner, string Caption, string mark = "", DialogSettingType SettingType = DialogSettingType.OkAndCancel, DialogType WindowType = DialogType.Warning, double? width = null, double? height = null, bool showChecked = false, string checkedString = "")
        {
            DialogResultType result = DialogResultType.None;
            DispatcherHelper.UIDispatcher.Invoke(() =>
            {
                DialogWindow dialogwin = new DialogWindow(Owner);
                result = dialogwin.ShowDialog(Caption, mark, showChecked, checkedString, SettingType, WindowType, width, height);
            });
            return result;
        }

        /// <summary>
        /// 显示同步对话框
        /// </summary>
        /// <param name="description">主要信息描述</param>
        /// <param name="mark">备注信息</param>
        /// <param name="SettingType">按钮显示类型</param>
        /// <param name="width">对话框宽度</param>
        /// <param name="height">对话框高度</param>
        public static DialogResultType ShowDialog(this ViewModelBase ViewModel, string Caption, string mark = "", DialogSettingType SettingType = DialogSettingType.OkAndCancel, DialogType WindowType = DialogType.Warning, double? width = null, double? height = null, bool showChecked = false, string checkedString = "")
        {
            DialogResultType result = DialogResultType.None;
            DispatcherHelper.UIDispatcher.Invoke(() =>
            {
                DialogWindow dialogwin = new DialogWindow();
                dialogwin.Tag = ViewModel;
                result = dialogwin.ShowDialog(Caption, mark, showChecked, checkedString, SettingType, WindowType, width, height);
            });
            return result;
        }

        public static DialogResultType ShowDialogConfirm(this ViewModelBase ViewModel, out bool select, string Caption, string selectstring = "", string mark = "", DialogSettingType SettingType = DialogSettingType.OkAndCancel, DialogType WindowType = DialogType.Warning, double? width = null, double? height = null)
        {
            DialogResultType result = DialogResultType.None;

            bool selected = false;
            DispatcherHelper.UIDispatcher.Invoke(() =>
            {
                DialogWindow dialogwin = new DialogWindow();
                dialogwin.Tag = ViewModel;
                result = dialogwin.ShowDialog(Caption, mark, true, selectstring, SettingType, WindowType, width, height);

                selected = dialogwin.Checked;
            });
            select = selected;
            return result;
        }

        public static DialogResultType ShowDialog(DialogWindowSettings Settings)
        {
            DialogResultType result = DialogResultType.None;
            DispatcherHelper.UIDispatcher.Invoke(() =>
            {
                DialogWindow dialogwin = new DialogWindow();
                dialogwin.SettingWindow(Settings);
                result = dialogwin.ShowDialog();
            });
            return result;
        }
        public static DialogResultType ShowDialog(this ViewModelBase ViewModel, DialogWindowSettings Settings)
        {
            DialogResultType result = DialogResultType.None;
            DispatcherHelper.UIDispatcher.Invoke(() =>
            {
                DialogWindow dialogwin = new DialogWindow();
                dialogwin.SettingWindow(Settings);
                result = dialogwin.ShowDialog();
            });
            return result;
        }
        public static DialogResultType ShowDialog(this Window Owner, DialogWindowSettings Settings)
        {
            DialogResultType result = DialogResultType.None;
            DispatcherHelper.UIDispatcher.Invoke(() =>
            {
                DialogWindow dialogwin = new DialogWindow(Owner);
                dialogwin.SettingWindow(Settings);
                result = dialogwin.ShowDialog();
            });
            return result;
        }

        public static DialogResultType ShowDialog(this UserControl userControl, string Caption, string mark = "", DialogSettingType SettingType = DialogSettingType.OkAndCancel, DialogType WindowType = DialogType.Warning, double width = 300, double? height = null, bool showChecked = false, string checkedString = "")
        {
            DialogResultType result = DialogResultType.None;
            DispatcherHelper.UIDispatcher.Invoke(() =>
            {
                DialogWindow dialogwin = new DialogWindow();
                dialogwin.Tag = userControl;
                result = dialogwin.ShowDialog(Caption, mark, showChecked, checkedString, SettingType, WindowType, width, height);
            });
            return result;
        }
    }
}
