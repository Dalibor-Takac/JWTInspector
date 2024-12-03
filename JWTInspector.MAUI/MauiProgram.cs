using JWTInspector.MAUI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace JWTInspector.MAUI;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .Services
                .AddTransient<ITokenProvider, TokenProvider>()
                .AddTransient<ITokenVerificationKeyProvider, TokenVerificationKeyProvider>()
                .AddTransient<JsonWebTokenHandler>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
