namespace AGILE2024_BE.Data.Database
{
    public class RegisterUsers
    {
        public int? Id_User { get; set; }
        public required string Email { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public string? Title_Before { get; set; }
        public string? Title_After { get; set; }
    }
}