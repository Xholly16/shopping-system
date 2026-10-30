using System.Net;
using System.Text;
using System.Text.Json;

namespace ShoppingSystem.ApiTests.Helpers;

/// <summary>One simulated browser: keeps its own session cookie.</summary>
public class ApiClient : IDisposable
{
    public static string BaseUrl =>
        Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost:7070";

    private readonly HttpClient _http;

    public record Response(HttpStatusCode Status, string Body)
    {
        public JsonDocument Json() => JsonDocument.Parse(Body);
    }

    public ApiClient()
    {
        var handler = new HttpClientHandler { CookieContainer = new CookieContainer(), UseCookies = true };
        _http = new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) };
    }

    public Task<Response> Get(string path) => Send(HttpMethod.Get, path, null);
    public Task<Response> Post(string path, object? body = null) => Send(HttpMethod.Post, path, body ?? new { });
    public Task<Response> Delete(string path) => Send(HttpMethod.Delete, path, null);

    public async Task<Response> Send(HttpMethod method, string path, object? body)
    {
        var request = new HttpRequestMessage(method, path);
        if (body != null)
            request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        var response = await _http.SendAsync(request);
        return new Response(response.StatusCode, await response.Content.ReadAsStringAsync());
    }

    public static string UniqueName() => "user" + Guid.NewGuid().ToString("N")[..8];

    public async Task<string> SignUp()
    {
        var name = UniqueName();
        var r = await Post("/api/signup",
            new { username = name, email = name + "@test.com", password = "password123" });
        Assert.That(r.Status, Is.EqualTo(HttpStatusCode.Created), "sign up should succeed");
        return name;
    }

    public async Task AddUsbCable(int quantity)
    {
        var r = await Post("/api/cart", new { productId = 5, quantity });
        Assert.That(r.Status, Is.EqualTo(HttpStatusCode.OK), "adding to cart should succeed");
    }

    public void Dispose() => _http.Dispose();
}
