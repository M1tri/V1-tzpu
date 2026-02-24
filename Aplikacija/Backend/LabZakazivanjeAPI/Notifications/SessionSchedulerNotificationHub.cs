namespace LabZakazivanjeAPI.Notifications;

using Microsoft.AspNetCore.SignalR;

public class SessionSchedulerNotificationHub : Hub
{
    public async Task SendNotification(string message)
    {
        await Clients.All.SendAsync("ReceiveNotification", message);
    }
}