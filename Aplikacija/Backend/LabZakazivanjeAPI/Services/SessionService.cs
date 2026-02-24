using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using LabZakazivanjeAPI.Helpers;
using LabZakazivanjeAPI.Models;
using LabZakazivanjeAPI.Models.DTOs;
using LabZakazivanjeAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LabZakazivanjeAPI.Services;

public class SessionService : ISessionService
{
    private readonly AppDBContext m_context;
    private readonly IVLRService m_vlrService;

    public SessionService(AppDBContext context, IVLRService vlrService)
    {
        m_context = context;
        m_vlrService = vlrService;
    }

    public async Task<ServiceResult<IEnumerable<ViewSessionDTO>>> GetSessions()
    {
        var sessions = await m_context.Sessions
        .Include(s => s.Prostorija)
        .Include(s => s.Aktivnost)
        .ThenInclude(a => a!.Tip)
        .ToListAsync();

        List<ViewSessionDTO> sessionViews = [];

        foreach (var s in sessions)
        {
            sessionViews.Add(new ViewSessionDTO
            {
                Id = s.Id,
                NazivProstorije = s.Prostorija!.Naziv,
                NazivAktivnosti = s.Aktivnost!.Name,
                TipAktivnosti = s.Aktivnost!.Tip!.Naziv,
                Datum = s.Datum,
                VremePocetka = s.VremePocetka,
                VremeKraja = s.VremeKraja,
                Stanje = s.Stanje,
                AutomatskiPocetak = s.AutomatskiPocetak,
                AutomatskiKraj = s.AutomatskiKraj,
                AutomatskoKrajnjeStanje = s.AutomatskoStanjeZavrsavanja
            });
        }

        return ServiceResult<IEnumerable<ViewSessionDTO>>.Ok(sessionViews);
    }

    public async Task<ServiceResult<IEnumerable<ViewSessionDTO>>> GetSessionsInRoom(int roomId)
    {
        var sessions = await m_context.Sessions
        .Where(s => s.RoomId == roomId)
        .Include(s => s.Prostorija)
        .Include(s => s.Aktivnost)
        .ThenInclude(a => a!.Tip)
        .ToListAsync();
        List<ViewSessionDTO> sessionViews = [];

        foreach (var s in sessions)
        {
            sessionViews.Add(new ViewSessionDTO
            {
                Id = s.Id,
                NazivProstorije = s.Prostorija!.Naziv,
                NazivAktivnosti = s.Aktivnost!.Name,
                TipAktivnosti = s.Aktivnost!.Tip!.Naziv,
                Datum = s.Datum,
                VremePocetka = s.VremePocetka,
                VremeKraja = s.VremeKraja,
                Stanje = s.Stanje,
                AutomatskiPocetak = s.AutomatskiPocetak,
                AutomatskiKraj = s.AutomatskiKraj,
                AutomatskoKrajnjeStanje = s.AutomatskoStanjeZavrsavanja
            });
        }

        return ServiceResult<IEnumerable<ViewSessionDTO>>.Ok(sessionViews);        
    }

    public async Task<ServiceResult<ViewSessionDTO>> GetSession(int sessionId)
    {
        var sesija = await m_context.Sessions
        .Include(s => s.Prostorija)
        .Include(s => s.Aktivnost)
        .ThenInclude(a => a!.Tip)
        .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (sesija == null)
            return ServiceResult<ViewSessionDTO>.Error("Ne postoji sesija");
        
        ViewSessionDTO view = new ViewSessionDTO
        {
            Id = sesija.Id,
            NazivProstorije = sesija.Prostorija!.Naziv,
            NazivAktivnosti = sesija.Aktivnost!.Name,
            TipAktivnosti = sesija.Aktivnost!.Tip!.Naziv,
            Datum = sesija.Datum,
            VremePocetka = sesija.VremePocetka,
            VremeKraja = sesija.VremeKraja,
            Stanje = sesija.Stanje,
            AutomatskiPocetak = sesija.AutomatskiPocetak,
            AutomatskiKraj = sesija.AutomatskiKraj,
            AutomatskoKrajnjeStanje = sesija.AutomatskoStanjeZavrsavanja
        };

        return ServiceResult<ViewSessionDTO>.Ok(view);
    }

    public async Task<ServiceResult<ViewSessionDTO>> AddSession(CreateSessionDTO s)
    {   
        var room = await m_context.Rooms.Where(r => r.Id == s.RoomId).FirstOrDefaultAsync();
        if (room == null)
        {
            return ServiceResult<ViewSessionDTO>.Error("Nepostojeca soba");
        }

        var activity = await m_context.Activities.Include(a => a.Tip).FirstOrDefaultAsync(a => a.Id == s.AktivnostId);
        if (activity == null)
        {
            return ServiceResult<ViewSessionDTO>.Error("Nepostojeća aktivnost!");
        }

        Session sesija = new Session
        {
            Prostorija = room,
            Aktivnost = activity,
            Datum = s.Datum,
            VremePocetka = s.VremePocetka,
            VremeKraja = s.VremeKraja,
            AutomatskiPocetak = s.AutomatskiPocetak,
            AutomatskiKraj = s.AutomatskiKraj,
            AutomatskoStanjeZavrsavanja = s.AutomatskoKrajnjeStanje,
            Stanje = SessionState.PLANNED,
        };

        await m_context.Sessions.AddAsync(sesija);
        await m_context.SaveChangesAsync();

        ViewSessionDTO view = new()
        {
            Id = sesija.Id,
            NazivProstorije = sesija.Prostorija.Naziv,
            NazivAktivnosti = sesija.Aktivnost.Name,
            TipAktivnosti = activity.Tip!.Naziv,
            Datum = sesija.Datum,
            VremePocetka = sesija.VremePocetka,
            VremeKraja = sesija.VremeKraja,
            Stanje = sesija.Stanje,
            AutomatskiKraj = sesija.AutomatskiKraj,
            AutomatskiPocetak = sesija.AutomatskiPocetak,
            AutomatskoKrajnjeStanje = sesija.AutomatskoStanjeZavrsavanja
        };

        return ServiceResult<ViewSessionDTO>.Ok(view);
    }


    public async Task<ServiceResult<ViewSessionDTO>> CloneSession(int sessionId)
    {
        var sesija = await m_context.Sessions
        .Include(s => s.Prostorija)
        .Include(s => s.Aktivnost)
        .ThenInclude(a => a!.Tip)
        .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (sesija == null)
            return ServiceResult<ViewSessionDTO>.Error("Nepostojeca sesija!");

        Session s = new Session
        {
            Prostorija = sesija.Prostorija,
            Aktivnost = sesija.Aktivnost,
            Datum = sesija.Datum,
            VremePocetka = sesija.VremePocetka,
            VremeKraja = sesija.VremeKraja,
            AutomatskiKraj = sesija.AutomatskiKraj,
            AutomatskiPocetak = sesija.AutomatskiPocetak,
            AutomatskoStanjeZavrsavanja = sesija.AutomatskoStanjeZavrsavanja,
            Stanje = SessionState.PLANNED
        };

        await m_context.Sessions.AddAsync(s);
        await m_context.SaveChangesAsync();

        ViewSessionDTO view = new()
        {
            Id = s.Id,
            NazivProstorije = s.Prostorija!.Naziv,
            NazivAktivnosti = s.Aktivnost!.Name,
            TipAktivnosti = s.Aktivnost!.Tip!.Naziv,
            Datum = s.Datum,
            VremePocetka = s.VremePocetka,
            VremeKraja = s.VremeKraja,
            Stanje = s.Stanje,
            AutomatskiKraj = s.AutomatskiKraj,
            AutomatskiPocetak = s.AutomatskiPocetak,
            AutomatskoKrajnjeStanje = s.AutomatskoStanjeZavrsavanja
        };

        return ServiceResult<ViewSessionDTO>.Ok(view);
    }

    public async Task<ServiceResult<string>> DeleteSession(int sessionId)
    {
        var sesija = await m_context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);

        if (sesija == null)
            return ServiceResult<string>.Error("Nepostojeca sesija!");

        if (sesija.Stanje != SessionState.PLANNED && sesija.Stanje != SessionState.FINISHED)
            return ServiceResult<string>.Error("Mozete brisati samo zavrsenu ili planiranu sesiju");

        m_context.Sessions.Remove(sesija);
        await m_context.SaveChangesAsync();

        return ServiceResult<string>.Ok("Uspesno");
    }

    public async Task<ServiceResult<ViewSessionDTO>> EditSession(UpdateSessionDTO s)
    {
        var sesija = await m_context
        .Sessions
        .Include(s => s.Prostorija)
        .Include(s => s.Aktivnost)
        .ThenInclude(a => a!.Tip)
        .FirstOrDefaultAsync(se => se.Id == s.Id);

        if (sesija == null)
            return ServiceResult<ViewSessionDTO>.Error("Ne postoji sesija sa tim Id-em");

        if (s.RoomId != null)
        {
            var room = await m_context.Rooms.FirstOrDefaultAsync(r => r.Id == s.RoomId);

            if (room == null)
                return ServiceResult<ViewSessionDTO>.Error("Ne postoji soba sa tim Id-em");

            sesija.Prostorija = room;
        }

        if (s.AktivnostId != null)
        {
            var aktivnost = await m_context
            .Activities
            .Include(s => s.Tip)
            .FirstOrDefaultAsync(a => a.Id == s.AktivnostId);

            if (aktivnost == null)
                return ServiceResult<ViewSessionDTO>.Error("Ne postoji ta aktivnost");

            sesija.Aktivnost = aktivnost;
        }

        if (s.Datum.HasValue)
            sesija.Datum = s.Datum.Value;
        
        if (s.VremePocetka.HasValue)
            sesija.VremePocetka = s.VremePocetka.Value;
        
        if (s.VremeKraja.HasValue)
            sesija.VremeKraja = s.VremeKraja.Value;
        
        if (s.AutomatskiPocetak.HasValue)
            sesija.AutomatskiPocetak = s.AutomatskiPocetak.Value;
        
        if (s.AutomatskiKraj.HasValue)
        {
            sesija.AutomatskiKraj = s.AutomatskiKraj.Value;

            if (s.AutomatskiKraj.Value == true)
            {
                if (!s.AutomatskoKrajnjeStanje.HasValue)
                    return ServiceResult<ViewSessionDTO>.Error("Podesen automatski kraj ali nije poslato stanje u koje se prelazi");
                
                sesija.AutomatskoStanjeZavrsavanja = s.AutomatskoKrajnjeStanje.Value;
            } 
        }

        m_context.Sessions.Update(sesija);
        await m_context.SaveChangesAsync();

        ViewSessionDTO view = new()
        {
            Id = sesija.Id,
            NazivProstorije = sesija.Prostorija!.Naziv,
            NazivAktivnosti = sesija.Aktivnost!.Name,
            TipAktivnosti = sesija.Aktivnost!.Tip!.Naziv,
            Datum = sesija.Datum,
            VremePocetka = sesija.VremePocetka,
            VremeKraja = sesija.VremeKraja,
            Stanje = sesija.Stanje,
            AutomatskiPocetak = sesija.AutomatskiPocetak,
            AutomatskiKraj = sesija.AutomatskiKraj,
            AutomatskoKrajnjeStanje = sesija.AutomatskoStanjeZavrsavanja
        };

        return ServiceResult<ViewSessionDTO>.Ok(view);
    }
    public async Task<ServiceResult<Dictionary<int, ResourceInfo>>> GetSessionResourceStatus(int sessionId)
    {
        Dictionary<int, ResourceInfo> result = [];

        var sesija = await m_context.Sessions
        .Include(s => s.Prostorija)
        .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (sesija == null)
            return ServiceResult<Dictionary<int, ResourceInfo>>.Error("Ne postoji sesija!");

        var seatIds = RoomRasporedParser.GetSeatIds(sesija.Prostorija!.Raspored);
        foreach (int seat in seatIds)
        {
            result.Add(seat, new ResourceInfo(VLRStatus.NULL, null));
        }

        var vlrStatuses = await m_context.ActiveVLRs
        .Where(v => v.SessionId == sessionId && v.SeatId != null)
        .Select(v => new {v.SeatId, v.Status, v.UserId})
        .ToListAsync();

        foreach (var v in vlrStatuses)
        {
            result[v.SeatId!.Value] = new ResourceInfo(v.Status, v.UserId);
        }

        return ServiceResult<Dictionary<int, ResourceInfo>>.Ok(result);
    }

    public async Task<ServiceResult<string>> PromoteAsNext(int sessionId)
    {
        var s = await m_context.Sessions
        .Where(s => s.Id == sessionId)
        .Include(s => s.Prostorija)
        .Include(s => s.Aktivnost)
        .FirstOrDefaultAsync();

        if (s == null)
        {
            return ServiceResult<string>.Error("Nista");
        }

        if (s.Stanje != SessionState.PLANNED)
        {
            return ServiceResult<string>.Error("Sesija mora biti u planned stanju da bi se proglasila za Next");
        }

        Room room = s.Prostorija!;

        if (m_context.Sessions.Where(s => s.RoomId == room.Id && s.Stanje == SessionState.NEXT).Any())
        {
            return ServiceResult<string>.Error("U ovoj prostoriji već postoji NEXT sesija!");
        }

        Activity activity = s.Aktivnost!;
        List<string> VLRIds = activity.VLRIDS;

        int count = room.Capacity;
        foreach (var vlrId in VLRIds)
        {
            await m_vlrService.GenerateIdleVLRs(vlrId, sessionId, room.Id, count);
        }

        s.Stanje = SessionState.NEXT;
        m_context.Sessions.Update(s);

        await m_context.SaveChangesAsync();

        return ServiceResult<string>.Ok("Sredjeno");
    }

    public async Task<ServiceResult<string>> DemoteToPlanned(int sessionId)
    {
        var sesija = await m_context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);
        if (sesija == null)
            return ServiceResult<string>.Error("Nepostojeca sesija");

        if (sesija.Stanje != SessionState.NEXT)
            return ServiceResult<string>.Error("Ova akcija je primenjiva samo na sesije u NEXT stanju");
        
        var vlrs = await m_context.ActiveVLRs.Where(v => v.SessionId == sessionId).ToListAsync();

        foreach (var v in vlrs)
            m_context.ActiveVLRs.Remove(v);

        sesija.Stanje = SessionState.PLANNED;
        m_context.Sessions.Update(sesija);
        await m_context.SaveChangesAsync();

        return ServiceResult<string>.Ok("Sesija uspesno vracena u PLANNED");
    }

    public async Task<ServiceResult<string>> Activate(int sessionId)
    {
        var sesija = await m_context.Sessions
        .Include(s => s.Prostorija)
        .Include(s => s.Aktivnost)
        .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (sesija == null)
            return ServiceResult<string>.Error("Ne postoji sesija sa tim ID-em");
        
        if (sesija.Stanje != SessionState.NEXT)
            return ServiceResult<string>.Error("Sesija mora biti u stanju NEXT da bi presala u aktivnu");

        if (m_context.Sessions
            .Where(s => s.RoomId == sesija.RoomId && s.Stanje == SessionState.ACTIVE).Any())
        {
            return ServiceResult<string>.Error("U ovoj prostoriji već postoji ACTIVE sesija!");
        }

        var result = await GetSessionResourceStatus(sesija.Id);
        Dictionary<int, ResourceInfo> seatStatuses;
        if (result.Success)
        {
            seatStatuses = result.Data!;
        }
        else
        {
            return ServiceResult<string>.Error("Greska pri nabavljanju statusa mesta");
        }

        foreach (var seatId in seatStatuses.Keys)
        {
            var res = await m_vlrService.PrepareVLR(sesija.Id, seatId, sesija.RoomId);
            if (!res.Success)
                Console.WriteLine($"Neuspeh za seatId {seatId} : {res.ErrorMessage}");
        }

        sesija.Stanje = SessionState.ACTIVE;
        m_context.Sessions.Update(sesija);
        await m_context.SaveChangesAsync();
        return ServiceResult<string>.Ok("Dodeljeno");
    }

    public async Task<ServiceResult<string>> Fade(int sessionId)
    {
        var sesija = await m_context.Sessions
        .Include(s => s.Prostorija)
        .Include(s => s.Aktivnost)
        .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (sesija == null)
            return ServiceResult<string>.Error("Ne postoji sesija sa tim ID-em");
        
        if (sesija.Stanje != SessionState.ACTIVE)
            return ServiceResult<string>.Error("Sesija mora biti u stanju ACTIVE da bi presala u FADING!");

        if (m_context.Sessions.Where(s => s.RoomId == sesija.RoomId && s.Stanje == SessionState.FADING).Any())
            return ServiceResult<string>.Error("U ovoj prostoriji već postoji FADING sesija!");
    
        sesija.Stanje = SessionState.FADING;

        m_context.Sessions.Update(sesija);

        var nonProvidedVlrs = await m_context.ActiveVLRs
        .Where(
            v => v.SessionId == sessionId && (
            v.Status != VLRStatus.PROVIDED && v.Status != VLRStatus.NULL
        )).ToListAsync();

        foreach (var vlr in nonProvidedVlrs)
        {
            // TODO ERROR CHECKING
            await m_vlrService.KillVLR(sessionId, vlr.SeatId!.Value);
        }

        await m_context.SaveChangesAsync();

        return ServiceResult<string>.Ok("Sesija presla u fading stanje");
    }

    public async Task<ServiceResult<string>> Terminate(int sessionId)
    {
                var sesija = await m_context.Sessions
        .Include(s => s.Prostorija)
        .Include(s => s.Aktivnost)
        .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (sesija == null)
            return ServiceResult<string>.Error("Ne postoji sesija sa tim ID-em");
        
        if (sesija.Stanje != SessionState.ACTIVE && sesija.Stanje != SessionState.FADING)
            return ServiceResult<string>.Error("Sesija mora biti u stanju ACTIVE/FADING da bi prešla u FINISHED!");
    
        sesija.Stanje = SessionState.FINISHED;

        m_context.Sessions.Update(sesija);

        var nonProvidedVlrs = await m_context.ActiveVLRs
        .Where(
            v => v.SessionId == sessionId && (
            v.Status == VLRStatus.READY || v.Status == VLRStatus.IN_PREPERATION || v.Status == VLRStatus.PROVIDED || v.Status == VLRStatus.RELEASED
        )).ToListAsync();

        foreach (var vlr in nonProvidedVlrs)
        {
            // TODO ERROR CHECKING
            await m_vlrService.KillVLR(sessionId, vlr.SeatId!.Value);
        }

        var allVlrs = await m_context.ActiveVLRs.Where(v => v.SessionId == sessionId).ToListAsync();
        foreach (var v in allVlrs)
            m_context.ActiveVLRs.Remove(v);

        await m_context.SaveChangesAsync();

        return ServiceResult<string>.Ok("Sesija presla u finished stanje");
    }
}