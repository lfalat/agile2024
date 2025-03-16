namespace AGILE2024_BE.Models.Response
{
    public class SuccessionPlanResponse
    {
        // Leaving employee
        public string LeavingFullName { get; set; }
        public string LeavingJobPosition { get; set; }
        public string LeavingDepartment { get; set; }
        public string Reason { get; set; }
        public DateOnly LeaveDate { get; set; }

        // Successor
        public string SuccessorFullName { get; set; }
        public string SuccessorJobPosition { get; set; }
        public string SuccessorDepartment { get; set; }
        public string ReadyStatus { get; set; }
    }
}