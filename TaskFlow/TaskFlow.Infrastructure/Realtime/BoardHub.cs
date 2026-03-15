using Microsoft.AspNetCore.SignalR;
using TaskFlow.Application.Common.Interfaces;

namespace TaskFlow.API.Hubs;

public class BoardHub : Hub<BoardHub>
{
   
    public async Task JoinBoard(string boardId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"board:{boardId}");
    }

   
    public async Task LeaveBoard(string boardId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"board:{boardId}");
    }
}