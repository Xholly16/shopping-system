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
