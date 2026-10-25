using Microsoft.EntityFrameworkCore;
using RazorPagesYacht.Data;
using RazorPagesYacht.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddDbContext<RazorPagesYachtContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RazorPagesYachtContext")
        ?? throw new InvalidOperationException("Connection string 'RazorPagesYachtContext' not found.")));

var app = builder.Build();

// Наповнення БД при старті
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<RazorPagesYachtContext>();
    context.Database.Migrate();   // creates/updates the DB from migrations
    SeedData.Initialize(services);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRequestLocalization("en-US");
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();  

app.Run();