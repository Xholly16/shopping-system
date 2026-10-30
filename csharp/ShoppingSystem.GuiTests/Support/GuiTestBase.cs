using OpenQA.Selenium;
using ShoppingSystem.GuiTests.Pages;

namespace ShoppingSystem.GuiTests.Support;

public abstract class GuiTestBase
{
    protected IWebDriver Driver = null!;
    protected AuthPage Auth = null!;
    protected ShopPage Shop = null!;

    [SetUp]
    public void StartBrowser()
    {
        Driver = DriverFactory.Create();
        Auth = new AuthPage(Driver);
        Shop = new ShopPage(Driver);
    }

    [TearDown]
    public void StopBrowser()
    {
        if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
        {
            var dir = Path.Combine(TestContext.CurrentContext.WorkDirectory, "screenshots");
            Directory.CreateDirectory(dir);
            var file = Path.Combine(dir, TestContext.CurrentContext.Test.Name + ".png");
            ((ITakesScreenshot)Driver).GetScreenshot().SaveAsFile(file);
            TestContext.AddTestAttachment(file);
        }
        Driver.Quit();
    }

    protected static string UniqueName() => "gui" + Guid.NewGuid().ToString("N")[..8];

    /// <summary>Creates an account through the UI and waits for the shop page.</summary>
    protected string SignUpNewUser()
    {
        var name = UniqueName();
        Auth.Open().SignUp(name, name + "@test.com", "password123");
        Shop.WaitUntilLoaded();
        return name;
    }
}
