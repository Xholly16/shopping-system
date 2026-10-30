using System.Net;
using ShoppingSystem.ApiTests.Helpers;

namespace ShoppingSystem.ApiTests;

[TestFixture]
public class CheckoutApiTests : ApiTestBase
{
    private const string GoodCard = "4242424242424242";

    [SetUp]
    public async Task LogIn() => await Api.SignUp();

    [Test]
    public async Task Checkout_WithCard_ReturnsOrder_AndNeverTheFullCardNumber()
    {
        await Api.AddUsbCable(2);

        var r = await Api.Post("/api/checkout",
            new { method = "CARD", cardNumber = GoodCard, expiry = "12/40", cvv = "123" });

        Assert.Multiple(() =>
        {
            Assert.That(r.Status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(r.Body, Does.Contain("\"cardLast4\":\"4242\""));
            Assert.That(r.Body, Does.Not.Contain(GoodCard));
        });
    }

    [Test]
    public async Task Checkout_CashOnDelivery_NeedsNoCardDetails()
    {
        await Api.AddUsbCable(1);

        var r = await Api.Post("/api/checkout", new { method = "CASH_ON_DELIVERY" });

        Assert.Multiple(() =>
        {
            Assert.That(r.Status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(r.Body, Does.Contain("CASH_ON_DELIVERY"));
        });
    }

    [Test]
    public async Task Checkout_EmptyCart_Returns400()
    {
        var r = await Api.Post("/api/checkout", new { method = "CASH_ON_DELIVERY" });

        Assert.Multiple(() =>
        {
            Assert.That(r.Status, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(r.Body, Does.Contain("Cart is empty"));
        });
    }

    [TestCase("1234", "12/40", "123")]
    [TestCase("4242424242424241", "12/40", "123")]
    [TestCase(GoodCard, "01/20", "123")]
    [TestCase(GoodCard, "13/30", "123")]
    [TestCase(GoodCard, "12/40", "12")]
    public async Task Checkout_InvalidCardDetails_Returns400_AndKeepsCart(string card, string expiry, string cvv)
    {
        await Api.AddUsbCable(1);

        var r = await Api.Post("/api/checkout", new { method = "CARD", cardNumber = card, expiry, cvv });
        var cart = await Api.Get("/api/cart");

        Assert.Multiple(() =>
        {
            Assert.That(r.Status, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(cart.Body, Does.Contain("USB Cable"));
        });
    }

    [Test]
    public async Task Checkout_ReducesStockByTheQuantityBought()
    {
        int StockOfUsbCable(string body)
        {
            using var doc = System.Text.Json.JsonDocument.Parse(body);
            return doc.RootElement.EnumerateArray()
                .First(p => p.GetProperty("id").GetInt32() == 5).GetProperty("stock").GetInt32();
        }

        var before = StockOfUsbCable((await Api.Get("/api/products")).Body);
        await Api.AddUsbCable(1);
        await Api.Post("/api/checkout", new { method = "CASH_ON_DELIVERY" });
        var after = StockOfUsbCable((await Api.Get("/api/products")).Body);

        Assert.That(after, Is.EqualTo(before - 1));
    }

    [Test]
    public async Task Orders_ListsOnlyTheLoggedInUsersOrders()
    {
        await Api.AddUsbCable(1);
        await Api.Post("/api/checkout", new { method = "CASH_ON_DELIVERY" });
        using var other = new ApiClient();
        await other.SignUp();

        var mine = await Api.Get("/api/orders");
        var theirs = await other.Get("/api/orders");

        Assert.Multiple(() =>
        {
            Assert.That(mine.Body, Does.Contain("USB Cable"));
            Assert.That(theirs.Body, Is.EqualTo("[]"));
        });
    }
}
