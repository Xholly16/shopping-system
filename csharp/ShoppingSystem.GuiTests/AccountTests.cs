using ShoppingSystem.GuiTests.Support;

namespace ShoppingSystem.GuiTests;

[TestFixture]
public class AccountTests : GuiTestBase
{
    [Test]
    public void SignUp_ValidDetails_ShowsTheShop()
    {
        var name = SignUpNewUser();

        Assert.That(Shop.LoggedInUser, Is.EqualTo(name));
    }

    [Test]
    public void SignUp_PasswordsDoNotMatch_ShowsError()
    {
        Auth.Open().SignUp(UniqueName(), "a@test.com", "password123", confirm: "different123");

        Assert.That(Auth.WaitForMessage("do not match"), Does.Contain("Passwords do not match"));
    }

    [TestCase("ab", "a@test.com", "password123", "Username must be")]
    [TestCase("validname1", "a@b", "password123", "valid email")]
    [TestCase("validname1", "a@test.com", "short", "at least 8")]
    public void SignUp_InvalidDetails_ShowsServerError(string user, string email, string pass, string expected)
    {
        Auth.Open().SignUp(user, email, pass);

        Assert.That(Auth.WaitForMessage(expected), Does.Contain(expected));
    }

    [Test]
    public void LogIn_WrongPassword_ShowsError()
    {
        Auth.Open().LogIn("nobody_here", "wrongpass99");

        Assert.That(Auth.WaitForMessage("Invalid username or password"), Is.Not.Empty);
    }

    [Test]
    public void LogOut_ReturnsToTheLoginForm()
    {
        SignUpNewUser();

        Shop.LogOut();

        Assert.That(Auth.IsShowingLoginForm(), Is.True);
    }
}
