using System.Text.Json;
using YachtCatalog.Models;
namespace YachtCatalog.Services;

public sealed class CatalogApiClient(HttpClient client)
{
    public async Task<List<Product>> GetProductsAsync(string endpoint, CancellationToken cancellationToken)
    {
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        deadline.CancelAfter(TimeSpan.FromSeconds(10));
        using var response = await client.GetAsync(endpoint, HttpCompletionOption.ResponseHeadersRead, deadline.Token);
        response.EnsureSuccessStatusCode();
        using var stream = await response.Content.ReadAsStreamAsync(deadline.Token);
        var rows = await JsonSerializer.DeserializeAsync(stream,
            CatalogJsonContext.Default.ListCatalogItemDto, deadline.Token)
            ?? throw new JsonException("API повернув null замість масиву.");
        if (rows.Count > 100) throw new JsonException("Для навчального каталогу дозволено до 100 яхт.");
        var ids = new HashSet<int>();
        var products = new List<Product>();
        foreach (var row in rows)
        {
            if (row.Id <= 0 || !ids.Add(row.Id) || string.IsNullOrWhiteSpace(row.Name) ||
                row.UnitPrice < 0 || row.UnitPrice > 1000000m || row.OptionPrice < 0 || row.OptionPrice > 1000000m ||
                row.UnitPrice != decimal.Round(row.UnitPrice, 2) || row.OptionPrice != decimal.Round(row.OptionPrice, 2))
                throw new JsonException("API повернув некоректний ID, назву або ціну.");
            string image = row.ImageName is "albatross.png" or "breeze.png" or "dolphin.png" or "wave.png"
                ? row.ImageName : "albatross.png";
            products.Add(new Product
            {
                Id = row.Id, Name = row.Name, Description = row.Description ?? "",
                ImageName = image, ImageDescription = row.Name,
                UnitPrice = row.UnitPrice, OptionName = row.OptionName ?? "Додаткова опція",
                OptionPrice = row.OptionPrice
            });
        }
        return products;
    }
}
