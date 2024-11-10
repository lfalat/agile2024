using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models
{
    public class JobPosition
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [InverseProperty("JobPosition")]
        public virtual List<Level> Levels { get; set; } = new List<Level>();
        public virtual List<Organization> Organizations { get; set; } = new List<Organization>();

        public required string Name { get; set; }
        public required string Code { get; set; }
        public required DateTime LastEdited { get; set; } = DateTime.Now;
        public required DateTime Created { get; set; } = DateTime.Now;
        public required bool Archived { get; set; } = false;
    }
}
