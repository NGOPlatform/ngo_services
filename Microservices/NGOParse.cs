using ExcelDataReader;
using System.Text;

namespace Microservices
{
    internal class NGOParse
    {
        // Parser din XLSX in CSV 
        public static void SaveAsCsv(string excelFilePath, string destinationCsvFilePath)
        {
            Console.WriteLine("Starting file " + excelFilePath + "...");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using var stream = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            IExcelDataReader? reader = null;

            if (excelFilePath.EndsWith(".xlsx"))
                reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            if (reader == null)
                throw new Exception("Fisierul nu este de tip Excel!");

            var ds = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = false
                }
            });

            StreamWriter csv = new(destinationCsvFilePath, false);
            var csvContent = string.Empty;
            int row_no = 0;
            while (row_no < ds.Tables[0].Rows.Count)
            {
                var arr = new List<string?>();
                for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                {
                    if (ds.Tables[0].Rows[row_no][i] == null)
                        arr.Add("");
                    else
                        arr.Add(ds.Tables[0].Rows[row_no][i].ToString());
                }
                row_no++;
                csvContent += string.Join("ඞ", arr) + "\n";
                csv.Write(csvContent);
                csvContent = string.Empty;

                Console.WriteLine("Parsing row " + row_no.ToString());
            }

            csv.Close();
        }
    }
}