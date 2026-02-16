using System.Security.Cryptography.X509Certificates;
using LabZakazivanjeAPI.Helpers;
using LabZakazivanjeAPI.Models;
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

    public async Task<ServiceResult<IEnumerable<Room>>> GetRooms()
    {
        var rooms = await m_context.Rooms.ToListAsync();
        return ServiceResult<IEnumerable<Room>>.Ok(rooms);
    }

    public async Task<ServiceResult<Room>> AddRooms(Room r)
    {
        Room room = new Room
        {
            Naziv = r.Naziv,
            Raspored = r.Raspored,
            Capacity = r.Capacity
        };

        await m_context.Rooms.AddAsync(room);
        await m_context.SaveChangesAsync();

        return ServiceResult<Room>.Ok(room);
    }
}