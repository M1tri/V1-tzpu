using LabZakazivanjeAPI.Models;
using LabZakazivanjeAPI.Models.DTOs;

namespace LabZakazivanjeAPI.Services.Interfaces;
public record ResourceInfo(string VlrStatus, int? UserId);

public interface ISessionService
{
    Task<ServiceResult<IEnumerable<ViewSessionDTO>>> GetSessions();
    Task<ServiceResult<IEnumerable<ViewSessionDTO>>> GetSessionsInRoom(int roomId);
    Task<ServiceResult<ViewSessionDTO>> GetSession(int sessionId);
    Task<ServiceResult<ViewSessionDTO>> AddSession(CreateSessionDTO s);
    Task<ServiceResult<ViewSessionDTO>> EditSession(UpdateSessionDTO s);
    Task<ServiceResult<ViewSessionDTO>> CloneSession(int sessionId);
    Task<ServiceResult<string>> DeleteSession(int sessionId);
    Task<ServiceResult<Dictionary<int, ResourceInfo>>> GetSessionResourceStatus(int sessionId);
    Task<ServiceResult<string>> PromoteAsNext(int sessionId);
    Task<ServiceResult<string>> DemoteToPlanned(int sessionId);
    Task<ServiceResult<string>> Activate(int sessionId);
    Task<ServiceResult<string>> Fade(int sessionId);
    Task<ServiceResult<string>> Terminate(int sessionId);
}