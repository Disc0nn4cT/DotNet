using System.ComponentModel.DataAnnotations;

namespace YachtCharter.Models;

// Як клієнт потрапляє до марини: самостійно або трансфером (тоді потрібна адреса).
public enum FulfillmentKind { Pickup, Delivery }

// Тимчасова модель форми. Не сутність БД: форма не має доступу до Id, ціни чи RequestId.
public class CheckoutInput : IValidatableObject
{
    [Required(ErrorMessage = "Укажіть ім’я.")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Ім’я має містити від 2 до 60 символів.")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Укажіть телефон.")]
    [RegularExpression(@"^\+380[0-9]{9}$", ErrorMessage = "Формат телефону: +380 і дев’ять цифр, без пробілів.")]
    public string Phone { get; set; } = "";

    [EnumDataType(typeof(FulfillmentKind), ErrorMessage = "Оберіть допустимий спосіб прибуття.")]
    public FulfillmentKind Fulfillment { get; set; } = FulfillmentKind.Pickup;

    [StringLength(200, ErrorMessage = "Адреса має містити не більш як 200 символів.")]
    public string Address { get; set; } = "";

    [StringLength(500, ErrorMessage = "Коментар має містити не більш як 500 символів.")]
    public string Comment { get; set; } = "";

    // Власне правило теми: оренда без шкіпера вимагає посвідчення шкіпера.
    // RequiresLicense не вводить користувач — його встановлює сторінка (і повторно сервіс) за складом кошика.
    public bool RequiresLicense { get; set; }
    public bool HasSkipperLicense { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if ((Name ?? "").Trim().Length < 2)
            yield return new ValidationResult("Укажіть щонайменше два символи імені без крайніх пробілів.", new[] { nameof(Name) });

        if (Fulfillment == FulfillmentKind.Delivery && (Address ?? "").Trim().Length < 5)
            yield return new ValidationResult("Для трансферу вкажіть адресу щонайменше з 5 символів.", new[] { nameof(Address) });

        if (RequiresLicense && !HasSkipperLicense)
            yield return new ValidationResult("Для оренди без шкіпера підтвердіть, що маєте посвідчення шкіпера.", new[] { nameof(HasSkipperLicense) });
    }
}
