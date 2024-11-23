namespace AGILE2024_BE.Models.Response
{
    public class GoalResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GoalCategoryId { get; set; }
        public string GoalCategoryName { get; set; }
        public Guid? StatusId { get; set; }
        public string? StatusName { get; set; }

        public DateTime DueDate { get; set; }
        public DateTime? FinishedDate { get; set; }
        public int? FullfilmentRate { get; set; }

    }
}
