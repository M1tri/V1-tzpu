using LabZakazivanjeAPI.Models;

namespace LabZakazivanjeAPI.Services.Interfaces;

public interface ISessionService
{
    Task<ServiceResult<IEnumerable<Session>>> GetSessions();
    Task<ServiceResult<Session>> AddSession(Session s);
    Task<ServiceResult<Dictionary<int, VLRStatus?>>> GetSessionResourceStatus(int sessionId);
    Task<ServiceResult<string>> PromoteAsNext(int sessionId);
    Task<ServiceResult<string>> Activate(int sessionId);
    Task<ServiceResult<string>> Fade(int sessionId);
    Task<ServiceResult<string>> Terminate(int sessionId);
}