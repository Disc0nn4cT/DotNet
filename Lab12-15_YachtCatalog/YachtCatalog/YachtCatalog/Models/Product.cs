namespace YachtCatalog.Models;

public sealed class Product
{
    // Максимальна тривалість оренди в добах.
    public const int MaxQuantity = 14;

    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    public string ImageName { get; init; } = "";
    public decimal UnitPrice { get; init; }      // ціна оренди за добу
    public string OptionName { get; init; } = "";
    public decimal OptionPrice { get; init; }    // доплата за шкіпера за добу

    public decimal CalculateTotal(int quantity, bool includeOption)
    {
        if (quantity < 1 || quantity > MaxQuantity)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        // Додаткова опція оплачується для кожної доби оренди.
        decimal pricePerItem = UnitPrice + (includeOption ? OptionPrice : 0m);
        return pricePerItem * quantity;
    }
}
