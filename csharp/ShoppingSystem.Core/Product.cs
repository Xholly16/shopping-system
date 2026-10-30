namespace ShoppingSystem.Core;

public class Product
{
    public int Id { get; }
    public string Name { get; }
    public decimal Price { get; }
    public int Stock { get; private set; }

    public Product(int id, string name, decimal price, int stock)
    {
        if (price < 0 || stock < 0)
            throw new ArgumentException("Price and stock cannot be negative");

        Id = id;
        Name = name;
        Price = price;
        Stock = stock;
    }

    public void ReduceStock(int quantity)
    {
        if (quantity <= 0 || quantity > Stock)
            throw new ArgumentException($"Invalid quantity: {quantity}");

        Stock -= quantity;
    }
}
