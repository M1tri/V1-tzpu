using LabZakazivanjeAPI.Models;
using LabZakazivanjeAPI.Models.DTOs;

namespace LabZakazivanjeAPI.Services.Interfaces;

public interface IVLRService
{
    Task<ServiceResult<string>> GenerateIdleVLRs(string VLRID, int sessionId, int roomId, int count);
    Task<ServiceResult<ActiveVLR>> PrepareVLR(int sessionId, int seatId, int roomId);
    Task<ServiceResult<ActiveVLR>> ReadyVLR(int sessionId, int seatId, string ip);
    Task<ServiceResult<ActiveVLR>> ProvideVLR(int sessionId, int seatId, int userId);
    Task<ServiceResult<ActiveVLR>> ReleaseVLR(int sessionId, int seatId, int userId);
    Task<ServiceResult<ActiveVLR>> KillVLR(int sessionId, int seatId);
    Task<ServiceResult<VLRStatusInfoDTO>> GetStatusInfo(string vlrStatus);
    Task<ServiceResult<IEnumerable<VLRStatusInfoDTO>>> GetAllInfo();
    Task<ServiceResult<VLRStatusInfoDTO>> AddStatus(string statusName, string symbol, string color);
}
