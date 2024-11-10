using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models
{
    public class Department
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [ForeignKey(nameof(Organization) + "Id")]
        public required Organization? Organization { get; set; }

        [ForeignKey(nameof(ParentDepartment) + "Id")]
        public Department? ParentDepartment { get; set; }

        [InverseProperty("Department")]
        public ICollection<EmployeeCard> EmployeeCards { get; } = [];

        public required string Name { get; set; }
        public required string Code { get; set; }
        public DateTime LastEdited { get; set; } = DateTime.Now;
        public DateTime Created { get; set; } = DateTime.Now;
        public required bool Archived { get; set; } = false;
    }
}
