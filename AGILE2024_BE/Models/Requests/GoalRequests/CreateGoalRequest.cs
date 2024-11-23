namespace AGILE2024_BE.Models.Requests.GoalRequests
{
    public class CreateGoalRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string GoalCategoryId { get; set; } = string.Empty;  
        public string DueDate { get; set; } = string.Empty;  
        public List<string> EmployeeIds { get; set; } = new List<string>();  
    }

}
