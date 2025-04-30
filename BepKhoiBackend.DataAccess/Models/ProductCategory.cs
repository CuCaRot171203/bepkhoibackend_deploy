namespace BepKhoiBackend.DataAccess.Models
{
    public partial class ProductCategory
    {
        public ProductCategory()
        {
            Menus = new HashSet<Menu>();
        }

        public int ProductCategoryId { get; set; }
        public string ProductCategoryTitle { get; set; } = null!;
        public bool? IsDelete { get; set; }

        public virtual ICollection<Menu> Menus { get; set; }
    }
}
