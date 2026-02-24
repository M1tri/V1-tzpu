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
using LabZakazivanjeAPI.Clients.Interfaces;
using Castle.Components.DictionaryAdapter.Xml;

namespace Test;

public class VLRTest
{
    private AppDBContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDBContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDBContext(options);
    }

    private void SetupRoomActivityAndStatuses(AppDBContext context, out Room r, out Activity a) 
    {
        r = new Room { Naziv = "R1", Capacity = 2, Raspored = "((1:12.12.12.1, 2:12.12.12.2))" };
        context.Rooms.Add(r);

        ActivityClass ac = new ActivityClass { Naziv = "AOR-LAB-VEZBE" };
        context.ActivityClasses.Add(ac);

        a = new Activity { Name = "AOR-LV1", Tip = ac, VLRIDS = new List<string> { "racunar" } };
        context.Activities.Add(a);

        context.VLRStatusInfos.AddRange(new VLRStatusInfo[]
        {
            new() { Naziv = "NULL", Symbol = "N", Color = "Gray" },
            new() { Naziv = "READY", Symbol = "R", Color = "Green" }
        });

        context.SaveChanges();
    }

    private Mock<IInfrastructureClient> GetInfrastructureMock()
    {
        var infraMock = new Mock<IInfrastructureClient>();

        infraMock.Setup(c => c.CloneVM(It.IsAny<string>()))
                .ReturnsAsync((true, "vlr123"));

        infraMock.Setup(c => c.PrepareVM(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);

        infraMock.Setup(c => c.SetVMIp(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(true);

        return infraMock;
    }

    private Session AddSession(AppDBContext context)
    {
        SetupRoomActivityAndStatuses(context, out Room r, out Activity a);

        Session session = new Session
        {
            Datum = DateOnly.FromDateTime(DateTime.Now),
            VremePocetka = TimeOnly.FromDateTime(DateTime.Now),
            VremeKraja = TimeOnly.FromDateTime(DateTime.Now),
            Stanje = SessionState.ACTIVE,
            Prostorija = r,
            Aktivnost = a,
            AutomatskiPocetak = false,
            AutomatskiKraj = false,
            AutomatskoStanjeZavrsavanja = null
        };

        context.Sessions.Add(session);
        context.SaveChanges();

        return session;        
    }

    [Fact]
    public async Task TestGenerateIdleVLRs()
    {
        var context = GetDbContext();

        var infraMock = GetInfrastructureMock();
        var service = new VLRService(context, infraMock.Object);

        Session session = AddSession(context);
        var result = await service.GenerateIdleVLRs(session.Aktivnost!.VLRIDS[0], session.Id, session.Prostorija!.Id, 1);

        Assert.True(result.Success);
        var vlrs = context.ActiveVLRs.ToList();
        Assert.Single(vlrs);
        Assert.Equal(VLRStatus.GENERATED_IDLE, vlrs[0].Status);
    }

    [Fact]
    public async Task TestPrepareIdleVLRsToReady()
    {
        var context = GetDbContext();

        var infraMock = GetInfrastructureMock();
        var service = new VLRService(context, infraMock.Object);

        Session session = AddSession(context);

        var vlr = new ActiveVLR 
        {
            VLRID = "vlr1",
            SesijaVlasnik = session,
            RoomId = session.RoomId, 
            Status = VLRStatus.GENERATED_IDLE,
            LastStatusChange = DateTime.Now 
        };

        context.ActiveVLRs.Add(vlr);

        context.SaveChanges();

        var result = await service.PrepareVLR(session.Id, 1, session.RoomId);

        Assert.True(result.Success);
        Assert.Equal(VLRStatus.READY, result.Data!.Status);
    }

    [Fact]
    public async Task TestProivdeReadyVLR()
    {
         var context = GetDbContext();

        var infraMock = GetInfrastructureMock();
        var service = new VLRService(context, infraMock.Object);

        Session session = AddSession(context);

        var vlr = new ActiveVLR 
        {
            VLRID = "vlr1",
            SesijaVlasnik = session,
            RoomId = session.RoomId, 
            Status = VLRStatus.READY,
            LastStatusChange = DateTime.Now,
            SeatId = 1,
            IP = "12.12.12.1"
        };

        context.ActiveVLRs.Add(vlr);

        context.SaveChanges();

        var result = await service.ProvideVLR(session.Id, 1, 101);

        Assert.True(result.Success);
        Assert.Equal(VLRStatus.PROVIDED, result.Data!.Status);
        Assert.Equal(101, result.Data.UserId);      
    }

    [Fact]    
    public async Task TestReleaseProvidedVLR()
    {
        var context = GetDbContext();

        var infraMock = GetInfrastructureMock();
        var service = new VLRService(context, infraMock.Object);

        Session session = AddSession(context);

        var vlr = new ActiveVLR 
        {
            VLRID = "vlr1",
            SesijaVlasnik = session,
            RoomId = session.RoomId, 
            Status = VLRStatus.READY,
            LastStatusChange = DateTime.Now,
            SeatId = 1,
            UserId = 101,
            IP = "12.12.12.1"
        };

        context.ActiveVLRs.Add(vlr);

        context.SaveChanges();

        var result = await service.ReleaseVLR(session.Id, 1, 101);

        Assert.True(result.Success);
        Assert.Equal(VLRStatus.RELEASED, result.Data!.Status);
    }

       [Fact]    
    public async Task TestReleaseReadyVLR()
    {
        var context = GetDbContext();

        var infraMock = GetInfrastructureMock();
        var service = new VLRService(context, infraMock.Object);

        Session session = AddSession(context);

        var vlr = new ActiveVLR 
        {
            VLRID = "vlr1",
            SesijaVlasnik = session,
            RoomId = session.RoomId, 
            Status = VLRStatus.READY,
            LastStatusChange = DateTime.Now,
            SeatId = 1,
            IP = "12.12.12.1"
        };

        context.ActiveVLRs.Add(vlr);

        context.SaveChanges();

        var result = await service.ReleaseVLR(session.Id, 1, 101);

        Assert.False(result.Success);
        Assert.Equal(VLRStatus.READY, vlr.Status);
    }

    [Fact]    
    public async Task TestProvideReleasedVLR()
    {
        var context = GetDbContext();

        var infraMock = GetInfrastructureMock();
        var service = new VLRService(context, infraMock.Object);

        Session session = AddSession(context);

        var vlr = new ActiveVLR 
        {
            VLRID = "vlr1",
            SesijaVlasnik = session,
            RoomId = session.RoomId, 
            Status = VLRStatus.RELEASED,
            LastStatusChange = DateTime.Now,
            SeatId = 1,
            IP = "12.12.12.1"
        };

        context.ActiveVLRs.Add(vlr);

        context.SaveChanges();

        var result = await service.ProvideVLR(session.Id, 1, 101);

        Assert.False(result.Success);
        Assert.Equal(VLRStatus.RELEASED, vlr.Status);
    }
}