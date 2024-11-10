namespace AGILE2024_BE.Models.Requests.JobPositionRequests
{
    public class EditJobPostitionRequest
    {
        public List<string> Levels { get; set; } = [];
        public List<Guid> OrganizationsID { get; set; } = [];
        public required string Name { get; set; }
        public required string Code { get; set; }
        public required Guid ID { get; set; }
    }
}
