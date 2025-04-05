using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models.Entity.Adaptation
{
    public class AdaptationState
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string DescriptionState { get; set; }
    }
}
