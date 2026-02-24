using LabZakazivanjeAPI.Models;
using LabZakazivanjeAPI.Models.DTOs;
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

    [HttpGet("GetSessionsInRoom")]
    public async Task<ActionResult<IEnumerable<ViewSessionDTO>>> GetSessionsInRoom([FromQuery] int roomId)
    {
        var sessions = await m_sessionService.GetSessionsInRoom(roomId);

        if (sessions.Success)
        {
            return Ok(sessions.Data);
        }
        else
        {
            return BadRequest(sessions.ErrorMessage);
        }
    }

    [HttpGet("GetSession")]
    public async Task<ActionResult<ViewSessionDTO>> GetSession([FromQuery] int sessionId)
    {
        var session = await m_sessionService.GetSession(sessionId);

        if (session.Success)
        {
            return Ok(session.Data);
        }
        else
        {
            return BadRequest(session.ErrorMessage);
        }
    }

    [HttpPost("AddSession")]
    public async Task<ActionResult<Session>> AddSession([FromBody] CreateSessionDTO s)
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

    [HttpPost("CloneSession")]
    public async Task<ActionResult<Session>> CloneSession([FromQuery] int sessionId)
    {
        var session = await m_sessionService.CloneSession(sessionId);

        if (session.Success)
        {
            return Ok(session.Data);
        }
        else
        {
            return BadRequest(session.ErrorMessage);
        }
    }

    [HttpDelete("DeleteSession")]
    public async Task<ActionResult<Session>> DeleteSessiona([FromQuery] int sessionId)
    {
        var session = await m_sessionService.DeleteSession(sessionId);

        if (session.Success)
        {
            return Ok(session.Data);
        }
        else
        {
            return BadRequest(session.ErrorMessage);
        }
    }

    [HttpPut("EditSession")]
    public async Task<ActionResult<Session>> EditSession([FromBody] UpdateSessionDTO s)
    {
        var session = await m_sessionService.EditSession(s);

        if (session.Success)
        {
            return Ok(session.Data);
        }
        else
        {
            return BadRequest(session.ErrorMessage);
        }
    }

    [HttpGet("GetSessionResourceStatus")]
    public async Task<ActionResult<Dictionary<int, string>>> GetSessionResourceStatus(int sessionId)
    {
        var statuses = await m_sessionService.GetSessionResourceStatus(sessionId);

        if (statuses.Success)
        {
            return Ok(statuses.Data);
        }
        else
        {
            return BadRequest(statuses.ErrorMessage);
        }
    }
}