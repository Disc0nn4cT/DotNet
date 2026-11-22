using Microsoft.EntityFrameworkCore;
using YachtCharter.Components;
using YachtCharter.Data;
using YachtCharter.Services;

var builder = WebApplication.CreateBuilder(args);

// Реєстрація Razor-компонентів і підтримки інтерактивності на сервері.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// SQLite: файл charter.db поруч із .csproj. Фабрика створює короткоживучі контексти.
var databasePath = Path.Combine(builder.Environment.ContentRootPath, "charter.db");
builder.Services.AddDbContextFactory<YachtDbContext>(options =>
    options.UseSqlite($"Data Source={databasePath}"));

// Scoped = один екземпляр на інтерактивну сесію (вкладку браузера).
builder.Services.AddScoped<CatalogService>();
builder.Services.AddScoped<OrderState>();

// Каталог попередньої роботи не змінюємо. Замовлення мають окрему базу orders.db.
var ordersPath = Path.Combine(builder.Environment.ContentRootPath, "orders.db");
builder.Services.AddDbContextFactory<OrderDbContext>(options =>
    options.UseSqlite($"Data Source={ordersPath}"));
builder.Services.AddScoped<PurchaseService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAntiforgery();

app.MapStaticAssets();

// HTTP API каталогу: той самий список яхт у форматі JSON.
app.MapGet("/api/yachts", async (CatalogService catalog) =>
    await catalog.GetYachtsAsync());

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Створення бази та початкове наповнення (seed) під час запуску.
await SeedData.InitializeAsync(
    app.Services.GetRequiredService<IDbContextFactory<YachtDbContext>>());

// Створення бази замовлень, якщо її ще немає (тестових замовлень не додаємо).
await using (var ordersDb = await app.Services
    .GetRequiredService<IDbContextFactory<OrderDbContext>>().CreateDbContextAsync())
{
    await ordersDb.Database.EnsureCreatedAsync();
}

app.Run();
