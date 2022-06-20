using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace JWTInspector.Models;
public class TokenViewModel: INotifyPropertyChanged
{
    private string? _tokenString;
    public string? TokenString
    {
        get { return _tokenString; }
        set
        {
            _tokenString = value;
            OnPropertyChanged(nameof(TokenString));
            if (!string.IsNullOrEmpty(value))
                ParseToken(value);
        }
    }

    private Token? _token;
    public Token? Token { get { return _token; } set { _token = value; OnPropertyChanged(nameof(Token)); } }

    private string? _errorMessage;
    public string? ErrorMessage { get { return _errorMessage; } set { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); } }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private const char TOKEN_COMPONENT_DELIMITER = '.';

    public void ParseToken(string token)
    {
        var tokenComponents = token
            .Split(TOKEN_COMPONENT_DELIMITER)
            .ToArray();

        if (tokenComponents.Length != 3)
        {
            ErrorMessage = "Serialized token value is invalid, it does not contain 3 components";
            return;
        }

        var opts = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        opts.Converters.Add(new JsonStringEnumConverter());
        try
        {
            var header = JsonSerializer.Deserialize<Header>(DecodeSegment(tokenComponents.First()), opts)!;
            var body = JsonNode.Parse(DecodeSegment(tokenComponents.Skip(1).First()))!;
            var signature = tokenComponents.Last();
            Token = new Token(header, body.AsObject().Select(kvp => new BodyElement(kvp.Key, kvp.Value!.ToJsonString(), kvp.ToToolTip())), signature);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    private string DecodeSegment(string input)
    {
        return Encoding.UTF8.GetString(Convert.FromBase64String(input.PadRight(input.Length + (4 - input.Length % 4) % 4, '=')));
    }
}

public static class JsonNodeExtensions
{
    private static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public static string? ToToolTip(this KeyValuePair<string, JsonNode?> item)
    {
        if (item.Value is null || item.Value is not JsonValue)
        {
            return string.Empty;
        }

        var value = item.Value.AsValue();
        return item.Key switch
        {
            "exp" => string.Format("Expires on {0}", UnixEpoch.AddSeconds((long)value).ToString("o")),
            "nbf" => string.Format("Valid from {0}", UnixEpoch.AddSeconds((long)value).ToString("o")),
            "iat" => string.Format("Issued on {0}", UnixEpoch.AddSeconds((long)value).ToString("o")),
            _ => null
        };
    }
}

public class Token: INotifyPropertyChanged
{
    private Header? _header;
    public Header? Header { get { return _header; } set { _header = value; OnNotifyPropertyChanged(nameof(Header)); } }
    private ObservableCollection<BodyElement> _body;
    public ObservableCollection<BodyElement> Body { get { return _body; } set { _body = value; OnNotifyPropertyChanged(nameof(Body)); } }
    private string _signature;
    public string Signature { get { return _signature; } set { _signature = value; OnNotifyPropertyChanged(nameof(Signature)); } }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnNotifyPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public Token(Header? header, IEnumerable<BodyElement> body, string signature)
    {
        _header = header;
        _body = new ObservableCollection<BodyElement>(body);
        _signature = signature;
    }
}

public record BodyElement(string Key, string Value, string? ToolTip) { }

public record Header([property: JsonPropertyName("alg")] TokenSignatureAlgorithm Algorithm, [property: JsonPropertyName("typ")] TokenKind Kind) { }

public enum TokenSignatureAlgorithm
{
    HS256,
    HS384,
    HS512,
    RS256,
    RS384,
    RS512,
    ES256,
    ES384,
    ES512,
    PS256,
    PS384,
    PS512,
}

public enum TokenKind
{
    JWT
}
