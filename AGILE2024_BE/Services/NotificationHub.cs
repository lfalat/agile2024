﻿using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace AGILE2024_BE
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
                Console.WriteLine($"User {userId} connected with ConnectionId: {Context.ConnectionId}");
            }

            await base.OnConnectedAsync();
        }
    }
}
