using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models
{
    public class ReviewQuestion
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        [ForeignKey(nameof(ReviewRecipient) + "Id")]
        public required ReviewRecipient goalAssignment { get; set; }
        public string? superiorDescription { get; set; }
        public string? employeeDescription { get; set; }
    }
}
