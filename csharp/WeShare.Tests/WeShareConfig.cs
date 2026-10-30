namespace WeShare.Tests;

public static class WeShareConfig
{
    /// <summary>Set WESHARE_URL to wherever the Docker container is exposed, e.g. http://localhost:8080</summary>
    public static string? BaseUrl => Environment.GetEnvironmentVariable("WESHARE_URL");

    public static string Require()
    {
        if (string.IsNullOrWhiteSpace(BaseUrl))
            Assert.Ignore("Set WESHARE_URL to run the WeShare tests");
        return BaseUrl!;
    }
}
