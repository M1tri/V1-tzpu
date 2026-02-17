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
        Where(v => v.SessionId == sessionId && v.Status == VLRStatus.IN_PREPERATION && v.SeatId == seatId)
        .FirstOrDefaultAsync();

        if (vlr == null)
            return ServiceResult<string>.Error("Ne postoji VLR u pripremi za taj seat");
        
        vlr.Status = VLRStatus.READY;
        vlr.IP = ip;

        m_context.ActiveVLRs.Update(vlr);
        await m_context.SaveChangesAsync();

        return ServiceResult<string>.Ok($"Dodeljen vlr uspesno");
    }

    public async Task<ServiceResult<ActiveVLR>> ProvideVLR(int sessionId, int seatId, int userId)
    {
        var sesija = await m_context.Sessions.FirstOrDefaultAsync(s=>s.Id == sessionId);

        if (sesija == null)
            return ServiceResult<ActiveVLR>.Error("Ne postoji sesija!");

        if (sesija.Stanje != SessionState.ACTIVE)
            return ServiceResult<ActiveVLR>.Error("Ova sesija nije aktivna i ne prihvata nove zahteve!");

        var vlr = await m_context.ActiveVLRs.FirstOrDefaultAsync(v => v.SessionId == sessionId && v.SeatId == seatId && v.Status == VLRStatus.READY);

        if (vlr == null)
            return ServiceResult<ActiveVLR>.Error("Ne postoji spreman VLR u toj sesisji!");

        vlr.Status = VLRStatus.PROVIDED;
        vlr.UserId = userId;

        m_context.ActiveVLRs.Update(vlr);
        await m_context.SaveChangesAsync();

        return ServiceResult<ActiveVLR>.Ok(vlr);   
    }

    public async Task<ServiceResult<ActiveVLR>> ReleaseVLR(int sessionId, int userId)
    {
        var sesija = await m_context.Sessions.FirstOrDefaultAsync(s=>s.Id == sessionId);

        if (sesija == null)
            return ServiceResult<ActiveVLR>.Error("Ne postoji sesija!");

        var vlr = await m_context.ActiveVLRs.FirstOrDefaultAsync(v => v.SessionId == sessionId && v.UserId == userId);

        if (vlr == null)
            return ServiceResult<ActiveVLR>.Error("Ne postoji VLR sa taj userId u toj sesisji!");

        if (sesija.Stanje == SessionState.FADING)
        {
            var aktivnaSesija = await m_context.Sessions.
            FirstOrDefaultAsync(s => s.RoomId == sesija.RoomId && s.Stanje == SessionState.ACTIVE);
            if (aktivnaSesija != null)
            {
                var preparedVlr = await m_context.ActiveVLRs
                .FirstOrDefaultAsync(v => v.RoomId == vlr.RoomId && v.SeatId == vlr.SeatId && v.Status == VLRStatus.IN_PREPERATION);

                if (preparedVlr != null)
                {
                    preparedVlr.Status = VLRStatus.READY;
                    preparedVlr.IP = vlr.IP;
                    m_context.ActiveVLRs.Update(preparedVlr);
                }
            }

            vlr.IP = null;
            vlr.SeatId = null;
            vlr.RoomId = null;
            vlr.UserId = null;
            vlr.Status = VLRStatus.NULL;
            m_context.ActiveVLRs.Update(vlr);
        }
        else
        {        
            vlr.Status = VLRStatus.RELEASED;
            vlr.UserId = null;
            m_context.ActiveVLRs.Update(vlr);
        }

        await m_context.SaveChangesAsync();

        return ServiceResult<ActiveVLR>.Ok(vlr);           
    }

    public async Task<ServiceResult<string>> KillVLR(int sessionId, int seatId)
    {
        var sesija = await m_context.Sessions.FirstOrDefaultAsync(s=>s.Id == sessionId);

        if (sesija == null)
            return ServiceResult<string>.Error("Ne postoji sesija!");

        var vlr = await m_context.ActiveVLRs.FirstOrDefaultAsync(v => v.SessionId == sessionId && v.SeatId == seatId);
        if (vlr == null)
            return ServiceResult<string>.Error("Ne postoji vlr na taj seat");
        

        // TODO PREPRAVI DA LEPSE BUDE A NE COPYPASTE
        var aktivnaSesija = await m_context.Sessions.
        FirstOrDefaultAsync(s => s.RoomId == sesija.RoomId && s.Stanje == SessionState.ACTIVE);
        if (aktivnaSesija != null)
        {
            var preparedVlr = await m_context.ActiveVLRs
            .FirstOrDefaultAsync(v => v.RoomId == vlr.RoomId && v.SeatId == vlr.SeatId && v.Status == VLRStatus.IN_PREPERATION);

            if (preparedVlr != null)
            {
                preparedVlr.Status = VLRStatus.READY;
                preparedVlr.IP = vlr.IP;
                m_context.ActiveVLRs.Update(preparedVlr);
            }
        }
    
        vlr.IP = null;
        vlr.SeatId = null;
        vlr.RoomId = null;
        vlr.UserId = null;
        vlr.Status = VLRStatus.NULL;

        m_context.ActiveVLRs.Update(vlr);
        await m_context.SaveChangesAsync();

        return ServiceResult<string>.Ok("Uspesno ubiven vlr");
    }
}