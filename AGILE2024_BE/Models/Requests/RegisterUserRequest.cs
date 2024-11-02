namespace AGILE2024_BE.Models.Requests
{
    public class RegisterUserRequest
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
        public required string Name { get; init; }
        public required string Surname { get; init; }
        public required string Role { get; init; }
        public string? Title_Before { get; init; }
        public string? Title_After { get; init; }
        public int? Id_Superior { get; init; }
    }
}
