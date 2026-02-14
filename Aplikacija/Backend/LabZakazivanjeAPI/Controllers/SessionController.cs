using LabZakazivanjeAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LabZakazivanjeAPI.Controllers;

[ApiController]
[Route("api/sessions")]
public class SessionController : ControllerBase
{
    private readonly AppDBContext m_context;

    public SessionController(AppDBContext context)
    {
        m_context = context;
    }

    [HttpGet("GetSessions")]
    public async Task<IEnumerable<Session>> GetSessions()
    {
        return await m_context.Sessions.ToListAsync();
    }

    [HttpPost("AddSession")]
    public async Task<ActionResult<Session>> AddSession([FromBody] Session s)
    {
        var room = await m_context.Rooms.Where(r => r.Id == s.RoomId).FirstOrDefaultAsync();
        if (room == null)
        {
            return BadRequest("Nepostojeća soba");
        }

        var activity = await m_context.Activities.Where(a => a.Id == s.ActivityId).FirstOrDefaultAsync();
        if (activity == null)
        {
            return BadRequest("Nepostojeća aktivnost!");
        }

        Session sesija = new Session
        {
            Prostorija = room,
            Aktivnost = activity,
            Datum = s.Datum,
            VremePocetka = s.VremePocetka,
            VremeKraja = s.VremeKraja,
            AutomatskiPocetak = s.AutomatskiPocetak,
            AutomatskiKraj = s.AutomatskiKraj,
            AutomatskoStanjeZavrsavanja = s.AutomatskoStanjeZavrsavanja,
            Stanje = SessionState.PLANNED,
        };

        await m_context.Sessions.AddAsync(sesija);
        await m_context.SaveChangesAsync();

        return Ok(sesija);
    }
}