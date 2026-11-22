var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var products = new[]
{
    new { Id=101, Name="Яхта «Альбатрос» · API", Description="Каталог отримано з вебсервісу чартеру.", ImageName="albatross.png", UnitPrice=6500m, OptionName="Шкіпер", OptionPrice=1500m },
    new { Id=102, Name="Яхта «Бриз» · API", Description="Компактна вітрильна яхта для першого чартеру.", ImageName="breeze.png", UnitPrice=4500m, OptionName="Шкіпер", OptionPrice=1500m },
    new { Id=103, Name="Катамаран «Дельфін» · API", Description="Стійкий катамаран із великим салоном для компанії.", ImageName="dolphin.png", UnitPrice=9000m, OptionName="Шкіпер і кок", OptionPrice=2500m }
};
app.MapGet("/api/products", () => products);
app.MapGet("/api/empty", () => Array.Empty<object>());
app.MapGet("/api/error", () => Results.StatusCode(503));
app.MapGet("/api/invalid", () => Results.Text("{ invalid json", "application/json"));
app.MapGet("/api/slow", async (CancellationToken token) =>
{
    await Task.Delay(TimeSpan.FromSeconds(15), token);
    return Results.Ok(products);
});
app.Run();
