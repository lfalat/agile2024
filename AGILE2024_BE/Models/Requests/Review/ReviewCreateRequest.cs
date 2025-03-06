namespace AGILE2024_BE.Models.Requests.Review
{
    public class ReviewCreateRequest
    {
        public Guid SenderId { get; set; }
        public List<string> employeeIds { get; set; }
        public DateTime EmployeeDeadline { get; set; }
        public DateTime SuperiorDeadline { get; set; }
    }
}
