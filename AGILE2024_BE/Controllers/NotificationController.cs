using AGILE2024_BE.Data;
using AGILE2024_BE.Models;
using AGILE2024_BE.Models.Identity;
using AGILE2024_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AGILE2024_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IHubContext<NotificationHub> hubContext;
        private UserManager<ExtendedIdentityUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private IConfiguration config;
        private AgileDBContext dbContext;

        public NotificationController(UserManager<ExtendedIdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config, AgileDBContext dbContext, IHubContext<NotificationHub> hubContext)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.config = config;
            this.dbContext = dbContext;
            this.hubContext = hubContext;
        }

        [HttpPost("Send-all")]
        //[Authorize]
        public async Task<IActionResult> SendNotification(string message, string link, EnumNotificationType notificationType)
        {

            await hubContext.Clients.All.SendAsync("ReceiveNotification", new
            {
                Message = message,
                Type = notificationType,
                Link = link.ToString(),
            });

            return Ok(new { message = "Notification sent!" });
        }

        [HttpGet("Notifications")]
        [Authorize]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found.");
            }

            var notifications = await dbContext.Notifications
                .Where(n => n.User.Id == userId && n.IsRead == false) 
                .OrderByDescending(n => n.CreatedAt) 
                .Take(10)
                .Select(n => new NotificationResponse
                {
                    Id = n.Id,
                    Message = n.Message,
                    Title = NotificationHelpers.GetNotificationTitle(n.NotificationType),
                    ReferencedItem = n.ReferencedItemId.ToString(),
                    NotificationType = n.NotificationType,
                    CreatedAt = n.CreatedAt,
                    IsRead = n.IsRead
                })
                .ToListAsync();

            return Ok(notifications);
        }


        [HttpGet("AllNotifications")]
        [Authorize]
        public async Task<IActionResult> GetNotifications(int count = 10, int page = 0)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found.");
            }

            var notifications = await dbContext.Notifications
                .Where(n => n.User.Id == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Skip(page * count)
                .Take(count)
                .Select(n => new NotificationResponse
                {
                    Id = n.Id,
                    Message = n.Message,
                    Title = NotificationHelpers.GetNotificationTitle(n.NotificationType),
                    ReferencedItem = n.ReferencedItemId.ToString(),
                    NotificationType = n.NotificationType,
                    CreatedAt = n.CreatedAt,
                    IsRead = n.IsRead
                })
                .ToListAsync();

            return Ok(notifications);
        }

        [HttpPut("MarkAsRead/{id}")]
        [Authorize]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {

            var notification = await dbContext.Notifications.FindAsync(id);

            if (notification == null)
            {
                return NotFound("Notification not found.");
            }

            notification.IsRead = true;
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("Delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteNotification(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found.");
            }

            var notification = await dbContext.Notifications
                .Where(n => n.Id == id && n.User.Id == userId)
                .FirstOrDefaultAsync();

            if (notification == null)
            {
                return NotFound("Notification not found.");
            }

            dbContext.Notifications.Remove(notification);
            await dbContext.SaveChangesAsync();

            return Ok(new { message = "Notification deleted successfully." });
        }
    }
}
