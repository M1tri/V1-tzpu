namespace LabZakazivanjeAPI.Services.Interfaces;

public interface IVLRService
{
    Task<ServiceResult<string>> GenerateIdleVLRs(string VLRID, int sessionId, int roomId, int count);
    Task<ServiceResult<string>> PrepareVLR(int sessionId, int seatId);
    Task<ServiceResult<string>> ReadyVLR(int sessionId, int seatId, string ip);
}
