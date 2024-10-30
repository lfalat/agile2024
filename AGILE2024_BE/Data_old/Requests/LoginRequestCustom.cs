namespace AGILE2024_BE.Data.Requests
{
    public class LoginRequestCustom
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
    }
}
