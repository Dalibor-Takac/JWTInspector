using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.ObjectModel;
using System.Text.Json;
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
                var claim = new JWTClaim(item.Name, item.Value.ToString(), item.Value.ToString());
                body.Add(claim);
            }
            return (default, new JWTToken(header, body, new JWTSignature(parts[2], SignatureState.Unknown)));
        }
        catch (Exception ex)
        {
            return ($"Failed to process token {ex.Message}", default);
        }
    }
}
