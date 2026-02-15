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
    
    public VLRController(ISessionService sessionService)
    {
        m_sessionService = sessionService;
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
}