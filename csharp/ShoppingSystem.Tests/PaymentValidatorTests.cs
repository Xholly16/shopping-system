using NUnit.Framework;
using ShoppingSystem.Core;

namespace ShoppingSystem.Tests;

[TestFixture]
public class PaymentValidatorTests
{
    [TestCase("4242424242424242", true)]
    [TestCase("4242 4242 4242 4242", true)]
    [TestCase("4242-4242-4242-4242", true)]
    [TestCase("4242424242424241", false)]
    [TestCase("1234", false)]
    [TestCase("", false)]
    [TestCase("abcd424242424242", false)]
    public void IsValidLuhn_VariousCardNumbers_ReturnsExpected(string number, bool expected)
    {
        Assert.That(PaymentValidator.IsValidLuhn(number), Is.EqualTo(expected));
    }

    [TestCase("123", true)]
    [TestCase("1234", true)]
    [TestCase("12", false)]
    [TestCase("12345", false)]
    [TestCase("abc", false)]
    [TestCase("", false)]
    [TestCase(null, false)]
    public void IsValidCvv_VariousInputs_ReturnsExpected(string? cvv, bool expected)
    {
        Assert.That(PaymentValidator.IsValidCvv(cvv), Is.EqualTo(expected));
    }
}
