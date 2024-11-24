using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models.Response.Feedback
{
    public class FeedbackRecipientResponse
    {
        public Guid id { get; set; }
        public required EmployeeCard employee { get; set; }
        public DateTime? recievedDate { get; set; }
        public required bool isRead { get; set; }
    }
}
