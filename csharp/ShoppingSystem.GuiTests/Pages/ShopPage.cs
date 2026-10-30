using OpenQA.Selenium;

namespace ShoppingSystem.GuiTests.Pages;

public class ShopPage : BasePage
{
    public ShopPage(IWebDriver driver) : base(driver) { }

    public ShopPage WaitUntilLoaded()
    {
        Visible(By.CssSelector("#products .product"));
        return this;
    }

    public string LoggedInUser => Visible(By.Id("who")).Text;

    public void Search(string text) => Type(Visible(By.Id("search")), text);

    public List<string> ProductNames() =>
        Driver.FindElements(By.CssSelector("#products .product strong")).Select(e => e.Text).ToList();

    public List<string> WaitForProductNames(int count) =>
        WaitFor(() => ProductNames() is { } names && names.Count == count ? names : null,
            count + " products");

    public void AddToCart(int productId, int quantity)
    {
        Type(Visible(By.Id("qty-" + productId)), quantity.ToString());
        Visible(By.CssSelector($"button[data-add='{productId}']")).Click();
    }

    public string WaitForCartToContain(string text) =>
        WaitFor(() =>
        {
            var cart = Driver.FindElement(By.Id("cart")).Text;
            return cart.Contains(text) ? cart : null;
        }, "cart containing '" + text + "'");

    public string CartText => Driver.FindElement(By.Id("cart")).Text;

    public void ClickCheckout() => Visible(By.Id("btn-checkout")).Click();

    public void PayWithCard(string card, string expiry, string cvv)
    {
        Type(Visible(By.Id("card")), card);
        Type(Visible(By.Id("exp")), expiry);
        Type(Visible(By.Id("cvv")), cvv);
        Visible(By.CssSelector("#pay-form button[type=submit]")).Click();
    }

    public void PayCashOnDelivery()
    {
        Visible(By.CssSelector("#pay-form input[value=CASH_ON_DELIVERY]")).Click();
        Visible(By.CssSelector("#pay-form button[type=submit]")).Click();
    }

    public OrdersPage OpenOrders()
    {
        Visible(By.CssSelector("button.nav[data-view='orders']")).Click();
        return new OrdersPage(Driver);
    }

    public void LogOut() => Visible(By.Id("logout")).Click();
}
