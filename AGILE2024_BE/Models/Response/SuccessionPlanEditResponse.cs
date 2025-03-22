namespace AGILE2024_BE.Models.Response
{
    public class SuccessionPlanEditResponse
    {
        public Guid id { get; set; }
        // Leaving employee
        public Guid LeavingId { get; set; }
        public string LeaveType { get; set; }
        public string LeavingFullName { get; set; }
        public string LeavingJobPosition { get; set; }
        public string LeavingDepartment { get; set; }
        public string LeavingOrganization { get; set; }
        public string Reason { get; set; }
        public DateOnly LeaveDate { get; set; }

        // Successor
        public string SuccessorId { get; set; }
        public string SuccessorFullName { get; set; }
        public string SuccessorJobPosition { get; set; }
        public string SuccessorDepartment { get; set; }
        public string ReadyStatusId { get; set; }
        public string ReadyStatus { get; set; }
        public List<SuccessionSkillResponse> Skills { get; set; } = new List<SuccessionSkillResponse>();

    }
}