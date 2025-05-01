using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using BepKhoiBackend.BusinessObject.dtos.InvoiceDto;
using System.Globalization;


public class InvoiceDocument : IDocument
{
    private readonly InvoicePdfDTO _invoice;

    public InvoiceDocument(InvoicePdfDTO invoice)
    {
        _invoice = invoice;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(10);
            page.Size(PageSizes.A7);
            page.Content().Column(col =>
            {
                col.Spacing(5);

                col.Item().Text("Bếp Khói").Bold().FontSize(14);
                col.Item().Text("Điện thoại: 0901234567").FontSize(10);
                col.Item().Text("HÓA ĐƠN BÁN HÀNG").Bold().FontSize(12).AlignCenter();

                col.Item().LineHorizontal(1);

                col.Item().Text($"Mã HĐ: {_invoice.InvoiceId}");
                col.Item().Text($"Khách hàng: {_invoice.CustomerName}");
                col.Item().Text($"Thời gian: {_invoice.CheckInTime:dd/MM/yyyy HH:mm}");

                col.Item().LineHorizontal(1);
                col.Item().Text("SẢN PHẨM").Bold();

                foreach (var detail in _invoice.InvoiceDetails)
                {
                    col.Item().Text($"{detail.ProductName} | SL: {detail.Quantity} x {detail.Price.ToString("C0", new CultureInfo("vi-VN"))}")
                              .FontSize(9);
                }

                col.Item().LineHorizontal(1);
                col.Item().Text($"Tạm tính: {_invoice.Subtotal.ToString("C0", new CultureInfo("vi-VN"))}");
                col.Item().Text($"VAT: {_invoice.TotalVat.ToString("C0", new CultureInfo("vi-VN"))}");
                col.Item().Text($"Tổng cộng: {_invoice.AmountDue.ToString("C0", new CultureInfo("vi-VN"))}").Bold();
            });
        });
    }
}