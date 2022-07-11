using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.ComponentModel;
using System.Text;

namespace JWTInspector.Models;
public partial class VerificationKeyBase64EncodedModel: VerificationKeyModel, INotifyPropertyChanged
{
    [NotifyPropertyChange("EncodedKey")]
    private string? _encodedKey;
    [NotifyPropertyChange("IsBase64Encoded")]
    private bool _isBase64Encoded;

    public event PropertyChangedEventHandler? PropertyChanged;

    public override bool IsJwtSignatureSupported(TokenSignatureAlgorithm alg)
    {
        if (string.IsNullOrEmpty(EncodedKey))
            return false;

        var keyBytes = KeyBytes;
        return (alg == TokenSignatureAlgorithm.HS256 && keyBytes.Length * 8 >= 256)
            || (alg == TokenSignatureAlgorithm.HS384 && keyBytes.Length * 8 >= 384)
            || (alg == TokenSignatureAlgorithm.HS512 && keyBytes.Length * 8 >= 512);
    }

    public byte[] KeyBytes => IsBase64Encoded ? Convert.FromBase64String(EncodedKey) : Encoding.UTF8.GetBytes(EncodedKey);

    public override (SignatureState state, string? error) VerifySignature(string token, TokenSignatureAlgorithm alg)
    {
        if (string.IsNullOrEmpty(EncodedKey))
            return (SignatureState.Unknown, default);
        var handler = new JsonWebTokenHandler();
        var result = handler.ValidateToken(token, new TokenValidationParameters()
        {
            ValidateActor = false,
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = false,
            ValidateTokenReplay = false,
            IssuerSigningKey = new SymmetricSecurityKey(KeyBytes)
        });
        return (result.IsValid ? SignatureState.Valid : SignatureState.Invalid, result.Exception?.Message);
    }

    public override void SubscribeToChanges(PropertyChangedEventHandler callback)
    {
        PropertyChanged += callback;
    }
}
