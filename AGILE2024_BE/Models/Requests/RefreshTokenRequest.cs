namespace AGILE2024_BE.Models.Requests
{
    public class RefreshTokenRequest
    {
        public required string JWTToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
