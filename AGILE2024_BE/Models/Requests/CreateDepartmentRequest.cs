namespace AGILE2024_BE.Models.Requests
{
    public class CreateDepartmentRequest
    {
       
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
            public string? OrganizationName { get; set; }
            public DateTime Created { get; set; }
            public DateTime LastEdited { get; set; }
            public bool Archived { get; set; }
            public Guid? ParentDepartmentId { get; set; }
            public Guid? ChildDepartmentId { get; set; }
    
    }
}
