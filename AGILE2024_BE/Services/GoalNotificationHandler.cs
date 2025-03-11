
using AGILE2024_BE.Data;
using AGILE2024_BE.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AGILE2024_BE.Services
{
    public class GoalNotificationHandler : INotificationHandler
    {
        private readonly AgileDBContext dbContext;
        private readonly IHubContext<NotificationHub> hubContext;

        public GoalNotificationHandler(AgileDBContext dbContext, IHubContext<NotificationHub> hubContext)
        {
            this.dbContext = dbContext;
            this.hubContext = hubContext;
        }

        public async Task HandleNotificationAsync(CancellationToken cancellationToken)
        {
            //await HandleGoalUnsentReminder(cancellationToken);
            await HandleGoalUnfinishedReminder(cancellationToken);
        }

        private async Task HandleGoalUnfinishedReminder(CancellationToken cancellationToken)
        {
          var upcomingGoals = await dbContext.Goals.Include(x => x.status)
                                                     .Include(x => x.employee)
                                                     .Include(x => x.employee.User)
                                                     .Include(x => x.category)
                                                     .Where(goal => goal.dueDate <= DateTime.UtcNow.AddDays(1) &&
                                                            goal.dueDate >= DateTime.UtcNow &&
                                                           (goal.status.description.Equals("Prebiehajúci") || goal.status.description.Equals("Nezačatý")))
                                                     .ToListAsync();

            var existingNotifications = await dbContext.Notifications
                .Where(n => upcomingGoals.Select(y => y.id).Contains(n.ReferencedItemId) &&
                            n.NotificationType == EnumNotificationType.GoalUnfinishedReminderNotificationType)
                .ToListAsync();

            List<Notification> notifications = new List<Notification>();

            foreach (var goal in upcomingGoals)
            {
                var existingNotification = existingNotifications.FirstOrDefault(n => n.ReferencedItemId == goal.id);

                if (existingNotification == null)
                {
                    var notification = new Notification
                    {
                        Id = Guid.NewGuid(),
                        User = goal.employee.User,
                        ReferencedItemId = goal.id,
                        Message = $"Termín pre splnenie Vášho priradeného cieľa '{goal.name}' sa blíži. Prosím, venujte pozornosť jeho dokončeniu !!",
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false,
                        NotificationType = EnumNotificationType.GoalUnfinishedReminderNotificationType
                    };

                    notifications.Add(notification);

                    var notificationResponse = new NotificationResponse
                    {
                        Id = notification.Id,
                        Message = notification.Message,
                        Title = NotificationHelpers.GetNotificationTitle(notification.NotificationType),
                        ReferencedItem = goal.id.ToString(),
                        NotificationType = notification.NotificationType,
                        CreatedAt = notification.CreatedAt,
                        IsRead = notification.IsRead
                    };

                    await hubContext.Clients.User(goal.employee.User.Id)
                        .SendAsync("ReceiveNotification", notificationResponse, cancellationToken);
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
