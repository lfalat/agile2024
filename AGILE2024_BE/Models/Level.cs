using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models
{
    public class Level
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [ForeignKey(nameof(JobPosition) + "Id")]
        public JobPosition? JobPosition { get; set; }

        [InverseProperty("Level")]
        public ICollection<EmployeeCard> EmployeeCards { get; } = [];

        public string? Name { get; set; }
    }
}
