using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models
{
    public class FeedbackRecipient
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        [ForeignKey(nameof(FeedbackRequest) + "Id")]
        public required FeedbackRequest feedbackRequest { get; set; }
        [ForeignKey(nameof(EmployeeCard) + "Id")]
        public required EmployeeCard employee { get; set; }
        public DateTime? recievedDate { get; set; }
        public required bool isRead { get; set; }
        [ForeignKey(nameof(FeedbackRequestStatus) + "Id")]
        public required FeedbackRequestStatus status { get; set; }
    }
}
