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
    private Token? _token;
    public Token? Token { get { return _token; } set { _token = value; OnPropertyChanged(nameof(Token)); } }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private const char TOKEN_COMPONENT_DELIMITER = '.';

    public void ParseToken(string token)
    {
        var tokenComponents = token
            .Split(TOKEN_COMPONENT_DELIMITER)
            .ToArray();

        if (tokenComponents.Length != 3)
            throw new ArgumentException("Serialized token value is invalid, it does not contain 3 components", nameof(token));

        var opts = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        opts.Converters.Add(new JsonStringEnumConverter());
        var header = JsonSerializer.Deserialize<Header>(DecodeSegment(tokenComponents.First()), opts)!;
        var body = JsonNode.Parse(DecodeSegment(tokenComponents.Skip(1).First()))!;
        var signature = tokenComponents.Last();
        Token = new Token(header, body.AsObject().Select(kvp => new KeyValuePair<string, string>(kvp.Key, kvp.Value!.ToJsonString())), signature);
    }

    private string DecodeSegment(string input)
    {
        return Encoding.UTF8.GetString(Convert.FromBase64String(input.PadRight(input.Length + (4 - input.Length % 4) % 4, '=')));
    }
}

public class Token: INotifyPropertyChanged
{
    private Header _header;
    public Header Header { get { return _header; } set { _header = value; OnNotifyPropertyChanged(nameof(Header)); } }
    private ObservableCollection<KeyValuePair<string, string>> _body;
    public ObservableCollection<KeyValuePair<string, string>> Body { get { return _body; } set { _body = value; OnNotifyPropertyChanged(nameof(Body)); } }
    private string _signature;
    public string Signature { get { return _signature; } set { _signature = value; OnNotifyPropertyChanged(nameof(Signature)); } }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnNotifyPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public Token(Header header, IEnumerable<KeyValuePair<string, string>> body, string signature)
    {
        _header = header;
        _body = new ObservableCollection<KeyValuePair<string, string>>(body);
        _signature = signature;
    }
}

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
