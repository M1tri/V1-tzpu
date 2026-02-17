using LabZakazivanjeAPI.Models;
using LabZakazivanjeAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabZakazivanjeAPI.Controllers;

[ApiController]
[Route("api/vlr")]
public class VLRController : ControllerBase
{
    private readonly ISessionService m_sessionService;
    private readonly IVLRService m_vlrService;
    
    public VLRController(ISessionService sessionService, IVLRService vlrService)
    {
        m_sessionService = sessionService;
        m_vlrService = vlrService;
    }

    [HttpPut("PromoteAsNext")]
    public async Task<ActionResult> PromoteAsNext([FromQuery] int sessionId)
    {
        var result = await m_sessionService.PromoteAsNext(sessionId);

        if (result.Success)
        {
            return Ok(result.Data);
        }
        else
        {
            return BadRequest(result.ErrorMessage);
        }
    }

    [HttpPut("Activate")]
    public async Task<ActionResult> Activate([FromQuery] int sessionId)
    {
        var result = await m_sessionService.Activate(sessionId);

        if (result.Success)
        {
            return Ok(result.Data);
        }
        else
        {
            return BadRequest(result.ErrorMessage);
        }        
    }

    [HttpPut("Fade")]
    public async Task<ActionResult> Fade([FromQuery] int sessionId)
    {
        var result = await m_sessionService.Fade(sessionId);

        if (result.Success)
        {
            return Ok(result.Data);
        }
        else
        {
            return BadRequest(result.ErrorMessage);
        }        
    }

    [HttpPut("Terminate")]
    public async Task<ActionResult> Terminate([FromQuery] int sessionId)
    {
        var result = await m_sessionService.Terminate(sessionId);

        if (result.Success)
        {
            return Ok(result.Data);
        }
        else
        {
            return BadRequest(result.ErrorMessage);
        }        
    }

    [HttpPut("Provide")]
    public async Task<ActionResult> Provide([FromQuery] int sessionId, [FromQuery] int seatId, [FromQuery] int userId)
    {
        var result = await m_vlrService.ProvideVLR(sessionId, seatId, userId);

        if (result.Success)
        {
            return Ok(result.Data);
        }
        else
        {
            return BadRequest(result.ErrorMessage);
        }        
    }

    [HttpPut("Release")]
    public async Task<ActionResult> Release([FromQuery] int sessionId, [FromQuery] int userId)
    {
        var result = await m_vlrService.ReleaseVLR(sessionId, userId);

        if (result.Success)
        {
            return Ok(result.Data);
        }
        else
        {
            return BadRequest(result.ErrorMessage);
        }        
    }
}