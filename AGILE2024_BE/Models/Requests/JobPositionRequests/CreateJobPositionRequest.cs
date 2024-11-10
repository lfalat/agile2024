namespace AGILE2024_BE.Models.Requests.JobPositionRequests
{
    public class CreateJobPositionRequest
    {
        public List<string> Levels { get; set; } = [];
        public List<Guid> OrganizationsID { get; set;  } = [];
        public required string Name { get; set; }
        public required string Code { get; set; }
    }
}
