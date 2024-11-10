namespace AGILE2024_BE.Models.Requests
{
    public class ArchiveOrganizationRequest
    {
        public required Guid Id { get; set; }
        public required bool Archive {  get; set; }
    }
}