﻿using JWTInspector.MAUI.Services;
using Microsoft.Extensions.Logging;

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
                .AddTransient<ITokenVerificationKeyProvider, TokenVerificationKeyProvider>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
