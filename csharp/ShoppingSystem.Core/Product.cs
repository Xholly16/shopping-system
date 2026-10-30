namespace ShoppingSystem.Core;

public class Product
{
    public int Id { get; }
    public string Name { get; }
    public decimal Price { get; }
    public int Stock { get; private set; }

    public Product(int id, string name, decimal price, int stock)
    {
        throw new NotImplementedException();
    }

    public void ReduceStock(int quantity)
    {
        throw new NotImplementedException();
    }
}
