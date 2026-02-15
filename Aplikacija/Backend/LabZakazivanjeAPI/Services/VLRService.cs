using LabZakazivanjeAPI.Models;
using LabZakazivanjeAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LabZakazivanjeAPI.Services;

public class VLRService : IVLRService
{
    private AppDBContext m_context;

    public VLRService(AppDBContext context)
    {
        m_context = context;
    }

    public async Task<ServiceResult<string>> GenerateIdleVLRs(string VLRID, int sessionId, int roomId, int count)
    {
        var session = await m_context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session == null)
            return ServiceResult<string>.Error("Ne postoji sesija!");

        for (int i = 0; i < count; i++)
        {
            ActiveVLR vlrInstance = new ActiveVLR
            {
                VLRID = VLRID,
                SesijaVlasnik = session,
                RoomId = roomId,
                Status = VLRStatus.GENERATED_IDLE,
                LastStatusChange = DateTime.Now
            };

            await m_context.ActiveVLRs.AddAsync(vlrInstance);
        }

        await m_context.SaveChangesAsync();

        return ServiceResult<string>.Ok("Uspesno kreirani IDLE VLR-ovi");
    }

    public async Task<ServiceResult<string>> PrepareVLR(int sessionId, int seatId)
    {
        var sesija = await m_context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);
        
        if (sesija == null)
            return ServiceResult<string>.Error("Ne postoji sesija s id");

        // prvi u slobodan dodeljuje se na taj seatId   
        var vlr = await m_context.ActiveVLRs.
        Where(v => v.SessionId == sessionId && v.Status == VLRStatus.GENERATED_IDLE)
        .FirstOrDefaultAsync();

        if (vlr == null)
            return ServiceResult<string>.Error("Ne postoji slobodan VLR za taj seat");
        
        vlr.SeatId = seatId;
        vlr.Status = VLRStatus.IN_PREPERATION;

        m_context.ActiveVLRs.Update(vlr);
        await m_context.SaveChangesAsync();

        return ServiceResult<string>.Ok($"Dodeljen vlr uspesno");
    }

    public async Task<ServiceResult<string>> ReadyVLR(int sessionId, int seatId, string ip)
    {
        var sesija = await m_context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);
        
        if (sesija == null)
            return ServiceResult<string>.Error("Ne postoji sesija s id");

        // prvi koji je u pripremi
        var vlr = await m_context.ActiveVLRs.
        Where(v => v.SessionId == sessionId && v.Status == VLRStatus.IN_PREPERATION)
        .FirstOrDefaultAsync();

        if (vlr == null)
            return ServiceResult<string>.Error("Ne postoji VLR u pripremi za taj seat");
        
        vlr.Status = VLRStatus.READY;
        vlr.IP = ip;

        m_context.ActiveVLRs.Update(vlr);
        await m_context.SaveChangesAsync();

        return ServiceResult<string>.Ok($"Dodeljen vlr uspesno");
    }
}