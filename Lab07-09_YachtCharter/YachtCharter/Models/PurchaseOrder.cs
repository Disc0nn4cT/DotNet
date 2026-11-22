using System.ComponentModel.DataAnnotations.Schema;

namespace YachtCharter.Models;

// Збережене замовлення (заголовок). Одне замовлення — багато позицій.
public class PurchaseOrder
{
    public int Id { get; set; }                  // номер замовлення, використовується в URL /orders/{Id}
    public Guid RequestId { get; set; }          // ключ однієї спроби оформлення (захист від дубля)
    public DateTime CreatedUtc { get; set; }
    // У лабораторній 9 ці поля порожні; форма покупця з'явиться в лабораторній 10.
    public string CustomerName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string DeliveryAddress { get; set; } = "";
    public string Comment { get; set; } = "";
    public List<PurchaseItem> Items { get; set; } = new();
    [NotMapped] public decimal Total => Items.Sum(item => item.Total);
}

// Позиція замовлення — знімок назви яхти, опцій і ціни на момент оформлення.
public class PurchaseItem
{
    public int Id { get; set; }
    public int PurchaseOrderId { get; set; }     // зовнішній ключ на PurchaseOrder
    public string ProductName { get; set; } = "";
    public string Options { get; set; } = "";    // напр. "3 доби · зі шкіпером"
    public int Quantity { get; set; }            // кількість яхт
    public decimal UnitPrice { get; set; }       // ціна однієї яхти за весь термін
    [NotMapped] public decimal Total => UnitPrice * Quantity;
}
