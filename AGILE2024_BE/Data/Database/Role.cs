namespace AGILE2024_BE.Data.Database
{
    public class Role
    {
        public int Id_Role { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
