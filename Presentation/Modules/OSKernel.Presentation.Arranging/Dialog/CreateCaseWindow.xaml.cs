using OSKernel.Presentation.Models.Enums;
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

namespace OSKernel.Presentation.Arranging.Dialog
{
    /// <summary>
    /// Interaction logic for CreateCaseWindow.xaml
    /// </summary>
    public partial class CreateCaseWindow
    {
        #region 保存结果

        /// <summary>
        /// 唯一ID
        /// </summary>
        public string GuidID { get; set; }

        /// <summary>
        /// 方案名称
        /// </summary>
        public string CaseName { get; set; }

        /// <summary>
        /// 可用教室数
        /// </summary>
        //public int LimitRoom { get; set; }

        /// <summary>
        /// 方案类型
        /// </summary>
        public CaseTypeEnum CaseType { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 是否自动
        /// </summary>
        public bool IsAuto { get; set; }

        #endregion

        public CreateCaseWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.txt_case.Focus();
        }

        private void Btn_save_Click(object sender, RoutedEventArgs e)
        {
            this.GuidID = Guid.NewGuid().ToString();
            this.CaseName = txt_case.Text;
            this.CreateDate = DateTime.Now;

            this.IsAuto = false;
            this.DialogResult = true;
        }

        private void Btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.CaseType = CaseTypeEnum.Administrative;
        }

        private void RadioButton_Click_1(object sender, RoutedEventArgs e)
        {
            this.CaseType = CaseTypeEnum.Mixed;
        }

        private void RadioButton_Click_2(object sender, RoutedEventArgs e)
        {
            this.CaseType = CaseTypeEnum.Mixed;
        }
    }
}
