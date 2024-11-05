using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models
{
    public class Organization
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [InverseProperty("Organization")]
        public ICollection<Department> RelatedDepartments { get; } = [];

        public ICollection<JobPosition> JobPositions { get; set; } = [];

        [ForeignKey(nameof(Location) + "Id")]
        public Location? Location { get; set; }
        public required string Name { get; set; }
        public required string Code { get; set; }
        public required DateTime LastEdited { get; set; } = DateTime.Now;
        public required DateTime Created { get; set; } = DateTime.Now;
        public required bool Archived { get; set; } = false;
    }
}
