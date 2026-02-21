namespace LabZakazivanjeAPI.Models.DTOs;

using LabZakazivanjeAPI.Helpers;

public class CreateRoomDTO
{
    public required string Naziv {get; set;}
    public required int Capacity {get; set;}
    public required string Raspored {get; set;}
}

public class ViewRoomDTO
{
    public int Id {get; set;}
    public required string Naziv {get; set;}
    public required int Capacity {get; set;}
    public required List<List<Seat>> Raspored {get; set;}
}