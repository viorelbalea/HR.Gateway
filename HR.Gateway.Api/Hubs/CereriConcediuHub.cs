using Microsoft.AspNetCore.SignalR;

namespace HR.Gateway.Api.Hubs;

/// <summary>
/// SignalR hub pentru notificări real-time despre schimbări de status ale cererilor de concediu
/// </summary>
public sealed class CereriConcediuHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
