using System.Security.Cryptography.X509Certificates;
using LabZakazivanjeAPI.Helpers;
using LabZakazivanjeAPI.Models;
using LabZakazivanjeAPI.Models.DTOs;
using LabZakazivanjeAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LabZakazivanjeAPI.Services;

public class RoomsService : IRoomsService
{
    private readonly AppDBContext m_context;

    public RoomsService(AppDBContext context)
    {
        m_context = context;
    }

    public async Task<ServiceResult<IEnumerable<ViewRoomDTO>>> GetRooms()
    {
        var rooms = await m_context.Rooms.ToListAsync();

        List<ViewRoomDTO> roomViews = [];

        foreach (var r in rooms)
        {
            List<List<Seat>> raspored = RoomRasporedParser.ParseRaspored(r.Raspored);

            roomViews.Add(new ViewRoomDTO
            {
                Id = r.Id,
                Naziv = r.Naziv,
                Capacity = r.Capacity,
                Raspored = raspored
            });
        }

        return ServiceResult<IEnumerable<ViewRoomDTO>>.Ok(roomViews);
    }

    public async Task<ServiceResult<ViewRoomDTO>> GetRoom(int roomId)
    {
        var room = await m_context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);

        if (room == null)
            return ServiceResult<ViewRoomDTO>.Error("Ne postoji soba");

        ViewRoomDTO view = new ViewRoomDTO
        {
            Id = room.Id,
            Naziv = room.Naziv,
            Capacity = room.Capacity,
            Raspored = RoomRasporedParser.ParseRaspored(room.Raspored)
        };

        return ServiceResult<ViewRoomDTO>.Ok(view);
    }

    public async Task<ServiceResult<ViewRoomDTO>> AddRooms(CreateRoomDTO r)
    {
        Room room = new Room
        {
            Naziv = r.Naziv,
            Raspored = r.Raspored,
            Capacity = r.Capacity
        };

        await m_context.Rooms.AddAsync(room);
        await m_context.SaveChangesAsync();

        ViewRoomDTO view = new ViewRoomDTO
        {
            Id = room.Id,
            Naziv = room.Naziv,
            Capacity = room.Capacity,
            Raspored = RoomRasporedParser.ParseRaspored(room.Raspored)
        };

        return ServiceResult<ViewRoomDTO>.Ok(view);
    }
}