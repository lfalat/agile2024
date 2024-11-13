using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models
{
    public class ReviewRecipient
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        [ForeignKey(nameof(GoalAssignment) + "Id")]
        public required GoalAssignment goalAssignment { get; set; }
        public string? superiorDescription { get; set; }
        public string? employeeDescription { get; set; }
    }
}
