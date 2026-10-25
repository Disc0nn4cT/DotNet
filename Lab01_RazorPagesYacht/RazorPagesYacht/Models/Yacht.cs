using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RazorPagesYacht.Models;

public class Yacht
{
    public int Id { get; set; }

    public string? Name { get; set; }        

    public string? Shipyard { get; set; }   

    public string? Category { get; set; }    

    [DataType(DataType.Date)]
    public DateTime LaunchDate { get; set; }   

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }
}