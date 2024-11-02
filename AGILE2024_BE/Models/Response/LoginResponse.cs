namespace AGILE2024_BE.Models.Response
{
    public class LoginResponse
    {
        public required string JwtToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
