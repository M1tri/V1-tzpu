using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LabZakazivanjeAPI.Models;

public static class VLRStatus
{
    public static readonly string GENERATED_IDLE = "generated_idle";
    public static readonly string IN_PREPERATION = "in_preparation";
    public static readonly string READY = "ready";
    public static readonly string RELEASED = "released";
    public static readonly string PROVIDED = "provided";
    public static readonly string NULL = "null";
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
    public required string Status {get; set;}
}