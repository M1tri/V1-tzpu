namespace LabZakazivanjeAPI.Controllers;

using LabZakazivanjeAPI.Models;
using LabZakazivanjeAPI.Models.DTOs;
using LabZakazivanjeAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/rooms")]
public class RoomsController : ControllerBase
{
    private readonly IRoomsService m_roomsService;

    public RoomsController(IRoomsService roomsService)
    {
        m_roomsService = roomsService;
    }

    [HttpGet("GetRooms")]
    public async Task<ActionResult<IEnumerable<ViewRoomDTO>>> GetRooms()
    {
        var rooms = await m_roomsService.GetRooms();

        if (rooms.Success)
        {
            return Ok(rooms.Data);
        }
        else
        {
            return BadRequest(rooms.ErrorMessage);
        }
    }

    [HttpPost("AddRoom")]
    public async Task<ActionResult<ViewRoomDTO>> AddRoom([FromBody] CreateRoomDTO r)
    {
        var room = await m_roomsService.AddRooms(r);

        if (room.Success)
        {
            return Ok(room.Data);
        }
        else
        {
            return BadRequest(room.ErrorMessage);
        }
    }
}