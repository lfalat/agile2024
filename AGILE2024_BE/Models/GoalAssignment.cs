using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models
{
    public class GoalAssignment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        [ForeignKey(nameof(Goal) + "Id")]
        public required Goal goal { get; set; }
        [ForeignKey(nameof(EmployeeCard) + "Id")]
        public required EmployeeCard employee { get; set; }
    }
}
