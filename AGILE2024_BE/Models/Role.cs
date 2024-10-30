using System.Collections.ObjectModel;

namespace AGILE2024_BE.Models
{
    public class Role
    {
        public int Id_role { get; set; }
        public string Label { get; set; }
        public string? Description { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
