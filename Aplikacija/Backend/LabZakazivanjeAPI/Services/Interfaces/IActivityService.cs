using System.Diagnostics;
using System.Runtime.CompilerServices;
using LabZakazivanjeAPI.Models;
using LabZakazivanjeAPI.Models.DTOs;

namespace LabZakazivanjeAPI.Services.Interfaces;

public interface IActivityService
{
    Task<ServiceResult<IEnumerable<ViewActivityDTO>>> GetActivites();
    Task<ServiceResult<ViewActivityDTO>> GetActivity(int activityId);
    Task<ServiceResult<ViewActivityDTO>> AddActivity(CreateActivityDTO a);
}