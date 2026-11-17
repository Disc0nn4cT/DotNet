namespace YachtCharter.Models;

// Підтверджена позиція замовлення — «знімок» вибору. Після створення не змінюється (init).
public class OrderLine
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public int YachtId { get; init; }
    public string YachtName { get; init; } = "";
    public int Days { get; init; }
    public string Crew { get; init; } = "";
    public int Quantity { get; init; }              // кількість яхт (флотилія)
    public decimal UnitPrice { get; init; }         // ціна оренди однієї яхти за весь термін
    public decimal Total => UnitPrice * Quantity;
}
