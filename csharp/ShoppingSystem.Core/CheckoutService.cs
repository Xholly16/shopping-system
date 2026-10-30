namespace ShoppingSystem.Core;

public class CheckoutService
{
    private int _nextOrderId = 1;

    public Order Checkout(Cart cart)
    {
        if (cart.IsEmpty)
            throw new InvalidOperationException("Cart is empty");

        foreach (var item in cart.Items)
        {
            if (item.Quantity > item.Product.Stock)
                throw new ArgumentException($"Not enough stock for {item.Product.Name}");
        }

        foreach (var item in cart.Items)
            item.Product.ReduceStock(item.Quantity);

        var order = new Order(_nextOrderId++, cart.Items.ToList(), cart.Total);
        cart.Clear();
        return order;
    }
}
