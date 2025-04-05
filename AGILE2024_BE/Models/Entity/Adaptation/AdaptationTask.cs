using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models.Entity.Adaptation
{
    public class AdaptationTask
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey(nameof(Adaptation) + "Id")]
        public Adaptation Adaptation { get; set; }
        public string DescriptionTask { get; set; }
        public bool IsDone { get; set; } = false;
        public DateTime FinishDate { get; set; }
    }
}
