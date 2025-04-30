using System;
using System.Collections.Generic;

namespace BepKhoiBackend.DataAccess.Models
{
    public partial class DiscountCampaignDetail
    {
        public int DiscountDetailId { get; set; }
        public int DiscountId { get; set; }
        public int ProductId { get; set; }

        public virtual DiscountCampaign Discount { get; set; } = null!;
        public virtual Menu Product { get; set; } = null!;
    }
}
