using LabZakazivanjeAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabZakazivanjeAPI.Controllers;

[ApiController]
[Route("api/activities")]
public class ActivityController : ControllerBase
{
    private AppDBContext m_context;

    public ActivityController(AppDBContext context)
    {
        m_context = context;
    }

    [HttpGet("GetActivities")]
    public async Task<IEnumerable<Activity>> GetActivities()
    {
        return await m_context.Activities.ToListAsync();
    }

    [HttpPost("AddActivity")]
    public async Task<ActionResult<Activity>> AddActivity([FromBody] Activity a)
    {
        Activity activity = new Activity
        {
            Name = a.Name,
            VLRIDS = a.VLRIDS
        };

        await m_context.Activities.AddAsync(activity);
        await m_context.SaveChangesAsync();

        return Ok(activity);
    }
}