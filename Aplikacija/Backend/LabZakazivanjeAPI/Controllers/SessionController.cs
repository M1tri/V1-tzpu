using LabZakazivanjeAPI.Models;
using LabZakazivanjeAPI.Services;
using LabZakazivanjeAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LabZakazivanjeAPI.Controllers;

[ApiController]
[Route("api/sessions")]
public class SessionController : ControllerBase
{
    private readonly ISessionService m_sessionService;

    public SessionController(ISessionService sessionService)
    {
        m_sessionService = sessionService;
    }

    [HttpGet("GetSessions")]
    public async Task<ActionResult<IEnumerable<Session>>> GetSessions()
    {
        var sessions = await m_sessionService.GetSessions();

        if (sessions.Success)
        {
            return Ok(sessions.Data);
        }
        else
        {
            return BadRequest(sessions.ErrorMessage);
        }
    }

    [HttpPost("AddSession")]
    public async Task<ActionResult<Session>> AddSession([FromBody] Session s)
    {
        var session = await m_sessionService.AddSession(s);

        if (session.Success)
        {
            return Ok(session.Data);
        }
        else
        {
            return BadRequest(session.ErrorMessage);
        }
    }
}