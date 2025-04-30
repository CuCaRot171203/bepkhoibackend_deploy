using System;
using System.Collections.Generic;

namespace BepKhoiBackend.DataAccess.Models
{
    public partial class InvoiceDetail
    {
        public int InvoiceDetailId { get; set; }
        public int InvoiceId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal? ProductVat { get; set; }
        public string? ProductNote { get; set; }

        public virtual Invoice Invoice { get; set; } = null!;
        public virtual Menu Product { get; set; } = null!;
    }
}
