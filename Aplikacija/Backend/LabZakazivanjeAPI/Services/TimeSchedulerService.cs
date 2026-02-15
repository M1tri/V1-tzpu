using LabZakazivanjeAPI.Models;
using Microsoft.EntityFrameworkCore;

public class TimeSchedulerService : BackgroundService
{
    private readonly IServiceScopeFactory m_scopeFactory;

    public TimeSchedulerService(IServiceScopeFactory scopeFactory)
    {
        m_scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(60));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            using var scope = m_scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();

            var dan = DateOnly.FromDateTime(DateTime.Now);
            var vreme = TimeOnly.FromDateTime(DateTime.Now);

            var sesijeZaNext = await context.Sessions
            .Where(s => s.AutomatskiPocetak &&
                        s.Datum == dan && 
                        s.VremePocetka < vreme &&   
                        s.VremeKraja > vreme
                        && s.Stanje == SessionState.PLANNED
                    )
            .ToListAsync();


        }
    }
}