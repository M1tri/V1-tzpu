using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LabZakazivanjeAPI.Models;

public enum VLRStatus
{
    GENERATED_IDLE = 0,
    IN_PREPERATION = 1,
    READY = 2,
    RELEASED = 3,
    NULL = 4
}

public class ActiveVLR
{
    [Key]
    public int Id {get; set;}

    public int SessionId {get; set;}

    [ForeignKey(nameof(SessionId))]
    public Session? SesijaVlasnik {get; set;}

    public required string VLRID {get; set;}

    public string? IP {get; set;}
    public int? RoomId {get; set;}
    public int? SeatId {get; set;}
    public int? UserId {get; set;}
    public required DateTime LastStatusChange {get; set;}
    public required VLRStatus Status {get; set;}
}