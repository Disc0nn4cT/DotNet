using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesYacht.Models;

public class Yacht
{
    public int Id { get; set; }

    [Required]
    [StringLength(60, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(60)]
    public string Shipyard { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    [RegularExpression(@"^[A-ZА-ЯІЇЄҐ][a-zA-Zа-яА-ЯіїєґІЇЄҐ'\s-]*$",
        ErrorMessage = "Category must start with a capital letter and contain only letters, spaces, hyphens or apostrophes.")]
    public string Category { get; set; } = string.Empty;

    [Display(Name = "Launch Date")]
    [DataType(DataType.Date)]
    public DateTime LaunchDate { get; set; }

    [Range(0.0, 1_000_000_000.0)]
    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    [Display(Name = "Length (m)")]
    [Range(1.0, 500.0)]
    [Column(TypeName = "decimal(6, 2)")]
    public decimal Length { get; set; }
}