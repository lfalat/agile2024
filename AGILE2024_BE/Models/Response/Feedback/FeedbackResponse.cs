using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models.Response.Feedback
{
    public class FeedbackResponse
    {
        public Guid id { get; set; }
        public required EmployeeCard sender { get; set; }
        public required string title { get; set; }
        public required DateTime createDate { get; set; }
        public DateTime? sentAt { get; set; }
        public required FeedbackRequestStatus status { get; set; }
        public DateTime? recievedDate { get; set; }
        public required bool isRead { get; set; }
        public List<FeedbackQuestionResponse> feedbackQuestions { get; set; }
        public List<FeedbackRecipientResponse> feedbackRecipients { get; set; }
    }
}
