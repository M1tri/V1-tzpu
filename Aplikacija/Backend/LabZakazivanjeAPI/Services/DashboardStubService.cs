using System.Collections.Immutable;
using LabZakazivanjeAPI.Clients.Interfaces;

namespace LabZakazivanjeAPI.Services;

public class DashboardStubService : BackgroundService
{
    private readonly IConfiguration m_configuration;

    private readonly int sessijaId = 25;
   
    private record SequenceElement(bool Provide, int SeatId, int UserId, int TimeDelaySeconds);

    private readonly ImmutableArray<SequenceElement> Sequence = ImmutableArray.Create(
        new SequenceElement(false, 1, 101, 5),
        new SequenceElement(false, 2, 102, 5),
        new SequenceElement(false, 2, 103, 25),
        new SequenceElement(true, 1, 101, 10),
        new SequenceElement(true, 1, 101, 10),
        new SequenceElement(true, 1, 101, 10)
    );

    private readonly IDashboardClient m_dashboardClient;

    public DashboardStubService(IConfiguration configuration, IDashboardClient dashboardClient)
    {
        m_configuration = configuration;
        m_dashboardClient = dashboardClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var targetTime = m_configuration.GetValue<DateTime>("Dashboard:RunOnceAt");

        var now = DateTime.UtcNow;

        if (now < targetTime)
        {
            var delay = targetTime - now;
            await Task.Delay(delay, stoppingToken);
        }

        foreach (var seq in Sequence)
        {
            if (seq.Provide)
            {
                await m_dashboardClient.Provide(sessijaId, seq.SeatId, seq.UserId);
            }
            else
            {
                await m_dashboardClient.Release(sessijaId, seq.SeatId, seq.UserId);                
            }
            await Task.Delay(TimeSpan.FromSeconds(seq.TimeDelaySeconds), stoppingToken);
        }
    }
}