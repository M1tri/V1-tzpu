using Xunit;
using Microsoft.EntityFrameworkCore;
using LabZakazivanjeAPI.Services;
using LabZakazivanjeAPI.Services.Interfaces;
using LabZakazivanjeAPI.Models;
using Moq;
using Microsoft.AspNetCore.Mvc.Abstractions;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Connections;

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

    private void SetupRoomAndActivity(AppDBContext context, out Room r, out Activity a)
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

    private Mock<IVLRService> GetVlrMock()
    {
        var vlrMock = new Mock<IVLRService>();
        ActiveVLR vlr = new ActiveVLR
        {
            VLRID = "racunar",
            LastStatusChange = DateTime.Now,
            Status = "ready"
        };

        vlrMock.Setup(v => v.PrepareVLR(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
        .ReturnsAsync(ServiceResult<ActiveVLR>.Ok(vlr));

        return vlrMock;
    }

    private Session AddTestSession(AppDBContext context)
    {
        SetupRoomAndActivity(context, out Room r, out Activity a);

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

        return s;
    }

    [Fact]
    public async Task TestGetSessionWhenExists()
    {
        var context = GetDbContext();

        var vlrMock = GetVlrMock();
        var service = new SessionService(context, vlrMock.Object);

        SetupRoomAndActivity(context, out Room r, out Activity a);

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
        var vlrMock = GetVlrMock();
        
        var service = new SessionService(context, vlrMock.Object);

        Session s = AddTestSession(context);

        var result = await service.PromoteAsNext(s.Id);

        Assert.True(result.Success);
        Assert.Equal(SessionState.NEXT, s.Stanje);
    }
    
    [Fact]
    public async Task DemoteToPlanned()
    {
        var context = GetDbContext();
        var vlrMock = GetVlrMock();

        var service = new SessionService(context, vlrMock.Object);

        Session s = AddTestSession(context);
        await service.PromoteAsNext(s.Id);

        var result = await service.DemoteToPlanned(s.Id);

        Assert.True(result.Success);
        Assert.Equal(SessionState.PLANNED, s.Stanje);
    }

    [Fact]
    public async Task TestActivateSessionWhenExists()
    {
        var context = GetDbContext();
        var vlrMock = GetVlrMock();
        
        var service = new SessionService(context, vlrMock.Object);

        Session s = AddTestSession(context);
        await service.PromoteAsNext(s.Id);

        var result = await service.Activate(s.Id);

        Assert.True(result.Success);
        Assert.Equal(SessionState.ACTIVE, s.Stanje);
    }

    [Fact]
    public async Task TestFadeSessionWhenExists()
    {
        var context = GetDbContext();
        var vlrMock = GetVlrMock();
        
        var service = new SessionService(context, vlrMock.Object);

        Session s = AddTestSession(context);

        await service.PromoteAsNext(s.Id);
        await service.Activate(s.Id);

        var result = await service.Fade(s.Id);

        Assert.True(result.Success);
        Assert.Equal(SessionState.FADING, s.Stanje);
    }

    [Fact]
    public async Task TestTerminateActiveSessionWhenExists()
    {
        var context = GetDbContext();
        var vlrMock = GetVlrMock();
        
        var service = new SessionService(context, vlrMock.Object);

        Session s = AddTestSession(context);

        await service.PromoteAsNext(s.Id);
        await service.Activate(s.Id);

        var result = await service.Terminate(s.Id);

        Assert.True(result.Success);
        Assert.Equal(SessionState.FINISHED, s.Stanje);
    }

    [Fact]
    public async Task TerminateFadingSessionWhenExists()
    {
        var context = GetDbContext();
        var vlrMock = GetVlrMock();
        
        var service = new SessionService(context, vlrMock.Object);

        Session s = AddTestSession(context);

        await service.PromoteAsNext(s.Id);
        await service.Activate(s.Id);
        await service.Fade(s.Id);

        var result = await service.Terminate(s.Id);

        Assert.True(result.Success);
        Assert.Equal(SessionState.FINISHED, s.Stanje);
    }

    [Fact]
    public async Task TestActivatePlannedSession()
    {
        var context = GetDbContext();
        var vlrMock = GetVlrMock();

        var service = new SessionService(context, vlrMock.Object);
        Session s = AddTestSession(context);
        
        var result = await service.Activate(s.Id);

        Assert.False(result.Success);
        Assert.Equal(SessionState.PLANNED, s.Stanje);
    }

    [Fact]
    public async Task TestTerminatePlannedSession()
    {
        var context = GetDbContext();
        var vlrMock = GetVlrMock();

        var service = new SessionService(context, vlrMock.Object);
        Session s = AddTestSession(context);
        
        var result = await service.Terminate(s.Id);

        Assert.False(result.Success);
        Assert.Equal(SessionState.PLANNED, s.Stanje);
    }
}