using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.ViewModel;
using OSKernel.Presentation.CustomControl;
using OSKernel.Presentation.Models.Base;
using OSKernel.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;

namespace OSKernel.Presentation.Arranging.Mixed.Dialog
{
    public class ImportStudentSelectionWindowModel : CommonViewModel, IInitilize
    {
        public ICommand UploadFileCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<object>(uploadFile);
            }
        }

        public ICommand CreateTemplateCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(createTemplate);
            }
        }

        public ImportStudentSelectionWindowModel()
        {

        }

        [InjectionMethod]
        public void Initilize()
        {

        }

        void uploadFile(object obj)
        {
            ImportStudentSelectionWindow window = obj as ImportStudentSelectionWindow;

            // 上传文件
            System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();
            //openFileDialog1.InitialDirectory = "c:\\";
            open.Filter = "Microsoft Excel files(*.xls)|*.xls;*.xlsx";
            open.FilterIndex = 1;
            open.RestoreDirectory = true;
            open.AddExtension = true;
            open.CheckFileExists = true;
            open.CheckPathExists = true;
            open.ShowHelp = true;

            if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var excelResult = NPOIClass.ExcelToDataTable(open.FileName, false);
                if (excelResult.Item1 == null)
                {
                    this.ShowDialog("提示信息", excelResult.Item2, CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Error);
                    return;
                }
                else
                {
                    window.DT = excelResult.Item1;
                    window.DialogResult = true;
                }
            }
        }

        void createTemplate()
        {
            // 创建模板
            var cl = CommonDataManager.GetCLCase(base.LocalID);

            System.Windows.Forms.SaveFileDialog saveDialog = new System.Windows.Forms.SaveFileDialog();
            saveDialog.Filter = "Microsoft Excel files(*.xls)|*.xls;*.xlsx";
            saveDialog.FileName = $"走班导入学生选课模板.xls";

            var result = saveDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // 获取志愿
                //var preselections = (from c in cl.Courses
                //                     from cc in c.Levels
                //                     select new UIPreselection()
                //                     {
                //                         CourseID = c.ID,
                //                         Course = c.Name,
                //                         LevelID = cc.ID,
                //                         Level = cc.Name,
                //                     })?.ToList();

                // 添加列
                System.Data.DataTable dt = new System.Data.DataTable();
                dt.Columns.Add("姓名", Type.GetType("System.String"));

                foreach (var c in cl.Courses)
                {
                    dt.Columns.Add(c.Name, Type.GetType("System.String"));
                }

                foreach (var s in cl.Students)
                {
                    System.Data.DataRow newRow = dt.NewRow();
                    newRow["姓名"] = s.Name;
                    dt.Rows.Add(newRow);
                }

                var excelResult = NPOIClass.DataTableToExcel(dt, saveDialog.FileName);
                if (excelResult.Item1)
                {
                    this.ShowDialog("提示信息", "生成模板成功!", CustomControl.Enums.DialogSettingType.NoButton, CustomControl.Enums.DialogType.None);
                }
                else
                {
                    this.ShowDialog("提示信息", $"生成模板失败-{excelResult.Item2}!", CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Error);
                }

            }
        }
    }
}
