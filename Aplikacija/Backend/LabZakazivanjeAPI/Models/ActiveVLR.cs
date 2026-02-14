using System.ComponentModel.DataAnnotations;

namespace LabZakazivanjeAPI.Models;

public class ActiveVLR
{
    [Key]
    public int Id {get; set;}

    public required string VLRID {get; set;}

    public string? IP {get; set;}
    public int? RoomId {get; set;}
    public int? SeatId {get; set;}
    public int? UserId {get; set;}
    public DateTime LastStatusChange {get; set;}
}