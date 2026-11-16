using System.ComponentModel.DataAnnotations;

namespace TodoMvc.Models;

// [Display] gives Ukrainian names; GetEnumSelectList and GetDisplayName() use them
public enum Priority
{
    [Display(Name = "Низький")] Low = 0,
    [Display(Name = "Середній")] Medium = 1,
    [Display(Name = "Високий")] High = 2
}

public class TodoItem
{
    public int Id { get; set; }

    [Display(Name = "Назва")]
    [Required(ErrorMessage = "Вкажіть назву завдання.")]
    [StringLength(100, ErrorMessage = "Назва може містити не більше {1} символів.")]
    public string Title { get; set; } = "";

    [Display(Name = "Опис")]
    [StringLength(1000, ErrorMessage = "Опис може містити не більше {1} символів.")]
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }

    [Display(Name = "Готово?")]
    public bool IsDone { get; set; }

    [Display(Name = "Дедлайн")]
    [DataType(DataType.Date)]
    public DateTime? DueAt { get; set; }

    [Display(Name = "Пріоритет")]
    public Priority Priority { get; set; } = Priority.Medium;

    [Display(Name = "Створено")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}