using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace JWTInspector.Models;
public record JWTToken(JWTHeader Header, ObservableCollection<JWTClaim> Body, JWTSignature Signature)
{
    private const string TOKEN_PART_DELIMITER = ".";
    public static (string?, JWTToken?) Parse(string tokenText)
    {
        var parts = tokenText.Split(TOKEN_PART_DELIMITER);
        if (parts.Length != 3)
            return ("Token is not valid, it does not contain 3 parts", default);

        var jsonOpts = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        jsonOpts.Converters.Add(new JsonStringEnumConverter());
        try
        {
            var base64decoded = WebEncoders.Base64UrlDecode(parts[0]);
            var header = JsonSerializer.Deserialize<JWTHeader>(base64decoded, jsonOpts);
            if (header is null)
                return ("Token header could not be parsed", default);
            base64decoded = WebEncoders.Base64UrlDecode(parts[1]);
            var parsedBody = JsonDocument.Parse(base64decoded);
            var body = new ObservableCollection<JWTClaim>();
            foreach (var item in parsedBody.RootElement.EnumerateObject())
            {
                var claim = new JWTClaim(item.Name, item.Value, ClaimToTooltip(item.Name, item.Value));
                body.Add(claim);
            }
            return (default, new JWTToken(header, body, new JWTSignature(parts[2], SignatureState.Unknown)));
        }
        catch (Exception ex)
        {
            return ($"Failed to process token {ex.Message}", default);
        }
    }

    private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private static string? ClaimToTooltip(string name, JsonElement value)
    {
        return name switch
        {
            "exp" => string.Format("Expires on {0}", UnixEpoch.AddSeconds(value.GetInt64()).ToString("o")),
            "nbf" => string.Format("Valid from {0}", UnixEpoch.AddSeconds(value.GetInt64()).ToString("o")),
            "iat" => string.Format("Issued on {0}", UnixEpoch.AddSeconds(value.GetInt64()).ToString("o")),
            _ => null
        };
    }
}
