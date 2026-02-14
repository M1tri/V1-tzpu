using System.ComponentModel.DataAnnotations;

namespace LabZakazivanjeAPI.Models;

public class Room
{
    [Key]
    public int Id {get; set;}
    public required string Naziv {get; set;}
    public required string Raspored {get; set;}
}  