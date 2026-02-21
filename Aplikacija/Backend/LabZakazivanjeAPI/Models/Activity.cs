using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LabZakazivanjeAPI.Models;

public class Activity
{
    [Key]
    public int Id {get; set;}
    
    public int ActivityClassId {get; set;}
    [ForeignKey(nameof(ActivityClassId))]
    public ActivityClass? Tip {get; set;}

    public required string Name {get; set;}
    public List<string> VLRIDS {get; set;} = [];
}