using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace ShoppingSystem.GuiTests.Support;

public static class DriverFactory
{
    public static string BaseUrl =>
        Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost:7070";

    /// <summary>BROWSER=chrome|firefox (default firefox). HEADLESS=0 to watch the browser.</summary>
    public static IWebDriver Create()
    {
        var headless = Environment.GetEnvironmentVariable("HEADLESS") != "0";
        var browser = (Environment.GetEnvironmentVariable("BROWSER") ?? "firefox").ToLowerInvariant();

        if (browser == "chrome")
        {
            var options = new ChromeOptions();
            if (headless) options.AddArgument("--headless=new");
            options.AddArgument("--window-size=1280,900");
            return new ChromeDriver(options);
        }

        var firefox = new FirefoxOptions();
        if (headless) firefox.AddArgument("-headless");
        return new FirefoxDriver(firefox);
    }
}
