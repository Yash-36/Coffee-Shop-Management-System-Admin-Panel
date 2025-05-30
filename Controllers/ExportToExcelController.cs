using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using ClosedXML.Excel;

namespace Coffee_Shop.Controllers
{
    public class ExportToExcelController : Controller
    {
        private IConfiguration _configuration;
        public ExportToExcelController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #region ExportToExcel
        public IActionResult ExportToExcel(string table)
        {
            string str = _configuration.GetConnectionString("ConnectionString");
            SqlConnection conn = new SqlConnection(str);
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = $"pr_{table}_SelectAll";

            DataTable dt = new DataTable();
            SqlDataReader sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            if (dt.Rows.Count > 0)
            {
                dt.TableName = table;
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    var ws = workbook.Worksheets.Add(dt, table);
                    ws.Columns();
                    ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Style.Font.Bold = true;

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        workbook.SaveAs(memoryStream);
                        memoryStream.Position = 0;
                        return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{table}List.xlsx");
                    }
                }
            }
            return View();
        }
        #endregion
    }
}
