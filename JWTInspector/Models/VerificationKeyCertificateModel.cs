using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

namespace JWTInspector.Models;
public partial class VerificationKeyCertificateModel: VerificationKeyModel, INotifyPropertyChanged
{
    [NotifyPropertyChange("CertificateFile")]
    private string? _certificateFile;
    [NotifyPropertyChange("IncludedKeyKind")]
    private string? _includedKeyKind;
    [NotifyPropertyChange("HasPrivatekey")]
    private bool _hasPrivateKey;
    [NotifyPropertyChange("HasPublicKey")]
    private bool _hasPublicKey;

    public event PropertyChangedEventHandler? PropertyChanged;

    public override bool IsJwtSignatureSupported(TokenSignatureAlgorithm alg)
    {
        if (string.IsNullOrEmpty(CertificateFile))
            return false;

        var cert = new X509Certificate2(CertificateFile);
        var rsaKey = cert.PublicKey.GetRSAPublicKey();
        var ecKey = cert.PublicKey.GetECDsaPublicKey();
        return alg switch
        {
            TokenSignatureAlgorithm.RS256 or TokenSignatureAlgorithm.PS256 => rsaKey is not null && rsaKey.KeySize >= 256,
            TokenSignatureAlgorithm.RS384 or TokenSignatureAlgorithm.PS384 => rsaKey is not null && rsaKey.KeySize >= 384,
            TokenSignatureAlgorithm.RS512 or TokenSignatureAlgorithm.PS512 => rsaKey is not null && rsaKey.KeySize >= 512,
            TokenSignatureAlgorithm.ES256 => ecKey is not null && ecKey.KeySize >= 256,
            TokenSignatureAlgorithm.ES384 => ecKey is not null && ecKey.KeySize >= 384,
            TokenSignatureAlgorithm.ES512 => ecKey is not null && ecKey.KeySize >= 512,
            _ => false,
        };
    }

    public override (SignatureState state, string error) VerifySignature(string token, TokenSignatureAlgorithm alg)
    {
        var cert = new X509Certificate2(CertificateFile);
        SecurityKey? key = null;
        switch (alg)
        {
            case TokenSignatureAlgorithm.RS256:
            case TokenSignatureAlgorithm.RS384:
            case TokenSignatureAlgorithm.RS512:
            case TokenSignatureAlgorithm.PS256:
            case TokenSignatureAlgorithm.PS384:
            case TokenSignatureAlgorithm.PS512:
                key = new RsaSecurityKey(cert.PublicKey.GetRSAPublicKey());
                break;
            case TokenSignatureAlgorithm.ES256:
            case TokenSignatureAlgorithm.ES384:
            case TokenSignatureAlgorithm.ES512:
                key = new ECDsaSecurityKey(cert.PublicKey.GetECDsaPublicKey());
                break;
        }

        if (key is null)
            return (SignatureState.Unknown, "Certificate key couldn't be used to verify this type of token signature");

        var handler = new JsonWebTokenHandler();
        var result = handler.ValidateToken(token, new TokenValidationParameters()
        {
            ValidateActor = false,
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = false,
            ValidateTokenReplay = false,
            IssuerSigningKey = key
        });
        return (result.IsValid ? SignatureState.Valid : SignatureState.Invalid, result.Exception.Message);
    }

    public override void SubscribeToChanges(PropertyChangedEventHandler callback)
    {
        PropertyChanged += callback;
    }
}
