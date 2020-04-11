using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using OSKernel.Presentation.Utilities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment;
using VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment;

namespace OSKernel.Presentation.Utilities
{
    //NPOI方式
    //NPOI 是 POI 项目的 .NET 版本。POI是一个开源的Java读写Excel、WORD等微软OLE2组件文档的项目。使用 NPOI 你就可以在没有安装 Office 或者相应环境的机器上对 WORD/EXCEL 文档进行读写。
    //优点：读取Excel速度较快，读取方式操作灵活性
    //缺点：需要下载相应的插件并添加到系统引用当中。
    public class NPOIClass
    {
        /// <summary>
        /// 将excel导入到datatable
        /// </summary>
        /// <param name="filePath">excel路径</param>
        /// <param name="isColumnName">第一行是否是列名</param>
        /// <returns>返回datatable</returns>
        public static Tuple<DataTable, string> ExcelToDataTable(string filePath, bool isColumnName)
        {
            DataTable dataTable = null;
            FileStream fs = null;
            DataColumn column = null;
            DataRow dataRow = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            IRow row = null;
            ICell cell = null;
            int startRow = 0;
            try
            {
                using (fs = File.OpenRead(filePath))
                {
                    // 2007版本
                    if (filePath.IndexOf(".xlsx") > 0)
                        workbook = new XSSFWorkbook(fs);
                    // 2003版本
                    else if (filePath.IndexOf(".xls") > 0)
                        workbook = new HSSFWorkbook(fs);

                    if (workbook != null)
                    {
                        sheet = workbook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet
                        dataTable = new DataTable();
                        if (sheet != null)
                        {
                            int rowCount = sheet.LastRowNum;//总行数
                            if (rowCount > 0)
                            {
                                IRow firstRow = sheet.GetRow(0);//第一行
                                int cellCount = firstRow.LastCellNum;//列数

                                //构建datatable的列
                                if (isColumnName)
                                {
                                    startRow = 1;//如果第一行是列名，则从第二行开始读取
                                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                    {
                                        cell = firstRow.GetCell(i);
                                        if (cell != null)
                                        {
                                            if (cell.StringCellValue != null)
                                            {
                                                column = new DataColumn(cell.StringCellValue);
                                                dataTable.Columns.Add(column);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                    {
                                        column = new DataColumn("column" + (i + 1));
                                        dataTable.Columns.Add(column);
                                    }
                                }

                                //填充行
                                for (int i = startRow; i <= rowCount; ++i)
                                {
                                    row = sheet.GetRow(i);
                                    if (row == null) continue;

                                    dataRow = dataTable.NewRow();
                                    for (int j = row.FirstCellNum; j < cellCount; ++j)
                                    {
                                        cell = row.GetCell(j);
                                        if (cell == null)
                                        {
                                            dataRow[j] = "";
                                        }
                                        else
                                        {
                                            //CellType(Unknown = -1,Numeric = 0,String = 1,Formula = 2,Blank = 3,Boolean = 4,Error = 5,)
                                            switch (cell.CellType)
                                            {
                                                case CellType.Blank:
                                                    dataRow[j] = "";
                                                    break;
                                                case CellType.Numeric:
                                                    short format = cell.CellStyle.DataFormat;
                                                    //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理
                                                    if (format == 14 || format == 31 || format == 57 || format == 58)
                                                        dataRow[j] = cell.DateCellValue;
                                                    else
                                                        dataRow[j] = cell.NumericCellValue;
                                                    break;
                                                case CellType.String:
                                                    dataRow[j] = cell.StringCellValue;
                                                    break;
                                            }
                                        }
                                    }
                                    dataTable.Rows.Add(dataRow);
                                }
                            }
                        }
                    }
                }
                if (dataTable == null)
                {
                    return Tuple.Create(dataTable, "转换失败");
                }
                else
                {
                    return Tuple.Create(dataTable, string.Empty);
                }
            }
            catch (Exception ex)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                dataTable = null;
                return Tuple.Create(dataTable, ex.Message);
            }
        }

        /// <summary>
        /// 写入excel
        /// </summary>
        /// <param name="dt">datatable</param>
        /// <param name="strFile">strFile</param>
        /// <returns></returns>
        public static Tuple<bool, string> DataTableToExcel(DataTable dt, string strFile)
        {
            bool result = false;
            IWorkbook workbook = null;
            FileStream fs = null;
            IRow row = null;
            ISheet sheet = null;
            ICell cell = null;
            try
            {
                if (dt != null)
                {
                    workbook = new HSSFWorkbook();

                    var columnStyle = CreateNormalColumnCellStyle(workbook);
                    var rowStyle = CrateRowHeaderNormalCellStyle(workbook);

                    sheet = workbook.CreateSheet("Sheet0");//创建一个名称为Sheet0的表
                    int rowCount = dt.Rows.Count;//行数
                    int columnCount = dt.Columns.Count;//列数

                    //设置列头
                    row = sheet.CreateRow(0);//excel第一行设为列头
                    for (int c = 0; c < columnCount; c++)
                    {
                        cell = row.CreateCell(c);
                        cell.SetCellValue(dt.Columns[c].ColumnName);
                        cell.CellStyle = columnStyle;
                    }

                    //设置每行每列的单元格,
                    for (int i = 0; i < rowCount; i++)
                    {
                        row = sheet.CreateRow(i + 1);
                        for (int j = 0; j < columnCount; j++)
                        {
                            cell = row.CreateCell(j);//excel第二行开始写入数据
                            cell.SetCellValue(dt.Rows[i][j].ToString());
                            cell.CellStyle = rowStyle;
                        }
                    }
                    using (fs = File.OpenWrite(strFile))
                    {
                        workbook.Write(fs);//向打开的这个xls文件中写入数据
                        result = true;
                    }
                }
                return Tuple.Create(result, string.Empty);
            }
            catch (Exception ex)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                return Tuple.Create(result, ex.Message);
            }
        }

        /// <summary>
        /// Excel导入成Datable
        /// </summary>
        /// <param name="file">导入路径(包含文件名与扩展名)</param>
        /// <returns></returns>
        public static DataTable ExcelToTable(string file)
        {
            DataTable dt = new DataTable();
            IWorkbook workbook;
            string fileExt = Path.GetExtension(file).ToLower();
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                //XSSFWorkbook 适用XLSX格式，HSSFWorkbook 适用XLS格式
                if (fileExt == ".xlsx") { workbook = new XSSFWorkbook(fs); } else if (fileExt == ".xls") { workbook = new HSSFWorkbook(fs); } else { workbook = null; }
                if (workbook == null) { return null; }
                ISheet sheet = workbook.GetSheetAt(0);

                //表头  
                IRow header = sheet.GetRow(sheet.FirstRowNum);
                List<int> columns = new List<int>();
                for (int i = 0; i < header.LastCellNum; i++)
                {
                    object obj = GetValueType(header.GetCell(i));
                    if (obj == null || obj.ToString() == string.Empty)
                    {
                        dt.Columns.Add(new DataColumn("Columns" + i.ToString()));
                    }
                    else
                        dt.Columns.Add(new DataColumn(obj.ToString()));
                    columns.Add(i);
                }
                //数据  
                for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                {
                    DataRow dr = dt.NewRow();
                    bool hasValue = false;
                    foreach (int j in columns)
                    {
                        dr[j] = GetValueType(sheet.GetRow(i).GetCell(j));
                        if (dr[j] != null && dr[j].ToString() != string.Empty)
                        {
                            hasValue = true;
                        }
                    }
                    if (hasValue)
                    {
                        dt.Rows.Add(dr);
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// Datable导出成Excel
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="file">导出路径(包括文件名与扩展名)</param>
        public static void TableToExcel(DataTable dt, string file)
        {
            IWorkbook workbook;
            string fileExt = Path.GetExtension(file).ToLower();
            if (fileExt == ".xlsx") { workbook = new XSSFWorkbook(); } else if (fileExt == ".xls") { workbook = new HSSFWorkbook(); } else { workbook = null; }
            if (workbook == null) { return; }
            ISheet sheet = string.IsNullOrEmpty(dt.TableName) ? workbook.CreateSheet("Sheet1") : workbook.CreateSheet(dt.TableName);

            //表头  
            IRow row = sheet.CreateRow(0);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                ICell cell = row.CreateCell(i);
                cell.SetCellValue(dt.Columns[i].ColumnName);
            }

            //数据  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IRow row1 = sheet.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = row1.CreateCell(j);
                    cell.SetCellValue(dt.Rows[i][j].ToString());
                }
            }

            //转为字节数组  
            MemoryStream stream = new MemoryStream();
            workbook.Write(stream);
            var buf = stream.ToArray();

            //保存为Excel文件  
            using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                fs.Write(buf, 0, buf.Length);
                fs.Flush();
            }
        }

        /// <summary>
        /// 获取单元格类型
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private static object GetValueType(ICell cell)
        {
            if (cell == null)
                return null;
            switch (cell.CellType)
            {
                case CellType.Blank: //BLANK:  
                    return null;
                case CellType.Boolean: //BOOLEAN:  
                    return cell.BooleanCellValue;
                case CellType.Numeric: //NUMERIC:  
                    return cell.NumericCellValue;
                case CellType.String: //STRING:  
                    return cell.StringCellValue;
                case CellType.Error: //ERROR:  
                    return cell.ErrorCellValue;
                case CellType.Formula: //FORMULA:  
                default:
                    return "=" + cell.CellFormula;

            }
        }

        /// <summary>
        /// Excel转换成DataSet（.xlsx/.xls）
        /// </summary>
        /// <param name="filePath">Excel文件路径</param>
        /// <param name="strMsg"></param>
        /// <returns></returns>
        public static DataSet ExcelToDataSet(string filePath, out string strMsg)
        {
            strMsg = "";
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            string fileType = Path.GetExtension(filePath).ToLower();
            string fileName = Path.GetFileName(filePath).ToLower();
            try
            {
                ISheet sheet = null;
                int sheetNumber = 0;
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                if (fileType == ".xlsx")
                {
                    // 2007版本
                    XSSFWorkbook workbook = new XSSFWorkbook(fs);
                    sheetNumber = workbook.NumberOfSheets;
                    for (int i = 0; i < sheetNumber; i++)
                    {
                        string sheetName = workbook.GetSheetName(i);
                        sheet = workbook.GetSheet(sheetName);
                        if (sheet != null)
                        {
                            dt = GetSheetDataTable(sheet, out strMsg);
                            if (dt != null)
                            {
                                dt.TableName = sheetName.Trim();
                                ds.Tables.Add(dt);
                            }
                            else
                            {
                                MessageBox.Show("Sheet数据获取失败，原因：" + strMsg);
                            }
                        }
                    }
                }
                else if (fileType == ".xls")
                {
                    // 2003版本
                    HSSFWorkbook workbook = new HSSFWorkbook(fs);
                    sheetNumber = workbook.NumberOfSheets;
                    for (int i = 0; i < sheetNumber; i++)
                    {
                        string sheetName = workbook.GetSheetName(i);
                        sheet = workbook.GetSheet(sheetName);
                        if (sheet != null)
                        {
                            dt = GetSheetDataTable(sheet, out strMsg);
                            if (dt != null)
                            {
                                dt.TableName = sheetName.Trim();
                                ds.Tables.Add(dt);
                            }
                            else
                            {
                                MessageBox.Show("Sheet数据获取失败，原因：" + strMsg);
                            }
                        }
                    }
                }
                return ds;
            }
            catch (Exception ex)
            {
                strMsg = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 获取sheet表对应的DataTable
        /// </summary>
        /// <param name="sheet">Excel工作表</param>
        /// <param name="strMsg"></param>
        /// <returns></returns>
        private static DataTable GetSheetDataTable(ISheet sheet, out string strMsg)
        {
            strMsg = "";
            DataTable dt = new DataTable();
            string sheetName = sheet.SheetName;
            int startIndex = 0;// sheet.FirstRowNum;
            int lastIndex = sheet.LastRowNum;
            //最大列数
            int cellCount = 0;
            IRow maxRow = sheet.GetRow(0);
            for (int i = startIndex; i <= lastIndex; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row != null && cellCount < row.LastCellNum)
                {
                    cellCount = row.LastCellNum;
                    maxRow = row;
                }
            }
            //列名设置
            try
            {
                for (int i = 0; i < maxRow.LastCellNum; i++)
                {
                    dt.Columns.Add(Convert.ToChar(((int)'A') + i).ToString());
                }
            }
            catch
            {
                strMsg = "工作表" + sheetName + "中无数据";
                return null;
            }
            //数据填充
            for (int i = startIndex; i <= lastIndex; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow drNew = dt.NewRow();
                if (row != null)
                {
                    for (int j = row.FirstCellNum; j < row.LastCellNum; ++j)
                    {
                        if (row.GetCell(j) != null)
                        {
                            ICell cell = row.GetCell(j);
                            switch (cell.CellType)
                            {
                                case CellType.Blank:
                                    drNew[j] = "";
                                    break;
                                case CellType.Numeric:
                                    short format = cell.CellStyle.DataFormat;
                                    if (format == 14 || format == 31 || format == 57 || format == 58)
                                        drNew[j] = cell.DateCellValue;
                                    else
                                        drNew[j] = cell.NumericCellValue;
                                    if (cell.CellStyle.DataFormat == 177 || cell.CellStyle.DataFormat == 178 || cell.CellStyle.DataFormat == 188)
                                        drNew[j] = cell.NumericCellValue.ToString("#0.00");
                                    break;
                                case CellType.String:
                                    drNew[j] = cell.StringCellValue;
                                    break;
                                case CellType.Formula:
                                    try
                                    {
                                        drNew[j] = cell.NumericCellValue;
                                        if (cell.CellStyle.DataFormat == 177 || cell.CellStyle.DataFormat == 178 || cell.CellStyle.DataFormat == 188)
                                            drNew[j] = cell.NumericCellValue.ToString("#0.00");
                                    }
                                    catch
                                    {
                                        try
                                        {
                                            drNew[j] = cell.StringCellValue;
                                        }
                                        catch { }
                                    }
                                    break;
                                default:
                                    drNew[j] = cell.StringCellValue;
                                    break;
                            }
                        }
                    }
                }
                dt.Rows.Add(drNew);
            }
            return dt;
        }

        #region 导出课表

        public static Tuple<bool, string> DataTableToExcel(Dictionary<string, DataTable> dictionarys, string strFile, List<int> enableRow)
        {
            bool result = false;
            IWorkbook workbook = null;
            FileStream fs = null;
            IRow row = null;
            ICell cell = null;
            try
            {
                if (dictionarys != null)
                {
                    workbook = new HSSFWorkbook();

                    var columnStyle = CreateColumnCellStye(workbook);
                    var rowStyle = CreateRowCellStye(workbook);
                    var rowHeaderStyle = CreateRowHeaderCellStye(workbook);
                    var enableStyle = CreateRowEnableCellStye(workbook);

                    rowStyle.WrapText = true;

                    foreach (var dic in dictionarys)
                    {
                        var dt = dic.Value;

                        ISheet sheet = workbook.CreateSheet(dic.Key);

                        int rowCount = dt.Rows.Count;
                        int columnCount = dt.Columns.Count;

                        row = sheet.CreateRow(0);
                        row.Height = 60 * 15;

                        for (int c = 0; c < columnCount; c++)
                        {
                            sheet.SetColumnWidth(c, 200 * 25);

                            cell = row.CreateCell(c);                           
                            cell.SetCellValue(dt.Columns[c].ColumnName);
                            cell.CellStyle = columnStyle;
                        }

                        //设置每行每列的单元格,
                        for (int i = 0; i < rowCount; i++)
                        {
                            row = sheet.CreateRow(i + 1);
                            row.Height = 60 * 10;

                            short iCount = 1;
                            for (int j = 0; j < columnCount; j++)
                            {
                                cell = row.CreateCell(j);
                                cell.SetCellValue(dt.Rows[i][j].ToString());
                                
                                if (dt.Rows[i][j].ToString() != string.Empty)
                                {
                                    short iCurCount = (short)dt.Rows[i][j].ToString().Split('\n').Count();
                                    if (iCurCount > iCount) { iCount = iCurCount; }
                                }

                                if (j == 0)
                                {
                                    cell.CellStyle = rowHeaderStyle;
                                }
                                else
                                {
                                    var has = enableRow.Contains(i);
                                    if (has)
                                    {
                                        cell.CellStyle = enableStyle;
                                    }
                                    else
                                    {
                                        cell.CellStyle = rowStyle;
                                    }
                                }
                            }
                            row.Height = (short)(row.Height * ((iCount / 2) + 1) );
                        }
                    }

                    using (fs = File.OpenWrite(strFile))
                    {
                        workbook.Write(fs);
                        result = true;
                    }

                }
                return Tuple.Create(result, string.Empty);
            }
            catch (Exception ex)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                return Tuple.Create(result, ex.Message);
            }
        }


        public static Tuple<bool, string> DataTableToExcel(Dictionary<string, List<UIExcelExport>> dictionarys, string strFile, List<int> enableRow)
        {
            bool result = false;
            IWorkbook workbook = null;
            FileStream fs = null;
            try
            {
                if (dictionarys != null)
                {
                    workbook = new HSSFWorkbook();

                    #region 定义样式

                    var columnStyle = CreateColumnCellStye(workbook);
                    var rowStyle = CreateRowCellStye(workbook);
                    var rowHeaderStyle = CreateRowHeaderCellStye(workbook);
                    var enableStyle = CreateRowEnableCellStye(workbook);
                    rowStyle.WrapText = true;

                    #endregion

                    foreach (var dic in dictionarys)
                    {
                        ISheet sheet = workbook.CreateSheet(dic.Key);

                        int startIndex = 0;
                        dic.Value.ForEach(dt =>
                        {
                            FillSheet(dt, sheet, columnStyle, rowStyle, enableStyle, rowHeaderStyle, enableRow, startIndex);
                            startIndex = startIndex + dt.Table.Rows.Count + 2;
                        });
                    }

                    using (fs = File.OpenWrite(strFile))
                    {
                        workbook.Write(fs);
                        result = true;
                    }

                }
                return Tuple.Create(result, string.Empty);
            }
            catch (Exception ex)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                return Tuple.Create(result, ex.Message);
            }
        }

        static void FillSheet(UIExcelExport export, ISheet sheet, ICellStyle columnStyle, ICellStyle rowStyle, ICellStyle enableStyle, ICellStyle rowHeaderStyle, List<int> enableRow, int startIndex = 0)
        {
            var columnCount = export.Table.Columns.Count;
            var rowCount = export.Table.Rows.Count;
            var table = export.Table;

            #region 设置标题

            var title = sheet.CreateRow(startIndex);
            title.Height = 60 * 15;

            for (int c = 0; c < columnCount; c++)
            {
                sheet.SetColumnWidth(c, 200 * 25);
                var cell = title.CreateCell(c);
                cell.SetCellValue(export.Title);
                cell.CellStyle = columnStyle;
            }

            CellRangeAddress region = new CellRangeAddress(startIndex, startIndex, 0, columnCount - 1);
            sheet.AddMergedRegion(region);

            #endregion

            #region 设置头

            var row = sheet.CreateRow(startIndex + 1);
            row.Height = 60 * 15;

            for (int c = 0; c < columnCount; c++)
            {
                sheet.SetColumnWidth(c, 200 * 25);

                var cell = row.CreateCell(c);
                cell.SetCellValue(table.Columns[c].ColumnName);
                cell.CellStyle = columnStyle;
            }

            #endregion

            var newRowIndex = startIndex + 1;

            var newEnableRows = new List<int>();
            enableRow.ForEach(n =>
            {
                newEnableRows.Add(n + newRowIndex);
            });

            //设置每行每列的单元格,
            int tableRow = 0;
            for (int i = newRowIndex; i < newRowIndex + rowCount; i++)
            {
                row = sheet.CreateRow(i + 1);
                row.Height = 60 * 10;

                short iCount = 1;
                for (int j = 0; j < columnCount; j++)
                {
                    var cell = row.CreateCell(j);
                    cell.SetCellValue(table.Rows[tableRow][j].ToString());

                    if (table.Rows[tableRow][j].ToString() != string.Empty)
                    {
                        short iCurCount = (short)table.Rows[tableRow][j].ToString().Split('\n').Count();
                        if (iCurCount > iCount) { iCount = iCurCount; }
                    }

                    if (j == 0)
                    {
                        cell.CellStyle = rowHeaderStyle;
                    }
                    else
                    {
                        var has = newEnableRows.Contains(i);
                        if (has)
                        {
                            cell.CellStyle = enableStyle;
                        }
                        else
                        {
                            cell.CellStyle = rowStyle;
                        }
                    }
                }
                row.Height = (short)(row.Height * ((iCount / 2) + 1));

                tableRow += 1;
            }
        }

        #endregion

        static ICellStyle CreateColumnCellStye(IWorkbook workbook)
        {
            ICellStyle style = workbook.CreateCellStyle();

            //设置单元格的样式：水平对齐填充
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.WrapText = true;

            style.BorderTop = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.TopBorderColor = HSSFColor.Black.Index;
            style.BottomBorderColor = HSSFColor.Black.Index;
            style.LeftBorderColor = HSSFColor.Black.Index;
            style.RightBorderColor = HSSFColor.Black.Index;
            style.FillBackgroundColor = HSSFColor.SkyBlue.Index;

            //自动换行
            IFont font = workbook.CreateFont();
            font.FontName = "宋体";

            font.IsItalic = false;
            font.FontHeightInPoints = 15;
            font.Boldweight = (Int16)FontBoldWeight.Bold;
            style.SetFont(font);

            return style;
        }

        static ICellStyle CreateNormalColumnCellStyle(IWorkbook workbook)
        {
            ICellStyle style = workbook.CreateCellStyle();

            //设置单元格的样式：水平对齐填充
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.WrapText = true;

            style.BorderTop = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.TopBorderColor = HSSFColor.Black.Index;
            style.BottomBorderColor = HSSFColor.Black.Index;
            style.LeftBorderColor = HSSFColor.Black.Index;
            style.RightBorderColor = HSSFColor.Black.Index;
            style.FillBackgroundColor = HSSFColor.SkyBlue.Index;

            //自动换行
            IFont font = workbook.CreateFont();
            font.FontName = "宋体";
            font.IsItalic = false;
            font.Boldweight = (Int16)FontBoldWeight.Bold;
            style.SetFont(font);

            return style;
        }

        static ICellStyle CreateRowHeaderCellStye(IWorkbook workbook)
        {
            ICellStyle style = workbook.CreateCellStyle();

            //设置单元格的样式：水平对齐填充
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;

            style.BorderTop = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.TopBorderColor = HSSFColor.Black.Index;
            style.BottomBorderColor = HSSFColor.Black.Index;
            style.LeftBorderColor = HSSFColor.Black.Index;
            style.RightBorderColor = HSSFColor.Black.Index;
            style.WrapText = true;

            //自动换行
            IFont font = workbook.CreateFont();
            font.FontName = "宋体";

            font.IsItalic = false;
            font.FontHeightInPoints = 15;
            style.SetFont(font);

            return style;
        }

        static ICellStyle CrateRowHeaderNormalCellStyle(IWorkbook workbook)
        {
            ICellStyle style = workbook.CreateCellStyle();

            //设置单元格的样式：水平对齐填充
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;

            style.BorderTop = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.TopBorderColor = HSSFColor.Black.Index;
            style.BottomBorderColor = HSSFColor.Black.Index;
            style.LeftBorderColor = HSSFColor.Black.Index;
            style.RightBorderColor = HSSFColor.Black.Index;
            style.WrapText = true;

            //自动换行
            IFont font = workbook.CreateFont();
            font.FontName = "宋体";

            font.IsItalic = false;
            style.SetFont(font);

            return style;
        }

        static ICellStyle CreateRowCellStye(IWorkbook workbook)
        {
            ICellStyle style = workbook.CreateCellStyle();

            //设置单元格的样式：水平对齐填充
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;

            style.BorderTop = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.TopBorderColor = HSSFColor.Black.Index;
            style.BottomBorderColor = HSSFColor.Black.Index;
            style.LeftBorderColor = HSSFColor.Black.Index;
            style.RightBorderColor = HSSFColor.Black.Index;
            //style.WrapText = true;

            //自动换行
            IFont font = workbook.CreateFont();
            font.FontName = "宋体";

            font.IsItalic = false;
            font.FontHeightInPoints = 12;
            style.SetFont(font);

            return style;
        }

        static ICellStyle CreateRowEnableCellStye(IWorkbook workbook)
        {
            ICellStyle style = workbook.CreateCellStyle();

            //设置单元格的样式：水平对齐填充
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;

            style.BorderTop = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.TopBorderColor = HSSFColor.Black.Index;
            style.BottomBorderColor = HSSFColor.Black.Index;
            style.LeftBorderColor = HSSFColor.Black.Index;
            style.RightBorderColor = HSSFColor.Black.Index;

            style.FillForegroundColor = HSSFColor.Grey25Percent.Index;
            style.FillPattern = FillPattern.SolidForeground;

            return style;
        }
    }
}
