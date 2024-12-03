using JWTInspector.MAUI.ViewModels;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace JWTInspector.MAUI.Services;

public class TokenProvider : ITokenProvider
{
    private readonly JsonWebTokenHandler _handler;
    private static readonly IEnumerable<string> _commonClaimTypes = ["aud", "jti", "azp", "actort", "iat", "iss", "sub", "nbf", "exp"];

    public TokenProvider(JsonWebTokenHandler handler)
    {
        _handler = handler;
    }

    public void ParseToken(string token, TokenViewModel resultViewModel)
    {
        resultViewModel.Reset();

        if (string.IsNullOrEmpty(token))
            return;

        try
        {
            if (_handler.CanReadToken(token))
            {
                var parsed = _handler.ReadJsonWebToken(token);
                if (parsed is not null)
                {
                    SetTokenViewModel(parsed, resultViewModel);
                }
                else
                {
                    resultViewModel.ErrorMessage = "There was an error parsing the token";
                }
            }
            else
            {
                resultViewModel.ErrorMessage = "Provided text can't be parsed into a JWT token. Check if you copied everything and that the token text is not additionally encoded in some way";
            }
        }
        catch (Exception ex)
        {
            resultViewModel.ErrorMessage = ex.Message;
        }
    }

    private void SetTokenViewModel(JsonWebToken parsed, TokenViewModel resultViewModel)
    {
        resultViewModel.AddHeaderEntry("typ", parsed.Typ, "Token type, should read 'JWT'");
        resultViewModel.AddHeaderEntry("alg", parsed.Alg, "Token signature algorithm");
        resultViewModel.AddHeaderEntry("cty", parsed.Cty, "Token content type");
        resultViewModel.AddHeaderEntry("x5t", parsed.X5t, "x509 certificate thumbprint");
        resultViewModel.AddHeaderEntry("enc", parsed.Enc, "Encoding");
        resultViewModel.AddHeaderEntry("kid", parsed.Kid, "Key id used to sign the token");
        resultViewModel.AddHeaderEntry("zip", parsed.Zip, "Compression algorithm used to transform the token");

        resultViewModel.AddContentEntry("actort", parsed.Actor, null);
        resultViewModel.AddContentEntry("aud", string.Join(", ", parsed.Audiences), "Audience list");
        resultViewModel.AddContentEntry("azp", parsed.Azp, "Authorized party");
        resultViewModel.AddContentEntry("jti", parsed.Id, "JWT ID claim, used to prevent replay attacks");
        resultViewModel.AddContentEntry("iat", val => val != DateTime.MinValue, val => new DateTimeOffset(val).ToUnixTimeSeconds().ToString(), val => $"Issued at, {val:F}", parsed.IssuedAt);
        resultViewModel.AddContentEntry("iss", parsed.Issuer, "Principal that issued the token");
        resultViewModel.AddContentEntry("sub", parsed.Subject, "Token subject (user id)");
        resultViewModel.AddContentEntry("nbf", val => val != DateTime.MinValue, val => new DateTimeOffset(val).ToUnixTimeSeconds().ToString(), val => $"Valid from {val:F}", parsed.ValidFrom);
        resultViewModel.AddContentEntry("exp", val => val != DateTime.MinValue, val => new DateTimeOffset(val).ToUnixTimeSeconds().ToString(), val => $"Valid to {val:F}", parsed.ValidTo);

        foreach (var item in parsed.Claims)
        {
            if (_commonClaimTypes.Contains(item.Type))
                continue;

            resultViewModel.AddContentEntry(item.Type, item.Value, "Custom claim");
        }
    }

    public async Task ValidateToken(string token, SecurityKey key, TokenViewModel resultViewModel)
    {
        resultViewModel.Reset();
        try
        {
            var validationParams = new TokenValidationParameters()
            {
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
            };
            var parsedAndValidated = await _handler.ValidateTokenAsync(token, validationParams);

            if (parsedAndValidated is not null && parsedAndValidated.IsValid)
            {
                SetTokenViewModel((JsonWebToken)parsedAndValidated.SecurityToken, resultViewModel);
                resultViewModel.VerificationStatus = TokenVerificationStatus.Valid;
            }
            else
            {
                resultViewModel.ErrorMessage = parsedAndValidated?.Exception.Message ?? "Unknown error while validating token";
                resultViewModel.VerificationStatus = TokenVerificationStatus.Invalid;
            }
        }
        catch (Exception ex)
        {
            resultViewModel.ErrorMessage = ex.Message;
        }
    }
}

public interface ITokenProvider
{
    void ParseToken(string token, TokenViewModel resultViewModel);
    Task ValidateToken(string token, SecurityKey key, TokenViewModel resultViewModel);
}