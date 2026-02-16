using LabZakazivanjeAPI.Models;
using LabZakazivanjeAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabZakazivanjeAPI.Controllers;

[ApiController]
[Route("api/activities")]
public class ActivityController : ControllerBase
{
    private readonly IActivityService m_activityService;

    public ActivityController(IActivityService activityService)
    {
        m_activityService = activityService;
    }

    [HttpGet("GetActivities")]
    public async Task<ActionResult<IEnumerable<Activity>>> GetActivities()
    {
        var activities = await m_activityService.GetActivites();

        if (activities.Success)
        {
            return Ok(activities.Data);
        }
        else
        {
            return BadRequest(activities.ErrorMessage);
        }
    }

    [HttpPost("AddActivity")]
    public async Task<ActionResult<Activity>> AddActivity([FromBody] Activity a)
    {
        var activity = await m_activityService.AddActivity(a);

        if (activity.Success)
        {
            return Ok(activity.Data);
        }
        else
        {
            return BadRequest(activity.ErrorMessage);
        }
    }
}