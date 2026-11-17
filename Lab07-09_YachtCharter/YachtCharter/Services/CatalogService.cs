using Microsoft.EntityFrameworkCore;
using YachtCharter.Data;
using YachtCharter.Models;

namespace YachtCharter.Services;

// Читання каталогу з SQLite. Компоненти не працюють з DbContext напряму.
public class CatalogService
{
    private readonly IDbContextFactory<YachtDbContext> factory;

    public CatalogService(IDbContextFactory<YachtDbContext> factory)
    {
        this.factory = factory;
    }

    public async Task<List<Yacht>> GetYachtsAsync()
    {
        // Новий контекст на одну операцію; після await using він закривається.
        await using var db = await factory.CreateDbContextAsync();
        return await db.Yachts.AsNoTracking().OrderBy(y => y.Id).ToListAsync();
    }
}
