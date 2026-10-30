# QA Elective Progress

## Iteration 1 (C# / NUnit)
Solution in `csharp/`: ShoppingSystem.Core and ShoppingSystem.Tests.
Built with TDD (red, green and refactor commits in `git log`).
Uses Arrange-Act-Assert, [TestCase] data-driven tests, inheritance (ShopTestBase)
and composition (ProductBuilder). Run with `dotnet test csharp`.

## Iteration 2 (API testing)
`csharp/ShoppingSystem.ApiTests` (NUnit + HttpClient) tests the live Javalin API against `openapi.yaml`
(served at /openapi.yaml, Swagger UI at /docs.html). Covers auth, cart, checkout, orders, session
isolation, data-driven invalid input, spec coverage, and Swagger Petstore practice.
Run: start the app (`mvn -q compile exec:java`), then `dotnet test csharp/ShoppingSystem.ApiTests`.
Report: docs/api-test-report.html

## Iteration 3 (GUI testing)
`csharp/ShoppingSystem.GuiTests`: Selenium WebDriver programmed tests (not record and replay) using
Page Objects (AuthPage, ShopPage, OrdersPage, BasePage) to separate mechanics from test logic.
Covers sign up, log in/out, search, cart, card and cash payment, order history, and invalid-input
data-driven cases. Screenshots are saved on failure.
Run: start the app, then `dotnet test csharp/ShoppingSystem.GuiTests` (BROWSER=chrome, HEADLESS=0 optional).
Report: docs/gui-test-report.html
