using AGILE2024_BE.Models.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models
{
    public class Goal
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        public required string name { get; set; }
        public required string description { get; set; }
        [ForeignKey(nameof(GoalCategory) + "Id")]
        public required GoalCategory category { get; set; }
        [ForeignKey(nameof(GoalStatus) + "Id")]
        public required GoalStatus status { get; set; }
        public required DateTime dueDate { get; set; }
        public int? fullfilmentRate { get; set; }
        public DateTime? finishedDate { get; set; }
        [ForeignKey(nameof(EmployeeCard) + "Id")]
        public required EmployeeCard employee { get; set; }
    }
}
