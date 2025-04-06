namespace AGILE2024_BE.Models.Response
{
    public class AdaptationResponse
    {
        public Guid id { get; set; }

        public string nameEmployee {  get; set; }
        public string nameSupervisor { get; set; }
        public DateOnly readyDate { get; set; }
        public string taskState { get; set; }

        public DateOnly endDate { get; set; }
    }
}