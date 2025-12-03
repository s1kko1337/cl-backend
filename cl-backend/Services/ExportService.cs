using CsvHelper;
using CsvHelper.Configuration;
using OfficeOpenXml;
using System.Globalization;
using System.Text;

namespace cl_backend.Services
{
    /// <summary>
    /// Интерфейс сервиса экспорта данных
    /// </summary>
    public interface IExportService
    {
        /// <summary>
        /// Экспортирует данные в формат CSV
        /// </summary>
        /// <typeparam name="T">Тип экспортируемых данных</typeparam>
        /// <param name="data">Коллекция данных для экспорта</param>
        /// <returns>Массив байтов CSV-файла</returns>
        byte[] ExportToCsv<T>(IEnumerable<T> data);

        /// <summary>
        /// Экспортирует данные в формат Excel
        /// </summary>
        /// <typeparam name="T">Тип экспортируемых данных</typeparam>
        /// <param name="data">Коллекция данных для экспорта</param>
        /// <param name="sheetName">Название листа в Excel</param>
        /// <returns>Массив байтов Excel-файла</returns>
        byte[] ExportToExcel<T>(IEnumerable<T> data, string sheetName);
    }

    /// <summary>
    /// Сервис экспорта данных в CSV и Excel форматы
    /// </summary>
    public class ExportService : IExportService
    {
        /// <summary>
        /// Экспортирует данные в формат CSV
        /// </summary>
        /// <typeparam name="T">Тип экспортируемых данных</typeparam>
        /// <param name="data">Коллекция данных для экспорта</param>
        /// <returns>Массив байтов CSV-файла с кодировкой UTF-8</returns>
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

        /// <summary>
        /// Экспортирует данные в формат Excel с форматированием
        /// </summary>
        /// <typeparam name="T">Тип экспортируемых данных</typeparam>
        /// <param name="data">Коллекция данных для экспорта</param>
        /// <param name="sheetName">Название листа в Excel</param>
        /// <returns>Массив байтов Excel-файла с отформатированными заголовками</returns>
        public byte[] ExportToExcel<T>(IEnumerable<T> data, string sheetName)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add(sheetName);

            var dataList = data.ToList();
            if (dataList.Any())
            {
                worksheet.Cells["A1"].LoadFromCollection(dataList, true);

                using (var range = worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                worksheet.Cells.AutoFitColumns();
            }

            return package.GetAsByteArray();
        }
    }
}
