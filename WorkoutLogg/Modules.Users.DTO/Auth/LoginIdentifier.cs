using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Modules.Users.DTO.Auth;

public static class LoginIdentifier
{
    // International format; Russian domestic 8XXXXXXXXXX is accepted as +7XXXXXXXXXX.
    public static string? NormalizePhone(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 40) return null;
        var phone = Regex.Replace(value.Trim(), @"[\s()\-]", "");
        if (phone.Length == 11 && phone[0] == '8') phone = "+7" + phone[1..];
        else if (phone.Length == 11 && phone[0] == '7') phone = "+" + phone;
        return Regex.IsMatch(phone, @"^\+[1-9][0-9]{7,14}$") ? phone : null;
    }
    public static string? Normalize(string? value)
    {
        var text = value?.Trim();
        if (string.IsNullOrEmpty(text) || text.Length > 256) return null;
        return text.Contains('@') ? new EmailAddressAttribute().IsValid(text) ? text : null : NormalizePhone(text);
    }
}
