using Microsoft.AspNetCore.SignalR;

namespace WebApp.Hubs;

public class GameHub : Hub
{
    public async Task JoinGame(string gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
    }

    public async Task LeaveGame(string gameId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
    }

    public async Task NotifyMove(string gameId, int column)
    {
        await Clients.Group(gameId).SendAsync("ReceiveMove", column);
    }

    public async Task NotifyGameUpdate(string gameId)
    {
        await Clients.Group(gameId).SendAsync("GameUpdated");
    }
}