using System.Diagnostics;
using System.Runtime.CompilerServices;
using LabZakazivanjeAPI.Models;
using LabZakazivanjeAPI.Models.DTOs;

namespace LabZakazivanjeAPI.Services.Interfaces;

public interface IRoomsService
{
    Task<ServiceResult<IEnumerable<ViewRoomDTO>>> GetRooms();
    Task<ServiceResult<ViewRoomDTO>> AddRooms(CreateRoomDTO r);
}