using ShoppingSystem.GuiTests.Support;

namespace ShoppingSystem.GuiTests;

[TestFixture]
public class ShoppingTests : GuiTestBase
{
    private const int UsbCable = 5;

    [SetUp]
    public void LogIn() => SignUpNewUser();

    [Test]
    public void Search_Mouse_ShowsOnlyTheMouse()
    {
        Shop.Search("mou");

        Assert.That(Shop.WaitForProductNames(1), Is.EqualTo(new[] { "Mouse" }));
    }

    [Test]
    public void AddToCart_ShowsTheItemAndTotal()
    {
        Shop.AddToCart(UsbCable, 2);

        var cart = Shop.WaitForCartToContain("USB Cable x2");

        Assert.That(cart, Does.Contain("R158.00"));
    }

    [Test]
    public void AddMoreThanStock_ShowsError_AndCartStaysEmpty()
    {
        Shop.AddToCart(UsbCable, 9999);

        Assert.Multiple(() =>
        {
            Assert.That(Shop.WaitForMessage("Not enough stock"), Is.Not.Empty);
            Assert.That(Shop.CartText, Does.Contain("Cart is empty"));
        });
    }

    [Test]
    public void Checkout_WithEmptyCart_ShowsError()
    {
        Shop.ClickCheckout();

        Assert.That(Shop.WaitForMessage("Cart is empty"), Is.Not.Empty);
    }

    [Test]
    public void PayWithCard_PlacesOrder_AndItAppearsInOrderHistory()
    {
        Shop.AddToCart(UsbCable, 1);
        Shop.WaitForCartToContain("USB Cable");
        Shop.ClickCheckout();
        Shop.PayWithCard("4242 4242 4242 4242", "12/40", "123");

        Shop.WaitForMessage("Order #");
        var orders = Shop.OpenOrders().WaitForOrderContaining("USB Cable");

        Assert.Multiple(() =>
        {
            Assert.That(orders, Does.Contain("ending 4242"));
            Assert.That(orders, Does.Not.Contain("4242 4242 4242 4242"));
        });
    }

    [Test]
    public void PayCashOnDelivery_PlacesOrder()
    {
        Shop.AddToCart(UsbCable, 1);
        Shop.WaitForCartToContain("USB Cable");
        Shop.ClickCheckout();
        Shop.PayCashOnDelivery();

        Shop.WaitForMessage("Order #");

        Assert.That(Shop.OpenOrders().WaitForOrderContaining("Cash on delivery"), Is.Not.Empty);
    }

    [TestCase("1234", "12/40", "123", "Invalid card number")]
    [TestCase("4242424242424241", "12/40", "123", "Invalid card number")]
    [TestCase("4242424242424242", "01/20", "123", "expired")]
    [TestCase("4242424242424242", "12/40", "12", "Invalid CVV")]
    public void PayWithInvalidDetails_ShowsError_AndKeepsTheCart(string card, string expiry, string cvv, string expected)
    {
        Shop.AddToCart(UsbCable, 1);
        Shop.WaitForCartToContain("USB Cable");
        Shop.ClickCheckout();
        Shop.PayWithCard(card, expiry, cvv);

        Assert.Multiple(() =>
        {
            Assert.That(Shop.WaitForMessage(expected), Is.Not.Empty);
            Assert.That(Shop.CartText, Does.Contain("USB Cable"));
        });
    }
}
