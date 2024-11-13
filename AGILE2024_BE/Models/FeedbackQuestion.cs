using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models
{
    public class FeedbackQuestion
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        [ForeignKey(nameof(FeedbackRequest) + "Id")]
        public required FeedbackRequest request { get; set; }
        public required string text { get; set; }
        public required int order { get; set; }
    }
}
