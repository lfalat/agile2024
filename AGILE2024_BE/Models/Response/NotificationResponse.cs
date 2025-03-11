using AGILE2024_BE.Models;

namespace AGILE2024_BE.Services
{
    public class NotificationResponse
    {
        public Guid Id {get; set;}
        public string Message {get; set;}
        public string Title {get; set;}
        public string ReferencedItem {get; set;}
        public EnumNotificationType NotificationType {get; set;}
        public DateTime CreatedAt {get; set;}
        public bool IsRead {get; set;}
    }
}