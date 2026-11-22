using System.Globalization;
namespace YachtCatalog.Models;

public sealed class SavedSelection
{
    public int Id { get; set; }
    public string ProductName { get; set; } = "";
    public long UnitPriceKop { get; set; }
    public long OptionPriceKop { get; set; }
    public string OptionName { get; set; } = "";
    public int Quantity { get; set; } = 1;
    public bool IncludeOption { get; set; }
    public string Note { get; set; } = "";
    public decimal Total => (UnitPriceKop + (IncludeOption ? OptionPriceKop : 0)) / 100m * Quantity;
    public string Summary => $"{Quantity} діб · {Product.Money(Total)}";
    public string Details => IncludeOption ? OptionName : "Без додаткових послуг";

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ProductName) || Quantity < 1 || Quantity > Product.MaxQuantity ||
            UnitPriceKop < 0 || OptionPriceKop < 0 || Note.Length > 200)
            throw new ArgumentException("Перевірте назву, кількість діб (1–14), ціну та примітку (до 200 символів).");
    }
    public static SavedSelection FromProduct(Product product, int quantity, bool option) => new()
    {
        ProductName = product.Name, Quantity = quantity, IncludeOption = option,
        UnitPriceKop = checked((long)decimal.Round(product.UnitPrice * 100m, 0)),
        OptionPriceKop = checked((long)decimal.Round(product.OptionPrice * 100m, 0)),
        OptionName = product.OptionName
    };
}
