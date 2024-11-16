using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AGILE2024_BE.Models
{
    public class Level
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(JobPosition) + "Id")]
        public virtual JobPosition? JobPosition { get; set; }

        [JsonIgnore]
        [InverseProperty("Level")]
        public virtual ICollection<EmployeeCard> EmployeeCards { get; } = [];

        public string? Name { get; set; }
    }
}
