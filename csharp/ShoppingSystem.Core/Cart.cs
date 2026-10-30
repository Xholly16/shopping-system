namespace ShoppingSystem.Core;

public class Cart
{
    private readonly List<CartItem> _items = new();

    public IReadOnlyList<CartItem> Items => _items;
    public bool IsEmpty => _items.Count == 0;
    public decimal Total => _items.Sum(i => i.Subtotal);

    public void Add(Product product, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive");

        var index = _items.FindIndex(i => i.Product.Id == product.Id);
        var newQuantity = (index >= 0 ? _items[index].Quantity : 0) + quantity;

        if (newQuantity > product.Stock)
            throw new ArgumentException($"Not enough stock for {product.Name}");

        if (index >= 0)
            _items[index] = new CartItem(product, newQuantity);
        else
            _items.Add(new CartItem(product, newQuantity));
    }

    public bool Remove(int productId) =>
        _items.RemoveAll(i => i.Product.Id == productId) > 0;

    public void Clear() => _items.Clear();
}
