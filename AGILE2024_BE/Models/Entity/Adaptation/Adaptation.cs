using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models.Entity.Adaptation
{
    public class Adaptation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public DateOnly ReadyDate { get; set; }
        public DateOnly EndDate { get; set; }
        [ForeignKey(nameof(Employee) + "Id")]
        public EmployeeCard Employee { get; set; }
        [ForeignKey(nameof(CreatedEmployee) + "Id")]
        public EmployeeCard CreatedEmployee { get; set; }
        [ForeignKey(nameof(State) + "Id")]
        public AdaptationState State { get; set; }
    }
}
