namespace LabZakazivanjeAPI.Controllers;

using LabZakazivanjeAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/rooms")]
public class RoomsController : ControllerBase
{
    private readonly AppDBContext m_context;

    public RoomsController(AppDBContext context)
    {
        m_context = context;
    }

    [HttpGet("GetRooms")]
    public async Task<IEnumerable<Room>> GetRooms()
    {
        return await m_context.Rooms.ToListAsync();
    }

    [HttpPost("AddRoom")]
    public async Task<ActionResult<Room>> AddRoom([FromBody] Room r)
    {
        Room room = new Room
        {
            Naziv = r.Naziv,
            Raspored = r.Raspored
        };

        await m_context.Rooms.AddAsync(room);
        await m_context.SaveChangesAsync();
        return Ok(room);
    }
}