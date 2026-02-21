using System.ComponentModel.DataAnnotations;

namespace LabZakazivanjeAPI.Models;

public class VLRStatusInfo
{
    [Key]
    public int Id {get; set;}
    public required string Naziv {get; set;}
    public required string Symbol {get; set;}
    public required string Color {get; set;}
}