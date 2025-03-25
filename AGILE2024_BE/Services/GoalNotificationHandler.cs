
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
            List<Goal> upcomingGoals = new List<Goal>();
            try
            {
                upcomingGoals = await dbContext.Goals
                    .Include(x => x.status)
                    .Include(x => x.employee)
                    .Include(x => x.employee.User)
                    .Include(x => x.category)
                    .Where(goal => goal.dueDate <= DateTime.UtcNow.AddDays(1) &&
                                   goal.dueDate >= DateTime.UtcNow &&
                                   (goal.status.description.Equals("Prebiehajúci") || goal.status.description.Equals("Nezačatý")))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching upcoming goals: {ex.Message}");
                return;
            }

            List<Notification> existingNotifications = new List<Notification>();
            try
            {
                existingNotifications = await dbContext.Notifications
                    .Where(n => upcomingGoals.Select(y => y.id).Contains(n.ReferencedItemId) &&
                                n.NotificationType == EnumNotificationType.GoalUnfinishedReminderNotificationType)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching existing notifications: {ex.Message}");
                return;
            }

            var existingNotificationsDict = existingNotifications.ToDictionary(n => n.ReferencedItemId);

            List<Notification> notifications = new List<Notification>();

            foreach (var goal in upcomingGoals)
            {
                if (goal.employee?.User == null)
                {
                    continue;
                }

                try
                {
                    if (!existingNotificationsDict.ContainsKey(goal.id))
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

                        try
                        {
                            await hubContext.Clients.User(goal.employee.User.Id)
                                .SendAsync("ReceiveNotification", notificationResponse, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error sending notification to user {goal.employee.User.Id}: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing goal {goal.id}: {ex.Message}");
                }
            }

            if (notifications.Any())
            {
                try
                {
                    dbContext.Notifications.AddRange(notifications);
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving notifications to DB: {ex.Message}");
                }
            }
        }



    }
}
