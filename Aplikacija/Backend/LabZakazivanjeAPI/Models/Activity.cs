using System.ComponentModel.DataAnnotations;

namespace LabZakazivanjeAPI.Models;

public class Activity
{
    [Key]
    public int Id {get; set;}

    public required string Name {get; set;}
    public List<string> VLRIDS {get; set;} = [];
}