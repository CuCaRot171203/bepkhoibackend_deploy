using System;
using System.Collections.Generic;

namespace BepKhoiBackend.DataAccess.Models
{
    public partial class DiscountCampaign
    {
        public DiscountCampaign()
        {
            DiscountCampaignDetails = new HashSet<DiscountCampaignDetail>();
        }

        public int DiscountId { get; set; }
        public string DiscountTitle { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool DiscountByPercentage { get; set; }
        public decimal DiscountValue { get; set; }
        public string? Description { get; set; }
        public bool? Status { get; set; }
        public bool? IsDelete { get; set; }

        public virtual ICollection<DiscountCampaignDetail> DiscountCampaignDetails { get; set; }
    }
}
