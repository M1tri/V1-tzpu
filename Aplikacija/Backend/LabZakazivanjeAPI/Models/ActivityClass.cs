using System.ComponentModel.DataAnnotations;

namespace LabZakazivanjeAPI.Models;

public class ActivityClass
{
    [Key]
    public int Id {get; set;}
    public required string Naziv {get; set;}
}