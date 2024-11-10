namespace AGILE2024_BE.Models.Requests.DepartmentRequests
{
    public class ArchiveDepartmentRequest
    {
        public required Guid Id { get; set; }
        public required bool Archive { get; set; }
    }
}
