using LabZakazivanjeAPI.Models;
using LabZakazivanjeAPI.Notifications;
using LabZakazivanjeAPI.Services;
using LabZakazivanjeAPI.Services.Interfaces;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

public class SchedulerNotification
{
    public bool Success {get; set;}
    public int? RoomId {get; set;}
    public string? Message {get; set;}
}

public class TimeSchedulerService : BackgroundService
{
    private readonly IServiceScopeFactory m_scopeFactory;
    private readonly IHubContext<SessionSchedulerNotificationHub> m_hubContext;

    public TimeSchedulerService(IServiceScopeFactory scopeFactory, IHubContext<SessionSchedulerNotificationHub> hubContext)
    {
        m_scopeFactory = scopeFactory;
        m_hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(60));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            using var scope = m_scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();

            var sessionService = scope.ServiceProvider.GetRequiredService<ISessionService>();

            var dan = DateOnly.FromDateTime(DateTime.Now);
            var vreme = TimeOnly.FromDateTime(DateTime.Now);

            Console.WriteLine($"Proveravam za {dan} {vreme}");

            var sve = await context.Sessions.ToListAsync(cancellationToken : stoppingToken);
            foreach (var s in sve)
            {
                Console.WriteLine($"{s.Id} {s.Datum} {s.VremePocetka} {s.VremeKraja}");
            }

            var sesijeZaTerminate = await context.Sessions
            .Where(s => s.AutomatskiKraj &&
                        s.Datum == dan &&
                        s.VremeKraja < vreme &&
                        s.Stanje == SessionState.ACTIVE)
            .ToListAsync(cancellationToken : stoppingToken);

            foreach (var s in sesijeZaTerminate)
            {
                ServiceResult<string> result;
                if (s.AutomatskoStanjeZavrsavanja == SessionState.FADING)
                {
                    result = await sessionService.Fade(s.Id);
                }
                else
                {
                    result = await sessionService.Terminate(s.Id);
                }

                if (result.Success)
                {
                    await m_hubContext.Clients.All.SendAsync(
                        "ReceiveSchedulerNotification",
                        new SchedulerNotification
                        {
                            Success = true,
                            RoomId = s.RoomId
                        },
                        cancellationToken: stoppingToken
                    );
                }
                else
                {
                    await m_hubContext.Clients.All.SendAsync(
                        "ReceiveSchedulerNotification",
                        new SchedulerNotification
                        {
                            Success = false,
                            Message = result.ErrorMessage
                        },
                        cancellationToken: stoppingToken
                    );
                }
            }

            var sesijeZaActive = await context.Sessions
            .Where(s => s.AutomatskiPocetak &&
                        s.Datum == dan && 
                        s.VremePocetka < vreme &&   
                        s.VremeKraja > vreme && 
                        s.Stanje == SessionState.NEXT
                    )
            .ToListAsync(cancellationToken: stoppingToken);

            foreach (var s in sesijeZaActive)
            {
                var result = await sessionService.Activate(s.Id);

                if (result.Success)
                {
                    await m_hubContext.Clients.All.SendAsync(
                        "ReceiveSchedulerNotification",
                        new SchedulerNotification
                        {
                            Success = true,
                            RoomId = s.RoomId
                        },
                        cancellationToken: stoppingToken
                    );
                }
                else
                {
                    await m_hubContext.Clients.All.SendAsync(
                        "ReceiveSchedulerNotification",
                        new SchedulerNotification
                        {
                            Success = false,
                            Message = result.ErrorMessage
                        },
                        cancellationToken: stoppingToken
                    );
                }
            }
        }
    }
}