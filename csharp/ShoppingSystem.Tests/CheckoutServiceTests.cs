using NUnit.Framework;
using ShoppingSystem.Core;
using ShoppingSystem.Tests.Helpers;

namespace ShoppingSystem.Tests;

[TestFixture]
public class CheckoutServiceTests : ShopTestBase
{
    private readonly CheckoutService _service = new();

    [Test]
    public void Checkout_ValidCart_ReducesStockAndClearsCart()
    {
        // Arrange
        var laptop = new ProductBuilder().WithId(1).WithName("Laptop")
            .WithPrice(1000m).WithStock(3).Build();
        cart.Add(laptop, 2);

        // Act
        var order = _service.Checkout(cart);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(order.Total, Is.EqualTo(2000m));
            Assert.That(laptop.Stock, Is.EqualTo(1));
            Assert.That(cart.IsEmpty, Is.True);
        });
    }

    [Test]
    public void Checkout_EmptyCart_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => _service.Checkout(cart));
    }

    [Test]
    public void Checkout_StockChangedAfterAdding_FailsWithoutDeductingAnything()
    {
        // Arrange
        var first = new ProductBuilder().WithId(1).WithStock(5).Build();
        var second = new ProductBuilder().WithId(2).WithStock(5).Build();
        cart.Add(first, 2);
        cart.Add(second, 4);
        second.ReduceStock(3);

        // Act + Assert
        Assert.Throws<ArgumentException>(() => _service.Checkout(cart));
        Assert.That(first.Stock, Is.EqualTo(5), "stock must not change when checkout fails");
    }
}
