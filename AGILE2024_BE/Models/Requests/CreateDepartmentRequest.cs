namespace AGILE2024_BE.Models.Requests
{
    public class CreateDepartmentRequest
    {
       
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }

        public string? Organization { get; set; }
        public string? OrganizationId { get; set; }
            public string? OrganizationName { get; set; }
            public DateTime Created { get; set; }
            public DateTime LastEdited { get; set; }
            public bool Archived { get; set; }
            public string? ParentDepartmentId { get; set; }
            public List<string>? ChildDepartments { get; set; }
    
    }
}
