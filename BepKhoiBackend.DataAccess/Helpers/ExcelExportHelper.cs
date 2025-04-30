using ClosedXML.Excel;
using BepKhoiBackend.DataAccess.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

public static class ExcelExportHelper
{
    public static byte[] GenerateProductPriceExcel(List<Menu> products)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Product Prices");
        var now = DateTime.Now;

        worksheet.Range("A1:F1").Merge().Value = $"List of product by day {now:dd/MM/yyyy}";
        worksheet.Cell("A1").Style.Font.SetBold().Font.FontSize = 16;
        worksheet.Cell("A1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        var headers = new[] { "Product ID", "Product Name", "Cost Price", "Sell Price", "Sale Price", "VAT" };
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(2, i + 1).Value = headers[i];
        }

        int row = 3;
        foreach (var p in products)
        {
            worksheet.Cell(row, 1).Value = p.ProductId;
            worksheet.Cell(row, 2).Value = p.ProductName;
            worksheet.Cell(row, 3).Value = p.CostPrice;
            worksheet.Cell(row, 4).Value = p.SellPrice;
            worksheet.Cell(row, 5).Value = p.SalePrice;
            worksheet.Cell(row, 6).Value = p.ProductVat;
            row++;
        }

        worksheet.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public static (byte[], string, bool, string) HandleExcelExportException(Exception ex)
    {
        return ex switch
        {
            TimeoutException => (null, null, false, "Database timeout occurred. Please try again later."),
            DbUpdateException => (null, null, false, "Database error occurred. Please contact support."),
            SqlException => (null, null, false, "Database connection error. Please try again later."),
            IOException => (null, null, false, "An IO error occurred while creating the Excel file."),
            UnauthorizedAccessException => (null, null, false, "You do not have permission to export this data."),
            _ => (null, null, false, "An unexpected error occurred. Please contact admin.")
        };
    }
}