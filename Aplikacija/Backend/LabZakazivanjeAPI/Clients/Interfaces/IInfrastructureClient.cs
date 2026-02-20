namespace LabZakazivanjeAPI.Clients.Interfaces;

public interface IInfrastructureClient
{
    Task<(bool, string)> CloneVM(string template);
    Task<bool> PrepareVM(string vlrid, int roomId, int seatId);
    Task<bool> SetVMIp(string vlrid, int roomId, int seatId, string ip);    
    Task<bool> ReleaseVM(string vlrid, int roomId, int seatId);
}