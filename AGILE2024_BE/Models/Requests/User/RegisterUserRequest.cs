namespace AGILE2024_BE.Models.Requests.User
{
    public class RegisterUserRequest
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
        public required string ConfirmPassword { get; init; }
        public required string Name { get; init; }
        public required string Surname { get; init; }
        public required string Role { get; init; }
        public string? TitleBefore { get; init; }
        public string? TitleAfter { get; init; }
        //public int? Id_Superior { get; init; }
    }
}
