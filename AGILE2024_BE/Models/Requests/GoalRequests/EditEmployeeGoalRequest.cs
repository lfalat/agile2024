namespace AGILE2024_BE.Models.Requests.GoalRequests
{
    public class EditEmployeeGoalRequest
    {
        public string Status { get; set; }
        public int? FullfilmentRate { get; set; }
        public DateTime? FinishedDate { get; set; }
    }
}
