using Microsoft.AspNetCore.SignalR;
using FinancialProfileManagerAPI.Models;
using Serilog;

namespace FinancialProfileManagerAPI.Hubs;

public class FinancialHub : Hub
{
    public async Task SendNotification(string message)
    {
        await Clients.All.SendAsync("ReceiveNotification", message);
        Log.Information($"Notification sent: {message}");
    }

    public async Task UserCreated(UserProfile user)
    {
        await Clients.All.SendAsync("UserCreated", user);
        Log.Information($"User created notification sent: {user.Id}");
    }

    public async Task UserUpdated(UserProfile user)
    {
        await Clients.All.SendAsync("UserUpdated", user);
        Log.Information($"User updated notification sent: {user.Id}");
    }

    public async Task UserDeleted(Guid userId)
    {
        await Clients.All.SendAsync("UserDeleted", userId);
        Log.Information($"User deleted notification sent: {userId}");
    }

    public async Task TransactionAdded(string userName, UserTransaction transaction)
    {
        await Clients.All.SendAsync("TransactionAdded", new { userName, transaction });
        Log.Information($"Transaction notification sent for {userName}");
    }

    public override async Task OnConnectedAsync()
    {
        Log.Information($"Client connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        Log.Information($"Client disconnected: {Context.ConnectionId}");
        await base.OnDisconnectedAsync(exception);
    }
}
