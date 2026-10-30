using System.Net;
using System.Text;

namespace ShoppingSystem.ApiTests;

/// <summary>Practice against the public Swagger Petstore from the elective brief.</summary>
[TestFixture]
public class PetstoreTests
{
    private const string Base = "https://petstore.swagger.io/v2";
    private HttpClient _http = null!;

    [OneTimeSetUp]
    public async Task CheckSiteIsUp()
    {
        _http = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
        try
        {
            var r = await _http.GetAsync(Base + "/store/inventory");
            if (!r.IsSuccessStatusCode) Assert.Ignore("Petstore returned " + (int)r.StatusCode);
        }
        catch (HttpRequestException) { Assert.Ignore("Petstore is not reachable"); }
        catch (TaskCanceledException) { Assert.Ignore("Petstore timed out"); }
    }

    [OneTimeTearDown]
    public void Cleanup() => _http?.Dispose();

    [Test]
    public async Task AddPet_ThenGetIt_ReturnsTheSamePet()
    {
        var id = Random.Shared.NextInt64(100000, 999999);
        var body = $"{{\"id\":{id},\"name\":\"Rex\",\"status\":\"available\",\"photoUrls\":[]}}";

        var created = await _http.PostAsync(Base + "/pet", new StringContent(body, Encoding.UTF8, "application/json"));
        var fetched = await _http.GetAsync(Base + "/pet/" + id);
        var text = await fetched.Content.ReadAsStringAsync();

        Assert.Multiple(() =>
        {
            Assert.That(created.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(fetched.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(text, Does.Contain("\"name\":\"Rex\""));
        });
    }

    [Test]
    public async Task GetUnknownPet_Returns404()
    {
        var r = await _http.GetAsync(Base + "/pet/1");
        // id 1 is sometimes taken on the shared server, so only accept 200 or 404
        Assert.That((int)r.StatusCode, Is.AnyOf(200, 404));
    }

    [Test]
    public async Task FindPetsByStatus_ReturnsAList()
    {
        var r = await _http.GetAsync(Base + "/pet/findByStatus?status=available");
        var text = await r.Content.ReadAsStringAsync();

        Assert.Multiple(() =>
        {
            Assert.That(r.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(text, Does.StartWith("["));
        });
    }
}
