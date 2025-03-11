using AGILE2024_BE.Data;
using AGILE2024_BE.Models;
using AGILE2024_BE.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace AGILE2024_BE
{
    public interface INotificationHandler
    {
        public Task HandleNotificationAsync(CancellationToken cancellationToken);
    }

    public class NotificationService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IServiceProvider serviceProvider;
        private readonly IHubContext<NotificationHub> hubContext;
        private readonly List<INotificationHandler> notificationHandlers;

        public NotificationService(IServiceScopeFactory scopeFactory, IServiceProvider serviceProvider, IHubContext<NotificationHub> hubContext)
        {
            this.scopeFactory = scopeFactory;
            this.serviceProvider = serviceProvider;
            this.hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var handlers = scope.ServiceProvider.GetServices<INotificationHandler>();

                    foreach (var handler in handlers)
                    {
                        await handler.HandleNotificationAsync(stoppingToken);
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}

