namespace LabZakazivanjeAPI.Clients.Interfaces;

public interface IDashboardClient
{
    Task<bool> Provide(int sessionId, int seatId, int userId);
    Task<bool> Release(int sessionId, int seatId, int userId);
}