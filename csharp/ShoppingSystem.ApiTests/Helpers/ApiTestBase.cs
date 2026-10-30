namespace ShoppingSystem.ApiTests.Helpers;

public abstract class ApiTestBase
{
    protected ApiClient Api = null!;

    [SetUp]
    public void BaseSetUp() => Api = new ApiClient();

    [TearDown]
    public void BaseTearDown() => Api.Dispose();
}
