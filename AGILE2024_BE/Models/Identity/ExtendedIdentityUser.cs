using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models.Identity
{
    public class ExtendedIdentityUser : IdentityUser
    {
        [ForeignKey(nameof(Superior) + "Id")]
        public ExtendedIdentityUser? Superior { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public required string Name { get; set; }

        public string? MiddleName { get; set; }
        public required string Surname { get; set; }
        public string? Title_after { get; set; }
        public string? Title_before { get; set; }
        public string? ProfilePicLink { get; set; }
    }
}
