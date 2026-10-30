using System.Net;
using ShoppingSystem.ApiTests.Helpers;

namespace ShoppingSystem.ApiTests;

[TestFixture]
public class SpecTests : ApiTestBase
{
    [Test]
    public async Task OpenApiSpec_IsServed()
    {
        var r = await Api.Get("/openapi.yaml");

        Assert.Multiple(() =>
        {
            Assert.That(r.Status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(r.Body, Does.StartWith("openapi: 3."));
        });
    }

    [TestCase("/api/products")]
    [TestCase("/api/signup")]
    [TestCase("/api/login")]
    [TestCase("/api/logout")]
    [TestCase("/api/me")]
    [TestCase("/api/cart")]
    [TestCase("/api/cart/{id}")]
    [TestCase("/api/checkout")]
    [TestCase("/api/orders")]
    public async Task EveryEndpoint_IsDocumentedInTheSpec(string path)
    {
        var r = await Api.Get("/openapi.yaml");
        Assert.That(r.Body, Does.Contain("  " + path + ":"));
    }

    [Test]
    public async Task SecuredOperations_DocumentA401_AndTheApiReturnsIt()
    {
        var spec = (await Api.Get("/openapi.yaml")).Body;
        var r = await Api.Get("/api/cart");

        Assert.Multiple(() =>
        {
            Assert.That(spec, Does.Contain("\"401\""));
            Assert.That(r.Status, Is.EqualTo(HttpStatusCode.Unauthorized));
        });
    }
}
