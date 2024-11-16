using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models
{
    public class Review
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        [ForeignKey(nameof(EmployeeCard) + "Id")]
        public required EmployeeCard sender { get; set; }
        public required int counter { get; set; }
        public required DateTime employeeEndDate { get; set; }
        public required DateTime superiorEndDate { get; set; }
        public required DateTime createDate { get; set; }
        public DateTime? endDate { get; set; }
    }
}
