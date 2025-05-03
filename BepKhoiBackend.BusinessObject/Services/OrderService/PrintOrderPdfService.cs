using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BepKhoiBackend.DataAccess.Abstract.OrderAbstract;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Fonts;

public class PrintOrderPdfService
{
    private readonly IOrderRepository _orderRepository;

    public PrintOrderPdfService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

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

    public async Task<byte[]> GenerateTempInvoicePdfAsync(int orderId)
    {
        var order = await _orderRepository.GetOrderWithDetailsAsync(orderId);
        if (order == null)
        {
            throw new KeyNotFoundException($"Order with ID {orderId} not found.");
        }

        try
        {
            using (var ms = new MemoryStream())
            {
                var document = new PdfDocument();
                var page = document.AddPage();
                page.Width = XUnit.FromMillimeter(70); // Chiều rộng hóa đơn 70mm (Máy in nhiệt)
                var gfx = XGraphics.FromPdfPage(page);
                var defaultFont = new XFont("LiberationSans", 8);

                // Thông tin cửa hàng
                string storeName = "Bep Khoi";
                string storePhone = "0901234567";

                // Thiết lập khoảng cách
                double leftMargin = 5;
                double rightMargin = 5;
                double yPosition = 10;

                // Hàm vẽ văn bản
                void DrawText(string text, double y, XFont font = null, XBrush brush = null, bool removeDiacritics = true)
                {
                    font ??= defaultFont;
                    if (removeDiacritics && text != null)
                    {
                        text = RemoveDiacritics(text);
                    }

                    var textWidth = gfx.MeasureString(text, font).Width;
                    var availableWidth = page.Width - XUnit.FromMillimeter(leftMargin + rightMargin);

                    if (textWidth > availableWidth)
                    {
                        text = TruncateText(text, availableWidth, font, gfx);
                    }

                    gfx.DrawString(text, font, brush ?? XBrushes.Black, XUnit.FromMillimeter(leftMargin), XUnit.FromMillimeter(y));
                }

                // Hàm cắt ngắn văn bản nếu quá dài
                string TruncateText(string text, double maxWidth, XFont font, XGraphics gfx)
                {
                    string truncatedText = text;
                    while (gfx.MeasureString(truncatedText + "...", font).Width > maxWidth && truncatedText.Length > 0)
                    {
                        truncatedText = truncatedText.Substring(0, truncatedText.Length - 1);
                    }
                    return truncatedText + "...";
                }

                // Hàm vẽ đường phân cách
                void DrawSeparator(double y)
                {
                    gfx.DrawLine(XPens.Black, XUnit.FromMillimeter(leftMargin), XUnit.FromMillimeter(y),
                                 page.Width - XUnit.FromMillimeter(rightMargin), XUnit.FromMillimeter(y));
                }

                // Header - Thông tin cửa hàng
                DrawText(storeName, yPosition, new XFont("LiberationSans", 10, XFontStyleEx.Bold));
                yPosition += 8;
                DrawText($"Dien thoai: {storePhone}", yPosition);
                yPosition += 10;

                // Tiêu đề hóa đơn
                DrawText("HOA DON TAM TINH", yPosition, new XFont("LiberationSans", 10, XFontStyleEx.Bold));
                yPosition += 10;
                DrawSeparator(yPosition);
                yPosition += 5;

                // Thông tin hóa đơn
                DrawText($"Ma Hoa Don: {order.OrderId}", yPosition);
                yPosition += 8;
                DrawText($"Ngay tao: {order.CreatedTime:dd/MM/yyyy HH:mm}", yPosition);
                yPosition += 10;

                // Thông tin giao hàng (nếu có)
                if (order.DeliveryInformation != null)
                {
                    DrawSeparator(yPosition);
                    yPosition += 5;
                    DrawText("THONG TIN GIAO HANG", yPosition, new XFont("LiberationSans", 9, XFontStyleEx.Bold));
                    yPosition += 8;
                    DrawText($"Ten nguoi nhan: {order.DeliveryInformation.ReceiverName}", yPosition);
                    yPosition += 8;
                    DrawText($"So dien thoai: {order.DeliveryInformation.ReceiverPhone}", yPosition);
                    yPosition += 8;
                    DrawText($"Dia chi: {order.DeliveryInformation.ReceiverAddress}", yPosition);
                    yPosition += 8;
                    if (!string.IsNullOrWhiteSpace(order.DeliveryInformation.DeliveryNote))
                    {
                        DrawText($"Ghi chu: {order.DeliveryInformation.DeliveryNote}", yPosition);
                        yPosition += 8;
                    }
                }

                // Danh sách sản phẩm
                DrawSeparator(yPosition);
                yPosition += 5;
                DrawText("SAN PHAM", yPosition, new XFont("LiberationSans", 9, XFontStyleEx.Bold));
                yPosition += 8;

                foreach (var detail in order.OrderDetails)
                {
                    DrawText($"{detail.ProductName ?? ""}", yPosition);
                    yPosition += 8;
                    DrawText($"SL: {detail.Quantity} x {detail.Price.ToString("N0", new CultureInfo("vi-VN")) + " VND"}", yPosition, removeDiacritics: false);
                    yPosition += 8;
                    if (!string.IsNullOrWhiteSpace(detail.ProductNote))
                    {
                        DrawText($"Ghi chu: {detail.ProductNote}", yPosition);
                        yPosition += 8;
                    }
                }

                DrawSeparator(yPosition);
                yPosition += 5;

                // Tổng kết hóa đơn
                DrawText($"Tong so luong: {order.TotalQuantity}", yPosition);
                yPosition += 8;
                DrawText($"Tong tien: {order.AmountDue.ToString("N0", new CultureInfo("vi-VN")) + " VND"}",
                         yPosition, new XFont("LiberationSans", 9, XFontStyleEx.Bold), removeDiacritics: false);

                // Lưu PDF vào MemoryStream
                document.Save(ms);
                return ms.ToArray();
            }
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException("Lỗi khi lưu PDF vào bộ nhớ: " + ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Lỗi không xác định khi tạo PDF: " + ex.Message, ex);
        }
    }
}