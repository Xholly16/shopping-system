using OpenQA.Selenium;

namespace ShoppingSystem.GuiTests.Pages;

public class OrdersPage : BasePage
{
    public OrdersPage(IWebDriver driver) : base(driver) { }

    public string WaitForOrderContaining(string text) =>
        WaitFor(() =>
        {
            var orders = Driver.FindElement(By.Id("orders")).Text;
            return orders.Contains(text) ? orders : null;
        }, "orders containing '" + text + "'");

    public string Text => Visible(By.Id("orders")).Text;
}
