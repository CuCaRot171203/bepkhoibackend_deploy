using System;
using System.Collections.Generic;

namespace BepKhoiBackend.DataAccess.Models
{
    public partial class Menu
    {
        public Menu()
        {
            DiscountCampaignDetails = new HashSet<DiscountCampaignDetail>();
            InvoiceDetails = new HashSet<InvoiceDetail>();
            OrderCancellationHistories = new HashSet<OrderCancellationHistory>();
            OrderDetails = new HashSet<OrderDetail>();
            ProductImages = new HashSet<ProductImage>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int ProductCategoryId { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? ProductVat { get; set; }
        public string? Description { get; set; }
        public int UnitId { get; set; }
        public bool? IsAvailable { get; set; }
        public bool? Status { get; set; }
        public bool? IsDelete { get; set; }

        public virtual ProductCategory ProductCategory { get; set; } = null!;
        public virtual Unit Unit { get; set; } = null!;
        public virtual ICollection<DiscountCampaignDetail> DiscountCampaignDetails { get; set; }
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; }
        public virtual ICollection<OrderCancellationHistory> OrderCancellationHistories { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
    }
}
