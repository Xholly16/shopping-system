namespace ShoppingSystem.Core;

public class Order
{
    public int Id { get; }
    public IReadOnlyList<CartItem> Items { get; }
    public decimal Total { get; }

    public Order(int id, IReadOnlyList<CartItem> items, decimal total)
    {
        Id = id;
        Items = items;
        Total = total;
    }
}
