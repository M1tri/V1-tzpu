using Xunit;
using Microsoft.EntityFrameworkCore;
using LabZakazivanjeAPI.Services;
using LabZakazivanjeAPI.Services.Interfaces;
using LabZakazivanjeAPI.Models;
using Moq;
using Microsoft.AspNetCore.Mvc.Abstractions;
using System.Threading.Tasks;
using LabZakazivanjeAPI.Models.DTOs;

namespace Test;

public class RoomTest
{
    private AppDBContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDBContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDBContext(options);
    }

    private void CommonSetup(AppDBContext context, out Room r)
    {
        Room room = new Room
        {
            Naziv = "R1",
            Capacity = 10,
            Raspored = "((1:12.12.12.1,2:12.12.12.2))"
        };

        context.Rooms.Add(room);
        context.SaveChanges();

        r = room;
    }

    [Fact]
    public async Task TestGetRooms()
    {
        var context = GetDbContext();
        var service = new RoomsService(context);

        CommonSetup(context, out Room r1);
        CommonSetup(context, out Room r2);

        var result = await service.GetRooms();

        Assert.True(result.Success);
        Assert.Equal(2, result.Data!.Count());
    }

    [Fact]
    public async Task TestGetRoomWhenExists()
    {
        var context = GetDbContext();
        var service = new RoomsService(context);

        CommonSetup(context, out Room r);

        var result = await service.GetRoom(r.Id);

        Assert.True(result.Success);
        Assert.Equal(r.Id, result.Data!.Id);
        Assert.Equal(r.Naziv, result.Data.Naziv);
    }

    [Fact]
    public async Task TestGetRoomWhenNotExists()
    {
        var context = GetDbContext();
        var service = new RoomsService(context);

        var result = await service.GetRoom(100);

        Assert.False(result.Success);
        Assert.Equal("Ne postoji soba", result.ErrorMessage);
    }

    [Fact]
    public async Task TestAddRooms()
    {
        var context = GetDbContext();
        var service = new RoomsService(context);

        CreateRoomDTO room = new CreateRoomDTO
        {
            Naziv = "R1",
            Capacity = 2,
            Raspored = "((1:12.12.12.1,2:12.12.12.2))"
        };

        var result = await service.AddRooms(room);

        Assert.True(result.Success);
        Assert.Equal(room.Naziv, result.Data!.Naziv);
        Assert.Equal(room.Capacity, result.Data.Capacity);
    }
}