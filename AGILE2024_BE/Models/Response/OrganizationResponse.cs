namespace AGILE2024_BE.Models.Response
{
    public class OrganizationResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public DateTime LastEdited { get; set; }

        public DateTime Created { get; set; }

        public bool Archived { get; set; }
        public string? LocationName { get; set; }
    }
}
