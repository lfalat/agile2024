using AGILE2024_BE.Models.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models
{

    public class Notification
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required Guid Id { get; set; }
        [ForeignKey(nameof(User) + "Id")]
        public required ExtendedIdentityUser User { get; set; }
        //ID, ktoré sa odkazuje na inú entitu (určí podľa typu notifikácie)
        public required Guid ReferencedItemId { get; set; }
        
        public required string Message { get; set; }
        public required DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public required bool IsRead { get; set; } = false;
        public required EnumNotificationType NotificationType { get; set; }
    }
}
