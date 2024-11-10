namespace AGILE2024_BE.Models.Response
{
    public class UserIdentityResponse
    {
        public string? id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? TitleBefore { get; set; }
        public string? TitleAfter { get; set; }
        public string? Role { get; set; }
        public required string Email { get; set; }
        public string? ProfilePicLink { get; set; }
    }
}
