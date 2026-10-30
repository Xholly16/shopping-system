namespace ShoppingSystem.Core;

public class Cart
{
    public IReadOnlyList<CartItem> Items => throw new NotImplementedException();
    public bool IsEmpty => throw new NotImplementedException();
    public decimal Total => throw new NotImplementedException();

    public void Add(Product product, int quantity) => throw new NotImplementedException();
    public bool Remove(int productId) => throw new NotImplementedException();
    public void Clear() => throw new NotImplementedException();
}
