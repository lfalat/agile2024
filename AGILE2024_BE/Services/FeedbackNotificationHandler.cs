
using AGILE2024_BE.Data;
using AGILE2024_BE.Models;
using AGILE2024_BE.Models.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AGILE2024_BE.Services
{
    public class FeedbackNotificationHandler : INotificationHandler
    {
        private readonly AgileDBContext dbContext;
        private readonly IHubContext<NotificationHub> hubContext;

        public FeedbackNotificationHandler(AgileDBContext dbContext, IHubContext<NotificationHub> hubContext)
        {
            this.dbContext = dbContext;
            this.hubContext = hubContext;
        }

        public async Task HandleNotificationAsync(CancellationToken cancellationToken)
        {
            var feedbacks = await dbContext.FeedbackRecipients
                .Include(x => x.status)
                .Include(x => x.employee.User)
                .Include(x => x.feedbackRequest)
                .Where(
                    x => x.isRead &&
                    x.status.description == EnumFeedbackRequestStatus.Nevyplnený.ToString()
                ).ToListAsync();

            var existingNotifications = await dbContext.Notifications
                .Where(n => feedbacks.Select(y => y.id).Contains(n.ReferencedItemId) &&
                            n.NotificationType == EnumNotificationType.FeedbackUnsentReminderNotificationType)
                .ToListAsync();

            List<Notification> notifications = new();

            foreach (var feedback in feedbacks)
            {
                var existingNotification = existingNotifications.FirstOrDefault(n => n.ReferencedItemId == feedback.id);

                if (existingNotification == null)
                {
                    var notification = new Notification
                    {
                        CreatedAt = DateTime.Now,
                        Id = new Guid(),
                        Message = $"Bola zaznamenená neodoslaná spätná väzba {feedback.feedbackRequest.title}. Prosím vráťte sa a dokončite operáciu !",
                        IsRead = false,
                        NotificationType = EnumNotificationType.FeedbackUnsentReminderNotificationType,
                        ReferencedItemId = feedback.id,
                        User = feedback.employee.User
                    };

                    notifications.Add(notification);
                }
            }

            foreach(var notifacation in notifications)
            {
                var response = new NotificationResponse
                {
                    CreatedAt = notifacation.CreatedAt,
                    Id = notifacation.Id,
                    IsRead = notifacation.IsRead,
                    Message = notifacation.Message,
                    NotificationType = notifacation.NotificationType,
                    ReferencedItem = notifacation.ReferencedItemId.ToString(),
                    Title = NotificationHelpers.GetNotificationTitle(notifacation.NotificationType)
                };

                await hubContext.Clients.User(notifacation.User.Id)
                    .SendAsync("ReceiveNotification", response, cancellationToken);
            }

            if (notifications.Any())
            {
                dbContext.Notifications.AddRange(notifications);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
