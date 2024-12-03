using JWTInspector.MAUI.ViewModels;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace JWTInspector.MAUI.Services;

public class TokenVerificationKeyProvider : ITokenVerificationKeyProvider
{
    public SecurityKey GetSecurityKey(TokenKeyViewModel keysModel, KeySource source)
    {
        //throw new NotImplementedException();
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes("bla"));
    }
}

public interface ITokenVerificationKeyProvider
{
    SecurityKey GetSecurityKey(TokenKeyViewModel keysModel, KeySource source);
}

public enum KeySource
{
    Characters,
    File
}
