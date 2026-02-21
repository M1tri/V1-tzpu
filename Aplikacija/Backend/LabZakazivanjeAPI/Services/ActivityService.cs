using System.Security.Cryptography.X509Certificates;
using LabZakazivanjeAPI.Helpers;
using LabZakazivanjeAPI.Models;
using LabZakazivanjeAPI.Models.DTOs;
using LabZakazivanjeAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LabZakazivanjeAPI.Services;

public class ActivityService: IActivityService
{
    private readonly AppDBContext m_context;

    public ActivityService(AppDBContext context)
    {
        m_context = context;
    }

    public async Task<ServiceResult<IEnumerable<ViewActivityDTO>>> GetActivites()
    {
        var activities = await m_context
        .Activities
        .Include(s => s.Tip)
        .ToListAsync();
        
        List<ViewActivityDTO> views = [];

        foreach (var v in activities)
        {
            views.Add(new ViewActivityDTO
            {
                Id = v.Id,
                Tip = v.Tip!.Naziv,
                Naziv = v.Name,
                VLRIDs = v.VLRIDS
            });
        }

        return ServiceResult<IEnumerable<ViewActivityDTO>>.Ok(views);
    }

    public async Task<ServiceResult<ViewActivityDTO>> AddActivity(CreateActivityDTO a)
    {
        var tip = await m_context.ActivityClasses.FirstOrDefaultAsync(t => t.Id == a.ActivityClassId);

        if (tip == null)
        {
            return ServiceResult<ViewActivityDTO>.Error("Ne postoji ta klasa!");
        }

        Activity activity = new Activity
        {
            Tip = tip,
            Name = a.Naziv,
            VLRIDS = a.VLRIDs
        };

        await m_context.Activities.AddAsync(activity);
        await m_context.SaveChangesAsync();

        ViewActivityDTO view = new ViewActivityDTO
        {
            Id = activity.Id,
            Naziv = activity.Name,
            Tip = tip.Naziv,
            VLRIDs = activity.VLRIDS
        };

        return ServiceResult<ViewActivityDTO>.Ok(view);
    }
}