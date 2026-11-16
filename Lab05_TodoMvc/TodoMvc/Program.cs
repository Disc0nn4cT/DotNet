using Microsoft.EntityFrameworkCore;
using TodoMvc.Data;
using TodoMvc.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// SQLite: the database is a single file todo.db in the project folder
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? "Data Source=todo.db"));

var app = builder.Build();

// Create/update the database from migrations + seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.TodoItems.Any())
    {
        db.TodoItems.AddRange(
            new TodoItem
            {
                Title = "Спробувати додаток",
                Description = "Відкрити список і перевірити фільтри.",
                Priority = Priority.High,
                DueAt = DateTime.Today.AddDays(1)
            },
            new TodoItem { Title = "Додати нове завдання", Priority = Priority.Medium },
            new TodoItem { Title = "Позначити виконаним", Priority = Priority.Low, IsDone = true }
        );
        db.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=TodoItems}/{action=Index}/{id?}")
   .WithStaticAssets();

app.Run();