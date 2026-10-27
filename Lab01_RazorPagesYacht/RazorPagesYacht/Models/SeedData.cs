using Microsoft.EntityFrameworkCore;
using RazorPagesYacht.Data;

namespace RazorPagesYacht.Models;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = new RazorPagesYachtContext(
            serviceProvider.GetRequiredService<DbContextOptions<RazorPagesYachtContext>>());

        if (context.Yacht.Any())
        {
            return; // БД вже наповнена
        }

        context.Yacht.AddRange(
    new Yacht
    {
        Name = "Borealis",
        Shipyard = "Aperture Science",
        Category = "Research Vessel",
        LaunchDate = new DateTime(2007, 10, 10), 
        Price = 0.00M,
        Length = 120.00M
    },
    new Yacht
    {
        Name = "Undra",
        Shipyard = "GlobeMaster",
        Category = "Explorer Yacht",
        LaunchDate = new DateTime(2023, 1, 1),   
        Price = 1000000.00M,
        Length = 15.24M
    },
    new Yacht
    {
        Name = "Elco 26",
        Shipyard = "Elco",
        Category = "Motor Launch",
        LaunchDate = new DateTime(1992, 1, 1),
        Price = 80000.00M,
        Length = 7.92M
    },
    new Yacht
    {
        Name = "Maryland 37",
        Shipyard = "Fountaine Pajot",
        Category = "Power Catamaran",
        LaunchDate = new DateTime(2005, 1, 1),
        Price = 229900.00M,
        Length = 11.28M
    },
    new Yacht
    {
        Name = "Leviathan",
        Shipyard = "Oceanco",
        Category = "Research Vessel",
        LaunchDate = new DateTime(2025, 8, 1),    
        Price = 500000000.00M,
        Length = 111.00M
    }
);

        context.SaveChanges();
    }
}