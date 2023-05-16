using CsvHelper;
using Infrastructure.Files.Maps;
using OfficeOpenXml;
using System.Globalization;
using System.IO;

namespace Infrastructure.Files
{
    public class FileBuilder : IFileBuilder
    {
        public byte[] BuildCSVFile<T>(IEnumerable<T> source, string exportDataType)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream);

            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                switch (exportDataType)
                {
                    case "history":
                        csv.Context.RegisterClassMap<EvaluationHistoryClassMap>();

                        break;
                    case "evaluation":
                        csv.Context.RegisterClassMap<EvaluationClassMap>();
                        break;
                    case "evaluationsTypeQualityRate":
                        csv.Context.RegisterClassMap<EvaluationTypeQualityRateClassMap>();
                        break;
                    case "agentsEvaluationsQualityRate":
                        csv.Context.RegisterClassMap<AgentEvaluationQualityRateClassMap>();
                        break; 
                    case "generalQualityRate":
                        csv.Context.RegisterClassMap<GeneralQualityRateClassMap>();
                        break;
                    default: ;
                        break;
                }

                csv.WriteRecords(source);
            }

            var bytes = memoryStream.ToArray();
            return bytes;
        }

        public byte[] BuildExcelFile(DataTable dataTable)
        {
            using (ExcelPackage excelPackage = new())
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");

                var tableBody = worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);
               
                worksheet.Cells.AutoFitColumns();
                return excelPackage.GetAsByteArray();
            }
        }
    }
}
