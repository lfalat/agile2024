using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models
{
    public class FeedbackAnswer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        [ForeignKey(nameof(FeedbackQuestion) + "Id")]
        public required FeedbackQuestion request { get; set; }
        public required string text { get; set; }
        public required DateTime answeredDate{ get; set; }
    }
}
