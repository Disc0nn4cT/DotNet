using Microsoft.EntityFrameworkCore;
using YachtCharter.Models;

namespace YachtCharter.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IDbContextFactory<YachtDbContext> factory)
    {
        await using var db = await factory.CreateDbContextAsync();

        // Створює файл бази й таблицю, якщо їх ще немає (це не міграції).
        await db.Database.EnsureCreatedAsync();

        // Якщо яхти вже є — повторно не додаємо.
        if (await db.Yachts.AnyAsync()) return;

        db.Yachts.AddRange(
            new Yacht { Name = "Бриз", Type = "Вітрильна яхта", Cabins = 3, ImageUrl = "images/yacht-1.svg",
                        Description = "Компактна вітрильна яхта для першого чартеру.", BasePrice = 4500m },
            new Yacht { Name = "Альбатрос", Type = "Вітрильна яхта", Cabins = 4, ImageUrl = "images/yacht-2.svg",
                        Description = "Простора вітрильна яхта для сімейних походів.", BasePrice = 6500m },
            new Yacht { Name = "Дельфін", Type = "Катамаран", Cabins = 4, ImageUrl = "images/yacht-3.svg",
                        Description = "Стійкий катамаран із великим салоном.", BasePrice = 9000m },
            new Yacht { Name = "Чорноморець", Type = "Катамаран", Cabins = 6, ImageUrl = "images/yacht-4.svg",
                        Description = "Великий катамаран для компанії друзів.", BasePrice = 12000m },
            new Yacht { Name = "Хвиля", Type = "Моторна яхта", Cabins = 2, ImageUrl = "images/yacht-5.svg",
                        Description = "Швидка моторна яхта для коротких прогулянок.", BasePrice = 11000m },
            new Yacht { Name = "Аврора", Type = "Моторна яхта", Cabins = 3, ImageUrl = "images/yacht-6.svg",
                        Description = "Комфортна моторна яхта з флайбриджем.", BasePrice = 15000m }
        );
        await db.SaveChangesAsync();
    }
}
