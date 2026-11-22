namespace YachtCatalog.Models;

public sealed class CatalogItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string ImageName { get; set; } = "";
    public decimal UnitPrice { get; set; }
    public string OptionName { get; set; } = "";
    public decimal OptionPrice { get; set; }
}
