using OpenQA.Selenium;
using ShoppingSystem.GuiTests.Support;

namespace ShoppingSystem.GuiTests.Pages;

public class AuthPage : BasePage
{
    public AuthPage(IWebDriver driver) : base(driver) { }

    public AuthPage Open()
    {
        Driver.Navigate().GoToUrl(DriverFactory.BaseUrl);
        Visible(By.Id("login-form"));
        return this;
    }

    public void SignUp(string username, string email, string password, string? confirm = null)
    {
        Visible(By.Id("tab-signup")).Click();
        Type(Visible(By.Id("su-user")), username);
        Type(Visible(By.Id("su-email")), email);
        Type(Visible(By.Id("su-pass")), password);
        Type(Visible(By.Id("su-pass2")), confirm ?? password);
        Visible(By.CssSelector("#signup-form button[type=submit]")).Click();
    }

    public void LogIn(string username, string password)
    {
        Visible(By.Id("tab-login")).Click();
        Type(Visible(By.Id("login-user")), username);
        Type(Visible(By.Id("login-pass")), password);
        Visible(By.CssSelector("#login-form button[type=submit]")).Click();
    }

    public bool IsShowingLoginForm()
    {
        Visible(By.Id("login-form"));
        return true;
    }
}
