using OSKernel.Presentation.Core;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Utilities;
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
    /// <summary>
    /// 导入学生志愿
    /// </summary>
    public partial class ImportStudentSelectionWindow
    {
        /// <summary>
        /// 数据表格
        /// </summary>
        public System.Data.DataTable DT { get; set; }

        public ImportStudentSelectionWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.DataContext = CacheManager.Instance.UnityContainer.Resolve<ImportStudentSelectionWindowModel>();
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            string[] file = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (file.Length > 0)
            {
                var dropFile = file[0];
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(dropFile);
                if (fileInfo.Extension.Equals(".xls") || fileInfo.Extension.Equals(".xlsx"))
                {
                    var excelResult = NPOIClass.ExcelToDataTable(dropFile, false);
                    if (excelResult.Item1 == null)
                    {
                        this.ShowDialog("提示信息", excelResult.Item2, CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Error);
                        return;
                    }
                    else
                    {
                        this.DT = excelResult.Item1;
                        this.DialogResult = true;
                    }
                }
                else
                {
                    this.ShowDialog("提示信息", "支支持xls,xlsx 类型文件!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Error);
                    return;
                }
            }
        }
    }
}
