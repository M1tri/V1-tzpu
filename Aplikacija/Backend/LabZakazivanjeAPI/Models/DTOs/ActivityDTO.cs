namespace LabZakazivanjeAPI.Models.DTOs;

public class CreateActivityDTO
{
    public required int ActivityClassId {get; set;}
    public required string Naziv {get; set;}
    public required List<string> VLRIDs {get; set;}
}

public class ViewActivityDTO
{
    public required int Id {get; set;}
    public required string Tip {get; set;}
    public required string Naziv {get; set;}
    public required List<string> VLRIDs {get; set;}
}