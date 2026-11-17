namespace YachtCharter.Models;

// Одна яхта з каталогу (товар). Не змінюється, коли користувач налаштовує оренду.
public class Yacht
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Type { get; set; } = "";          // вітрильна / катамаран / моторна
    public int Cabins { get; set; }                 // кількість кают
    public string ImageUrl { get; set; } = "";      // шлях від wwwroot, наприклад images/yacht-1.svg
    public decimal BasePrice { get; set; }          // ціна оренди за добу, грн
}
