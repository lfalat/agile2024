
using AGILE2024_BE.Data;
using AGILE2024_BE.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AGILE2024_BE.Services
{
    public class DummyNotificationHandler : INotificationHandler
    {
        private readonly AgileDBContext dbContext;
        private readonly IHubContext<NotificationHub> hubContext;

        public DummyNotificationHandler(AgileDBContext dbContext, IHubContext<NotificationHub> hubContext)
        {
            this.dbContext = dbContext;
            this.hubContext = hubContext;
        }

        public async Task HandleNotificationAsync(CancellationToken cancellationToken)
        {
            var upcomingGoals = await dbContext.Goals.Include(x => x.status)
                                                     .Include(x => x.employee)
                                                     .Include(x => x.employee.User)
                                                     .Include(x => x.category)
                                                     .Where(goal => goal.dueDate <= DateTime.UtcNow.AddDays(1) &&
                                                           (goal.status.description.Equals("Prebiehajúci") || goal.status.description.Equals("Nezačatý")))
                                                     .ToListAsync();

            var notificationResponse = new NotificationResponse
            {
                Id = Guid.Empty,
                CreatedAt = DateTime.UtcNow,
                Title = "Title",
                Message = "Sprava notifikacie",
                IsRead = false,
                NotificationType = EnumNotificationType.ReviewUnsentReminderNotificationType,
                
            };

            await hubContext.Clients.All.SendAsync("ReceiveNotification", notificationResponse, cancellationToken);
        }
    }
}
