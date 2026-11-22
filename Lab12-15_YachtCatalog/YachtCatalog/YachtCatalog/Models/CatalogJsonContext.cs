using System.Text.Json.Serialization;
namespace YachtCatalog.Models;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(List<CatalogItemDto>))]
public partial class CatalogJsonContext : JsonSerializerContext { }
