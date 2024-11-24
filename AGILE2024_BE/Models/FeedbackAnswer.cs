using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AGILE2024_BE.Models
{
    public class FeedbackAnswer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        [ForeignKey(nameof(FeedbackQuestion) + "Id")]
        public required FeedbackQuestion request { get; set; }
        [ForeignKey(nameof(FeedbackRecipient) + "Id")]
        public required FeedbackRecipient recipient { get; set; }
        public required string text { get; set; }
    }
}
