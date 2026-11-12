namespace RazorPagesYacht.Models;

// Shared texts for validation attributes. {0} = field display name, {1}/{2} = limits
public static class ValidationMessages
{
    public const string Required = "Поле «{0}» обов’язкове.";
    public const string MaxLength = "Поле «{0}» може містити не більше {1} символів.";
    public const string LengthRange = "Поле «{0}» має містити від {2} до {1} символів.";
    public const string Range = "Значення поля «{0}» має бути від {1} до {2}.";
}