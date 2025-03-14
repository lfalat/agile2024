using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models.Entity.SuccessionPlan
{
    public class ReadyStatus
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }

        public string description { get; set; }
    }
}
