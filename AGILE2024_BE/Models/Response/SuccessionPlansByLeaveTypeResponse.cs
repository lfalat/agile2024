namespace AGILE2024_BE.Models.Response
{
    public class SuccessionPlansByLeaveTypeResponse
    {
        public Guid LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; }
        public List<SuccessionPlanResponse> SuccessionPlans { get; set; } = new();
    }
}