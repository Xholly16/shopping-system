using NUnit.Framework;
using ShoppingSystem.Core;

namespace ShoppingSystem.Tests.Helpers;

public abstract class ShopTestBase
{
    protected Cart cart = null!;
    protected Product mouse = null!;

    [SetUp]
    public void BaseSetUp()
    {
        cart = new Cart();
        mouse = new ProductBuilder().WithId(2).Build();
    }
}
