namespace BepKhoiBackend.DataAccess.Models
{
    public partial class Unit
    {
        public Unit()
        {
            Menus = new HashSet<Menu>();
        }

        public int UnitId { get; set; }
        public string UnitTitle { get; set; } = null!;
        public bool? IsDelete { get; set; }

        public virtual ICollection<Menu> Menus { get; set; }
    }
}
