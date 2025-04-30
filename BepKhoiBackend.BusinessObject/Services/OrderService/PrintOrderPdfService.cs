using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using BepKhoiBackend.DataAccess.Abstract.OrderAbstract;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using BepKhoiBackend.BusinessObject.dtos.InvoiceDto;

public class PrintOrderPdfService 
{
    private readonly IOrderRepository _orderRepository;

    public PrintOrderPdfService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
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
                var defaultFont = new XFont("Arial", 8);

                // Thông tin cửa hàng
                string storeName = "Bếp Khói";
                string storePhone = "0901234567";

                // Thiết lập khoảng cách
                double leftMargin = 5;
                double rightMargin = 5;
                double yPosition = 10;

                // Hàm vẽ văn bản
                void DrawText(string text, double y, XFont font = null, XBrush brush = null)
                {
                    font ??= defaultFont;
                    gfx.DrawString(text, font, brush ?? XBrushes.Black, XUnit.FromMillimeter(leftMargin), XUnit.FromMillimeter(y));
                }

                // Hàm vẽ đường phân cách
                void DrawSeparator(double y)
                {
                    gfx.DrawLine(XPens.Black, XUnit.FromMillimeter(leftMargin), XUnit.FromMillimeter(y),
                                 page.Width - XUnit.FromMillimeter(rightMargin), XUnit.FromMillimeter(y));
                }

                // Header - Thông tin cửa hàng
                DrawText(storeName, yPosition, new XFont("Arial", 10, XFontStyleEx.Bold));
                yPosition += 8;
                DrawText($"Điện thoại: {storePhone}", yPosition);
                yPosition += 10;

                // Tiêu đề hóa đơn
                DrawText("HÓA ĐƠN TẠM TÍNH", yPosition, new XFont("Arial", 10, XFontStyleEx.Bold));
                yPosition += 10;
                DrawSeparator(yPosition);
                yPosition += 5;

                // Thông tin hóa đơn
                DrawText($"Mã Hóa Đơn: {order.OrderId}", yPosition);
                yPosition += 8;
                DrawText($"Ngày tạo: {order.CreatedTime:dd/MM/yyyy HH:mm}", yPosition);
                yPosition += 10;
                DrawSeparator(yPosition);
                yPosition += 5;

                // Danh sách sản phẩm
                DrawText("SẢN PHẨM", yPosition, new XFont("Arial", 9, XFontStyleEx.Bold));
                yPosition += 8;

                foreach (var detail in order.OrderDetails)
                {
                    DrawText($"{detail.ProductName}", yPosition);
                    yPosition += 8;
                    DrawText($"SL: {detail.Quantity} x {detail.Price.ToString("C", new CultureInfo("vi-VN"))}", yPosition);
                    yPosition += 8;
                }

                DrawSeparator(yPosition);
                yPosition += 5;

                // Tổng kết hóa đơn
                DrawText($"Tổng số lượng: {order.TotalQuantity}", yPosition);
                yPosition += 8;
                DrawText($"Tổng tiền: {order.AmountDue.ToString("C", new CultureInfo("vi-VN"))}",
                         yPosition, new XFont("Arial", 9, XFontStyleEx.Bold));

                // Lưu PDF vào MemoryStream
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
