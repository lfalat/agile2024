namespace AGILE2024_BE.Models.Requests.User
{
    public class LoginUserRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
