using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RazorPagesYacht.Data;
using RazorPagesYacht.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("RazorPagesYachtContext")
    ?? throw new InvalidOperationException("Connection string 'RazorPagesYachtContext' not found.");

// Two contexts, one database
builder.Services.AddDbContext<RazorPagesYachtContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity with roles + built-in login/register pages
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // no email confirmation for the demo
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

// Policy "admins only"
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Admin"));
});

// Close the whole /Yachts folder to everyone except admins
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Yachts", "RequireAdministratorRole");
});

var app = builder.Build();

// Create/update both databases and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    services.GetRequiredService<RazorPagesYachtContext>().Database.Migrate();
    services.GetRequiredService<ApplicationDbContext>().Database.Migrate();
    SeedData.Initialize(services);
    await IdentitySeeder.SeedAdminAsync(services, app.Configuration);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRequestLocalization("en-US");
app.UseStaticFiles();      // needed later for uploaded photos
app.UseRouting();
app.UseAuthentication();   // who are you?
app.UseAuthorization();    // what are you allowed to do?

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();