using System.Text.RegularExpressions;
using Vogen;

namespace Api.Domain.ValueObjects;

[ValueObject<string>]
public partial struct Email
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static Validation Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Validation.Invalid("El email no puede estar vacío.");
        }

        var trimmed = value.Trim();
        if (!EmailRegex.IsMatch(trimmed))
        {
            return Validation.Invalid("El formato del email no es válido.");
        }

        return Validation.Ok;
    }
}
