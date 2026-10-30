using NUnit.Framework;
using ShoppingSystem.Core;

namespace ShoppingSystem.Tests;

[TestFixture]
public class CartTests
{
    private Cart _cart = null!;
    private Product _mouse = null!;

    [SetUp]
    public void SetUp()
    {
        _cart = new Cart();
        _mouse = new Product(2, "Mouse", 250m, 5);
    }

    [Test]
    public void Add_ValidItem_UpdatesTotal()
    {
        // Act
        _cart.Add(_mouse, 2);

        // Assert
        Assert.That(_cart.Total, Is.EqualTo(500m));
    }

    [Test]
    public void Add_SameProductTwice_MergesQuantity()
    {
        // Act
        _cart.Add(_mouse, 2);
        _cart.Add(_mouse, 1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(_cart.Items, Has.Count.EqualTo(1));
            Assert.That(_cart.Items[0].Quantity, Is.EqualTo(3));
        });
    }

    [TestCase(0)]
    [TestCase(-3)]
    public void Add_InvalidQuantity_Throws(int quantity)
    {
        Assert.Throws<ArgumentException>(() => _cart.Add(_mouse, quantity));
    }

    [Test]
    public void Add_MoreThanStock_Throws()
    {
        Assert.Throws<ArgumentException>(() => _cart.Add(_mouse, 6));
    }

    [Test]
    public void Remove_MissingItem_ReturnsFalse()
    {
        Assert.That(_cart.Remove(99), Is.False);
    }

    [Test]
    public void Remove_ExistingItem_ReturnsTrueAndEmptiesCart()
    {
        // Arrange
        _cart.Add(_mouse, 1);

        // Act
        var removed = _cart.Remove(2);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(removed, Is.True);
            Assert.That(_cart.IsEmpty, Is.True);
        });
    }
}
