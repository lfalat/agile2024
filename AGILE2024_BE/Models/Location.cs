using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models
{
    public class Location
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Code { get; set; }
        public required string City { get; set; }
        public required string ZipCode { get; set; }
        public required string Adress { get; set; }
        //širka
        public double? Latitude { get; set; }
        //dlžka
        public double? Longitude { get; set; }
        public DateTime LastEdited { get; set; } = DateTime.Now;
        public DateTime Created { get; set; } = DateTime.Now;
        public bool Archived { get; set; } = false;
    }
}
