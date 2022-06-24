using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Windows;

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

    private string? _signatureVerificationKey;
    public string? SignatureVerificationKey
    {
        get { return _signatureVerificationKey; }
        set
        {
            _signatureVerificationKey = value;
            VerifySignature();
            OnPropertyChanged(nameof(SignatureVerificationKey));
        }
    }

    private bool _isSignatureBase64Encoded;
    public bool IsSignatureBase64Encoded
    {
        get { return _isSignatureBase64Encoded; }
        set
        {
            _isSignatureBase64Encoded = value;
            VerifySignature();
            OnPropertyChanged(nameof(IsSignatureBase64Encoded));
        }
    }

    private string? _signatureVerificationFile;
    public string? SignatureVerificationFile
    {
        get { return _signatureVerificationFile; }
        set
        {
            _signatureVerificationFile = value;
            VerifySignature();
            OnPropertyChanged(nameof(SignatureVerificationFile));
        }
    }

    private Visibility _clearTokenVisible = Visibility.Collapsed;
    public Visibility ClearTokenVisible { get { return _clearTokenVisible; } set { _clearTokenVisible = value; OnPropertyChanged(nameof(ClearTokenVisible)); } }

    private Visibility _tokenValidSign = Visibility.Collapsed;
    public Visibility TokenValidSign { get { return _tokenValidSign; } set { _tokenValidSign = value; OnPropertyChanged(nameof(TokenValidSign)); } }

    private Visibility _tokenInvalidSign = Visibility.Collapsed;
    public Visibility TokenInvalidSign { get { return _tokenInvalidSign; } set { _tokenInvalidSign = value; OnPropertyChanged(nameof(TokenInvalidSign)); } }

    private Visibility _instructionsText = Visibility.Visible;
    public Visibility InstrationText { get { return _instructionsText; } set { _instructionsText = value; OnPropertyChanged(nameof(InstrationText)); } }

    public void Clear()
    {
        TokenString = null;
        Token = null;
        ErrorMessage = null;
        SignatureVerificationKey = null;
        SignatureVerificationFile = null;
        IsSignatureBase64Encoded = false;
        ClearTokenVisible = Visibility.Collapsed;
        TokenValidSign = Visibility.Collapsed;
        TokenInvalidSign = Visibility.Collapsed;
        InstrationText = Visibility.Visible;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private const char TOKEN_COMPONENT_DELIMITER = '.';

    public void ParseToken(string token)
    {
        ErrorMessage = null;
        var tokenComponents = token
            .Split(TOKEN_COMPONENT_DELIMITER)
            .ToArray();

        InstrationText = Visibility.Collapsed;

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
            ClearTokenVisible = Visibility.Visible;
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

    public void VerifySignature()
    {
        var verificationKey = GetVerificationKey();
        var verificationHandler = new JsonWebTokenHandler();
        var validationResult = verificationHandler.ValidateToken(TokenString, new TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = verificationKey,
            RequireExpirationTime = false
        });
        TokenValidSign = validationResult.IsValid ? Visibility.Visible : Visibility.Collapsed;
        TokenInvalidSign = validationResult.IsValid ? Visibility.Collapsed : Visibility.Visible;
    }

    public SecurityKey? GetVerificationKey()
    {
        if (_token is null || _token.Header is null)
            return null;

        byte[] keyBytes;
        if (string.IsNullOrEmpty(SignatureVerificationFile))
        {
            if (string.IsNullOrEmpty(SignatureVerificationKey))
                return null;
            else
            {
                keyBytes = IsSignatureBase64Encoded ? Convert.FromBase64String(SignatureVerificationKey) : Encoding.ASCII.GetBytes(SignatureVerificationKey);
            }
        }
        else
        {
            keyBytes = File.ReadAllBytes(SignatureVerificationFile);
        }
        switch (_token.Header.Algorithm)
        {
            case TokenSignatureAlgorithm.HS256:
            case TokenSignatureAlgorithm.HS384:
            case TokenSignatureAlgorithm.HS512:
                return new SymmetricSecurityKey(keyBytes);
            case TokenSignatureAlgorithm.RS256:
            case TokenSignatureAlgorithm.RS384:
            case TokenSignatureAlgorithm.RS512:
            case TokenSignatureAlgorithm.PS256:
            case TokenSignatureAlgorithm.PS384:
            case TokenSignatureAlgorithm.PS512:
                {
                    var rsaPubKey = new X509Certificate2(keyBytes).GetRSAPublicKey();
                    if (rsaPubKey is null)
                        return null;
                    return new RsaSecurityKey(rsaPubKey);
                }
            case TokenSignatureAlgorithm.ES256:
            case TokenSignatureAlgorithm.ES384:
            case TokenSignatureAlgorithm.ES512:
                {
                    var ecdsaPubkey = new X509Certificate2(keyBytes).GetECDsaPublicKey();
                    if (ecdsaPubkey is null)
                        return null;
                    return new ECDsaSecurityKey(ecdsaPubkey);
                }
        }
        return null;
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
