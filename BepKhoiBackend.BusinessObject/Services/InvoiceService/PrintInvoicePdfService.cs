using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Globalization;
using System.IO;
using BepKhoiBackend.BusinessObject.dtos.InvoiceDto;
using PdfSharp.Fonts;
using System.Text;

namespace BepKhoiBackend.BusinessObject.Services.InvoiceService
{
    public class PrintInvoicePdfService
    {
        public string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            text = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char c in text)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
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
                    var document = new PdfDocument();
                    var page = document.AddPage();
                    page.Width = XUnit.FromMillimeter(70);
                    var gfx = XGraphics.FromPdfPage(page);
                    var defaultFont = new XFont("LiberationSans", 8);
                    string storeName = "Bep Khoi";
                    string storePhone = "0901234567";
                    double leftMargin = 5;
                    double rightMargin = 5;
                    double yPosition = 10;

                    // Helper function to draw text with margins  
                    void DrawText(string text, double y, XFont font = null, XBrush brush = null, bool removeDiacritics = true)
                    {
                        font ??= defaultFont;
                        if (removeDiacritics)
                            text = RemoveDiacritics(text ?? "");

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
                    DrawText(storeName, yPosition, new XFont("LiberationSans", 10, XFontStyleEx.Bold));
                    yPosition += 8;
                    DrawText($"Dien thoai: {storePhone}", yPosition);
                    yPosition += 10;

                    // Header  
                    DrawText("HOA DON BAN HANG", yPosition, new XFont("LiberationSans", 10, XFontStyleEx.Bold));
                    yPosition += 10;

                    // Separator below header  
                    DrawSeparator(yPosition);
                    yPosition += 5;

                    // Invoice Information  
                    DrawText($"Ma Hoa Don: {invoice.InvoiceId}", yPosition);
                    yPosition += 8;
                    DrawText($"Khach hang: {invoice.CustomerName}", yPosition);
                    yPosition += 8;
                    DrawText($"Thoi gian: {invoice.CheckInTime:dd/MM/yyyy HH:mm}", yPosition);
                    yPosition += 10;

                    // Separator before product section  
                    DrawSeparator(yPosition);
                    yPosition += 5;

                    // Product Header  
                    DrawText("SAN PHAM", yPosition, new XFont("LiberationSans", 9, XFontStyleEx.Bold));
                    yPosition += 8;

                    // Product entries  
                    foreach (var detail in invoice.InvoiceDetails)
                    {
                        DrawText($"{detail.ProductName ?? ""} SL: {detail.Quantity} x {detail.Price.ToString("N0", new CultureInfo("vi-VN")) + " VND"}", yPosition, removeDiacritics: true);
                        yPosition += 8;
                    }

                    // Separator before summary section  
                    DrawSeparator(yPosition);
                    yPosition += 5;

                    // Summary Section  
                    DrawText($"Tong tien (Chua VAT): {invoice.Subtotal.ToString("N0", new CultureInfo("vi-VN")) + " VND"}", yPosition, removeDiacritics: false);
                    yPosition += 8;
                    DrawText($"Thue VAT: {invoice.TotalVat.ToString("N0", new CultureInfo("vi-VN")) + " VND"}", yPosition, removeDiacritics: false);
                    yPosition += 8;
                    DrawText($"Chi phi khac: {invoice.OtherPayment.ToString("N0", new CultureInfo("vi-VN")) + " VND"}", yPosition, removeDiacritics: false);
                    yPosition += 8;
                    DrawText($"Giam gia: {invoice.InvoiceDiscount.ToString("N0", new CultureInfo("vi-VN")) + " VND"}", yPosition, removeDiacritics: false);
                    yPosition += 8;
                    DrawText($"Tong thanh toan: {invoice.AmountDue.ToString("N0", new CultureInfo("vi-VN")) + " VND"}",
                             yPosition, new XFont("LiberationSans", 9, XFontStyleEx.Bold), removeDiacritics: false);

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

    public class CustomFontResolver : IFontResolver
    {
        private readonly string regularPath = "/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf";
        private readonly string boldPath = "/usr/share/fonts/truetype/liberation/LiberationSans-Bold.ttf";

        public string DefaultFontName => "LiberationSans";

        public byte[] GetFont(string faceName)
        {
            try
            {
                return faceName switch
                {
                    "LiberationSans-Bold" => File.ReadAllBytes(boldPath),
                    _ => File.ReadAllBytes(regularPath),
                };
            }
            catch (Exception ex)
            {
                throw new FileNotFoundException($"Không thể tải font {faceName}: {ex.Message}", ex);
            }
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            if (isBold) return new FontResolverInfo("LiberationSans-Bold");
            return new FontResolverInfo("LiberationSans");
        }
    }
}