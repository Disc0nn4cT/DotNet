using Microsoft.EntityFrameworkCore;
using RazorPagesYacht.Models;


namespace RazorPagesYacht.Data;

public class RazorPagesYachtContext(DbContextOptions<RazorPagesYachtContext> options) : DbContext(options)
{
    public DbSet<Yacht> Yacht { get; set; } = default!;
}