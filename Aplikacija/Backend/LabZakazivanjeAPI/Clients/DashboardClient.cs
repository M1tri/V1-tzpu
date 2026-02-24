using LabZakazivanjeAPI.Clients.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LabZakazivanjeAPI.Clients;

public class DashboardClient : IDashboardClient
{
    private readonly HttpClient m_httpClient;

    public DashboardClient(HttpClient httpClient)
    {
        m_httpClient = httpClient;
    }

    public async Task<bool> Provide(int sessionId, int seatId, int userId)
    {
        var response = await m_httpClient.PutAsync(
            $"api/vlr/Provide?sessionId={sessionId}&seatId={seatId}&userId={userId}",
            null);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> Release(int sessionId, int seatId, int userId)
    {
        var response = await m_httpClient.PutAsync(
            $"api/vlr/Release?sessionId={sessionId}&seatId={seatId}&userId={userId}",
            null);

        return response.IsSuccessStatusCode;
    }
}