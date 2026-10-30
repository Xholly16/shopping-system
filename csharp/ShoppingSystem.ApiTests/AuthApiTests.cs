using System.Net;
using ShoppingSystem.ApiTests.Helpers;

namespace ShoppingSystem.ApiTests;

[TestFixture]
public class AuthApiTests : ApiTestBase
{
    [Test]
    public async Task Products_AreAvailableWithoutLogin()
    {
        var r = await Api.Get("/api/products");
        using var doc = r.Json();

        Assert.Multiple(() =>
        {
            Assert.That(r.Status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(doc.RootElement.GetArrayLength(), Is.GreaterThanOrEqualTo(5));
        });
    }

    [TestCase("GET", "/api/cart")]
    [TestCase("POST", "/api/cart")]
    [TestCase("DELETE", "/api/cart")]
    [TestCase("DELETE", "/api/cart/1")]
    [TestCase("POST", "/api/checkout")]
    [TestCase("GET", "/api/orders")]
    [TestCase("GET", "/api/me")]
    [TestCase("POST", "/api/logout")]
    public async Task SecuredEndpoint_WhenLoggedOut_Returns401(string method, string path)
    {
        var r = await Api.Send(new HttpMethod(method), path, method == "POST" ? new { } : null);
        Assert.That(r.Status, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task SignUp_ValidUser_Returns201AndNeverReturnsPassword()
    {
        var name = ApiClient.UniqueName();
        var r = await Api.Post("/api/signup",
            new { username = name, email = name + "@test.com", password = "password123" });

        Assert.Multiple(() =>
        {
            Assert.That(r.Status, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(r.Body, Does.Contain(name));
            Assert.That(r.Body.ToLower(), Does.Not.Contain("password"));
        });
    }

    [Test]
    public async Task SignUp_DuplicateUsername_Returns400()
    {
        var name = await Api.SignUp();
        using var other = new ApiClient();

        var r = await other.Post("/api/signup",
            new { username = name, email = "x@test.com", password = "password123" });

        Assert.That(r.Status, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [TestCase("ab", "a@b.com", "password123")]
    [TestCase("validname", "not-an-email", "password123")]
    [TestCase("validname", "a@b.com", "short")]
    public async Task SignUp_InvalidInput_Returns400(string username, string email, string password)
    {
        var r = await Api.Post("/api/signup", new { username, email, password });
        Assert.That(r.Status, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Login_CorrectPassword_Returns200()
    {
        var name = await Api.SignUp();
        using var other = new ApiClient();

        var r = await other.Post("/api/login", new { username = name, password = "password123" });

        Assert.That(r.Status, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Login_WrongPassword_Returns400()
    {
        var name = await Api.SignUp();
        using var other = new ApiClient();

        var r = await other.Post("/api/login", new { username = name, password = "wrongpass99" });

        Assert.That(r.Status, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Logout_EndsTheSession()
    {
        await Api.SignUp();
        await Api.Post("/api/logout");

        var r = await Api.Get("/api/cart");

        Assert.That(r.Status, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}
