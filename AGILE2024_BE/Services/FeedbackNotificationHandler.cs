
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

            var existingNotificationsDict = existingNotifications.ToDictionary(n => n.ReferencedItemId);
            List<Notification> notifications = new();

            foreach (var feedback in feedbacks)
            {
                // Skip feedbacks with null employee.User or feedbackRequest
                if (feedback.employee?.User == null || feedback.feedbackRequest == null)
                {
                    continue;
                }

                if (!existingNotificationsDict.ContainsKey(feedback.id))
                {
                    var notification = new Notification
                    {
                        CreatedAt = DateTime.Now,
                        Id = Guid.NewGuid(),
                        Message = $"Bola zaznamenená neodoslaná spätná väzba {feedback.feedbackRequest.title}. Prosím vráťte sa a dokončite operáciu!",
                        IsRead = false,
                        NotificationType = EnumNotificationType.FeedbackUnsentReminderNotificationType,
                        ReferencedItemId = feedback.id,
                        User = feedback.employee.User
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

                try
                {
                    await hubContext.Clients.User(notification.User.Id)
                        .SendAsync("ReceiveNotification", response, cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending notification: {ex.Message}");
                }
            }

            if (notifications.Any())
            {
                dbContext.Notifications.AddRange(notifications);
                await dbContext.SaveChangesAsync();
            }
        }

    }
}
