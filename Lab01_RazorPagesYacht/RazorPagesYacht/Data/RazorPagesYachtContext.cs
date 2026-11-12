using Microsoft.EntityFrameworkCore;
using RazorPagesYacht.Models;

namespace RazorPagesYacht.Data;

public class RazorPagesYachtContext(DbContextOptions<RazorPagesYachtContext> options) : DbContext(options)
{
    public DbSet<Yacht> Yacht { get; set; } = default!;
    public DbSet<Review> Reviews { get; set; } = default!;

    public DbSet<Feedback> Feedbacks { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // One review per user per yacht
        modelBuilder.Entity<Review>()
            .HasIndex(r => new { r.YachtId, r.UserId })
            .IsUnique();

        // Yacht 1 —— * Review; deleting a yacht deletes its reviews
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Yacht)
            .WithMany(y => y.Reviews)
            .HasForeignKey(r => r.YachtId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}