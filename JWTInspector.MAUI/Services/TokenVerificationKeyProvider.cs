using JWTInspector.MAUI.ViewModels;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace JWTInspector.MAUI.Services;

public class TokenVerificationKeyProvider : ITokenVerificationKeyProvider
{
    public SecurityKey? GetSecurityKey(TokenKeyViewModel keysModel, KeySource source)
    {
        return source switch
        {
            KeySource.Characters => GetCharactersSecurityKey(keysModel.Characters),
            KeySource.File => GetFileSecurityKey(keysModel.File),
            _ => null
        };
    }

    private SecurityKey? GetFileSecurityKey(FileKeyViewModel file)
    {
        return null; // not supported yet
    }

    private SecurityKey? GetCharactersSecurityKey(CharacterKeyViewModel characters)
    {
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
