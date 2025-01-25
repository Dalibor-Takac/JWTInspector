using JWTInspector.MAUI.ViewModels;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;

namespace JWTInspector.MAUI.Services;

public class TokenVerificationKeyProvider : ITokenVerificationKeyProvider
{
    public SecurityKey? GetSecurityKey(TokenKeyViewModel keysModel, KeySource source)
    {
        keysModel.KeyErrorMessage = null;
        return source switch
        {
            KeySource.Characters => GetCharactersSecurityKey(keysModel),
            KeySource.File => GetFileSecurityKey(keysModel),
            _ => null
        };
    }

    private SecurityKey? GetFileSecurityKey(TokenKeyViewModel tokenViewModel)
    {
        var file = tokenViewModel.File;
        X509Certificate2? cert = null;
        try
        {
            try
            {
                cert = X509CertificateLoader.LoadCertificateFromFile(file.FilePath);
            }
            catch (CryptographicException)
            {
                try
                {
                    if (string.IsNullOrEmpty(file.FileDecriptionPassword))
                    {
                        cert = X509CertificateLoader.LoadPkcs12FromFile(file.FilePath, null);
                    }
                    else
                    {
                        cert = X509CertificateLoader.LoadPkcs12FromFile(file.FilePath, file.FileDecriptionPassword);
                    }
                }
                catch (CryptographicException)
                {
                    var rsa = RSA.Create();
                    try
                    {
                        rsa.ImportFromPem(File.ReadAllText(file.FilePath));
                        return new RsaSecurityKey(rsa);
                    }
                    catch (CryptographicException)
                    {
                        var ecdsa = ECDsa.Create();
                        ecdsa.ImportFromPem(File.ReadAllText(file.FilePath));
                        return new ECDsaSecurityKey(ecdsa);
                    }
                }
            }
        }
        catch (CryptographicException ex)
        {
            tokenViewModel.KeyErrorMessage = ex.Message;
            return null;
        }

        var (result, error) = ExtractPublicKey(cert);
        tokenViewModel.KeyErrorMessage = error;
        return result;
    }

    private (SecurityKey? Key, string? Error) ExtractPublicKey(X509Certificate2 cert)
    {
        var rsaPublickey = cert.GetRSAPublicKey();
        if (rsaPublickey != null)
        {
            return (new RsaSecurityKey(rsaPublickey), null);
        }

        var ecdsaPublickey = cert.GetECDsaPublicKey();
        if (ecdsaPublickey != null)
        {
            return (new ECDsaSecurityKey(ecdsaPublickey), null);
        }

        return (null, "No public key found in certificate file");
    }

    private SecurityKey? GetCharactersSecurityKey(TokenKeyViewModel tokenViewModel)
    {
        var characters = tokenViewModel.Characters;
        if (string.IsNullOrEmpty(characters.KeyCharacters))
            return null;

        var intermediate = characters.IsUrlEncoded ? Uri.UnescapeDataString(characters.KeyCharacters) : characters.KeyCharacters;
        var bytes = characters.IsBase64Encoded ? Convert.FromBase64String(intermediate) : Encoding.UTF8.GetBytes(intermediate);
        return new SymmetricSecurityKey(bytes);
    }
}

public interface ITokenVerificationKeyProvider
{
    SecurityKey? GetSecurityKey(TokenKeyViewModel keysModel, KeySource source);
}

public enum KeySource
{
    Characters,
    File
}
