using JWTInspector.Helpers;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;

namespace JWTInspector.Models;
public partial class VerificationKeyCertificateModel: VerificationKeyModel, INotifyPropertyChanged
{
    [NotifyPropertyChange("CertificateFile")]
    private string? _certificateFile;
    [NotifyPropertyChange("IncludedKeyKind")]
    private string? _includedKeyKind;
    [NotifyPropertyChange("ErrorMessage")]
    private string? _errorMessage;

    public event PropertyChangedEventHandler? PropertyChanged;

    public override bool IsJwtSignatureSupported(TokenSignatureAlgorithm alg)
    {
        if (string.IsNullOrEmpty(CertificateFile))
            return false;

        var keyImportResult = PublicKeyFromFileImportHelper.ImportKeysFromFile(CertificateFile);

        return alg switch
        {
            TokenSignatureAlgorithm.RS256 or TokenSignatureAlgorithm.PS256 => keyImportResult.RsaKey is not null && keyImportResult.RsaKey.KeySize >= 256,
            TokenSignatureAlgorithm.RS384 or TokenSignatureAlgorithm.PS384 => keyImportResult.RsaKey is not null && keyImportResult.RsaKey.KeySize >= 384,
            TokenSignatureAlgorithm.RS512 or TokenSignatureAlgorithm.PS512 => keyImportResult.RsaKey is not null && keyImportResult.RsaKey.KeySize >= 512,
            TokenSignatureAlgorithm.ES256 => keyImportResult.EcKey is not null && keyImportResult.EcKey.KeySize >= 256,
            TokenSignatureAlgorithm.ES384 => keyImportResult.EcKey is not null && keyImportResult.EcKey.KeySize >= 384,
            TokenSignatureAlgorithm.ES512 => keyImportResult.EcKey is not null && keyImportResult.EcKey.KeySize >= 512,
            _ => false,
        };
    }

    public override (SignatureState state, string? error) VerifySignature(string token, TokenSignatureAlgorithm alg)
    {
        var keyImportResult = PublicKeyFromFileImportHelper.ImportKeysFromFile(CertificateFile);
        SecurityKey? key = null;
        switch (alg)
        {
            case TokenSignatureAlgorithm.RS256:
            case TokenSignatureAlgorithm.RS384:
            case TokenSignatureAlgorithm.RS512:
            case TokenSignatureAlgorithm.PS256:
            case TokenSignatureAlgorithm.PS384:
            case TokenSignatureAlgorithm.PS512:
                key = new RsaSecurityKey(keyImportResult.RsaKey);
                break;
            case TokenSignatureAlgorithm.ES256:
            case TokenSignatureAlgorithm.ES384:
            case TokenSignatureAlgorithm.ES512:
                key = new ECDsaSecurityKey(keyImportResult.EcKey);
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
        return (result.IsValid ? SignatureState.Valid : SignatureState.Invalid, result.Exception?.Message);
    }

    public override void SubscribeToChanges(PropertyChangedEventHandler callback)
    {
        PropertyChanged += callback;
    }
}
