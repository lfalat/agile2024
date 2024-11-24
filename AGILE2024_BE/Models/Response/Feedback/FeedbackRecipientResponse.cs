using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models.Response.Feedback
{
    public class FeedbackRecipientResponse
    {
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string employeeId { get; set; }
        public DateTime? recievedDate { get; set; }
        public DateTime? sentDate { get; set; }
        public required bool isRead { get; set; }
        public string? title { get; set; }
        public required string status { get; set; }
        public required DateTime createDate { get; set; }
        public List<FeedbackQuestionResponse> feedbackQuestions { get; set; } = [];
    }
}
