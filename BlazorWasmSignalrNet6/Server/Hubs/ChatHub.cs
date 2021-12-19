using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BlazorWasmSignalrNet6.Server.Hubs;

[Authorize]
public class ChatHub : Hub
{
    [Authorize]
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}