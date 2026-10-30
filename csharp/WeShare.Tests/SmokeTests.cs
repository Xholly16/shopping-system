namespace WeShare.Tests;

[TestFixture]
public class SmokeTests
{
    [Test]
    public async Task WeShare_IsReachable()
    {
        var url = WeShareConfig.Require();
        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };

        var response = await http.GetAsync(url);

        Assert.That((int)response.StatusCode, Is.LessThan(500));
    }
}
