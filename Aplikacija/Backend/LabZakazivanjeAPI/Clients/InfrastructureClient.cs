using LabZakazivanjeAPI.Clients.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LabZakazivanjeAPI.Clients;

public class InfrastructureClient : IInfrastructureClient
{
    private readonly HttpClient m_httpClient;

    public InfrastructureClient(HttpClient httpClient)
    {
        m_httpClient = httpClient;
    }

    public async Task<(bool, string)> CloneVM(string template)
    {
        var response = await m_httpClient.GetAsync(
            $"api/infrastructure/CloneVM?template={template}");

        if (response.IsSuccessStatusCode)
        {
            var vlr = await response.Content.ReadAsStringAsync();
            return (true, vlr);
        }
        else
        {
            return (false, "");
        }
    }

    public async Task<bool> PrepareVM(string vlrid, int roomId, int seatId)
    {
        var response = await m_httpClient.PostAsync(
            $"api/infrastructure/PrepareVM?vlrid={vlrid}&roomId={roomId}&seatId={seatId}",
            null);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ReleaseVM(string vlrid, int roomId, int seatId)
    {
        var response = await m_httpClient.PostAsync(
            $"api/infrastructure/ReleaseVM?vlrId={vlrid}&roomId={roomId}&seatId={seatId}",
            null);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> SetVMIp(string vlrid, int roomId, int seatId, string ip)
    {
        var response = await m_httpClient.PostAsync(
            $"api/infrastructure/SetVMIp?vlrId={vlrid}&roomId={roomId}&seatId={seatId}&ip={ip}",
            null);

        return response.IsSuccessStatusCode;
    }
}