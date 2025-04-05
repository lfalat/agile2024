using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models.Entity.Adaptation
{
    public class AdaptationDoc
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey(nameof(Adaptation) + "Id")]
        public Adaptation Adaptation { get; set; }
        public string DescriptionDocs { get; set; }
        public string FilePath { get; set; }
    }
}
