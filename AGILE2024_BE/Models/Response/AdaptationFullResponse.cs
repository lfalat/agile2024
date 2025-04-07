namespace AGILE2024_BE.Models.Response
{
    public class AdaptationFullResponse
    {
        public Guid id { get; set; }

        public EmployeeCardResponse employee { get; set; }
        public EmployeeCardResponse creator { get; set; }

        public string employeeName { get; set; }
        public string creatorName { get; set; }

        public List<TaskResponse> tasks { get; set; }
        public List<DocumentResponse> documents { get; set; }

        public DateOnly readyDate { get; set; }
        public DateOnly? endDate { get; set; }
    }

    public class TaskResponse
    {
        public Guid id { get; set; }
        public string description { get; set; }
        public DateTime finishDate { get; set; }
        public bool isDone { get; set; }
    }

    public class DocumentResponse
    {
        public Guid id { get; set; }
        public string description { get; set; }
        public string file { get; set; }
    }
}