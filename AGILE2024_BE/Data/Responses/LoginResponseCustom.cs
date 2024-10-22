namespace AGILE2024_BE.Data.Responses
{
    public class LoginResponseCustom
    {
        public required string Role { get; set; }
        public required string Email { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public string? Title_Before { get; set; }
        public string? Title_After { get; set; }
        public required string Token { get; set; }
    }
}
