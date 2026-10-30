using NUnit.Framework;
using ShoppingSystem.Core;

namespace ShoppingSystem.Tests;

[TestFixture]
public class ProductTests
{
    [Test]
    public void Constructor_ValidValues_SetsProperties()
    {
        // Arrange + Act
        var product = new Product(1, "Mouse", 250m, 5);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(product.Id, Is.EqualTo(1));
            Assert.That(product.Name, Is.EqualTo("Mouse"));
            Assert.That(product.Price, Is.EqualTo(250m));
            Assert.That(product.Stock, Is.EqualTo(5));
        });
    }

    [Test]
    public void Constructor_NegativePrice_Throws()
    {
        Assert.Throws<ArgumentException>(() => new Product(1, "Mouse", -1m, 5));
    }

    [Test]
    public void Constructor_NegativeStock_Throws()
    {
        Assert.Throws<ArgumentException>(() => new Product(1, "Mouse", 10m, -1));
    }

    [Test]
    public void ReduceStock_ValidQuantity_ReducesStock()
    {
        // Arrange
        var product = new Product(1, "Mouse", 250m, 5);

        // Act
        product.ReduceStock(2);

        // Assert
        Assert.That(product.Stock, Is.EqualTo(3));
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(6)]
    public void ReduceStock_InvalidQuantity_Throws(int quantity)
    {
        // Arrange
        var product = new Product(1, "Mouse", 250m, 5);

        // Act + Assert
        Assert.Throws<ArgumentException>(() => product.ReduceStock(quantity));
    }
}
