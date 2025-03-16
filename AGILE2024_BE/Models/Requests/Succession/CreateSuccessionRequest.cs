namespace AGILE2024_BE.Models.Requests
{
    public class CreateSuccessionRequest

    {
        public required string LeavingId { get; set; }
        public required string LeaveReason { get; set; }
        public required string LeaveDate { get; set; }

        public string SuccessorId { get; set; }
        public required string ReadyStatus { get; set; }

    }
}
