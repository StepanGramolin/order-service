namespace OrderService.DataAccess.Postgres.Models;

public sealed class Order
{
    // ID заказа
    public long OrderId { get; set; }

    public long ProductId { get; set; }
    public int Amount { get; set; }
    public string EmailClient { get; set; } = null!;
    public decimal Price { get; set; }
    public string PhoneNumber { get; set; } = null!;
}