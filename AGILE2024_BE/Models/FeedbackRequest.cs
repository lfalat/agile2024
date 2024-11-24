using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models
{
    public class FeedbackRequest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        [ForeignKey(nameof(EmployeeCard) + "Id")]
        public required EmployeeCard sender { get; set; }
        public required string title { get; set; }
        public required DateTime createDate { get; set; }
    }
}
