namespace AGILE2024_BE.Models.Response
{
    public class LoginResponse
    {
        public required string NewJwtToken { get; set; }
        public required string NewRefreshToken { get; set; }
    }
}
