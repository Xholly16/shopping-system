using OpenQA.Selenium;
using ShoppingSystem.GuiTests.Support;

namespace ShoppingSystem.GuiTests.Pages;

/// <summary>Low-level mechanics (waiting, finding) live here, so tests only describe user actions.</summary>
public abstract class BasePage
{
    protected readonly IWebDriver Driver;

    protected BasePage(IWebDriver driver) => Driver = driver;

    protected T WaitFor<T>(Func<T?> probe, string what) where T : class
    {
        var end = DateTime.UtcNow.AddSeconds(10);
        while (DateTime.UtcNow < end)
        {
            try
            {
                var result = probe();
                if (result != null) return result;
            }
            catch (WebDriverException) { /* element not ready yet, keep polling */ }
            Thread.Sleep(100);
        }
        throw new WebDriverTimeoutException("Timed out waiting for " + what);
    }

    protected IWebElement Visible(By by) =>
        WaitFor(() =>
        {
            var element = Driver.FindElements(by).FirstOrDefault();
            return element is { Displayed: true } ? element : null;
        }, by.ToString());

    protected static void Type(IWebElement field, string text)
    {
        field.Clear();
        field.SendKeys(text);
    }

    public string WaitForMessage(string contains) =>
        WaitFor(() =>
        {
            var text = Driver.FindElement(By.Id("msg")).Text;
            return text.Contains(contains, StringComparison.OrdinalIgnoreCase) ? text : null;
        }, "message containing '" + contains + "'");
}
