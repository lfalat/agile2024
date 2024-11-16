
namespace AGILE2024_BE.Models.Requests
{
    public class UpdateLocationRequest

    {
        public required string Name { get; set; }
        public required string Code { get; set; }
        public required string City { get; set; }
        public required string ZipCode { get; set; }

        public List<string>? Organizations { get; set; }
        public List<string>? OrganizationsID { get; set; } 

        public required string Adress { get; set; }
        //širka
        public required double? Latitude { get; set; }
        //dlžka
        public required double? Longitude { get; set; }
    }
}
