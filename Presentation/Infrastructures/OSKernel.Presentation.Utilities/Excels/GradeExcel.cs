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

namespace OSKernel.Presentation.Utilities.Excels
{
    /// <summary>
    /// 年级课表
    /// </summary>
    public static class GradeExcel
    {
        public static Tuple<bool, string> Export(GradeExcelModel grade, string file)
        {

            #region  创建Workbook

            IWorkbook workbook = new HSSFWorkbook();

            #endregion

            // 创建列样式
            var columnCellStyle = CreateColumnCellStye(workbook);
            var headerStyle = CreateHeaderCellStye(workbook);

            #region 创建ISheet

            ISheet sheet = workbook.CreateSheet(grade.Header);
            sheet.SetColumnWidth(0, 200 * 25);

            var weekday = grade.Weeks.Count;
            var period = grade.Periods.Count;
            var columnCount = (weekday * period) + 1;

            // 设置标题
            var header = sheet.CreateRow(0);
            header.Height = 60 * 15;

            // 设置创建时间
            var timeRow = sheet.CreateRow(1);
            timeRow.Height = 30 * 15;

            for (int i = 0; i < columnCount; i++)
            {
                var headerCell = header.CreateCell(i);
                var timeCell = timeRow.CreateCell(i);
                if (i == 0)
                {
                    headerCell.SetCellValue(grade.Header);
                    timeCell.SetCellValue(grade.CreateDateString);
                }
                timeCell.CellStyle = columnCellStyle;
                headerCell.CellStyle = headerStyle;
            }

            CellRangeAddress region1 = new CellRangeAddress(0, 0, 0, columnCount - 1);
            sheet.AddMergedRegion(region1);

            CellRangeAddress region2 = new CellRangeAddress(1, 1, 0, columnCount - 1);
            sheet.AddMergedRegion(region2);


            // 创建星期
            var week = sheet.CreateRow(2);
            var periodRow = sheet.CreateRow(3);

            for (int i = 0; i < columnCount; i++)
            {
                var weekCell = week.CreateCell(i);
                var periodCell = periodRow.CreateCell(i);
                if (i == 0)
                {
                    weekCell.SetCellValue("星期");
                    periodCell.SetCellValue("节次");
                }
                weekCell.CellStyle = columnCellStyle;
                periodCell.CellStyle = columnCellStyle;
            }

            //  临时节次
            var tempPeriodList = new List<string>();

            // 星期
            for (int i = 1; i <= weekday; i++)
            {
                // 添加节次
                grade.Periods.ForEach(p =>
                {
                    tempPeriodList.Add(p);
                });

                if (i == 1)
                {
                    week.GetCell(i).SetCellValue(grade.Weeks[i - 1]);
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, i, period));
                }
                else
                {
                    var start = ((i - 1) * period) + 1;
                    week.GetCell(start).SetCellValue(grade.Weeks[i - 1]);
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, start, (period * i)));
                }
            }

            // 节次
            for (int i = 0; i < tempPeriodList.Count; i++)
            {
                // 从第二列开始获取
                var periodCell = periodRow.GetCell(i + 1);
                periodCell.SetCellValue(tempPeriodList[i]);
            }

            // 填充内容
            if (grade.ClassesDictionary != null)
            {
                // 行起始位置
                int rowStart = 4;

                // 班级字典
                if (grade.ClassesDictionary != null)
                {
                    foreach (var cd in grade.ClassesDictionary)
                    {
                        // 创建新数据
                        var newRow = sheet.CreateRow(rowStart);
                        var values = cd.Value;

                        // 创建单元格
                        var columnIndex = 0;

                        // 创建班级单元格
                        var classCell = newRow.CreateCell(columnIndex);
                        classCell.CellStyle = columnCellStyle;
                        classCell.SetCellValue(cd.Key);

                        for (int i = 0; i < values.Count; i++)
                        {
                            columnIndex += 1;
                            var newCell = newRow.CreateCell(columnIndex);
                            newCell.CellStyle = columnCellStyle;
                            newCell.SetCellValue(values[i]);
                        }

                        // 累加计算
                        rowStart += 1;
                    }
                }

                foreach (var cd in grade.ClassesDictionary)
                {
                    var className = cd.Key;

                }
            }

            #endregion

            using (var fs = File.OpenWrite(file))
            {
                workbook.Write(fs);
            }

            return Tuple.Create<bool, string>(true, "");
        }

        static ICellStyle CreateColumnCellStye(IWorkbook workbook)
        {
            ICellStyle style = workbook.CreateCellStyle();

            //设置单元格的样式：水平对齐填充
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.WrapText = true;
            style.VerticalAlignment = VerticalAlignment.Center;

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
            font.FontHeightInPoints = 11;
            style.SetFont(font);

            return style;
        }


        static ICellStyle CreateHeaderCellStye(IWorkbook workbook)
        {
            ICellStyle style = workbook.CreateCellStyle();

            //设置单元格的样式：水平对齐填充
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.WrapText = true;
            style.VerticalAlignment = VerticalAlignment.Center;

            //style.BorderTop = BorderStyle.Thin;
            //style.BorderBottom = BorderStyle.Thin;
            //style.BorderLeft = BorderStyle.Thin;
            //style.BorderRight = BorderStyle.Thin;
            //style.TopBorderColor = HSSFColor.Black.Index;
            //style.BottomBorderColor = HSSFColor.Black.Index;
            //style.LeftBorderColor = HSSFColor.Black.Index;
            //style.RightBorderColor = HSSFColor.Black.Index;
            //style.FillBackgroundColor = HSSFColor.SkyBlue.Index;

            //自动换行
            IFont font = workbook.CreateFont();
            font.FontName = "宋体";

            font.IsItalic = false;
            font.FontHeightInPoints = 20;
            font.IsBold = true;
            style.SetFont(font);

            return style;
        }
    }
}
