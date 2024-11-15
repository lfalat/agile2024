namespace AGILE2024_BE.Models.Response
{
    public class LocationResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Adress { get; set; }

        public List<string>? Organizations { get; set; }
        //širka
        public double? Latitude { get; set; }
        //dlžka
        public double? Longitude { get; set; }
        public DateTime LastEdited { get; set; }
        public DateTime Created { get; set; }
        public bool Archived { get; set; }
    }
}
