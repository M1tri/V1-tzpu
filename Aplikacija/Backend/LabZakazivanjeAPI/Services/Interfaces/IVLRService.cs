using LabZakazivanjeAPI.Models;
using LabZakazivanjeAPI.Models.DTOs;

namespace LabZakazivanjeAPI.Services.Interfaces;

public interface IVLRService
{
    Task<ServiceResult<string>> GenerateIdleVLRs(string VLRID, int sessionId, int roomId, int count);
    Task<ServiceResult<string>> PrepareVLR(int sessionId, int seatId);
    Task<ServiceResult<string>> ReadyVLR(int sessionId, int seatId, string ip);
    Task<ServiceResult<ActiveVLR>> ProvideVLR(int sessionId, int seatId, int userId);
    Task<ServiceResult<ActiveVLR>> ReleaseVLR(int sessionId, int userId);
    Task<ServiceResult<string>> KillVLR(int sessionId, int seatId);
    Task<ServiceResult<VLRStatusInfoDTO>> GetStatusInfo(string vlrStatus);
    Task<ServiceResult<VLRStatusInfoDTO>> AddStatus(string statusName, string symbol, string color);
}
