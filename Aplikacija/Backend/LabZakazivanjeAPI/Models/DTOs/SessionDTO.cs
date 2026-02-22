namespace LabZakazivanjeAPI.Models.DTOs;

public class CreateSessionDTO
{
    public int RoomId {get; set;}
    public int AktivnostId {get; set;}
    public required DateOnly Datum {get; set;}
    public required TimeOnly VremePocetka {get; set;}
    public required TimeOnly VremeKraja {get; set;}
    public required bool AutomatskiPocetak {get; set;}
    public required bool AutomatskiKraj {get; set;}
    public required SessionState AutomatskoKrajnjeStanje {get; set;}
}

public class UpdateSessionDTO
{
    public int Id {get; set;}
    public int? AktivnostId {get; set;} = null;
    public int? RoomId {get; set;} = null;
    public DateOnly? Datum {get; set;} = null;
    public TimeOnly? VremePocetka {get; set;} = null;
    public TimeOnly? VremeKraja {get; set;} = null;
    public bool? AutomatskiPocetak {get; set;} = null;
    public bool? AutomatskiKraj {get; set;} = null;
    public SessionState? AutomatskoKrajnjeStanje {get; set;} = null;
}

public class ViewSessionDTO
{
    public int Id {get; set;}
    public required string NazivProstorije {get; set;}
    public required string NazivAktivnosti {get; set;}
    public required string TipAktivnosti {get;set;}
    public required DateOnly Datum {get; set;}
    public required TimeOnly VremePocetka {get; set;}
    public required TimeOnly VremeKraja {get; set;}
    public required SessionState Stanje{get;set;}
    public required bool AutomatskiPocetak {get; set;}
    public required bool AutomatskiKraj {get; set;}
    public required SessionState? AutomatskoKrajnjeStanje {get; set;}
}