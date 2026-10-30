using System.Text.RegularExpressions;

namespace ShoppingSystem.Core;

public static class PaymentValidator
{
    public static bool IsValidLuhn(string number)
    {
        var digits = number.Replace(" ", "").Replace("-", "");
        if (digits.Length < 13 || digits.Length > 19 || !digits.All(c => c is >= '0' and <= '9'))
            return false;

        var sum = 0;
        var doubleIt = false;
        for (var i = digits.Length - 1; i >= 0; i--)
        {
            var d = digits[i] - '0';
            if (doubleIt)
            {
                d *= 2;
                if (d > 9) d -= 9;
            }
            sum += d;
            doubleIt = !doubleIt;
        }
        return sum % 10 == 0;
    }

    public static bool IsValidCvv(string? cvv) =>
        cvv != null && Regex.IsMatch(cvv, @"^\d{3,4}$");
}
