using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models.Entity.SuccessionPlan
{
    public class SuccessionPlan
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required Guid id { get; set; }
        [ForeignKey("successorId")]
        public EmployeeCard? successor { get; set; }
        [ForeignKey("leavingPersonId")]
        public required EmployeeCard leavingPerson { get; set; }

        [ForeignKey(nameof(LeaveType) + "Id")]
        public required LeaveType leaveType { get; set; }

        [ForeignKey(nameof(ReadyStatus) + "Id")]
        public required ReadyStatus readyStatus { get; set; }

        public required string reason { get; set; }
        public required DateOnly leaveDate { get; set; }
        public required bool isExternal { get; set; }
    }
}
