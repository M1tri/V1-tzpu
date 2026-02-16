using System.Security.Cryptography.X509Certificates;
using LabZakazivanjeAPI.Helpers;
using LabZakazivanjeAPI.Models;
using LabZakazivanjeAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LabZakazivanjeAPI.Services;

public class ActivityService: IActivityService
{
    private readonly AppDBContext m_context;

    public ActivityService(AppDBContext context)
    {
        m_context = context;
    }

    public async Task<ServiceResult<IEnumerable<Activity>>> GetActivites()
    {
        var activities = await m_context.Activities.ToListAsync();
        return ServiceResult<IEnumerable<Activity>>.Ok(activities);
    }

    public async Task<ServiceResult<Activity>> AddActivity(Activity a)
    {
        Activity activity = new Activity
        {
            Name = a.Name,
            VLRIDS = a.VLRIDS
        };

        await m_context.Activities.AddAsync(activity);
        await m_context.SaveChangesAsync();

        return ServiceResult<Activity>.Ok(activity);
    }
}