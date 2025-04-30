using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Globalization;
using System.IO;
using BepKhoiBackend.BusinessObject.dtos.InvoiceDto;
namespace BepKhoiBackend.BusinessObject.Services.InvoiceService
{
    public class PrintInvoicePdfService
    {
        public byte[] GenerateInvoicePdf(InvoicePdfDTO invoice)
        {
            try
            {
                if (invoice == null)
                {
                    throw new ArgumentNullException(nameof(invoice), "Dữ liệu hóa đơn bị null.");
                }

                using (var ms = new MemoryStream())
                {
                    // Create PDF document with a width of 70mm  
                    var document = new PdfDocument();
                    var page = document.AddPage();
                    page.Width = XUnit.FromMillimeter(70); // Set width to 70mm  
                    var gfx = XGraphics.FromPdfPage(page);
                    var defaultFont = new XFont("Arial", 8); // Default font setting  
                    // Store information (Example data)  
                    string storeName = "Bếp Khói";
                    string storePhone = "0901234567";
                    // Margins   
                    double leftMargin = 5; // 5mm left margin  
                    double rightMargin = 5; // 5mm right margin  
                    double yPosition = 10; // Start position  

                    // Helper function to draw text with margins  
                    void DrawText(string text, double y, XFont font = null, XBrush brush = null)
                    {
                        // Use the default font if none specified  
                        font ??= defaultFont;

                        var textWidth = gfx.MeasureString(text, font).Width;
                        var availableWidth = page.Width - XUnit.FromMillimeter(leftMargin + rightMargin);

                        if (textWidth > availableWidth)
                        {
                            text = TruncateText(text, availableWidth, font, gfx);
                        }

                        gfx.DrawString(text, font, brush ?? XBrushes.Black, XUnit.FromMillimeter(leftMargin), XUnit.FromMillimeter(y));
                    }

                    // Helper function to truncate text  
                    string TruncateText(string text, double maxWidth, XFont font, XGraphics gfx)
                    {
                        string truncatedText = text;
                        while (gfx.MeasureString(truncatedText + "...", font).Width > maxWidth && truncatedText.Length > 0)
                        {
                            truncatedText = truncatedText.Substring(0, truncatedText.Length - 1);
                        }
                        return truncatedText + "...";
                    }

                    // Helper function to draw a separator line  
                    void DrawSeparator(double y)
                    {
                        gfx.DrawLine(XPens.Black, XUnit.FromMillimeter(leftMargin), XUnit.FromMillimeter(y),
                                     page.Width - XUnit.FromMillimeter(rightMargin), XUnit.FromMillimeter(y));
                    }
                    // Draw Store Information at the top  
                    DrawText(storeName, yPosition, new XFont("Arial", 10, XFontStyleEx.Bold));
                    yPosition += 8;
                    DrawText($"Điện thoại: {storePhone}", yPosition);
                    yPosition += 10;
                    // Header  
                    DrawText("HÓA ĐƠN BÁN HÀNG", yPosition, new XFont("Arial", 10, XFontStyleEx.Bold));
                    yPosition += 10;

                    // Separator below header  
                    DrawSeparator(yPosition);
                    yPosition += 5;

                    // Invoice Information  
                    DrawText($"Mã Hóa Đơn: {invoice.InvoiceId}", yPosition);
                    yPosition += 8;
                    DrawText($"Khách hàng: {invoice.CustomerName}", yPosition);
                    yPosition += 8;
                    DrawText($"Thời gian: {invoice.CheckInTime:dd/MM/yyyy HH:mm}", yPosition);
                    yPosition += 10;

                    // Separator before product section  
                    DrawSeparator(yPosition);
                    yPosition += 5;

                    // Product Header  
                    DrawText("SẢN PHẨM", yPosition, new XFont("Arial", 9, XFontStyleEx.Bold));
                    yPosition += 8;

                    // Product entries  
                    foreach (var detail in invoice.InvoiceDetails)
                    {
                        DrawText($"{detail.ProductName}" + $"SL: {detail.Quantity} x {detail.Price.ToString("C", new CultureInfo("vi-VN"))}", yPosition);
                        yPosition += 8; // Move down for next product  
                    }

                    // Separator before summary section  
                    DrawSeparator(yPosition);
                    yPosition += 5;

                    // Summary Section  
                    DrawText($"Tổng tiền (Chưa VAT): {invoice.Subtotal.ToString("C", new CultureInfo("vi-VN"))}", yPosition);
                    yPosition += 8;
                    DrawText($"Thuế VAT: {invoice.TotalVat.ToString("C", new CultureInfo("vi-VN"))}", yPosition);
                    yPosition += 8;
                    DrawText($"Tổng thanh toán: {invoice.AmountDue.ToString("C", new CultureInfo("vi-VN"))}",
                             yPosition, new XFont("Arial", 9, XFontStyleEx.Bold));

                    // Save the PDF to memory  
                    document.Save(ms);
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Lỗi khi tạo PDF: " + ex.Message, ex);
            }
        }

    }
}