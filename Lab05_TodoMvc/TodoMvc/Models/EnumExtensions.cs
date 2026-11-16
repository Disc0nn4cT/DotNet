using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace TodoMvc.Models;

public static class EnumExtensions
{
    // Priority.High.GetDisplayName() -> "Високий" (reads the [Display] attribute)
    public static string GetDisplayName(this Enum value)
    {
        var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
        return member?.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? value.ToString();
    }
}