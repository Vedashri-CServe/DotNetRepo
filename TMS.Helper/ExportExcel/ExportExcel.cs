using ClosedXML.Excel;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using TMS.Helper.UtilityHelper;

namespace TMS.Helper.ExportExcel
{
    public static class ExportExcel
    {
        const int DEFAULT_COLUMN_WIDTH = 20;        // Set each column to be 20 wide
        public static byte[] GetExcelFileFormTable(DataTable data, string sheetName)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(sheetName);
            worksheet.Cell("A1").InsertTable(data);
            worksheet.Columns().AdjustToContents();
            var workbookBytes = new byte[0];
            using (var ms = new MemoryStream())
            {
                workbook.SaveAs(ms);
                workbookBytes = ms.ToArray();
            }
            return workbookBytes;
        }

        public static byte[] GetExcelFileFormDataSet(DataSet dataSet, bool hasDescription = false, IDictionary<string, string>? keyValuePairs = null, bool applyFormatting = false)
        {
            var workbook = new XLWorkbook();
            workbook.SetUse1904DateSystem(true);

            foreach (DataTable dt in dataSet.Tables)
            {
                var worksheet = workbook.Worksheets.Add(dt.TableName);
                if (hasDescription)
                {
                    if (keyValuePairs is null)
                        throw new ArgumentNullException(nameof(keyValuePairs), "description cannot be null");

                    int index = 1;
                    foreach (KeyValuePair<string, string> kvp in keyValuePairs)
                    {
                        worksheet.Cell(index, 1).Value = kvp.Key;
                        worksheet.Cell(index, 2).Value = kvp.Value;
                        index++;
                    }
                }
                var startCell = hasDescription ? $"A{keyValuePairs!.Count + 3}" : "A1";
                worksheet.Cell(startCell).InsertTable(dt);

                var excelRange = worksheet.Range($"{startCell}:{worksheet.LastCellUsed()}");
                excelRange.AddToNamed("DataColumns", XLScope.Worksheet);

                worksheet = AddExcelFormatting(workbook, dt.TableName, excelRange, applyFormatting);

                worksheet.Columns().AdjustToContents();
            }

            var workbookBytes = new byte[0];
            using (var ms = new MemoryStream())
            {
                workbook.SaveAs(ms);
                workbookBytes = ms.ToArray();
            }
            return workbookBytes;
        }

        public static IXLWorksheet AddExcelFormatting(XLWorkbook xLWorkbook, string WorkSheetName, IXLRange range, bool applyFormatting = false)
        {
            IXLWorksheet worksheet = xLWorkbook.Worksheet(WorkSheetName);
            
            if (applyFormatting)
            {
                var DateColumnsRange = worksheet.Range($"L6:{range.LastColumn().LastCell()}");
                var headerRange = worksheet.Range($"L5:{range.LastColumn().LastCell()}");

                foreach (var xlCell in headerRange.Cells())
                {
                    var resolvedDate = ConvertDateTimeString(xlCell.Value.GetText());

                    if (!string.IsNullOrEmpty(resolvedDate))
                    {
                        var col = xlCell.WorksheetColumn().ColumnLetter();
                        var row = DateColumnsRange.LastRow().RowNumber();

                        if (Convert.ToDateTime(resolvedDate).DayOfWeek == DayOfWeek.Saturday || Convert.ToDateTime(resolvedDate).DayOfWeek == DayOfWeek.Sunday)
                            worksheet.Range(xlCell.CellBelow(), worksheet.Cell(row, col)).Style.Fill.SetBackgroundColor(XLColor.LightGray);
                    }

                    
                    if (xlCell.WorksheetRow().RowNumber() > 5)
                    {
                        var dateRow = 5;
                        var datecol = xlCell.WorksheetColumn().ColumnLetter();

                        resolvedDate = ConvertDateTimeString(worksheet.Cell(dateRow, datecol).Value.GetText());

                        TimeSpan halfHourTime = new TimeSpan(4, 30, 0);
                        TimeSpan overTime = new TimeSpan(8, 45, 0);
                        TimeSpan absentTime = new TimeSpan(0, 0, 0);
                        var timeValue = string.IsNullOrEmpty(xlCell.GetValue<string?>()) ? "00:00:00_A" : xlCell.GetValue<string?>();
                        var timeCellValue = timeValue!.Split('_').First().GetTimeSpan();
                        var timeCellFormat = timeValue!.Split('_').Last();

                        // for null values but not in weekend
                        if (timeCellValue != null && timeCellValue == absentTime
                           && !(Convert.ToDateTime(resolvedDate).DayOfWeek == DayOfWeek.Saturday || Convert.ToDateTime(resolvedDate).DayOfWeek == DayOfWeek.Sunday))
                        {
                            xlCell.Style.Fill.SetBackgroundColor(XLColor.Red);
                            xlCell.Value = "A";
                            xlCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        }

                        // for half day
                        else if (timeCellValue <= halfHourTime && timeCellFormat == "H")
                        {
                            xlCell.Style.Fill.SetBackgroundColor(XLColor.Yellow);
                            xlCell.Value = timeCellValue;
                        }
                        // for overtime
                        else if (timeCellValue >= overTime && timeCellFormat == "O")
                        {
                            xlCell.Style.Fill.SetBackgroundColor(XLColor.LightPastelPurple);
                            xlCell.Value = timeCellValue;
                        }
                        //for present
                        else if (timeCellValue != null && timeCellFormat == "P")
                            xlCell.Value = timeCellValue;
                        // for weekend absent values
                        else if (timeCellValue != null && timeCellValue == absentTime
                           && (Convert.ToDateTime(resolvedDate).DayOfWeek == DayOfWeek.Saturday || Convert.ToDateTime(resolvedDate).DayOfWeek == DayOfWeek.Sunday))
                        {
                            xlCell.Value = string.Empty;
                        }
                     
                    }
                }   
            }

            worksheet.Columns().AdjustToContents();
            return worksheet;
        }

        public static string? ConvertDateTimeString(string dateTime)
        {
            if (!string.IsNullOrEmpty(dateTime))
            {
                var dateTimeArray = dateTime.Split("/");
                if (dateTimeArray.Length > 1)
                {
                    DateTime resolvedDate = new DateTime(Convert.ToInt32(dateTimeArray[2]), Convert.ToInt32(dateTimeArray[1]), Convert.ToInt32(dateTimeArray[0]));
                    return resolvedDate.ToString("MM/dd/yyyy");
                }
                return null;
            }
            else { return null; }
        }
    }

    public static class DataConvert
    {
        public static DataTable ListToDataTable<T>(this List<T> list, string tableName = "")
        {
            DataTable dt = new DataTable();
            dt.TableName = tableName;
            foreach (PropertyInfo info in typeof(T).GetProperties())
            {
                var displayName = ((DisplayNameAttribute)info.GetCustomAttribute(typeof(DisplayNameAttribute)))?.DisplayName;
                dt.Columns.Add(new DataColumn(string.IsNullOrEmpty(displayName) ? info.Name : displayName, GetNullableType(info.PropertyType)));
            }
            foreach (T t in list)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo info in typeof(T).GetProperties())
                {
                    var colName = info.Name;
                    var displayName = ((DisplayNameAttribute)info.GetCustomAttribute(typeof(DisplayNameAttribute)))?.DisplayName;
                    if (!string.IsNullOrEmpty(displayName))
                    {
                        colName = displayName;
                    }

                    if (!IsNullableType(info.PropertyType))
                        row[colName] = info.GetValue(t, null);
                    else
                        row[colName] = (info.GetValue(t, null) ?? DBNull.Value);
                }
                dt.Rows.Add(row);
            }
            return dt;
        }
        public static Type GetNullableType(Type t)
        {
            Type returnType = t;
            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                returnType = Nullable.GetUnderlyingType(t);
            }
            return returnType;
        }
        private static bool IsNullableType(Type type)
        {
            return (type == typeof(string) ||
                    type.IsArray ||
                    (type.IsGenericType &&
                     type.GetGenericTypeDefinition().Equals(typeof(Nullable<>))));
        }
    }
}
