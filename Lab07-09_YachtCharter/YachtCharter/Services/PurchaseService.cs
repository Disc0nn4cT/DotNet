using Microsoft.EntityFrameworkCore;
using YachtCharter.Data;
using YachtCharter.Models;

namespace YachtCharter.Services;

// Перевірка та збереження замовлень. Сторінки не працюють з OrderDbContext напряму.
public class PurchaseService(
    IDbContextFactory<OrderDbContext> factory,
    CatalogService catalog)
{
    public async Task<int> PlaceAsync(IReadOnlyList<OrderLine> lines, Guid requestId)
    {
        // 1. Порожній ключ не може позначати конкретну спробу оформлення.
        if (requestId == Guid.Empty)
            throw new ArgumentException("Відсутній ідентифікатор операції.");

        // 2. Якщо цю спробу вже збережено — повертаємо її номер без дубля.
        await using var db = await factory.CreateDbContextAsync();
        var existing = await db.Orders.SingleOrDefaultAsync(o => o.RequestId == requestId);
        if (existing is not null) return existing.Id;

        // 3. Кількість рядків і актуальний каталог.
        if (lines.Count is 0 or > 50)
            throw new ArgumentException("Замовлення має містити від 1 до 50 позицій.");

        var products = (await catalog.GetYachtsAsync()).ToDictionary(p => p.Id);
        var order = new PurchaseOrder { RequestId = requestId, CreatedUtc = DateTime.UtcNow };

        foreach (var line in lines)
        {
            if (!products.TryGetValue(line.YachtId, out var product))
                throw new ArgumentException("Яхта більше недоступна.");

            // 4. Відновлюємо чернетку і перераховуємо ціну за серверними правилами.
            var draft = new YachtDraft
            {
                Days = line.Days,
                Crew = YachtDraft.ParseCrew(line.Crew),
                Quantity = line.Quantity
            };
            var checkedLine = draft.CreateLine(product);

            // 5. Не приймаємо підмінену або застарілу ціну без згоди користувача.
            if (checkedLine.UnitPrice != line.UnitPrice)
                throw new ArgumentException("Ціна змінилася. Видаліть позицію й додайте її з каталогу знову.");

            // 6. Знімок назви, опцій і ціни.
            order.Items.Add(new PurchaseItem
            {
                ProductName = product.Name,
                Options = $"{YachtDraft.DaysText(checkedLine.Days)} · {checkedLine.Crew}",
                Quantity = checkedLine.Quantity,
                UnitPrice = checkedLine.UnitPrice
            });
        }

        db.Orders.Add(order);
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException)
        {
            // Повтор одночасного запиту з тим самим ключем не створює дубль.
            await using var retry = await factory.CreateDbContextAsync();
            var saved = await retry.Orders.AsNoTracking()
                .SingleOrDefaultAsync(o => o.RequestId == requestId);
            if (saved is not null) return saved.Id;
            throw;
        }

        // 7. Номер нового запису — для переходу на /orders/{id}.
        return order.Id;
    }

    public async Task<List<PurchaseOrder>> GetAllAsync()
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Orders.AsNoTracking().Include(o => o.Items)
            .OrderByDescending(o => o.Id).ToListAsync();
    }

    public async Task<PurchaseOrder?> GetAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Orders.AsNoTracking().Include(o => o.Items)
            .SingleOrDefaultAsync(o => o.Id == id);
    }
}
