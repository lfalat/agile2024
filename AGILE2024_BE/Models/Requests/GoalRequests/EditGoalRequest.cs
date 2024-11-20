namespace AGILE2024_BE.Models.Requests.GoalRequests
{
    public class EditGoalRequest
    {
        //public string Id { get; set; }  
        public string Name { get; set; }    
        public string Description { get; set; }  
        public string GoalCategoryId { get; set; }  
        public string Status { get; set; }  
        public DateTime DueDate { get; set; }  
        public int FullfilmentRate { get; set; }  
        public DateTime FinishedDate { get; set; }  
        //public List<string> EmployeeIds { get; set; }  
    }

}
