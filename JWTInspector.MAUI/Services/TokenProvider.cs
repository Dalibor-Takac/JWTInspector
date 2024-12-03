using JWTInspector.MAUI.ViewModels;
using Microsoft.IdentityModel.Tokens;

namespace JWTInspector.MAUI.Services;

public class TokenProvider : ITokenProvider
{
    public void ParseToken(string token, TokenViewModel resultViewModel)
    {
        //throw new NotImplementedException();
    }

    public void ValidateToken(string token, SecurityKey key, TokenViewModel resultViewModel)
    {
        //throw new NotImplementedException();
    }
}

public interface ITokenProvider
{
    void ParseToken(string token, TokenViewModel resultViewModel);
    void ValidateToken(string token, SecurityKey key, TokenViewModel resultViewModel);
}