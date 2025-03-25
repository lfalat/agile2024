namespace AGILE2024_BE.Models.Requests
{
    public class CreateSuccessionRequest

    {
        public required string LeavingId { get; set; }
        public required string LeaveReason { get; set; }
        public required string LeaveType { get; set; }
        public required DateOnly LeaveDate { get; set; }

        public string? SuccessorId { get; set; }
        public required string ReadyStatus { get; set; }

        public List<SuccessionSkillRequest> Skills { get; set; }
        public bool isNotified { get; set; }
    }
    public class SuccessionSkillRequest
    {
        public string Description { get; set; }
        public bool IsReady { get; set; }

        
    }
}
