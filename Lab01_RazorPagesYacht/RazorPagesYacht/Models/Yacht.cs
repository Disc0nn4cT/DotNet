using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesYacht.Models;

public class Yacht
{
    public int Id { get; set; }

    [Display(Name = "Назва")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(60, MinimumLength = 3, ErrorMessage = ValidationMessages.LengthRange)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Верф")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(60, ErrorMessage = ValidationMessages.MaxLength)]
    public string Shipyard { get; set; } = string.Empty;

    [Display(Name = "Категорія")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(30, ErrorMessage = ValidationMessages.MaxLength)]
    [RegularExpression(@"^[A-ZА-ЯІЇЄҐ][a-zA-Zа-яА-ЯіїєґІЇЄҐ'\s-]*$",
        ErrorMessage = "Категорія має починатися з великої літери й містити лише літери, пробіли, дефіси або апострофи.")]
    public string Category { get; set; } = string.Empty;

    [Display(Name = "Дата спуску на воду")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
    public DateTime LaunchDate { get; set; }

    [Display(Name = "Ціна")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [Range(0.0, 1_000_000_000.0, ErrorMessage = ValidationMessages.Range)]
    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    [Display(Name = "Довжина (м)")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [Range(1.0, 500.0, ErrorMessage = ValidationMessages.Range)]
    [Column(TypeName = "decimal(6, 2)")]
    public decimal Length { get; set; }

    [Display(Name = "Фото")]
    public string? PhotoPath { get; set; }

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}