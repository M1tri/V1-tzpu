using System.Diagnostics;
using System.Runtime.CompilerServices;
using LabZakazivanjeAPI.Models;

namespace LabZakazivanjeAPI.Services.Interfaces;

public interface IActivityService
{
    Task<ServiceResult<IEnumerable<Models.Activity>>> GetActivites();
    Task<ServiceResult<Models.Activity>> AddActivity(Models.Activity a);
}