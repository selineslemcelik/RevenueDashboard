using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RevenueDashboard.Models;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Npgsql;
using Microsoft.AspNetCore.Authorization;

namespace RevenueDashboard.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    //[Authorize(Roles = "Admin")]
    public IActionResult UploadExcel()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult UploadExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return Json(new { success = false, message = "Dosya seçilmedi!" });

        using var stream = file.OpenReadStream();
        using var workbook = new XLWorkbook(stream);

        var worksheet = workbook.Worksheet(1);

        using var conn = new NpgsqlConnection(
            "Host=localhost;Port=5432;Database=revenuedb;Username=selineslemcelik"
        );

        conn.Open();

        int rowCount = worksheet.LastRowUsed().RowNumber();
        int inserted = 0;

        for (int row = 2; row <= rowCount; row++)
        {
            string dateText = worksheet.Cell(row, 1).GetString();

            if (!DateTime.TryParse(dateText, out DateTime date))
            {
                return Json(new { success = false, message = $"Tarih okunamadı. Satır: {row} Değer: {dateText}" });
            }

            string company = worksheet.Cell(row, 2).GetString();
            string channel = worksheet.Cell(row, 3).GetString();
            decimal revenue = worksheet.Cell(row, 4).GetValue<decimal>();
            string platform = worksheet.Cell(row, 5).GetString();
            string country = worksheet.Cell(row, 6).GetString();

            using var cmd = new NpgsqlCommand(
                @"INSERT INTO revenues
                    (date, channel_name, company, revenue, platform, country)
                    VALUES
                    (@date, @channel, @company, @revenue, @platform, @country)",
                conn);

            cmd.Parameters.AddWithValue("date", date);
            cmd.Parameters.AddWithValue("channel", channel);
            cmd.Parameters.AddWithValue("company", company);
            cmd.Parameters.AddWithValue("revenue", revenue);
            cmd.Parameters.AddWithValue("platform", platform);
            cmd.Parameters.AddWithValue("country", country);

            cmd.ExecuteNonQuery();
            inserted++;
        }

        return Json(new { success = true, count = inserted });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
