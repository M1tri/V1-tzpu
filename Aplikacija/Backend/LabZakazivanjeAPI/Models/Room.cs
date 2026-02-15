using System.ComponentModel.DataAnnotations;

namespace LabZakazivanjeAPI.Models;

public class Room
{
    [Key]
    public int Id {get; set;}
    public required string Naziv {get; set;}
    public required int Capacity {get; set;}
    public required string Raspored {get; set;}

    /* 
    (
    (1:12.13.14.43, , 2:12.23.21),
    (3:12.13.14.43, 4:12.23.21, ),
    ...
    )
    */
}  