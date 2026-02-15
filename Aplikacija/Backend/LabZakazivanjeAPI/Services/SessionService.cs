using System.Security.Cryptography.X509Certificates;
using LabZakazivanjeAPI.Helpers;
using LabZakazivanjeAPI.Models;
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

    public async Task<ServiceResult<IEnumerable<Session>>> GetSessions()
    {
        var sessions = await m_context.Sessions.ToListAsync();
        return ServiceResult<IEnumerable<Session>>.Ok(sessions);
    }

    public async Task<ServiceResult<Session>> AddSession(Session s)
    {   
        var room = await m_context.Rooms.Where(r => r.Id == s.RoomId).FirstOrDefaultAsync();
        if (room == null)
        {
            return ServiceResult<Session>.Error("Nepostojeca soba");
        }

        var activity = await m_context.Activities.Where(a => a.Id == s.ActivityId).FirstOrDefaultAsync();
        if (activity == null)
        {
            return ServiceResult<Session>.Error("Nepostojeća aktivnost!");
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
            AutomatskoStanjeZavrsavanja = s.AutomatskoStanjeZavrsavanja,
            Stanje = SessionState.PLANNED,
        };

        await m_context.Sessions.AddAsync(sesija);
        await m_context.SaveChangesAsync();

        return ServiceResult<Session>.Ok(sesija);
    }

    public async Task<ServiceResult<Dictionary<int, VLRStatus?>>> GetSessionResourceStatus(int sessionId)
    {
        Dictionary<int, VLRStatus?> result = [];

        var sesija = await m_context.Sessions
        .Include(s => s.Prostorija)
        .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (sesija == null)
            return ServiceResult<Dictionary<int, VLRStatus?>>.Error("Ne postoji sesija!");

        var raspored = RoomRasporedParser.ParseRaspored(sesija.Prostorija!.Raspored);

        foreach (var row in raspored)
        {
            foreach (var seat in row)
            {
                if (seat.Id != null && seat.IP != null)
                    result.Add(seat.Id!.Value, null);
            }
        }

        var vlrStatuses = await m_context.ActiveVLRs
        .Where(v => v.SessionId == sessionId && v.SeatId != null)
        .Select(v => new {v.SeatId, v.Status})
        .ToListAsync();

        foreach (var v in vlrStatuses)
        {
            result[v.SeatId!.Value] = v.Status;
        }

        return ServiceResult<Dictionary<int, VLRStatus?>>.Ok(result);
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
            return ServiceResult<string>.Error("U datoj sobi vec postoji Next session");
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
            return ServiceResult<string>.Error("U toj prostoriji vec postoji aktivna sesija!");
        }

        var result = await GetSessionResourceStatus(sesija.Id);
        Dictionary<int, VLRStatus?> seatStatuses;
        if (result.Success)
        {
            seatStatuses = result.Data!;
        }
        else
        {
            return ServiceResult<string>.Error("Greska pri nabavljanju statsua mesta");
        }

        // prvo se svima dodeli seatId i oznace da se spremaju, ali jos nemaju IP
        foreach (var seatId in seatStatuses.Keys)
        {
            var res = await m_vlrService.PrepareVLR(sesija.Id, seatId);
            if (!res.Success)
                Console.WriteLine($"Neuspeh za seatId {seatId} : {res.ErrorMessage}");
        }

        var fadingSession = await m_context.Sessions
        .FirstOrDefaultAsync(s => s.RoomId == sesija.RoomId && s.Stanje == SessionState.FADING);

        // TODO metoda koja vraca prikladinju strukturu
        var seatIps = RoomRasporedParser.ParseRaspored(sesija.Prostorija!.Raspored);

        if (fadingSession == null)
        {
            // nema fading sesije s kojom bi bio moguc konflikt, slobodno dodeljujemo IP-eve
            foreach (var row in seatIps)
            {
                foreach (var seat in row)
                {
                    if (seat.Id != null && seat.IP != null)
                        await m_vlrService.ReadyVLR(sessionId, seat.Id.Value, seat.IP);
                }
            }
        }
        else
        {
            
        }
        
        sesija.Stanje = SessionState.ACTIVE;
        m_context.Sessions.Update(sesija);
        await m_context.SaveChangesAsync();

        return ServiceResult<string>.Ok("Dodeljeno");
    }

    public Task<ServiceResult<string>> Fade(int sessionId)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<string>> Terminate(int sessionId)
    {
        throw new NotImplementedException();
    }
}