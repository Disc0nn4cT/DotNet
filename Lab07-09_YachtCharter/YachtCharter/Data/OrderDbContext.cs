using Microsoft.EntityFrameworkCore;
using YachtCharter.Models;

namespace YachtCharter.Data;

// Окрема база orders.db для замовлень; каталог у charter.db не змінюється.
public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
{
    public DbSet<PurchaseOrder> Orders => Set<PurchaseOrder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Унікальний індекс: дві однакові спроби не створять два записи.
        modelBuilder.Entity<PurchaseOrder>().HasIndex(o => o.RequestId).IsUnique();

        // Одне замовлення має багато позицій; видалення замовлення видаляє його позиції.
        modelBuilder.Entity<PurchaseOrder>().HasMany(o => o.Items).WithOne()
            .HasForeignKey(i => i.PurchaseOrderId).OnDelete(DeleteBehavior.Cascade);
    }
}
