using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models.Entity.SuccessionPlan
{
    public class SuccessionPlan
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        [ForeignKey(nameof(EmployeeCard) + "Id")]
        public EmployeeCard successor { get; set; }
        public EmployeeCard leavingPerson { get; set; }

        [ForeignKey(nameof(LeaveType) + "Id")]
        public LeaveType leaveType { get; set; }

        [ForeignKey(nameof(ReadyStatus) + "Id")]
        public ReadyStatus readyStatus { get; set; }

        public string reason { get; set; }
        public DateOnly leaveDate { get; set; }
        public bool isExternal { get; set; }
    }
}
