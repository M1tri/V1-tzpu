using Xunit;
using Microsoft.EntityFrameworkCore;
using LabZakazivanjeAPI.Services;
using LabZakazivanjeAPI.Services.Interfaces;
using LabZakazivanjeAPI.Models;
using Moq;
using Microsoft.AspNetCore.Mvc.Abstractions;
using System.Threading.Tasks;

namespace Test;

public class SessionTest
{
    private AppDBContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDBContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDBContext(options);
    }

    private void CommonSetup(AppDBContext context, out Room r, out Activity a)
    {
        Room room = new Room
        {
            Naziv = "R1",
            Capacity = 2,
            Raspored = "((1:12.12.12.1, 2:12.12.12.2))"
        };
        context.Rooms.Add(room);
        context.SaveChanges();

        ActivityClass ac = new ActivityClass
        {
            Naziv = "AOR-LAB-VEZBE"
        };
        context.ActivityClasses.Add(ac);
        context.SaveChanges();

        Activity activity = new Activity
        {
            Name = "AOR-LV1",
            Tip = ac,
            VLRIDS = new List<string> { "racunar" }
        };
        context.Activities.Add(activity);
        context.SaveChanges();

        r = room;
        a = activity;
    }

    [Fact]
    public async Task TestGetSessionWhenExists()
    {
        var context = GetDbContext();

        var vlrMock = new Mock<IVLRService>();
        var service = new SessionService(context, vlrMock.Object);

        CommonSetup(context, out Room r, out Activity a);

        Session s = new Session
        {
            Datum = DateOnly.FromDateTime(DateTime.Now),
            VremePocetka = TimeOnly.FromDateTime(DateTime.Now),
            VremeKraja = TimeOnly.FromDateTime(DateTime.Now),
            AutomatskiPocetak = false,
            AutomatskiKraj = false,
            AutomatskoStanjeZavrsavanja = null,
            Stanje = SessionState.PLANNED,
            Prostorija = r,
            Aktivnost = a
        };

        context.Sessions.Add(s);
        context.SaveChanges();

        var result = await service.GetSession(s.Id);

        Assert.True(result.Success);
        Assert.Equal(s.Id, result.Data!.Id);
    }

    [Fact]
    public async Task TestPromoteAsNext()
    {
        var context = GetDbContext();
        var vlrMock = new Mock<IVLRService>();
        var service = new SessionService(context, vlrMock.Object);

        CommonSetup(context, out Room r, out Activity a);

        Session s = new Session
        {
            Datum = DateOnly.FromDateTime(DateTime.Now),
            VremePocetka = TimeOnly.FromDateTime(DateTime.Now),
            VremeKraja = TimeOnly.FromDateTime(DateTime.Now),
            AutomatskiPocetak = false,
            AutomatskiKraj = false,
            AutomatskoStanjeZavrsavanja = null,
            Stanje = SessionState.PLANNED,
            Prostorija = r,
            Aktivnost = a
        };

        context.Sessions.Add(s);
        context.SaveChanges();

        var result = await service.PromoteAsNext(s.Id);

        Assert.True(result.Success);
    }
    
    [Fact]
    public async Task ActivateSessionWhenExists()
    {
        var context = GetDbContext();
        var vlrMock = new Mock<IVLRService>();
        var service = new SessionService(context, vlrMock.Object);

        CommonSetup(context, out Room r, out Activity a);

        Session s = new Session
        {
            Datum = DateOnly.FromDateTime(DateTime.Now),
            VremePocetka = TimeOnly.FromDateTime(DateTime.Now),
            VremeKraja = TimeOnly.FromDateTime(DateTime.Now),
            AutomatskiPocetak = false,
            AutomatskiKraj = false,
            AutomatskoStanjeZavrsavanja = null,
            Stanje = SessionState.PLANNED,
            Prostorija = r,
            Aktivnost = a
        };

        context.Sessions.Add(s);
        context.SaveChanges();

        await service.PromoteAsNext(s.Id);

        var result = await service.Activate(s.Id);

        Assert.True(result.Success);
    }
    
}