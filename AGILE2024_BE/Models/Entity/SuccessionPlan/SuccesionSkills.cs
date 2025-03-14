using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models.Entity.SuccessionPlan
{
    public class SuccesionSkills
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        [ForeignKey(nameof(SuccessionPlan) + "Id")]
        public SuccessionPlan successionPlan { get; set; }
        public string description { get; set; }
        public bool isReady { get; set; }
    }
}
