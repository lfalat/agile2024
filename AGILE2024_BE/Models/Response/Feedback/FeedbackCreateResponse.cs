namespace AGILE2024_BE.Models.Response.Feedback
{
    public class FeedbackCreateResponse
    {
        public List<string> employees { get; set; }
        public List<string> questions { get; set; }
        public string sender { get; set; }
    }
}
