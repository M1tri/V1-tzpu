using System.Diagnostics;
using System.Runtime.CompilerServices;
using LabZakazivanjeAPI.Models;

namespace LabZakazivanjeAPI.Services.Interfaces;

public interface IRoomsService
{
    Task<ServiceResult<IEnumerable<Room>>> GetRooms();
    Task<ServiceResult<Room>> AddRooms(Room r);
}