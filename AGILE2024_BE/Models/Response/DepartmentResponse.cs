namespace AGILE2024_BE.Models.Response
{
    public class DepartmentResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string? OrganizationName { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastEdited { get; set; }
        public bool Archived { get; set; }
        public Guid? ParentDepartmentId { get; set; }
    }

}
