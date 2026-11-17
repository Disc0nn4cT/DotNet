using Microsoft.EntityFrameworkCore;
using YachtCharter.Models;

namespace YachtCharter.Data;

public class YachtDbContext : DbContext
{
    public YachtDbContext(DbContextOptions<YachtDbContext> options) : base(options) { }

    public DbSet<Yacht> Yachts => Set<Yacht>();
}
