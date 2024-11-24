namespace AGILE2024_BE.Models.Response.Feedback
{
    public class FeedbackQuestionResponse
    {
        public string id { get; set; }
        public required string text { get; set; }
        public string answer { get; set; }
    }
}
