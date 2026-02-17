using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LabZakazivanjeAPI.Models;

public enum SessionState
{
    PLANNED = 0,
    NEXT = 1,
    ACTIVE = 2,
    FADING = 3,
    FINISHED = 4
};


public class Session
{
    [Key]
    public int Id {get; set;}
    public int RoomId {get; set;}
    public int ActivityId {get; set;}

    [ForeignKey(nameof(RoomId))]
    public Room? Prostorija {get; set;}

    [ForeignKey(nameof(ActivityId))]
    public Activity? Aktivnost {get; set;}
    
    public required DateOnly Datum {get; set;}
    public required TimeOnly VremePocetka {get; set;}
    public required TimeOnly VremeKraja {get; set;}
    
    public required SessionState Stanje {get; set;}

    public required bool AutomatskiPocetak {get; set;}
    public required bool AutomatskiKraj {get; set;}
    public SessionState? AutomatskoStanjeZavrsavanja {get; set;}

    public override string ToString()
    {
        return $"{Aktivnost.Name} {Prostorija.Naziv} {Stanje}";
    }
}