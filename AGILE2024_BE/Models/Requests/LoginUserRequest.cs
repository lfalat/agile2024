namespace AGILE2024_BE.Models.Requests
{
    public class LoginUserRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
