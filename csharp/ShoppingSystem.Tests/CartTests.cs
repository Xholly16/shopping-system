using NUnit.Framework;
using ShoppingSystem.Core;
using ShoppingSystem.Tests.Helpers;

namespace ShoppingSystem.Tests;

[TestFixture]
public class CartTests : ShopTestBase
{
    [Test]
    public void Add_ValidItem_UpdatesTotal()
    {
        // Act
        cart.Add(mouse, 2);

        // Assert
        Assert.That(cart.Total, Is.EqualTo(500m));
    }

    [Test]
    public void Add_SameProductTwice_MergesQuantity()
    {
        // Act
        cart.Add(mouse, 2);
        cart.Add(mouse, 1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(cart.Items, Has.Count.EqualTo(1));
            Assert.That(cart.Items[0].Quantity, Is.EqualTo(3));
        });
    }

    [TestCase(0)]
    [TestCase(-3)]
    public void Add_InvalidQuantity_Throws(int quantity)
    {
        Assert.Throws<ArgumentException>(() => cart.Add(mouse, quantity));
    }

    [Test]
    public void Add_MoreThanStock_Throws()
    {
        Assert.Throws<ArgumentException>(() => cart.Add(mouse, 6));
    }

    [Test]
    public void Remove_MissingItem_ReturnsFalse()
    {
        Assert.That(cart.Remove(99), Is.False);
    }

    [Test]
    public void Remove_ExistingItem_ReturnsTrueAndEmptiesCart()
    {
        // Arrange
        cart.Add(mouse, 1);

        // Act
        var removed = cart.Remove(2);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(removed, Is.True);
            Assert.That(cart.IsEmpty, Is.True);
        });
    }
}
