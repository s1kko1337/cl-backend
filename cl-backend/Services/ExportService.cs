using CsvHelper;
using CsvHelper.Configuration;
using OfficeOpenXml;
using System.Globalization;
using System.Text;

namespace cl_backend.Services
{
    public interface IExportService
    {
        byte[] ExportToCsv<T>(IEnumerable<T> data);
        byte[] ExportToExcel<T>(IEnumerable<T> data, string sheetName);
    }

    public class ExportService : IExportService
    {
        public ExportService()
        {
            // Устанавливаем лицензию для EPPlus (некоммерческое использование)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public byte[] ExportToCsv<T>(IEnumerable<T> data)
        {
            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);
            using var csvWriter = new CsvWriter(streamWriter, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                Encoding = Encoding.UTF8
            });

            csvWriter.WriteRecords(data);
            streamWriter.Flush();

            return memoryStream.ToArray();
        }

        public byte[] ExportToExcel<T>(IEnumerable<T> data, string sheetName)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add(sheetName);

            var dataList = data.ToList();
            if (dataList.Any())
            {
                // Загружаем данные в лист
                worksheet.Cells["A1"].LoadFromCollection(dataList, true);

                // Форматируем заголовки
                using (var range = worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Автоподгонка ширины колонок
                worksheet.Cells.AutoFitColumns();
            }

            return package.GetAsByteArray();
        }
    }
}
