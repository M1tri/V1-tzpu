using System.Runtime.Intrinsics.X86;
using LabZakazivanjeAPI.Clients;
using LabZakazivanjeAPI.Clients.Interfaces;
using LabZakazivanjeAPI.Models;
using LabZakazivanjeAPI.Models.DTOs;
using LabZakazivanjeAPI.Services.Interfaces;
using Microsoft.AspNetCore.StaticAssets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LabZakazivanjeAPI.Services;

public class VLRService : IVLRService
{
    private readonly AppDBContext m_context;
    private readonly IInfrastructureClient m_infrastructureClient;

    public VLRService(AppDBContext context, IInfrastructureClient infrastructureClient)
    {
        m_context = context;
        m_infrastructureClient = infrastructureClient;
    }

    public async Task<ServiceResult<string>> GenerateIdleVLRs(string VLRID, int sessionId, int roomId, int count)
    {
        var session = await m_context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session == null)
            return ServiceResult<string>.Error("Ne postoji sesija!");

        for (int i = 0; i < count; i++)
        {
            var response = await m_infrastructureClient.CloneVM(VLRID);

            string vlrInstanceId;
            if (response.Item1 == true)
            {
                vlrInstanceId = response.Item2;
            }
            else
            {
                return ServiceResult<string>.Error("Greska u infrastrukturi pri kloniranju!");
            }

            ActiveVLR vlrInstance = new ActiveVLR
            {
                VLRID = vlrInstanceId,
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

    public async Task<ServiceResult<ActiveVLR>> PrepareVLR(int sessionId, int seatId, int roomId)
    {
        var sesija = await m_context.Sessions.Include(s=>s.Prostorija).FirstOrDefaultAsync(s => s.Id == sessionId);
        
        if (sesija == null)
            return ServiceResult<ActiveVLR>.Error("Ne postoji sesija s id");

        var vlr = await m_context.ActiveVLRs.
        Where(v => v.SessionId == sessionId && (v.Status == VLRStatus.GENERATED_IDLE || v.SeatId == seatId))
        .FirstOrDefaultAsync();

        if (vlr == null)
            return ServiceResult<ActiveVLR>.Error("Ne postoji slobodan VLR za taj seat");
        
        var infrastructureResponse = await m_infrastructureClient.PrepareVM(vlr.VLRID, sesija.RoomId, seatId);

        if (!infrastructureResponse)
            return ServiceResult<ActiveVLR>.Error("Greska u infrastrukturi pri prripremi resursa");

        vlr.RoomId = roomId;
        vlr.SeatId = seatId;
        vlr.Status = VLRStatus.IN_PREPERATION;
        vlr.LastStatusChange = DateTime.Now;

        m_context.ActiveVLRs.Update(vlr);
        await m_context.SaveChangesAsync(); 

        int? fadingSesija = await m_context.Sessions.Where(s => s.RoomId == sesija.RoomId && s.Stanje == SessionState.FADING).Select(s => s.Id).FirstOrDefaultAsync();
        if (fadingSesija == null || !m_context.ActiveVLRs.Where(v => v.SessionId == fadingSesija && v.SeatId == seatId && v.Status == VLRStatus.PROVIDED).Any())
        {
            var seat = Helpers.RoomRasporedParser.GetSeatIps(sesija.Prostorija!.Raspored).FirstOrDefault(seat => seat.Id == seatId && seat.IP != null);
            if (seat == null)
            {
                return ServiceResult<ActiveVLR>.Error("Nevalidan seatId");
            }
            Console.WriteLine(seat.IP!);
            var response = await ReadyVLR(sessionId, seatId, seat.IP!);

            if (response.Success)
            {
                vlr = response.Data!;
            }
            else
            {
                return ServiceResult<ActiveVLR>.Error("Greska pri pripremi resursa");
            }
        }

        return ServiceResult<ActiveVLR>.Ok(vlr);
    }

    public async Task<ServiceResult<ActiveVLR>> ReadyVLR(int sessionId, int seatId, string ip)
    {
        var sesija = await m_context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);
        
        if (sesija == null)
            return ServiceResult<ActiveVLR>.Error("Ne postoji sesija s id");

        var vlr = await m_context.ActiveVLRs.
        Where(v => v.SessionId == sessionId && v.Status == VLRStatus.IN_PREPERATION && v.SeatId == seatId)
        .FirstOrDefaultAsync();

        if (vlr == null)
            return ServiceResult<ActiveVLR>.Error("Ne postoji VLR u pripremi za taj seat");
        
        bool infrastructureRespone = await m_infrastructureClient.SetVMIp(vlr.VLRID, sesija.RoomId, seatId, ip);

        if (!infrastructureRespone)
            return ServiceResult<ActiveVLR>.Error("Greska u infrastrukturi");

        vlr.Status = VLRStatus.READY;
        vlr.IP = ip;
        vlr.LastStatusChange = DateTime.Now;

        m_context.ActiveVLRs.Update(vlr);
        await m_context.SaveChangesAsync();

        return ServiceResult<ActiveVLR>.Ok(vlr);
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
        vlr.LastStatusChange = DateTime.Now;

        m_context.ActiveVLRs.Update(vlr);
        await m_context.SaveChangesAsync();

        return ServiceResult<ActiveVLR>.Ok(vlr);   
    }

    public async Task<ServiceResult<ActiveVLR>> ReleaseVLR(int sessionId, int seatId, int userId)
    {
        var sesija = await m_context.Sessions.FirstOrDefaultAsync(s=>s.Id == sessionId);

        if (sesija == null)
            return ServiceResult<ActiveVLR>.Error("Ne postoji sesija!");

        var vlr = await m_context.ActiveVLRs.FirstOrDefaultAsync(v => v.SessionId == sessionId && v.SeatId == seatId && v.UserId == userId);

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
                    preparedVlr.LastStatusChange = DateTime.Now;
                    m_context.ActiveVLRs.Update(preparedVlr);
                }
            }

            vlr.IP = null;
            vlr.RoomId = null;
            vlr.UserId = null;
            vlr.Status = VLRStatus.NULL;
            vlr.LastStatusChange = DateTime.Now;
            m_context.ActiveVLRs.Update(vlr);
        }
        else
        {        
            vlr.Status = VLRStatus.RELEASED;
            vlr.LastStatusChange = DateTime.Now;
            vlr.UserId = null;
            m_context.ActiveVLRs.Update(vlr);
        }

        await m_context.SaveChangesAsync();

        return ServiceResult<ActiveVLR>.Ok(vlr);           
    }

    public async Task<ServiceResult<ActiveVLR>> KillVLR(int sessionId, int seatId)
    {
        var sesija = await m_context.Sessions.FirstOrDefaultAsync(s=>s.Id == sessionId);

        if (sesija == null)
            return ServiceResult<ActiveVLR>.Error("Ne postoji sesija!");

        var vlr = await m_context.ActiveVLRs.FirstOrDefaultAsync(v => v.SessionId == sessionId && v.SeatId == seatId);
        if (vlr == null)
            return ServiceResult<ActiveVLR>.Error("Ne postoji vlr na taj seat");
        

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
                preparedVlr.LastStatusChange = DateTime.Now;
                preparedVlr.IP = vlr.IP;
                m_context.ActiveVLRs.Update(preparedVlr);
            }
        }
    
        vlr.IP = null;
        vlr.RoomId = null;
        vlr.UserId = null;
        vlr.Status = VLRStatus.NULL;
        vlr.LastStatusChange = DateTime.Now;
        
        m_context.ActiveVLRs.Update(vlr);
        await m_context.SaveChangesAsync();

        return ServiceResult<ActiveVLR>.Ok(vlr);
    }

    public async Task<ServiceResult<VLRStatusInfoDTO>> GetStatusInfo(string vlrStatus)
    {
        var statusInfo = await m_context.VLRStatusInfos.FirstOrDefaultAsync(s => s.Naziv == vlrStatus);

        if (statusInfo == null)
            return ServiceResult<VLRStatusInfoDTO>.Error("Nepoznat status!");
        
        return ServiceResult<VLRStatusInfoDTO>.Ok(new VLRStatusInfoDTO
        {
            Naziv = statusInfo.Naziv,
            Symbol = statusInfo.Symbol,
            Color = statusInfo.Color
        });
    }

    public async Task<ServiceResult<IEnumerable<VLRStatusInfoDTO>>> GetAllInfo()
    {
        var statusInfo = await m_context.VLRStatusInfos.ToListAsync();

        List<VLRStatusInfoDTO> infos = [];
        foreach (var s in statusInfo)
        {
            infos.Add(new VLRStatusInfoDTO
            {
                Naziv = s.Naziv,
                Symbol = s.Symbol,
                Color = s.Color
            });
        }

        return ServiceResult<IEnumerable<VLRStatusInfoDTO>>.Ok(infos);
    }

    public async Task<ServiceResult<VLRStatusInfoDTO>> AddStatus(string statusName, string symbol, string color)
    {
        var statusInfo = new VLRStatusInfo
        {
            Naziv = statusName,
            Symbol = symbol,
            Color = color
        };

        await m_context.VLRStatusInfos.AddAsync(statusInfo);
        await m_context.SaveChangesAsync();

        return ServiceResult<VLRStatusInfoDTO>.Ok(new VLRStatusInfoDTO 
        {
            Naziv = statusInfo.Naziv,
            Symbol = statusInfo.Symbol,
            Color = statusInfo.Color
        });
    }
}