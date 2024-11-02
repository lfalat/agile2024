using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models
{
    public class ExtendedIdentityUser : IdentityUser
    {
        [ForeignKey(nameof(Superior) + "Id")]
        public ExtendedIdentityUser? Superior { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Title_after { get; set; }
        public string? Title_before { get; set; }
    }
}
