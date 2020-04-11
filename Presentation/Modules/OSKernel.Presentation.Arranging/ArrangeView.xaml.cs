using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.DataManager;
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

namespace OSKernel.Presentation.Arranging
{
    /// <summary>
    /// 排课界面
    /// </summary>
    public partial class ArrangeView : UserControl
    {
        /// <summary>
        /// 排课ViewModel
        /// </summary>
        public ArrangeViewModel VM
        {
            get
            {
                return this.DataContext as ArrangeViewModel;
            }
        }

        public ArrangeView()
        {
            InitializeComponent();
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<ArrangeViewModel>();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                TextBlock textBlock = sender as TextBlock;
                StackPanel sp = textBlock.Parent as StackPanel;

                textBlock.Visibility = Visibility.Collapsed;
                TextBox textBox = sp.Children[1] as TextBox;
                textBox.Visibility = Visibility.Visible;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            StackPanel sp = textBox.Parent as StackPanel;

            textBox.Visibility = Visibility.Collapsed;
            TextBlock textBlock = sp.Children[0] as TextBlock;
            textBlock.Visibility = Visibility.Visible;

            var changeName = textBox.Text;
            Case model = textBox.DataContext as Case;

            // 如果和自己本身一直则返回
            if (changeName.Equals(model.Name))
                return;

            var can = VM.CanReCaseName(changeName);
            if (can)
            {
                model.Name = changeName;
                model.Serialize();

                if (model.CaseType == Models.Enums.CaseTypeEnum.Administrative)
                {
                    var cp = VM.CommonDataManager.GetCPCase(model.LocalID);
                    cp.Name = changeName;
                    cp.Serialize(model.LocalID);
                }
                else if (model.CaseType == Models.Enums.CaseTypeEnum.Mixed)
                {
                    var cl = VM.CommonDataManager.GetCLCase(model.LocalID);
                    cl.Name = changeName;
                    cl.Serialize(model.LocalID);
                }
            }
            else
            {
                textBox.Text = model.Name;
            }
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            StackPanel sp = textBox.Parent as StackPanel;

            if (e.Key == Key.Enter)
            {
                textBox.Visibility = Visibility.Collapsed;
                TextBlock textBlock = sp.Children[0] as TextBlock;
                textBlock.Visibility = Visibility.Visible;
            }
        }

        private void TextBox_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.Focus();
            textBox.SelectAll();
        }
    }
}
