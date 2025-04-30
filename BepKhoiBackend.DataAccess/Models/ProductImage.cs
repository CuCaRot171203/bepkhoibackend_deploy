using System;
using System.Collections.Generic;

namespace BepKhoiBackend.DataAccess.Models
{
    public partial class ProductImage
    {
        public int ProductImageId { get; set; }
        public int ProductId { get; set; }
        public string ProductImage1 { get; set; } = null!;

        public virtual Menu Product { get; set; } = null!;
    }
}
