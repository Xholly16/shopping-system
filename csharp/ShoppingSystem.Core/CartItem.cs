namespace ShoppingSystem.Core;

public class CartItem
{
    public Product Product { get; }
    public int Quantity { get; }
    public decimal Subtotal => Product.Price * Quantity;

    public CartItem(Product product, int quantity)
    {
        Product = product;
        Quantity = quantity;
    }
}
