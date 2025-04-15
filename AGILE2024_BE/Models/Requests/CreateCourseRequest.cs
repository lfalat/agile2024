using AGILE2024_BE.Models.Response;

namespace AGILE2024_BE.Models.Requests
{
    public class CreateCourseRequest
    {
        public string Name { get; set; }
        public string DetailDescription { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string CreatedUserId { get; set; }
        public string Type { get; set; }
        public int Version { get; set; }
        public List<string> Employees { get; set; }
        public List<FileRequest> Files { get; set; }

    }

    public class FileRequest
    {
        public string Description { get; set; }
        public string FilePath { get; set; }
    }

    public class FileUpload
    {
        public IFormFile? File { get; set; }
    }
}
