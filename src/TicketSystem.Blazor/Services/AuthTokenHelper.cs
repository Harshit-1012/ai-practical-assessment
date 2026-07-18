using System.Text;
using System.Text.Json;

namespace TicketSystem.Blazor.Services;

internal static class AuthTokenHelper
{
    public static bool IsTokenValid(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        var parts = token.Split('.');
        if (parts.Length != 3)
        {
            return false;
        }

        try
        {
            var payloadJson = DecodeBase64Url(parts[1]);
            using var document = JsonDocument.Parse(payloadJson);

            if (!document.RootElement.TryGetProperty("exp", out var expiryElement))
            {
                return false;
            }

            var expiry = DateTimeOffset.FromUnixTimeSeconds(expiryElement.GetInt64());
            return expiry > DateTimeOffset.UtcNow;
        }
        catch
        {
            return false;
        }
    }

    private static string DecodeBase64Url(string input)
    {
        var padded = input.Replace('-', '+').Replace('_', '/');
        var remainder = padded.Length % 4;
        if (remainder == 2)
        {
            padded += "==";
        }
        else if (remainder == 3)
        {
            padded += "=";
        }

        return Encoding.UTF8.GetString(Convert.FromBase64String(padded));
    }
}
