using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models
{
    public class Division
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public required string Title { get; set; }
    }
}
