
using AGILE2024_BE.Data;
using AGILE2024_BE.Models;
using AGILE2024_BE.Models.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AGILE2024_BE.Services
{
    public class ReviewNotificationHandler : INotificationHandler
    {
        private AgileDBContext dbContext;
        private IHubContext<NotificationHub> hubContext;

        public ReviewNotificationHandler(AgileDBContext dbContext, IHubContext<NotificationHub> hubContext)
        {
            this.dbContext = dbContext;
            this.hubContext = hubContext;
        }

        public async Task HandleNotificationAsync(CancellationToken cancellationToken)
        {
            try
            {
                var reviews = await dbContext.ReviewRecipents
                    .Where(x => !x.isSentEmployeeDesc && x.isSavedEmployeeDesc)
                    .Include(x => x.goalAssignment.employee.User)
                    .Include(x => x.goalAssignment.goal)
                    .ToListAsync();

                var existingNotifications = await dbContext.Notifications
                    .Where(n => reviews.Select(y => y.id).Contains(n.ReferencedItemId) &&
                                n.NotificationType == EnumNotificationType.ReviewUnsentReminderNotificationType)
                    .ToListAsync();

                List<Notification> notifications = new();

                foreach (var review in reviews)
                {
                    var existingNotification = existingNotifications.FirstOrDefault(n => n.ReferencedItemId == review.id);

                    if (existingNotification == null)
                    {
                        var notification = new Notification
                        {
                            CreatedAt = DateTime.Now,
                            Id = Guid.NewGuid(),
                            Message = $"Bol zaznamenaný neodoslaný posudok cieľa {review.goalAssignment.goal.name}. Prosím vrátte sa a dokončiťe operáciu!",
                            IsRead = false,
                            NotificationType = EnumNotificationType.ReviewUnsentReminderNotificationType,
                            ReferencedItemId = review.id,
                            User = review.goalAssignment.employee.User
                        };

                        notifications.Add(notification);
                    }
                }

                foreach (var notification in notifications)
                {
                    var response = new NotificationResponse
                    {
                        CreatedAt = notification.CreatedAt,
                        Id = notification.Id,
                        IsRead = notification.IsRead,
                        Message = notification.Message,
                        NotificationType = notification.NotificationType,
                        ReferencedItem = notification.ReferencedItemId.ToString(),
                        Title = NotificationHelpers.GetNotificationTitle(notification.NotificationType)
                    };

                    await hubContext.Clients.User(notification.User.Id)
                        .SendAsync("ReceiveNotification", response, cancellationToken);
                }

                if (notifications.Any())
                {
                    dbContext.Notifications.AddRange(notifications);
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleNotificationAsync: {ex.Message}");
            }
        }

    }
}
