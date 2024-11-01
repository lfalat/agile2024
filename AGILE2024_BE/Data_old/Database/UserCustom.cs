namespace AGILE2024_BE.Data.Models
{
    public class UserCustom
    {
        public int? Id_User { get; set; }
        public required int Id_Role { get; set; }
        public int? Id_Superior { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public string? Title_Before { get; set; }
        public string? Title_After { get; set; }
        public required byte[] Salt {  get; set; }
    }
}
