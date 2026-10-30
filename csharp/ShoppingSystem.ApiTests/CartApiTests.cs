using System.Net;
using ShoppingSystem.ApiTests.Helpers;

namespace ShoppingSystem.ApiTests;

[TestFixture]
public class CartApiTests : ApiTestBase
{
    [SetUp]
    public async Task LogIn() => await Api.SignUp();

    [Test]
    public async Task AddItem_ReturnsCartWithQuantityAndTotal()
    {
        var r = await Api.Post("/api/cart", new { productId = 5, quantity = 2 });
        using var doc = r.Json();
        var item = doc.RootElement.GetProperty("items")[0];

        Assert.Multiple(() =>
        {
            Assert.That(r.Status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(item.GetProperty("quantity").GetInt32(), Is.EqualTo(2));
            Assert.That(doc.RootElement.GetProperty("total").GetDecimal(), Is.EqualTo(158m));
        });
    }

    [TestCase(5, 9999)]
    [TestCase(5, 0)]
    [TestCase(5, -1)]
    [TestCase(999, 1)]
    public async Task AddItem_InvalidInput_Returns400(int productId, int quantity)
    {
        var r = await Api.Post("/api/cart", new { productId, quantity });
        Assert.That(r.Status, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task RemoveItem_EmptiesTheCart()
    {
        await Api.AddUsbCable(1);

        var r = await Api.Delete("/api/cart/5");
        using var doc = r.Json();

        Assert.That(doc.RootElement.GetProperty("items").GetArrayLength(), Is.EqualTo(0));
    }

    [Test]
    public async Task Carts_AreSeparatePerUser()
    {
        await Api.AddUsbCable(1);
        using var other = new ApiClient();
        await other.SignUp();

        var r = await other.Get("/api/cart");

        Assert.That(r.Body, Does.Not.Contain("USB Cable"));
    }
}
