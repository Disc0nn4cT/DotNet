using System.Globalization;

namespace YachtCatalog.Models;

public sealed class Product
{
    // Максимальна тривалість оренди в добах.
    public const int MaxQuantity = 14;

    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    public string ImageName { get; init; } = "";
    public string ImageDescription { get; init; } = "";
    public decimal UnitPrice { get; init; }      // ціна оренди за добу
    public string OptionName { get; init; } = "";
    public decimal OptionPrice { get; init; }    // доплата за добу

    public string PriceText => Money(UnitPrice) + " за добу";
    public string OptionText => $"{OptionName} (+{Money(OptionPrice)}/доба)";

    public static string Money(decimal value) =>
        value.ToString("N2", CultureInfo.GetCultureInfo("uk-UA")) + " грн";

    public decimal CalculateTotal(int quantity, bool includeOption)
    {
        if (quantity < 1 || quantity > MaxQuantity)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        // Опція оплачується за кожну добу оренди.
        decimal pricePerItem = UnitPrice + (includeOption ? OptionPrice : 0m);
        return pricePerItem * quantity;
    }
}
